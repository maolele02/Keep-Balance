using System;
using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool
{
    private Action<GameObject> getAction;
    private Action<GameObject> returnAction;

    private GameObject prefab;

    private List<GameObject> activedPool;
    private List<GameObject> disabledPool;

    public List<GameObject> ActivedPool
    {
        get
        {
            return activedPool;
        }
    }

    public int ActivedCount
    {
        get
        {
            return activedPool.Count;
        }
    }

    public GameObjectPool()
    {
        activedPool = new List<GameObject>();
        disabledPool = new List<GameObject>();
    }

    public void Init(GameObject prefab, Action<GameObject> getAction, Action<GameObject> returnAction)
    {
        this.prefab = prefab;
        this.getAction = getAction;
        this.returnAction = returnAction;
    }

    public GameObject Get()
    {
        GameObject go = null;
        if(disabledPool.Count == 0)
        {
            go = UnityEngine.Object.Instantiate(prefab);
            activedPool.Add(go);
        }
        else
        {
            go = disabledPool[0];
            disabledPool.RemoveAt(0);
            activedPool.Add(go);
        }
        
        go.SetActive(true);
        getAction?.Invoke(go);
        return go;
    }

    public void Return(GameObject go)
    {
        go.SetActive(false);
        activedPool.Remove(go);
        disabledPool.Add(go);
        returnAction?.Invoke(go);
    }
}