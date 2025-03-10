using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameHudHandler : MonoBehaviour
{
    #region Fields
    [SerializeField] TMP_Text player1Score;
    [SerializeField] TMP_Text player2Score;
    [SerializeField] GameObject EndScreenPanel;
    #endregion

    #region UnityMessages
    void Awake()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerCreated += ChangeScoreText;
            GameManager.Instance.CheckerLost += ChangeScoreText;
            GameManager.Instance.PlayerHasWon += ToggleEndScreen;
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
    public void ToggleEndScreen(Player player, bool showEndScreen)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject go = transform.GetChild(i).gameObject;
            if (go != EndScreenPanel)
            {
                go.SetActive(!showEndScreen); 
            }
        }

        if (EndScreenPanel != null)
        {
            EndScreenPanel.SetActive(showEndScreen);

            TMP_Text endScreenText = EndScreenPanel.GetComponentInChildren<TMP_Text>();
            if (endScreenText != null)
            {
                endScreenText.text = $"{player.name} Wins!";
            }
            else
            {
                Debug.LogError("No TMP_Text component found in EndScreenPanel!");
            }
        }
        else
        {
            Debug.LogError("EndScreenPanel is not assigned in the inspector!");
        }
    }

    #endregion
}
