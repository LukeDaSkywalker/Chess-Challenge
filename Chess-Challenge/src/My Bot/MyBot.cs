using ChessChallenge.API;
using System;
using System.Linq;

public class MyBot : IChessBot
{
    Transposition[] tpt = new Transposition[0x7FFFFF + 1];
    struct Transposition
    {
        public ulong zobristHash;
        public float evaluation;
        public sbyte depth;
        public byte flag;
    }
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
        int depthdepth = 0;
        float score = new();
        int endKingTableAmount = 6;
        int evaluatedPos = 0;


        float alphabeta(Board board, int depth, float alpha, float beta, bool root)
        {
            evaluatedPos++;
            float startingAlpha = alpha;
            float bestEval = float.MinValue;
            ref Transposition tp = ref tpt[board.ZobristKey & 0x7FFFFF];

            if (!root && tp.zobristHash == board.ZobristKey && tp.depth >= depth)
            {
                if (tp.flag == 1 || tp.flag == 2 && tp.evaluation >= beta || tp.flag == 3 && tp.evaluation <= alpha)
                {
                    return tp.evaluation;
                }
            }
            if (depth == 0) return Quiesce(alpha, beta, board);
            foreach (Move move in board.GetLegalMoves().OrderByDescending(move => (move == bestRootMove, move.CapturePieceType, move.PromotionPieceType - move.MovePieceType)))
            {
                board.MakeMove(move);
                if (board.IsInCheckmate())
                {
                    if (depth == depthdepth)
                    {
                        bestRootMove = move;
                        Console.WriteLine(bestRootMove);
                        Convert.ToUInt32(-1);
                    }
                    score = evaluation(board, depth);
                }
                else if (board.IsDraw())
                {
                    score = 0;
                }
                else
                {
                    score = -alphabeta(board, depth - 1, -beta, -alpha, false);

                }
                board.UndoMove(move);
                if (score > bestEval) bestEval = score;
                if (score >= beta)
                {
                    return beta;
                }
                if (score > alpha)
                {
                    alpha = score;
                    if (depth == depthdepth)
                    {
                        bestRootMove = move;
                    }
                }
                if (!bestRootMove.IsNull)
                    Convert.ToUInt32(timer.MillisecondsRemaining - 30 * timer.MillisecondsElapsedThisTurn);
            }
            tp.evaluation = bestEval;
            tp.zobristHash = board.ZobristKey;
            if (alpha <= startingAlpha)
                tp.flag = 3; //upper bound
            else if (alpha >= beta)
                tp.flag = 2; //lower bound
            else tp.flag = 1;
            tp.depth = (sbyte)depth;
            return alpha;
        }

        float evaluation(Board board, int depth)
        {
            eval = 0;
            if (board.IsInCheckmate())
            {
                eval = 1000000 - (depthdepth - depth);
            }
            else if (board.IsDraw())
            {
                eval = 0;
            }
            else
            {
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
                                    if (minorPiecesAmount <= endKingTableAmount)
                                    {
                                        eval += kingTableEnd[63 - pieceList2.GetPiece(i).Square.Index] / 100m;
                                    }
                                    else
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
                                    if (minorPiecesAmount <= endKingTableAmount)
                                    {
                                        eval -= kingTableEnd[pieceList2.GetPiece(i).Square.Index] / 100m;
                                    }
                                    else
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
                    eval *= -1;
            }
            /*if (minorPiecesAmount <= 4)
            {
                Square whiteKingSquare = board.GetKingSquare(true);
                Square blackKingSquare = board.GetKingSquare(false);
                int whiteKingDisToCenterFile = Math.Max(3 - whiteKingSquare.File, whiteKingSquare.File - 4);
                int whiteKingDisToCenterRank = Math.Max(3 - whiteKingSquare.Rank, whiteKingSquare.Rank - 4);
                int whiteKingDisFromCenter = whiteKingDisToCenterRank + whiteKingDisToCenterFile;
                int blackKingDisToCenterFile = Math.Max(3 - blackKingSquare.File, blackKingSquare.File - 4);
                int blackKingDisToCenterRank = Math.Max(3 - blackKingSquare.Rank, blackKingSquare.Rank - 4);
                int blackKingDisFromCenter = blackKingDisToCenterRank + blackKingDisToCenterFile;
                float kingEval = whiteKingDisFromCenter - blackKingDisFromCenter;
            }*/
            return (float)eval;
        }
        float Quiesce(float alpha, float beta, Board board)
        {
            float stand_pat = evaluation(board, 0);
            if (stand_pat >= beta) return beta;
            if (stand_pat > alpha) alpha = stand_pat;
            foreach (Move move in board.GetLegalMoves(true))
            {
                board.MakeMove(move);
                float score = -Quiesce(-beta, -alpha, board);
                board.UndoMove(move);
                if (score >= beta) return beta;
                if (score > alpha) alpha = score;
            }
            return alpha;

        }

        Console.WriteLine("V1.7");
        try
        {
            for (; ; )
            {
                alphabeta(board, ++depthdepth, float.NegativeInfinity, float.PositiveInfinity, true);
                //Console.WriteLine(depthdepth);
            }
        }
        catch
        {
            //Console.WriteLine(evaluatedPos);
            //Console.WriteLine(depthdepth);
            return bestRootMove;
        }
    }
}