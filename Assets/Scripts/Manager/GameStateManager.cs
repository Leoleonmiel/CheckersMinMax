using UnityEngine;
using System;

public class GameStateManager : Singleton<GameStateManager>
{
    #region Fields
    [Serializable]
    public enum State
    {
        Menu,
        PlayerVsPlayer,
        PlayerVsAI,
    }

    public State currentState;
    #endregion

    #region UnityMessages
    private void Start()
    {
        currentState = State.Menu;
    }

    private void Update()
    {
       
    }
    #endregion
}