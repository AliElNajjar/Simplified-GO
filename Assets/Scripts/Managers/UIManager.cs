using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text whiteScoreUI;
    [SerializeField] private Text blackScoreUI;
    [SerializeField] private int scoreToWin = 3;

    [SerializeField] private Button resetBtn;

    private int whiteScore;
    private int blackScore;

    private void Start()
    {
        GameManager.Instance.OnScore += HandleScoring;
        GameManager.Instance.OnGameReset += ResetUI;

        resetBtn.onClick.AddListener(ResetGame);
    }

    private void ResetUI()
    {
        whiteScore = 0;
        blackScore = 0;
        whiteScoreUI.text = string.Format("{0:00}", whiteScore);
        blackScoreUI.text = string.Format("{0:00}", blackScore);

    }

    private void ResetGame()
    {
        GameManager.Instance.ResetGame();
    }

    private void HandleScoring(Player scoringPlayer, int score)
    {
        if (scoringPlayer == Player.White)
        {
            whiteScore += score;
            whiteScoreUI.text = string.Format("{0:00}", whiteScore);
        }
            
        if (scoringPlayer == Player.Black)
        {
            blackScore += score;
            blackScoreUI.text = string.Format("{0:00}", blackScore);
        }


        if (whiteScore >= scoreToWin || blackScore >= scoreToWin)
            ResetGame();
    }
}
