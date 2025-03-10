﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    #region Fields
    [Header("Players Settings:")]
    [SerializeField] private Player playerPrefab;
    private BoardHandler boardHandler;
    private List<Player> players = new();
    private int nbOfPlayers = 2;

    private Dictionary<int, Checker> selectedCheckers = new();
    private Utils.PlayerID currentPlayerID;

    public event Action<Player> PlayerCreated;
    public event Action<Player> OnTurnSwitched;
    public Action<Player> CheckerLost;
    public Action<Player, bool> PlayerHasWon;

    private AIHandler aiHandler;
    #endregion

    #region Properties
    public Player Player1 => players.First();
    public Player Player2 => players.Last();
    public Player CurrentPlayer => players[(int)currentPlayerID];
    public Utils.PlayerID CurrentPlayerID => currentPlayerID;
    public AIHandler AIHandler => aiHandler;
    public Utils.AIDifficulty AIDifficulty => GameStateManager.Instance.aiDifficulty;
    #endregion


    #region UnityMessages
    protected override void Awake()
    {
        base.Awake();

        boardHandler = FindFirstObjectByType<BoardHandler>();
        if (GameStateManager.Instance != null)
        {
            switch (GameStateManager.Instance.currentState)
            {
                case GameStateManager.State.PlayerVsPlayer:
                    InitPlayersPvP();
                    break;
                case GameStateManager.State.PlayerVsAI:
                    InitPlayerVsAI();
                    if (boardHandler != null)
                    {
                        aiHandler = new AIHandler(boardHandler, AIDifficulty);
                    }
                    break;
                default:
                    Debug.LogError("No state given");
                    break;
            }
        }
    }

    private void Start()
    {
        InitPlayerScore();
        currentPlayerID = Utils.PlayerID.Player1;
    }

    private void Update()
    {

    }
    #endregion

    #region PublicMethods
    public void SelectCheckerForPlayer(Utils.PlayerID playerID, Checker checker)
    {
        if (playerID != currentPlayerID) { return; }
        if (playerID < 0 || (int)playerID >= players.Count) { return; }

        Player player = players[(int)playerID];
        if (!player.checkers.Contains(checker)) { return; }

        if (selectedCheckers.TryGetValue((int)playerID, out Checker previousChecker))
        {
            previousChecker.Deselect();
        }

        checker.Select();
        selectedCheckers[(int)playerID] = checker;
    }

    public void SwitchTurn()
    {
        currentPlayerID = currentPlayerID = (Utils.PlayerID)(((int)currentPlayerID + 1) % nbOfPlayers);
        OnTurnSwitched?.Invoke(CurrentPlayer);
    }

    #endregion

    #region PrivateMethods
    private void InitPlayersPvP()
    {
        for (int ID = 0; ID < nbOfPlayers; ID++)
        {
            Player newPlayer = Instantiate(playerPrefab, transform);
            newPlayer.Init((Utils.PlayerID)ID, Utils.PlayerType.Human);
            players.Add(newPlayer);
        }
    }

    private void InitPlayerVsAI()
    {
        // Create Player 1 (Human)
        Player player1 = Instantiate(playerPrefab, transform);
        player1.Init(Utils.PlayerID.Player1, Utils.PlayerType.Human);
        players.Add(player1);

        // Create Player 2 (AI)
        Player player2 = Instantiate(playerPrefab, transform);
        player2.Init(Utils.PlayerID.Player2, Utils.PlayerType.AI);
        players.Add(player2);
    }

    private void InitPlayerScore()
    {
        foreach (Player player in players)
        {
            PlayerCreated?.Invoke(player);
            Debug.Log(player);
        }
    }

    #endregion
}
