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
    private Utils.PlayerID currentPlayerID;

    public event Action<Player> PlayerCreated;
    public event Action<Utils.PlayerID> PlayerSwitched;
    public Action<Player> CheckerLost;

    #endregion

    #region Properties
    public Player Player1 => players.First();
    public Player Player2 => players.Last();
    public Player CurrentPlayer => players[(int)currentPlayerID];
    public Utils.PlayerID CurrentPlayerID => currentPlayerID;
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
        currentPlayerID = Utils.PlayerID.Player1;
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
        PlayerSwitched?.Invoke(currentPlayerID);
        //CameraManager.Instance.SwitchToPlayerView((int)currentPlayerID);
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
            PlayerCreated?.Invoke(player);
            Debug.Log(player);
        }
    }

    #endregion
}
