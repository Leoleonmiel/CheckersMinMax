using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CheckerHandler
{
    private List<Checker> highlightedCheckers = new List<Checker>();
    private List<Square> validMoves = new List<Square>(); 

    private Checker selectedChecker;
    private BoardHandler boardHandler;
    private Player currentPlayer;

    public CheckerHandler(BoardHandler boardHandler)
    {
        this.boardHandler = boardHandler;
        currentPlayer = GameManager.Instance.CurrentPlayer;
    }

    public void Update()
    {
        DetectCheckerClick();
        DetectSquareClick();
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
        if (!currentPlayer.checkers.Contains(checker)){return;}

        if (selectedChecker == checker)
        {
            DeselectChecker();
            return;
        }

        if (selectedChecker != null)
        {
            DeselectChecker();
            ClearHighlightedSquares();
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

        int rowDiff = Mathf.Abs(destination.row - currentSquare.row);
        if (rowDiff == 2) 
        {
            Square middleSquare = boardHandler.GetSquareAt((currentSquare.row + destination.row) / 2, (currentSquare.col + destination.col) / 2);
            if (middleSquare != null && middleSquare.HasChecker() && middleSquare.currentChecker.type != checker.type)
            {
                CaptureChecker(middleSquare.currentChecker); 
            }
        }

        destination.PlaceChecker(checker);
        checker.CurrentSquare = destination;

        Vector3 targetPosition = destination.transform.position + Vector3.up * 0.1f;
        checker.transform.DOMove(targetPosition, boardHandler.moveAnimationTime)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                PromoteIfKing(checker);
                selectedChecker = null;
                ClearHighlightedSquares();
                HighlightChecker(checker);
                boardHandler.SwitchTurn();
            });
    }

    private void PromoteIfKing(Checker checker)
    {
        if ((checker.type == Utils.Type.Black && checker.CurrentSquare.row == boardHandler.BoardSize - 1) ||
            (checker.type == Utils.Type.White && checker.CurrentSquare.row == 0))
        {
            checker.PromoteToKing();
        }
    }

    private void CaptureChecker(Checker capturedChecker)
    {
        Square checkerSquare = capturedChecker.CurrentSquare;
        checkerSquare.RemoveChecker();

        Player player = GameManager.Instance.Player1.checkers.Contains(capturedChecker) ? GameManager.Instance.Player1 : GameManager.Instance.Player2;
        player.checkers.Remove(capturedChecker);
        GameManager.Instance.CheckerLost?.Invoke(player);

        Object.Destroy(capturedChecker.gameObject);
    }

    private void HighlightValidMoves(Checker checker)
    {
        validMoves.Clear();
        Square currentSquare = checker.CurrentSquare;
        int direction = (checker.type == Utils.Type.Black) ? (int)Utils.Direction.Up : (int)Utils.Direction.Down;

        int[] colOffsets = { (int)Utils.Direction.Left, (int)Utils.Direction.Right };
        foreach (int colOffset in colOffsets)
        {
            Square targetSquare = boardHandler.GetSquareAt(currentSquare.row + direction, currentSquare.col + colOffset);
            if (targetSquare != null && !targetSquare.HasChecker())
            {
                validMoves.Add(targetSquare);
                targetSquare.Highlight(true);
            }

            Square jumpSquare = boardHandler.GetSquareAt(currentSquare.row + 2 * direction, currentSquare.col + 2 * colOffset);
            Square middleSquare = boardHandler.GetSquareAt(currentSquare.row + direction, currentSquare.col + colOffset);

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
                Square backwardSquare = boardHandler.GetSquareAt(currentSquare.row - direction, currentSquare.col + colOffset);
                if (backwardSquare != null && !backwardSquare.HasChecker())
                {
                    validMoves.Add(backwardSquare);
                    backwardSquare.Highlight(true);
                }

                Square backwardJumpSquare = boardHandler.GetSquareAt(currentSquare.row - 2 * direction, currentSquare.col + 2 * colOffset);
                Square backwardMiddleSquare = boardHandler.GetSquareAt(currentSquare.row - direction, currentSquare.col + colOffset);

                if (backwardMiddleSquare != null && backwardMiddleSquare.HasChecker() &&
                    backwardMiddleSquare.currentChecker.type != checker.type && backwardJumpSquare != null && !backwardJumpSquare.HasChecker())
                {
                    validMoves.Add(backwardJumpSquare);
                    backwardJumpSquare.Highlight(true);
                }
            }
        }
    }

    private void DeselectChecker()
    {
        if (selectedChecker != null)
        {
            selectedChecker.Deselect();
            selectedChecker = null;
        }
    }

    private void ClearHighlightedSquares()
    {
        foreach (Square square in validMoves)
        {
            square.Highlight(false);
        }
        validMoves.Clear();
    }

    private void HighlightChecker(Checker checker)
    {
        if (checker == selectedChecker)
        {
            checker.Highlight(true);
        }
    }

    public void ClearHighlightedCheckers()
    {
        foreach (Checker checker in highlightedCheckers)
        {
            checker.Highlight(false);
        }
        highlightedCheckers.Clear();
    }

    public void SetCurrentPlayer(Player player)
    {
        currentPlayer = player;
    }

    // Add highlighted checker to the list
    public void AddHighlightedChecker(Checker checker)
    {
        if (!highlightedCheckers.Contains(checker))
        {
            highlightedCheckers.Add(checker);
        }
    }
}
