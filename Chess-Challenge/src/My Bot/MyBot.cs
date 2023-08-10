using ChessChallenge.API;
using System;
using System.Diagnostics;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] EveryMove = board.GetLegalMoves();
        Move[] captureMoves = board.GetLegalMoves(true);
        System.Random rnd = new();
        int depth = 0;
        float bestEvaluation = -100;
        int[] pieceValues = { 0, 1, 3, 3, 5, 9, 0 };
        Move evaluationMove = Move.NullMove;
        foreach (Move move in EveryMove)
        {
            //depth = 0;
            if (huntCheckmate(board, move))
            {
                return move;
            }
        }
        foreach (Move move in captureMoves)
        {
            if (bestEvaluation < evaluation(board, move))
            {
                bestEvaluation = evaluation(board, move);
                evaluationMove = move;
            }
        }

        if (!evaluationMove.IsNull)
        {
            return evaluationMove;
        }

        return EveryMove[rnd.Next(EveryMove.Length)];


        bool huntCheckmate(Board board, Move moveToCheck)
        {
            board.MakeMove(moveToCheck);
            bool isCheck = board.IsInCheck();
            depth++;
            bool isMate = board.IsInCheckmate();
            Trace.WriteLine
            /*if (isCheck && !isMate && depth < 4)
            {
                huntCheckmate(board, moveToCheck);
            }
            if (board.IsInCheck() && depth < 4)
            {
                foreach(Move move in board.GetLegalMoves())
                {
                    board.MakeMove(move);
                    foreach(Move move2 in board.GetLegalMoves())
                    {
                        if (huntCheckmate(board, move2))
                        {
                            board.UndoMove(move);
                            return true;
                        }
                    }
                    board.UndoMove(move);
                }
            }*/
            board.UndoMove(moveToCheck);
            depth--;
            return isMate;
        }


        float evaluation(Board board, Move moveToEvaluate)
        {
            bool attacked = board.SquareIsAttackedByOpponent(moveToEvaluate.TargetSquare);
            board.MakeMove(moveToEvaluate);
            board.UndoMove(moveToEvaluate);

            return pieceValues[(int)moveToEvaluate.CapturePieceType] - pieceValues[(int)moveToEvaluate.MovePieceType];
        }
    }
}