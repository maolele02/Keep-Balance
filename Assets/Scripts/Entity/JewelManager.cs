using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XConfig;

public class JewelManager
{
    private static JewelManager instance;
    public static JewelManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance= new JewelManager();
            }
            return instance;
        }
    }

    private Action removeAction;

    // key: Jewel ID
    private Dictionary<int, List<Jewel>> playerJewels;
    private Dictionary<int, List<Jewel>> enemyJewels;
    private JewelManager()
    {
        playerJewels = new Dictionary<int, List<Jewel>>();
        enemyJewels = new Dictionary<int, List<Jewel>>();
    }

    public void AddRemoveListener(Action removeAction)
    {
        this.removeAction = removeAction;
    }

    /// <summary>
    /// 消除玩家和敌人之间相同宝石ID的宝石
    /// </summary>
    public void RemoveBalanceJewels()
    {
        List<Jewel> needRemoveJewels = new List<Jewel>();

        int playerJewelStarIndex = 0;

        foreach (var jewels in playerJewels)
        {
            if (jewels.Value == null || jewels.Value.Count == 0)
            {
                continue;
            }

            int playerObjectCount = jewels.Value.Count;
            int enemyObjectCount = 0;
            // 获取与当前宝石列表的宝石ID相同的敌方宝石列表
            if (enemyJewels.TryGetValue(jewels.Key, out var enemyJewe))
            {
                if (enemyJewe == null || enemyJewe.Count == 0)
                {
                    continue;
                }

                // 将enemy jewel需要移除的添加的移除列表中
                for (int i = 0; i < playerObjectCount; ++i)
                {
                    if (i > enemyJewe.Count - 1)
                    {
                        break;
                    }
                    needRemoveJewels.Add(enemyJewe[i]);
                    ++enemyObjectCount;
                }

                // 把要移除的gameObject从enemy jewel中去除
                for (int i = 0; i < needRemoveJewels.Count; ++i)
                {
                    enemyJewels[jewels.Key].Remove(needRemoveJewels[i]);
                    ++playerJewelStarIndex;
                }
            }

            for (int j = 0; j < enemyObjectCount; ++j)
            {
                needRemoveJewels.Add(jewels.Value[j]);
            }

            for (int j = playerJewelStarIndex; j < needRemoveJewels.Count; ++j)
            {
                playerJewels[jewels.Key].Remove(needRemoveJewels[j]);
            }
        }

        int removeCount = 0;
        for (int k = 0; k < needRemoveJewels.Count; ++k)
        {
            GamingView gamingViwe = UIManager.Instance.GetActivedWindow<GamingView>();
            if (gamingViwe == null) 
            {
                // Debug.LogError("GamingView have not be created.");
                return;
            }
            gamingViwe.JewelPool.Return(needRemoveJewels[k].gameObject);
            ++removeCount;
        }

        PlayerDataManager.Instance.AddScore(removeCount);
        removeAction?.Invoke();
    }

    public void AddJewel(GameObject go)
    {
        Jewel jewel = go.GetComponent<Jewel>();

        if (jewel.JewelType == JewelType.Player)
        {
            if (playerJewels.ContainsKey(jewel.JewelCfgID))
            {
                if (playerJewels[jewel.JewelCfgID] == null)
                {
                    playerJewels[jewel.JewelCfgID] = new List<Jewel>() { jewel };
                }
                else
                {
                    List<Jewel> jewels = playerJewels[jewel.JewelCfgID];
                    // 判断是否已经添加过
                    for (int i = 0; i < jewels.Count; ++i)
                    {
                        if (jewels[i].SpawnID == jewel.SpawnID)
                        {
                            return;
                        }
                    }
                    jewels.Add(jewel);
                    // playerJewels[jewel.JewelCfgID].Add(jewel);
                }
            }
            else
            {
                playerJewels.Add(jewel.JewelCfgID, new List<Jewel>() { jewel });
            }
        }
        else
        {
            if (enemyJewels.ContainsKey(jewel.JewelCfgID))
            {
                if (enemyJewels[jewel.JewelCfgID] == null)
                {
                    enemyJewels[jewel.JewelCfgID] = new List<Jewel>() { jewel };
                }
                else
                {
                    List<Jewel> jewels = enemyJewels[jewel.JewelCfgID];
                    // 判断是否已经添加过
                    for (int i = 0; i < jewels.Count; ++i)
                    {
                        if (jewels[i].SpawnID == jewel.SpawnID)
                        {
                            return;
                        }
                    }
                    jewels.Add(jewel);
                }
            }
            else
            {
                enemyJewels.Add(jewel.JewelCfgID, new List<Jewel>() { jewel });
            }
        }
    }

    public void RemoveJewel(JewelType jewelType, int jewelSpawnID)
    {
        if(jewelType == JewelType.Player)
        {
            if(playerJewels.TryGetValue(jewelSpawnID, out var jewels))
            {
                Jewel needRemove = null;
                foreach(var jewel in jewels)
                {
                    if(jewel.SpawnID == jewelSpawnID)
                    {
                        needRemove = jewel;
                        break;
                    }
                }
                if(needRemove != null)
                {
                    jewels.Remove(needRemove);
                }
            }
        }
        else
        {
            if (enemyJewels.TryGetValue(jewelSpawnID, out var jewels))
            {
                Jewel needRemove = null;
                foreach (var jewel in jewels)
                {
                    if (jewel.SpawnID == jewelSpawnID)
                    {
                        needRemove = jewel;
                        break;
                    }
                }
                if (needRemove != null)
                {
                    jewels.Remove(needRemove);
                }
            }
        }
    }
}
