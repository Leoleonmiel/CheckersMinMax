using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BoardHandler : MonoBehaviour
{
    #region Fields
    [SerializeField] private Transform boardStartPosition;
    [SerializeField] private Square squarePrefab;

    [SerializeField] private Material blackCheckerMaterial, whiteCheckerMaterial;
    [SerializeField] private Checker checkerPrefab;
    [SerializeField] private float moveAnimationTime = 0.5f;

    private Checker selectedChecker;
    private List<Square> validMoves = new();
    private List<Square> squareList = new();
    private List<Checker> highlightedCheckers = new();  // Track the currently highlighted checkers

    private int boardSize = 8;
    float squareSize;

    //public event Action<Player> checkerLost;
    #endregion

    #region UnityMessages
    void Awake()
    {
        if (!CalculateSquareSize()) { return; }

        for (int row = 0; row < boardSize; row++)
        {
            for (int col = 0; col < boardSize; col++)
            {
                Vector3 newPosition = CalculateSquarePosition(row, col);
                Square newSquare = Instantiate(squarePrefab, newPosition, Quaternion.identity, transform);

                int squareId = (row * boardSize) + col;
                newSquare.InitSquare(squareId, boardSize);

                Player player;
                if (newSquare.color == Square.Type.Black)
                {
                    if (row < 3 || row >= boardSize - 3)
                    {
                        if (row < 3)
                        {
                            player = GameManager.Instance.Player1;
                            player.type = Utils.Type.Black;
                        }
                        else
                        {
                            player = GameManager.Instance.Player2;
                            player.type = Utils.Type.White;
                        }
                        PlaceChecker(newPosition, row, col, ref player, ref newSquare);
                    }
                }
                squareList.Add(newSquare);
            }
        }

        HighlightCheckersThatCanMove();
    }

    void Update()
    {
        DetectSquareClick();
        DetectCheckerClick();
    }
    #endregion

    #region PrivateMethods
    private void PlaceChecker(Vector3 position, int row, int col, ref Player player, ref Square square)
    {
        Checker newChecker = Instantiate(checkerPrefab, position + Vector3.up * 0.1f, Quaternion.identity, player.transform);
        newChecker.Init(row * boardSize + col);
        newChecker.type = player.type;
        player.checkers.Add(newChecker);
        square.PlaceChecker(newChecker);
        newChecker.CurrentSquare = square;

        if (newChecker.meshRenderer != null)
        {
            newChecker.meshRenderer.material = (newChecker.type == Utils.Type.Black) ? blackCheckerMaterial : whiteCheckerMaterial;
        }
    }

    private void DetectCheckerClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Checker clickedChecker = hit.collider.GetComponent<Checker>();
                if (clickedChecker != null)
                {
                    OnCheckerClicked(clickedChecker);
                }
            }
        }
    }

    private void DetectSquareClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Square clickedSquare = hit.collider.GetComponent<Square>();
                if (clickedSquare != null)
                {
                    OnSquareClicked(clickedSquare);
                }
            }
        }
    }

    private void OnCheckerClicked(Checker checker)
    {
        if (!GameManager.Instance.CurrentPlayer.checkers.Contains(checker))
        {
            Debug.Log("❌ You cannot move the opponent's checkers!");
            return;
        }
        if (selectedChecker == checker)
        {
            DeselectChecker();
            return;
        }

        if (selectedChecker != null)
        {
            DeselectChecker();
            ClearHighlightedSquares();  // Clear previously highlighted squares
        }

        GameManager.Instance.SelectCheckerForPlayer(GameManager.Instance.CurrentPlayerID, checker);
        selectedChecker = checker;
        HighlightValidMoves(checker);
    }

    private void OnSquareClicked(Square square)
    {
        if (selectedChecker == null || !validMoves.Contains(square)) return;

        MoveChecker(selectedChecker, square);
    }

    private void MoveChecker(Checker checker, Square destination)
    {
        Square currentSquare = checker.CurrentSquare;
        currentSquare.RemoveChecker();

        // ✅ Determine if the move is a jump (capture)
        int rowDiff = Mathf.Abs(destination.row - currentSquare.row);
        if (rowDiff == 2) // Jump move
        {
            Square middleSquare = GetSquareAt((currentSquare.row + destination.row) / 2, (currentSquare.col + destination.col) / 2);
            if (middleSquare != null && middleSquare.HasChecker() && middleSquare.currentChecker.type != checker.type)
            {
                CaptureChecker(middleSquare.currentChecker); // ✅ Remove opponent's checker
            }
        }

        destination.PlaceChecker(checker);
        checker.CurrentSquare = destination;

        // ✅ Move the piece smoothly
        Vector3 targetPosition = destination.transform.position + Vector3.up * 0.1f;  // Ensure it's raised properly
        checker.transform.DOMove(targetPosition, moveAnimationTime)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                PromoteIfKing(checker);
                selectedChecker = null;
                ClearHighlightedSquares();
                HighlightChecker(checker);  // Reapply highlight after move
                SwitchTurn();
            });
    }

    private void HighlightChecker(Checker checker)
    {
        // Reapply highlight to the moved checker if it is the selected checker
        if (checker == selectedChecker)
        {
            checker.Highlight(true);  // Re-highlight the checker after it moves
        }
    }

    private void PromoteIfKing(Checker checker)
    {
        if ((checker.type == Utils.Type.Black && checker.CurrentSquare.row == boardSize - 1) ||
            (checker.type == Utils.Type.White && checker.CurrentSquare.row == 0))
        {
            checker.PromoteToKing();
        }
    }

    private void CaptureChecker(Checker capturedChecker)
    {
        Debug.Log($"Captured checker ID {capturedChecker.id}");

        Square checkerSquare = capturedChecker.CurrentSquare;
        checkerSquare.RemoveChecker();

        Player player = GameManager.Instance.Player1.checkers.Contains(capturedChecker) ? GameManager.Instance.Player1 : GameManager.Instance.Player2;
        player.checkers.Remove(capturedChecker);
        GameManager.Instance.CheckerLost?.Invoke(player);

        Destroy(capturedChecker.gameObject);
    }

    private void HighlightValidMoves(Checker checker)
    {
        validMoves.Clear();
        Square currentSquare = checker.CurrentSquare;
        int direction = (checker.type == Utils.Type.Black) ? 1 : -1;

        int[] colOffsets = { -1, 1 };
        foreach (int colOffset in colOffsets)
        {
            Square targetSquare = GetSquareAt(currentSquare.row + direction, currentSquare.col + colOffset);
            if (targetSquare != null && !targetSquare.HasChecker())
            {
                validMoves.Add(targetSquare);
                targetSquare.Highlight(true);
            }

            Square jumpSquare = GetSquareAt(currentSquare.row + 2 * direction, currentSquare.col + 2 * colOffset);
            Square middleSquare = GetSquareAt(currentSquare.row + direction, currentSquare.col + colOffset);

            if (middleSquare != null && middleSquare.HasChecker() &&
                middleSquare.currentChecker.type != checker.type && jumpSquare != null && !jumpSquare.HasChecker())
            {
                validMoves.Add(jumpSquare);
                jumpSquare.Highlight(true);
            }
        }

        if (checker.IsKing)
        {
            foreach (int colOffset in colOffsets)
            {
                Square backwardSquare = GetSquareAt(currentSquare.row - direction, currentSquare.col + colOffset);
                if (backwardSquare != null && !backwardSquare.HasChecker())
                {
                    validMoves.Add(backwardSquare);
                    backwardSquare.Highlight(true);
                }

                Square backwardJumpSquare = GetSquareAt(currentSquare.row - 2 * direction, currentSquare.col + 2 * colOffset);
                Square backwardMiddleSquare = GetSquareAt(currentSquare.row - direction, currentSquare.col + colOffset);

                if (backwardMiddleSquare != null && backwardMiddleSquare.HasChecker() &&
                    backwardMiddleSquare.currentChecker.type != checker.type && backwardJumpSquare != null && !backwardJumpSquare.HasChecker())
                {
                    validMoves.Add(backwardJumpSquare);
                    backwardJumpSquare.Highlight(true);
                }
            }
        }
    }

    private void ClearHighlightedSquares()
    {
        // Remove highlights from all squares
        foreach (Square square in validMoves)
        {
            square.Highlight(false);
        }
        validMoves.Clear();
    }

    private void ClearHighlightedCheckers()
    {
        // Only clear the highlighted checkers of the current player
        Player currentPlayer = GameManager.Instance.CurrentPlayer;

        foreach (Checker checker in highlightedCheckers)
        {
            checker.Highlight(false);  // Remove highlight from all checkers of the current player
        }

        highlightedCheckers.Clear();  // Clear the list of highlighted checkers
    }

    private Square GetSquareAt(int row, int col)
    {
        if (row < 0 || row >= boardSize || col < 0 || col >= boardSize)
            return null;

        return squareList[row * boardSize + col];
    }

    private void DeselectChecker()
    {
        if (selectedChecker != null)
        {
            selectedChecker.Deselect();
            selectedChecker = null;
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
        ClearHighlightedCheckers();  // Clear previously highlighted checkers

        Player currentPlayer = GameManager.Instance.CurrentPlayer;
        foreach (Checker checker in currentPlayer.checkers)
        {
            if (CanMove(checker))
            {
                checker.Highlight(true);  // Highlight checkers that can move
                highlightedCheckers.Add(checker);  // Keep track of highlighted checkers
            }
        }
    }

    private bool CanMove(Checker checker)
    {
        List<Square> validMovesForChecker = GetValidMovesForChecker(checker);
        return validMovesForChecker.Count > 0;
    }

    private List<Square> GetValidMovesForChecker(Checker checker)
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

    #endregion

    // Handle player turn switching
    public void SwitchTurn()
    {
        GameManager.Instance.SwitchTurn();
        HighlightCheckersThatCanMove();
    }

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
        Handles.color = Color.red;
        Handles.Label(labelPosition, $"({column},{line})");
    }
}
