using ChessChallenge.API;
using System;
using System.Diagnostics;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        int[] pieceValues = { 0, 1, 3, 3, 5, 9, 0 };
        decimal eval = 0;
        int[] pawnTable = {0,  0,  0,  0,  0,  0,  0,  0,
50, 50, 50, 50, 50, 50, 50, 50,
10, 10, 20, 30, 30, 20, 10, 10,
 5,  5, 10, 25, 25, 10,  5,  5,
 0,  0,  0, 20, 20,  0,  0,  0,
 5, -5,-10,  0,  0,-10, -5,  5,
 5, 10, 10,-20,-20, 10, 10,  5,
 0,  0,  0,  0,  0,  0,  0,  0};
        int[] knightTable = {-50,-40,-30,-30,-30,-30,-40,-50,
-40,-20,  0,  0,  0,  0,-20,-40,
-30,  0, 10, 15, 15, 10,  0,-30,
-30,  5, 15, 20, 20, 15,  5,-30,
-30,  0, 15, 20, 20, 15,  0,-30,
-30,  5, 10, 15, 15, 10,  5,-30,
-40,-20,  0,  5,  5,  0,-20,-40,
-50,-40,-30,-30,-30,-30,-40,-50};
        int[] bishopTable = {-20,-10,-10,-10,-10,-10,-10,-20,
-10,  0,  0,  0,  0,  0,  0,-10,
-10,  0,  5, 10, 10,  5,  0,-10,
-10,  5,  5, 10, 10,  5,  5,-10,
-10,  0, 10, 10, 10, 10,  0,-10,
-10, 10, 10, 10, 10, 10, 10,-10,
-10,  5,  0,  0,  0,  0,  5,-10,
-20,-10,-10,-10,-10,-10,-10,-20};
        int[] rookTable = {0,  0,  0,  0,  0,  0,  0,  0,
  5, 10, 10, 10, 10, 10, 10,  5,
 -5,  0,  0,  0,  0,  0,  0, -5,
 -5,  0,  0,  0,  0,  0,  0, -5,
 -5,  0,  0,  0,  0,  0,  0, -5,
 -5,  0,  0,  0,  0,  0,  0, -5,
 -5,  0,  0,  0,  0,  0,  0, -5,
  0,  0,  0,  5,  5,  0,  0,  0};
        int[] queenTable = {-20,-10,-10, -5, -5,-10,-10,-20,
-10,  0,  0,  0,  0,  0,  0,-10,
-10,  0,  5,  5,  5,  5,  0,-10,
 -5,  0,  5,  5,  5,  5,  0, -5,
  0,  0,  5,  5,  5,  5,  0, -5,
-10,  5,  5,  5,  5,  5,  0,-10,
-10,  0,  5,  0,  0,  0,  0,-10,
-20,-10,-10, -5, -5,-10,-10,-20};
        int[] kingTableMid = {-30,-40,-40,-50,-50,-40,-40,-30,
-30,-40,-40,-50,-50,-40,-40,-30,
-30,-40,-40,-50,-50,-40,-40,-30,
-30,-40,-40,-50,-50,-40,-40,-30,
-20,-30,-30,-40,-40,-30,-30,-20,
-10,-20,-20,-20,-20,-20,-20,-10,
 20, 20,  0,  0,  0,  0, 20, 20,
 20, 30, 10,  0,  0, 10, 30, 20};
        int[] kingTableEnd = {-50,-40,-30,-20,-20,-30,-40,-50,
-30,-20,-10,  0,  0,-10,-20,-30,
-30,-10, 20, 30, 30, 20,-10,-30,
-30,-10, 30, 40, 40, 30,-10,-30,
-30,-10, 30, 40, 40, 30,-10,-30,
-30,-10, 20, 30, 30, 20,-10,-30,
-30,-30,  0,  0,  0,  0,-30,-30,
-50,-30,-30,-30,-30,-30,-30,-50};
        int minorPiecesAmount = BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard) - BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Pawn, true)) - BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Pawn, true));
        Move bestRootMove = new();
        int depthdepth = 3;
        int quidepth = 2;
        int movestoconsider = 0;
        int movestoconsider2 = 0;
        float score = new();

        float alphabeta(Board board, int depth, float alpha, float beta)
        {
            if (depth == 0) return Quiesce(alpha, beta, board, quidepth);
            foreach (Move move in board.GetLegalMoves())
            {
                board.MakeMove(move);
                if (board.IsInCheckmate())
                {
                    if (depth == depthdepth)
                    {
                        bestRootMove = move;
                        return float.MaxValue;
                    }
                    score = float.MaxValue;
                }
                else
                {
                    if (board.IsDraw())
                    {
                        score = 0;
                    }
                    else
                    {
                    score = -alphabeta(board, depth - 1, -beta, -alpha);
                    }
                }
                board.UndoMove(move);
                movestoconsider++;
                if (score >= beta)
                {
                    return beta;
                }
                if (score > alpha)
                {
                    alpha = score;
                    if (depth == depthdepth) bestRootMove = move;
                }
            }
            return alpha;
        }

        float evaluation(Board board)   
        {
            eval = 0;
            PieceList[] pieceList = board.GetAllPieceLists();
            foreach (PieceList pieceList2 in pieceList)
            {
                for (int i = 0; i < pieceList2.Count; i++)
                {
                    if (pieceList2.IsWhitePieceList)
                    {
                        eval += pieceValues[(int)pieceList2.GetPiece(i).PieceType];
                        switch ((int)pieceList2.GetPiece(i).PieceType)
                        {
                            case 1:
                                eval += pawnTable[63 - pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 2:
                                eval += knightTable[63 - pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 3:
                                eval += bishopTable[63 - pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 4:
                                eval += rookTable[63 - pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 5:
                                eval += queenTable[63 - pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 6:
                                if (minorPiecesAmount <= 4)
                                {
                                    eval += kingTableEnd[63 - pieceList2.GetPiece(i).Square.Index] / 100m;
                                } else
                                {
                                    eval += kingTableMid[63 - pieceList2.GetPiece(i).Square.Index] / 100m;
                                }
                                break;
                        }
                    }
                    else
                    {
                        eval -= pieceValues[(int)pieceList2.GetPiece(i).PieceType];
                        switch ((int)pieceList2.GetPiece(i).PieceType)
                        {
                            case 1:
                                eval -= pawnTable[pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 2:
                                eval -= knightTable[pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 3:
                                eval -= bishopTable[pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 4:
                                eval -= rookTable[pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 5:
                                eval -= queenTable[pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                            case 6:
                                if (minorPiecesAmount <= 4)
                                {
                                    eval -= kingTableEnd[pieceList2.GetPiece(i).Square.Index] / 100m;
                                } else
                                {
                                    eval -= kingTableMid[pieceList2.GetPiece(i).Square.Index] / 100m;
                                }
                                break;
                        }
                    }
                }
                /*if (pieceList2.IsWhitePieceList)
                {
                    eval += pieceList2.Count * values[(int)pieceList2.TypeOfPieceInList];
                }
                else
                {
                    eval -= pieceList2.Count * values[(int)pieceList2.TypeOfPieceInList];
                }*/

            }
            if (!board.IsWhiteToMove)
            {
                eval = -1 * eval;
            }
            return (float)eval;
        }
        float Quiesce(float alpha, float beta, Board board, int depth)
        {
            float stand_pat = evaluation(board);
            if (stand_pat >= beta) return beta;
            if (stand_pat > alpha) alpha = stand_pat;
            if (depth == 0) return alpha;
            foreach(Move move in board.GetLegalMoves(true))
            {
                if (board.IsRepeatedPosition())
                {
                    board.UndoMove(move);
                    continue;
                }
                board.MakeMove(move);
                movestoconsider2++;
                float score = -Quiesce(-beta, -alpha, board, depth - 1);
                board.UndoMove(move);
                if (score >= beta) return beta;
                if (score > alpha) alpha = score;
            }
            return alpha;
            
        }
        if (BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard) < 10)
        {
            quidepth = 10;
            depthdepth = 6;
        }
        Console.WriteLine("Quiesce Eval: " + alphabeta(board, depthdepth, float.NegativeInfinity, float.PositiveInfinity));
        Console.WriteLine("alphabeta: " + movestoconsider);
        Console.WriteLine("quiesce: " + movestoconsider2);
        return bestRootMove;
    }
}