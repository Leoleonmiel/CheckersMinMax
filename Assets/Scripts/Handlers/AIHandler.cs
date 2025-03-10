using System;
using System.Collections.Generic;
using UnityEngine;

public class AIHandler
{
    #region Fields
    private int maxDepth;
    private readonly BoardHandler boardHandler;
    private int nodesExplored = 0;
    private (Checker, Square) previousMove = (null, null);
    private readonly Utils.AIDifficulty difficulty;

    private const int normalPieceValue = 5;
    private const int kingPieceValue = 10;
    private const int positionAdvanceBonus = 3;
    private const int endgameAgressionBonus = 8;

    #endregion

    #region PublicMethods
    public AIHandler(BoardHandler boardHandler, Utils.AIDifficulty difficulty)
    {
        this.boardHandler = boardHandler;
        this.difficulty = difficulty;

        switch (difficulty)
        {
            case Utils.AIDifficulty.Easy:
                maxDepth = 2;
                break;
            case Utils.AIDifficulty.Medium:
                maxDepth = 4;
                break;
            case Utils.AIDifficulty.Hard:
                maxDepth = 6;
                break;
            default:
                maxDepth = 4;
                break;
        }
    }

    public (Checker, Square) GetBestMove(Player aiPlayer)
    {
        nodesExplored = 0;
        int bestScore = int.MinValue;
        Checker bestChecker = null;
        Square bestSquare = null;
        List<(Checker, Square)> captureMoves = new List<(Checker, Square)>();

        foreach (Checker checker in aiPlayer.checkers)
        {
            List<Square> validMoves = boardHandler.GetValidMoves(checker);
            foreach (Square move in validMoves)
            {
                if (previousMove == (checker, move)) continue;

                int[,] boardState = ConvertBoardToMatrix(); 
                ApplyMove(boardState, checker, move);

                int score = Minimax(boardState, maxDepth, int.MinValue, int.MaxValue, false);

                if (IsCaptureMove(checker, move))
                {
                    captureMoves.Add((checker, move));
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestChecker = checker;
                    bestSquare = move;
                }
            }
        }

        if (captureMoves.Count > 0)
        {
            (bestChecker, bestSquare) = captureMoves[UnityEngine.Random.Range(0, captureMoves.Count)];
        }

        boardHandler.GameHudHandler.UpdateAIStats(nodesExplored, maxDepth, difficulty);
        previousMove = (bestChecker, bestSquare);

        return (bestChecker, bestSquare);
    }

    #endregion

    #region PrivateMethods
    private int Minimax(int[,] boardState, int depth, int alpha, int beta, bool isMaximizing)
    {
        if (depth == 0 || IsGameOver(boardState))
            return EvaluateBoard(boardState);

        nodesExplored++;

        if (isMaximizing)
        {
            int maxEval = int.MinValue;
            foreach ((Checker checker, Square square) in GenerateMoves(boardState, 1))
            {
                int[,] newBoard = CopyBoard(boardState);
                ApplyMove(newBoard, checker, square);
                int eval = Minimax(newBoard, depth - 1, alpha, beta, false);

                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);

                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach ((Checker checker, Square square) in GenerateMoves(boardState, 2))
            {
                int[,] newBoard = CopyBoard(boardState);
                ApplyMove(newBoard, checker, square);
                int eval = Minimax(newBoard, depth - 1, alpha, beta, true);

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);

                if (beta <= alpha) break; 
            }
            return minEval;
        }
    }

    private int EvaluateBoard(int[,] boardState)
    {
        int aiPieces = 0;
        int opponentPieces = 0;
        int score = 0;

        for (int row = 0; row < boardState.GetLength(0); row++)
        {
            for (int col = 0; col < boardState.GetLength(1); col++)
            {
                if (boardState[row, col] == 1) // AI pieces
                {
                    aiPieces++;
                    score += normalPieceValue;
                    if (row >= 5) score += positionAdvanceBonus;
                    if (IsKing(row, col)) score += kingPieceValue;
                }
                else if (boardState[row, col] == 2) // Opponent pieces
                {
                    opponentPieces++;
                    score -= normalPieceValue;
                    if (row <= 2) score -= positionAdvanceBonus;
                    if (IsKing(row, col)) score -= kingPieceValue;
                }
            }
        }

        if (aiPieces == 1 && opponentPieces > 1)
        {
            score += endgameAgressionBonus; 
        }

        return score;
    }

    private bool IsGameOver(int[,] boardState)
    {
        return GenerateMoves(boardState, 1).Count == 0 || GenerateMoves(boardState, 2).Count == 0;
    }

    private List<(Checker, Square)> GenerateMoves(int[,] boardState, int player)
    {
        List<(Checker, Square)> captureMoves = new List<(Checker, Square)>();
        List<(Checker, Square)> normalMoves = new List<(Checker, Square)>();

        foreach (Checker checker in (player == 1 ? GameManager.Instance.Player1.checkers : GameManager.Instance.Player2.checkers))
        {
            List<Square> validMoves = boardHandler.GetValidMoves(checker);
            foreach (Square move in validMoves)
            {
                if (IsCaptureMove(checker, move))
                {
                    captureMoves.Add((checker, move));
                }
                else
                {
                    normalMoves.Add((checker, move));
                }
            }
        }
        return captureMoves.Count > 0 ? captureMoves : normalMoves;
    }

    private bool IsCaptureMove(Checker checker, Square destination)
    {
        return Math.Abs(destination.row - checker.CurrentSquare.row) == 2;
    }

    private void ApplyMove(int[,] boardState, Checker checker, Square destination)
    {
        int oldRow = checker.CurrentSquare.row;
        int oldCol = checker.CurrentSquare.col;
        int newRow = destination.row;
        int newCol = destination.col;

        boardState[oldRow, oldCol] = 0;

        if (Math.Abs(newRow - oldRow) == 2)
        {
            int middleRow = (oldRow + newRow) / 2;
            int middleCol = (oldCol + newCol) / 2;
            boardState[middleRow, middleCol] = 0;
        }

        boardState[newRow, newCol] = (checker.type == Utils.Type.Black) ? 1 : 2;
    }

    private int[,] ConvertBoardToMatrix()
    {
        int[,] matrix = new int[8, 8];

        foreach (Square square in boardHandler.GetAllSquares())
        {
            if (square.HasChecker())
            {
                matrix[square.row, square.col] = (square.currentChecker.type == Utils.Type.Black) ? 1 : 2;
            }
        }
        return matrix;
    }

    private int[,] CopyBoard(int[,] boardState)
    {
        int[,] newBoard = new int[8, 8];
        Array.Copy(boardState, newBoard, boardState.Length);
        return newBoard;
    }

    private bool IsKing(int row, int col)
    {
        return (row == 7) || (row == 0);
    }
    #endregion
}
