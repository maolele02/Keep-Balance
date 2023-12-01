using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager
{
    private static UIManager instance;
    public Canvas Canvas { get; private set; }

    private Dictionary<Type, UIWindow> views;
    
    private UIManager() 
    {
        Init();
    }
    public static UIManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new UIManager();
            }
            return instance;
        }
    }

    private void Init()
    {
        Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        views = new Dictionary<Type, UIWindow>(3);
    }

    public void OpenWindow<T>(params object[] param) where T : UIWindow, new()
    {
        var type = typeof(T);
        if(views.TryGetValue(type, out UIWindow win))
        {
            win.OnEabled(param);
        }
        else
        {
            UIWindow view = new T();
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Views/" + view.PrefabName);
            GameObject go = UnityEngine.Object.Instantiate<GameObject>(prefab, Canvas.transform);
            UIWindow viewComponent = go.GetComponent<UIWindow>();
            views.Add(type, viewComponent);
            viewComponent.__Init();
            viewComponent.OnEabled(param);
        }
    }

    public T GetActivedWindow<T>() where T : UIWindow, new()
    {
        var type = typeof(T);
        if (views.TryGetValue(type, out UIWindow win))
        {
            if(win.IsActive)
            {
                return win as T;
            }
        }
        return null;
    }

    public void CloseWindow<T>() where T : UIWindow, new()
    {
        var type = typeof(T);
        if (views.TryGetValue(type, out UIWindow win))
        {
            if (win.IsActive)
            {
                win.OnDisabled();
            }
        }
    }
}
