using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum State
    {
        PlayerVsPlayer,
        PlayerVsAI,
        Win,
        Loose
    }

    public State currentState;

    // The current game mode, this will be set based on user input or game logic
    private void Start()
    {
        // Initialize with the default state, you could set this dynamically (e.g., through a menu)
        currentState = State.PlayerVsPlayer;
    }

    private void Update()
    {
        // Handle state transitions in the update method or via event triggers
        HandleState();
    }

    // Main FSM logic to handle transitions based on game state
    private void HandleState()
    {
        switch (currentState)
        {
            case State.PlayerVsPlayer:
                Debug.Log("Current Game Mode: Player vs Player");
                // Player vs Player specific logic
                // Check for player actions, and then move to next state if needed (e.g., if the game ends)
                break;

            case State.PlayerVsAI:
                Debug.Log("Current Game Mode: Player vs AI");
                // Player vs AI specific logic
                // Check for AI move and player move, then determine the result and move to the next state if needed
                break;

            case State.Win:
                Debug.Log("Game Over: You Win!");
                // Trigger win logic (e.g., display win screen, stop game)
                // Optionally, you could add a restart option to return to PlayerVsPlayer or PlayerVsAI
                break;

            case State.Loose:
                Debug.Log("Game Over: You Lose!");
                // Trigger lose logic (e.g., display lose screen, stop game)
                // Optionally, you could add a restart option to return to PlayerVsPlayer or PlayerVsAI
                break;

            default:
                Debug.LogError("Invalid Game State");
                break;
        }
    }

    // Switch to Player vs Player mode
    public void SetPlayerVsPlayer()
    {
        if (currentState != State.Win && currentState != State.Loose)  // Ensure game isn't over
        {
            currentState = State.PlayerVsPlayer;
            // Additional logic for initializing a Player vs Player game
        }
    }

    // Switch to Player vs AI mode
    public void SetPlayerVsAI()
    {
        if (currentState != State.Win && currentState != State.Loose)  // Ensure game isn't over
        {
            currentState = State.PlayerVsAI;
            // Additional logic for initializing a Player vs AI game
        }
    }

    // Set the game state to win
    public void SetWin()
    {
        currentState = State.Win;
        // Additional logic for handling the win state (e.g., showing win message)
    }

    // Set the game state to loose
    public void SetLoose()
    {
        currentState = State.Loose;
        // Additional logic for handling the loose state (e.g., showing lose message)
    }
}
