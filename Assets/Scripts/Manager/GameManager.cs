using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Fields
    [Header("Players Settings:")]
    [SerializeField] private Player playerPrefab;
    private List<Player> players = new();
    private int nbOfPlayers = 2;

    private Dictionary<int, Checker> selectedCheckers = new();
    private int currentPlayerID = 0;

    public event Action<Player> playerCreated;

    #endregion

    #region Properties
    public Player Player1 => players.First();
    public Player Player2 => players.Last();
    public Player CurrentPlayer => players[currentPlayerID];
    public int CurrentPlayerID => currentPlayerID;
    #endregion


    #region UnityMessages
    protected override void Awake()
    {
        base.Awake();
        InitPlayers();
    }

    private void Start()
    {
        InitPlayerScore();
        currentPlayerID = 0;
    }
    #endregion

    #region PublicMethods
    public void SelectCheckerForPlayer(int playerID, Checker checker)
    {
        if (playerID != currentPlayerID) { return; } 
        if (playerID < 0 || playerID >= players.Count) { return; }

        Player player = players[playerID];
        if (!player.checkers.Contains(checker)) { return; }

        if (selectedCheckers.TryGetValue(playerID, out Checker previousChecker))
        {
            previousChecker.Deselect();
        }

        checker.Select();
        selectedCheckers[playerID] = checker;
    }

    public void SwitchTurn()
    {
        currentPlayerID = (currentPlayerID + 1) % nbOfPlayers;
        CameraManager.Instance.SwitchToPlayerView(currentPlayerID);
        Debug.Log($"Turn switched! Now it's Player {currentPlayerID}'s turn.");
    }

    #endregion

    #region PrivateMethods
    private void InitPlayers()
    {
        for (int ID = 0; ID < nbOfPlayers; ID++)
        {
            Player newPlayer = Instantiate(playerPrefab, transform);
            newPlayer.Init((Utils.PlayerID)ID);
            players.Add(newPlayer);
        }
    }

    private void InitPlayerScore()
    {
        foreach (Player player in players)
        {
            playerCreated?.Invoke(player);
        }
    }

    #endregion
}
