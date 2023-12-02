﻿using ChessChallenge.API;
using Raylib_cs;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        int[] values = { 0, 1, 3, 3, 5, 9, 0 };
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
        int[] kingTable = {-30,-40,-40,-50,-50,-40,-40,-30,
-30,-40,-40,-50,-50,-40,-40,-30,
-30,-40,-40,-50,-50,-40,-40,-30,
-30,-40,-40,-50,-50,-40,-40,-30,
-20,-30,-30,-40,-40,-30,-30,-20,
-10,-20,-20,-20,-20,-20,-20,-10,
 20, 20,  0,  0,  0,  0, 20, 20,
 20, 30, 10,  0,  0, 10, 30, 20};
        float best = 0;

        Move minimax(Board board, int depth, Move theMove)
        {
            if (depth == 0)
            {
                return ;
            }
            if (board.IsWhiteToMove)
            {
                float highestEval = -1000;
                foreach(Move move in board.GetLegalMoves())
                {
                    board.MakeMove(move);
                    float val = minimax(board, depth - 1);
                    if(val > best)
                    {
                        best = val;
                    }
                }
            }
            else
            {
                float highestEval = 1000;
            }
            return;
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
                        eval += values[(int)pieceList2.GetPiece(i).PieceType];
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
                                eval += kingTable[63 - pieceList2.GetPiece(i).Square.Index] / 100m;
                                break;
                        }
                    }
                    else
                    {
                        eval -= values[(int)pieceList2.GetPiece(i).PieceType];
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
                                eval -= kingTable[pieceList2.GetPiece(i).Square.Index] / 100m;
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
            return (float)eval;
        }
        return;
    }
}