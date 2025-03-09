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
            GameManager.Instance.PlayerCreated += ChangeScoreText;
            GameManager.Instance.CheckerLost += ChangeScoreText;
        }
    }

    void Update()
    {
        
    }
    #endregion

    #region PrivateMethods
    private void ChangeScoreText(Player player)
    {
        if (player.ID == Utils.PlayerID.Player1)
        {
            player1Score.text = $"{Utils.PlayerID.Player1} : {player.checkers.Count.ToString()}";
        }
        else if (player.ID == Utils.PlayerID.Player2)
        {
            player2Score.text = $"{Utils.PlayerID.Player2} : {player.checkers.Count.ToString()}";
        }
    }
    #endregion

    #region PublicMethods

    #endregion
}
