using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager: Singleton<UIManager>
{
    public Canvas Canvas { get; private set; }

    private Dictionary<Type, UIWindow> views;
    
    public UIManager() 
    {
        Init();
    }

    private void Init()
    {
        GameObject canvasGameObject = new GameObject("Canvas");

        Canvas = canvasGameObject.AddComponent<Canvas>();
        Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0;
        canvasGameObject.AddComponent<GraphicRaycaster>();

        views = new Dictionary<Type, UIWindow>(3);

        GameObject eventSysetmGameObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
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
