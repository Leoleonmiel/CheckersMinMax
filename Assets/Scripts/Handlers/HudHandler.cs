using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudHandler : MonoBehaviour
{
    #region Fields
    [SerializeField] TMP_Text player1Score;  
    [SerializeField] TMP_Text player2Score;  
    #endregion

    #region UnityMessages
    void Awake()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerCreated += CreateScoreText;
            GameManager.Instance.CheckerLost += UpdateScore;
        }
    }

    void Update()
    {
        
    }
    #endregion

    #region PrivateMethods
    private void CreateScoreText(Player player)
    {
        Debug.Log("in CreateScoreText: " + player.ID);

        if (player.ID == Utils.PlayerID.Player1) 
        {
            player1Score.text = "Player 1: " + player.checkers.Count.ToString();
        }
        else if (player.ID == Utils.PlayerID.Player2) 
        {
            player2Score.text = "Player 2: " + player.checkers.Count.ToString();
        }
    }

    public void UpdateScore(Player player)
    {
        if (player.ID == Utils.PlayerID.Player1)  
        {
            player1Score.text = "Player 1: " + player.checkers.Count.ToString();
        }
        else if (player.ID == Utils.PlayerID.Player2)  
        {
            player2Score.text = "Player 2: " + player.checkers.Count.ToString();
        }
    }
    #endregion

    #region PublicMethods

    #endregion
}
