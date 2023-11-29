using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : MonoSingleton<UIWindow>
{
    protected CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();

        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Open()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
