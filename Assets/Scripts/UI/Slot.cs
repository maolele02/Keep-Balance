using UnityEngine;

public class Slot : MonoBehaviour
{
    private int slotIndex;
    public int SlotIndex
    { get { return slotIndex; } }

    private RectTransform rectTrans;
    public RectTransform RectTransform { get { return rectTrans; } }

    public Vector2 AnchoredPosition
    {
        get 
        {
            return rectTrans.anchoredPosition; 
        }
    }

    private void Start()
    {
        slotIndex = transform.GetSiblingIndex();
        rectTrans = GetComponent<RectTransform>();
    }
}
