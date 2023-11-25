using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        int[] values = { 0, 1, 3, 3, 5, 9, 0 };
        PieceList[] pieceList = board.GetAllPieceLists();
        float eval = 0;
        float highestEval = 99;
        Move bestMove = moves[0];
        int[] pawnTable = {0,  0,  0,  0,  0,  0,  0,  0,
50, 50, 50, 50, 50, 50, 50, 50,
10, 10, 20, 30, 30, 20, 10, 10,
 5,  5, 10, 25, 25, 10,  5,  5,
 0,  0,  0, 20, 20,  0,  0,  0,
 5, -5,-10,  0,  0,-10, -5,  5,
 5, 10, 10,-20,-20, 10, 10,  5,
 0,  0,  0,  0,  0,  0,  0,  0};


        if (board.IsWhiteToMove)
        {
            highestEval = -99;
        }

        foreach (Move move in moves)
        {
            switch (board.IsWhiteToMove)
            {
                case true:
                    if (highestEval < evaluation(board, move))
                    {
                        highestEval = evaluation(board, move);
                        bestMove = move;
                    }
                    break;
                case false:
                    if (evaluation(board, move) < highestEval)
                    {
                        highestEval = evaluation(board, move);
                        bestMove = move;
                    }
                    break;
            }
        }

        float evaluation(Board board, Move moveToEvaluate)
        {
            board.MakeMove(moveToEvaluate);
            eval = 0;
            foreach (PieceList pieceList2 in pieceList)
            {
                /*if (pieceList2.IsWhitePieceList)
                {
                    eval += pieceList2.Count * values[(int)pieceList2.TypeOfPieceInList];
                }
                else
                {
                    eval -= pieceList2.Count * values[(int)pieceList2.TypeOfPieceInList];
                }*/
                
            }
            board.UndoMove(moveToEvaluate);
            return eval;
        }
        return bestMove;
    }
}