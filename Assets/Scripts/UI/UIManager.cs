using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;
    public Canvas Canvas { get; private set; }
    
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
    }
}
