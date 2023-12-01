using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        Init();

        UIManager.Instance.OpenWindow<GamingView>();
    }

    private void Init()
    {
        PlayerDataManager.Instance.ResetCurrentScore();
        Jewel.CounterReset();

    }
}
