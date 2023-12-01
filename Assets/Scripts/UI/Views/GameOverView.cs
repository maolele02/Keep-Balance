using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : UIWindow
{
    public override string PrefabName => "GameOverView";

    [SerializeField][Header("��������")] private TextMeshProUGUI scoreText;

    [SerializeField][Header("�������˵���ť")] private Button backToMainViewBtn;
    [SerializeField][Header("���¿�ʼ��ť")] private Button restartBtn;

    public override void __Init()
    {
        base.__Init();

        SetBtn();
    }

    public override void OnEabled(params object[] param)
    {
        base.OnEabled(param);

        scoreText.text = "Score: " + PlayerDataManager.Instance.CurrentScore;
    }

    private void SetBtn()
    {
        backToMainViewBtn.onClick.AddListener(() =>
        {
            GameReset();
            UIManager.Instance.CloseWindow<GameOverView>();
        });

        restartBtn.onClick.AddListener(() =>
        {
            GameReset();
            UIManager.Instance.CloseWindow<GameOverView>();
        });
    }

    private void GameReset()
    {
        GameManager.Instance.GameReset();
    }
}
