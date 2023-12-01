using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Range(50f, 200f)]
    [SerializeField][Header("ÒÆ¶¯ËÙ¶È")] private float moveSpeed = 150f;
    [SerializeField][Header("¿¨ÅÆ¼ä¾à")] private float spacing = 20f;
    [SerializeField][Header("¿¨ÅÆÍ¼Æ¬")] private Image cardImg; 
    [SerializeField] private int cardIndex;

    private float cardWidth;
    private float targetPosX;
    private float jewelSpawnDelay = 1f;
    private Vector2 beforeDragPos;

    private Slot slot;

    private RectTransform rectTrans;
    private CanvasGroup canvasGroup;
    private Transform originParent;

    private bool isActive;
    private bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
            if(isActive)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    private Action endDragAction;
    // int: card id
    private Action<int, Slot> spawnAction;
    private Action<GameObject> spawnCompleteAction;

    private XConfig.Card.Data selfConfig;

    private Coroutine moveCoroutine;
    private Coroutine spawnCoroutine;

    public void ResetCardIndex()
    {
        cardIndex = transform.GetSiblingIndex();
        CalculateTargetPosX();
        StopMove();
        moveCoroutine = StartCoroutine(Move());
    }

    private void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Init(int cfgID, float containerWidth)
    {
        if(!XConfig.Card.Items.TryGetValue(cfgID, out selfConfig))
        {
            Debug.LogError($"card id: {cfgID} error.");
            return;
        }

        if(rectTrans == null)
        {
            rectTrans = GetComponent<RectTransform>();
            cardWidth = rectTrans.rect.width;
            originParent = transform.parent;
        }

        
        cardIndex = transform.GetSiblingIndex();
        rectTrans.anchoredPosition = new Vector2(containerWidth + cardWidth / 2f, 0f);
        CalculateTargetPosX();
    }

    public void SetJewelSpawnDelay(float delayTime)
    {
        jewelSpawnDelay = delayTime;
    }
    public void AddEndDragListener(Action listener)
    {
        endDragAction = listener;
    }

    public void AddSpawnListener(Action<int, Slot> listener)
    {
        spawnAction = listener;
    }

    public void AddSpawnCompleteListener(Action<GameObject> listener)
    {
        spawnCompleteAction = listener;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void BeginMove()
    {
        StopAllCoroutines();
        moveCoroutine = StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while(true)
        {
            float newPosX = rectTrans.anchoredPosition.x - moveSpeed * Time.deltaTime;
            if(newPosX < targetPosX)
            {
                rectTrans.anchoredPosition = new Vector2(targetPosX, rectTrans.anchoredPosition.y);
                break;
            }
            rectTrans.anchoredPosition = new Vector2(newPosX, rectTrans.anchoredPosition.y);
            yield return null;
        }
    }

    public void StopMove()
    {
        if(moveCoroutine!= null)
        {
            StopCoroutine(moveCoroutine);
        }
    }

    private void CalculateTargetPosX()
    {
        if(cardIndex < 0)
        {
            Debug.LogError($"cardIndex: {cardIndex} error.");
            return;
        }
        if(cardIndex == 0)
        {
            targetPosX = cardWidth / 2f;
        }
        else
        {
            targetPosX = (cardWidth + spacing) * cardIndex + cardWidth / 2f;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        beforeDragPos = rectTrans.anchoredPosition;
        StopMove();
        transform.SetParent(UIManager.Instance.Canvas.transform);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject raycastGameObject = eventData.pointerCurrentRaycast.gameObject;

        if(raycastGameObject != null && raycastGameObject.CompareTag(Tags.Slot))
        {
            slot = raycastGameObject.GetComponent<Slot>();
            raycastGameObject.transform.GetChild(0).gameObject.SetActive(true);
            StopAllCoroutines();
            endDragAction?.Invoke();
            StartCoroutine(SpawnJewel());
        }
        else
        {
            transform.SetParent(originParent);
            rectTrans.anchoredPosition = beforeDragPos;
        }

        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator SpawnJewel()
    {
        yield return new WaitForSeconds(jewelSpawnDelay);
        spawnAction?.Invoke(selfConfig.id, slot);
        spawnCompleteAction?.Invoke(gameObject);
    }
}
