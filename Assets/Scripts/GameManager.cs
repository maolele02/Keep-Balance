using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    void Start()
    {
        Init();

        UIManager.Instance.OpenWindow<GamingView>();
    }

    private void Init()
    {
        GameReset();

    }

    public void GameReset()
    {
        PlayerDataManager.Instance.ResetCurrentScore();
        Jewel.CounterReset();
        UIManager.Instance.OpenWindow<GamingView>();
    }
}
