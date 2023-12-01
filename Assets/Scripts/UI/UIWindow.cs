using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class UIWindow: MonoBehaviour
{
    public virtual string PrefabName { get; }
    protected CanvasGroup canvasGroup;

    public bool IsActive { get; protected set; }

    public virtual void __Init()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }


    public virtual void OnEabled(params object[] param)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        IsActive = true;
    }

    public virtual void OnDisabled()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        IsActive = false;
    }

}
