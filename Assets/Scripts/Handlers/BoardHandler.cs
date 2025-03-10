using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardHandler : MonoBehaviour
{
    #region Fields
    [SerializeField] private Transform boardStartPosition;
    [SerializeField] private Square squarePrefab;
    [SerializeField] private Material blackCheckerMaterial, whiteCheckerMaterial;
    [SerializeField] private Checker checkerPrefab;
    [SerializeField] public float moveAnimationTime = 0.5f;

    private List<Square> squareList = new List<Square>();
    private CheckerHandler checkerHandler; 

    private int boardSize = 8;
    private float squareSize;

    [SerializeField, Tooltip("Control number of rows with pieces")]
    private int nbOfStartingRows = 3;

    [SerializeField, Tooltip("Starting row offset for checkers")]
    private int startingRowOffset = 0;
    public int BoardSize => boardSize;
    #endregion

    #region UnityMessages
    void Awake()
    {
        if (!CalculateSquareSize()) return;

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                Vector3 newPosition = CalculateSquarePosition(row, col);
                Square newSquare = Instantiate(squarePrefab, newPosition, Quaternion.identity, transform);

                int squareId = (row * boardSize) + col;
                newSquare.InitSquare(squareId, boardSize);
                squareList.Add(newSquare);

                if (newSquare.color == Square.Type.Black)
                {
                    if ((row >= startingRowOffset && row < startingRowOffset + nbOfStartingRows) ||
                        (row >= boardSize - nbOfStartingRows - startingRowOffset && row < boardSize - startingRowOffset))
                    {
                        Player player;
                        if (row < boardSize / 2)
                        {
                            player = GameManager.Instance.Player1;
                            player.color = Utils.Type.Black;
                        }
                        else
                        {
                            player = GameManager.Instance.Player2;
                            player.color = Utils.Type.White;
                        }

                        PlaceChecker(newPosition, row, col, ref player, ref newSquare);
                    }
                }
            }
        }

        checkerHandler = new CheckerHandler(this);
        HighlightCheckersThatCanMove();
    }

    void Update()
    {
        checkerHandler.Update();
    }

    #endregion

    #region PrivateMethods

    private void PlaceChecker(Vector3 position, int row, int col, ref Player player, ref Square square)
    {
        Checker newChecker = Instantiate(checkerPrefab, position + Vector3.up * 0.1f, Quaternion.identity, player.transform);
        newChecker.Init(row * boardSize + col);
        newChecker.type = player.color;
        player.checkers.Add(newChecker);
        square.PlaceChecker(newChecker);
        newChecker.CurrentSquare = square;

        if (newChecker.meshRenderer != null)
        {
            newChecker.meshRenderer.material = (newChecker.type == Utils.Type.Black) ? blackCheckerMaterial : whiteCheckerMaterial;
        }
    }

    private bool CalculateSquareSize()
    {
        BoxCollider squareCollider = squarePrefab.GetComponent<BoxCollider>();
        if (squareCollider != null)
        {
            squareSize = squareCollider.size.x;
            return true;
        }
        else
        {
            Debug.LogError("Square prefab is missing a BoxCollider!");
            return false;
        }
    }

    private Vector3 CalculateSquarePosition(int row, int col)
    {
        return new Vector3(
            boardStartPosition.position.x + (col * squareSize),
            boardStartPosition.position.y,
            boardStartPosition.position.z + (row * squareSize)
        );
    }

    private void HighlightCheckersThatCanMove()
    {
        checkerHandler.ClearHighlightedSelectedCheckers(); 
        Player currentPlayer = GameManager.Instance.CurrentPlayer;
        foreach (Checker checker in currentPlayer.checkers)
        {
            if (CanMove(checker))
            {
                checker.Highlight(true); 
                checkerHandler.AddHighlightedChecker(checker); 
            }
        }
    }

    public bool CanMove(Checker checker)
    {
        List<Square> validMovesForChecker = GetValidMoves(checker);
        return validMovesForChecker.Count > 0;
    }

    private List<Square> GetValidMoves(Checker checker)
    {
        List<Square> validMovesForChecker = new List<Square>();
        Square currentSquare = checker.CurrentSquare;
        int direction = (checker.type == Utils.Type.Black) ? 1 : -1;

        int[] colOffsets = { -1, 1 };
        foreach (int colOffset in colOffsets)
        {
            Square targetSquare = GetSquareAt(currentSquare.row + direction, currentSquare.col + colOffset);
            if (targetSquare != null && !targetSquare.HasChecker())
            {
                validMovesForChecker.Add(targetSquare);
            }

            // Jump move (capture)
            Square jumpSquare = GetSquareAt(currentSquare.row + 2 * direction, currentSquare.col + 2 * colOffset);
            Square middleSquare = GetSquareAt(currentSquare.row + direction, currentSquare.col + colOffset);

            if (middleSquare != null && middleSquare.HasChecker() &&
                middleSquare.currentChecker.type != checker.type && jumpSquare != null && !jumpSquare.HasChecker())
            {
                validMovesForChecker.Add(jumpSquare);
            }
        }

        if (checker.IsKing)
        {
            foreach (int colOffset in colOffsets)
            {
                Square backwardSquare = GetSquareAt(currentSquare.row - direction, currentSquare.col + colOffset);
                if (backwardSquare != null && !backwardSquare.HasChecker())
                {
                    validMovesForChecker.Add(backwardSquare);
                }

                // Backward jump move (capture)
                Square backwardJumpSquare = GetSquareAt(currentSquare.row - 2 * direction, currentSquare.col + 2 * colOffset);
                Square backwardMiddleSquare = GetSquareAt(currentSquare.row - direction, currentSquare.col + colOffset);

                if (backwardMiddleSquare != null && backwardMiddleSquare.HasChecker() &&
                    backwardMiddleSquare.currentChecker.type != checker.type && backwardJumpSquare != null && !backwardJumpSquare.HasChecker())
                {
                    validMovesForChecker.Add(backwardJumpSquare);
                }
            }
        }

        return validMovesForChecker;
    }

    public Square GetSquareAt(int row, int col)
    {
        if (row < 0 || row >= boardSize || col < 0 || col >= boardSize)
            return null;

        return squareList[row * boardSize + col];
    }

    public void SwitchTurn()
    {
        GameManager.Instance.SwitchTurn();
        checkerHandler.SetCurrentPlayer(GameManager.Instance.CurrentPlayer);
        HighlightCheckersThatCanMove();
    }

    #endregion

    #region DebugMethods
    void OnDrawGizmos()
    {
        if (squarePrefab == null || boardStartPosition == null) { return; }
        if (!Application.isPlaying) { return; }
        if (!CalculateSquareSize()) { return; }

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                Vector3 squarePosition = CalculateSquarePosition(row, col);
                DrawSquareLabel(squarePosition, row, col);
            }
        }
    }

    private void DrawSquareLabel(Vector3 position, int line, int column)
    {
        Vector3 labelPosition = position + Vector3.up * 0.5f;
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.Label(labelPosition, $"({column},{line})");
    }
    #endregion
}

