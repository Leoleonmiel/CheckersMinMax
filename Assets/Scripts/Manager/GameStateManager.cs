using UnityEngine;
using System;
using TMPro;

public class GameStateManager : Singleton<GameStateManager>
{
    #region Fields
    [Serializable]
    public enum State
    {
        Menu,
        PlayerVsPlayer,
        PlayerVsAI,
        AIVsAI,
    }

    public State currentState;
    public Utils.AIDifficulty aiDifficulty = Utils.AIDifficulty.Easy;

    [SerializeField] private TMP_Dropdown difficultyDropdown;
    #endregion

    #region UnityMessages
    private void Start()
    {
        currentState = State.Menu;

        if (difficultyDropdown != null)
        {
            difficultyDropdown.onValueChanged.AddListener(SetDifficultyFromDropdown);
            difficultyDropdown.value = (int)aiDifficulty; 
        }
    }
    #endregion

    #region PublicMethods
    public void SetDifficulty(Utils.AIDifficulty difficulty)
    {
        aiDifficulty = difficulty;
        Debug.Log($"AI Difficulty Set to: {aiDifficulty}");
    }

    private void SetDifficultyFromDropdown(int index)
    {
        aiDifficulty = (Utils.AIDifficulty)index;
        Debug.Log($"AI Difficulty Changed to: {aiDifficulty}");
    }
    #endregion
}