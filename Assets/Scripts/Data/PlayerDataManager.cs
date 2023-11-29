using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    int score;
    int date;
    string playerName;
}

public class PlayerDataManager
{
    private static PlayerDataManager instance;
    public static PlayerDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerDataManager();
            }
            return instance;
        }
    }

    private int currentScore;
    public int CurrentScore => currentScore;
    
    public void AddScore(int score)
    {
        if(score > 0)
        {
            currentScore += score;
        }
    }

    public void ResetCurrentScore()
    {
        currentScore = 0;
    }
}
