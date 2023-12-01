using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamingView : UIWindow
{
    #region Field
    [SerializeField][Header("指针")] private RectTransform pointer;
    [SerializeField][Header("天平")] private Balance balance;

    [SerializeField][Header("卡牌容器")] private RectTransform cardContainer;
    [SerializeField][Header("卡牌预制体")] private GameObject cardPrefab;


    [SerializeField][Header("宝石预制体")] private GameObject jewelPrefab;
    [SerializeField][Header("电脑宝石生成区域")] private RectTransform enemySpawnArea;

    [SerializeField][Header("分数文字")] private TextMeshProUGUI scoreText;

    [SerializeField][Header("越界读秒提示界面")] private CanvasGroup outOfRangeTipCanvasGroup;
    [SerializeField][Header("越界读秒进度条")] private Image outOfRangeProgress;

    [SerializeField][Header("游戏配置")] private GameSetting gameSetting;

    private float angleRange;
    private float blockAreaHeight;
    private float cardContainerWidth;

    private GameObjectPool cardPool;
    private GameObjectPool jewelPool;

    private Coroutine outOfRangeProgressCoroutine;
    #endregion

    #region Property
    public override string PrefabName => "GamingView";
    public GameObjectPool CardPool => cardPool;
    public GameObjectPool JewelPool => jewelPool;

    public float AngleUpperLimit => gameSetting.angleUpperLimit;
    public float AngleLowerLimit => -AngleUpperLimit;
    #endregion

    private void Awake()
    {
        cardPool = new GameObjectPool();
        cardPool.Init(cardPrefab,
            (go) =>
            {
                go.transform.SetParent(cardContainer.transform, false);
                Card card = go.GetComponent<Card>();
                card.Show();
                card.SetJewelSpawnDelay(gameSetting.playerJewelSpawnDelay);

                card.AddEndDragListener(() =>
                {
                    card.Hide();
                    CardsContinueMove();
                });

                card.AddSpawnListener((cardID, slot) =>
                {
                    GameObject jewelObj = jewelPool.Get();
                    Jewel jewel = jewelObj.GetComponent<Jewel>();
                    // card id 与 jewel一一对应
                    jewel.Init(cardID, JewelType.Player);
                    jewelObj.transform.SetParent(slot.transform.parent, false);
                    slot.transform.GetChild(0).gameObject.SetActive(false);
                    Vector2 initalPosition = slot.AnchoredPosition;
                    initalPosition.x += slot.RectTransform.rect.width / 2;
                    jewelObj.GetComponent<RectTransform>().anchoredPosition = initalPosition;
                });

                card.AddSpawnCompleteListener((go) =>
                {
                    cardPool.Return(go);
                });
            },
            null);

        jewelPool = new GameObjectPool();

        jewelPool.Init(jewelPrefab,
            (go) =>
            {
                go.tag = Tags.Jewel;
                go.GetComponent<Jewel>().AddSpawnSpeed();
            }
            ,
            null);
    } 

    private void Start()
    {
        cardContainerWidth = cardContainer.rect.width;
        outOfRangeTipCanvasGroup.alpha = 0f;

        angleRange = AngleUpperLimit - AngleLowerLimit;
        if (angleRange < 1f)
        {
            Debug.LogError($"angleRangle: {angleRange} error.");
            return;
        }
        blockAreaHeight = pointer.transform.parent.GetComponent<RectTransform>().rect.height;
        if (blockAreaHeight <= 0.1f)
        {
            Debug.LogError($"blockAreaHeight: {blockAreaHeight} error.");
            return;
        }
        balance.AddRangeListener(MovePointer);
        balance.AngleLowerLimit = AngleLowerLimit;
        balance.AngleUpperLimit = AngleUpperLimit;
        JewelManager.Instance.AddRemoveListener(
            ()=>
            {
                scoreText.text = $"Score: {PlayerDataManager.Instance.CurrentScore}";
            });

        StopAllCoroutines();
        StartCoroutine(CardComming());
        StartCoroutine(SpawnEnemyJewel());
    }

    private void MovePointer(float balanceAngle)
    {
        float targetPosY = balanceAngle * blockAreaHeight / angleRange;
        pointer.anchoredPosition = new Vector2(0f, targetPosY);        
    
        if(Mathf.Abs(balanceAngle) >= AngleUpperLimit)  // 超出角度范围
        {
            DoOutOfRangeProgress();
        }
        // 未超出角度范围且之前的outOfRangeProgress尚未还原
        else if (outOfRangeProgress.fillAmount > 0.1f)
        {
            outOfRangeProgress.fillAmount = 0f;
            outOfRangeTipCanvasGroup.alpha = 0f;
            if(outOfRangeProgressCoroutine != null)
            {
                StopCoroutine(outOfRangeProgressCoroutine);
            }
        }
    }

    private void DoOutOfRangeProgress()
    {
        outOfRangeTipCanvasGroup.alpha = 1f;
        if(outOfRangeProgressCoroutine != null)
        {
            StopCoroutine(outOfRangeProgressCoroutine);
        }
        outOfRangeProgressCoroutine = StartCoroutine(_DoOutOfRangeProgress());
    }

    private IEnumerator _DoOutOfRangeProgress()
    {
        float timer = 0f;
        while(timer <= gameSetting.outOfRangeTimeLimit)
        {
            outOfRangeProgress.fillAmount = timer / gameSetting.outOfRangeTimeLimit;
            yield return null;
            timer += Time.deltaTime;
        }
        // outOfRangeTipCanvasGroup.alpha = 0f;
        GameOver();
    }

    private void GameOver()
    {
        StopAllCoroutines();
        UIManager.Instance.OpenWindow<GameOverView>();
    }

    private IEnumerator CardComming()
    {
        while(true)
        {
            if(cardPool.ActivedCount < gameSetting.cardStorageUpperLimit)
            {
                GameObject card = cardPool.Get();
                Card cardComponent = card.GetComponent<Card>();
                cardComponent.Init(1, cardContainerWidth);
                cardComponent.BeginMove();
                yield return new WaitForSeconds(gameSetting.spawnTimeSpacing);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void CardsContinueMove()
    {
        Card card = null;
        foreach(var go in cardPool.ActivedPool)
        {
            card = go.GetComponent<Card>();
            card.ResetCardIndex();
        }
    }

    private IEnumerator SpawnEnemyJewel()
    {
        int jewleID = -1;
        while (true)
        {
            jewleID = GetRandJewelID();
            GameObject jewelObj = jewelPool.Get();
            Jewel jewel = jewelObj.GetComponent<Jewel>();
            jewel.Init(jewleID, JewelType.Enemy);
            jewelObj.transform.SetParent(enemySpawnArea, false);
            Vector2 initalPosition = new Vector2(enemySpawnArea.rect.width / 2f, 0f);
            float shift = Random.Range(-80f, 80f);
            initalPosition.x += shift;
            jewelObj.GetComponent<RectTransform>().anchoredPosition = initalPosition;
            yield return new WaitForSeconds(gameSetting.enemyJewelSpawnDelay);
        }
    }

    /// <summary>
    /// 随机获得要生成的宝石的宝石配置ID
    /// </summary>
    /// <returns></returns>
    private int GetRandJewelID()
    {
        //var cardCfg = XConfig.Card.Items;
        //int resultID = cardCfg.Count;
        //int randValue = Random.Range(0, 1000);
        //int upperLimit = 0;
        //for(int i = cardCfg.Count; i > 0; --i)
        //{
        //    upperLimit += cardCfg[i].probability_ai;
        //    if(randValue >= cardCfg[i].probability_ai && randValue < upperLimit)
        //    {
        //        resultID = i;
        //        break;
        //    }
        //}
        //return resultID;

        // 测试用
        return 1;
    }


}
