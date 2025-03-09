using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    #region Fields
    [SerializeField] TMP_Text player1Score;  // Reference to Player 1's score text
    [SerializeField] TMP_Text player2Score;  // Reference to Player 2's score text

    List<TMP_Text> scoreTextList;
    #endregion

    #region UnityMessages
    void Start()
    {
        // Subscribe to the playerCreated event to initialize the score text
        GameManager.Instance.playerCreated += CreateScoreText;
    }

    void Update()
    {
        // Optionally update the score text here if needed (not required if only updating after events)
    }
    #endregion

    #region PrivateMethods
    private void CreateScoreText(Player player)
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
