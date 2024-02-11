using ChessChallenge.API;
using System;
using System.Linq;

public class MyBot : IChessBot
{
    Transposition[] tpt = new Transposition[0x3FFFFFF + 1];
    struct Transposition
    {
        public ulong zobristHash;
        public int evaluation;
        public sbyte depth;
        public byte flag;
    }
    readonly int[] mg_value = { 0, 82, 337, 365, 477, 1025, 0 };
    readonly int[] eg_value = { 0, 94, 281, 297, 512, 936, 0 };
    readonly int[] pieceValue = { 0, 100, 300, 300, 500, 900, 0 };
    readonly int[] mg_pawn_table = {
      0,   0,   0,   0,   0,   0,  0,   0,
    -11,  34, 126,  68,  95,  61, 134,  98,
    -20,  25,  56,  65,  31,  26,   7,  -6,
    -23,  17,  12,  23,  21,   6,  13, -14,
    -25,  10,   6,  17,  12,  -5,  -2, -27,
    -12,  33,   3,   3, -10,  -4,  -4, -26,
    -22,  38,  24, -15, -23, -20,  -1, -35,
      0,   0,   0,   0,   0,   0,  0,   0
};
    readonly int[] eg_pawn_table = {
      0,   0,   0,   0,   0,   0,   0,   0,
187, 165, 132, 147, 134, 158, 173, 178,
 84,  82,  53,  56,  67,  85, 100,  94,
 17,  17,   4,  -2,   5,  13,  24,  32,
 -1,   3,  -8,  -7,  -7,  -3,   9,  13,
 -8,  -1,  -5,   0,   1,  -6,   7,   4,
 -7,   2,   0,  13,  10,   8,   8,  13,
  0,   0,   0,   0,   0,   0,   0,   0
};
    readonly int[] mg_knight_table = {
    -107, -15, -97,  61, -49, -34, -89, -167,
 -17,   7,  62,  23,  36,  72, -41, -73,
  44,  73, 129,  84,  65,  37,  60, -47,
  22,  18,  69,  37,  53,  19,  17,  -9,
  -8,  21,  19,  28,  13,  16,   4, -13,
 -16,  25,  17,  19,  10,  12,  -9, -23,
 -19, -14,  18,  -1,  -3, -12, -53, -29,
 -23, -19, -28, -17, -33, -58, -21, -105
};
    readonly int[] eg_knight_table = {
    -99, -63, -27, -31, -28, -13, -38, -58,
 -52, -24, -25,  -9,  -2, -25,  -8, -25,
 -41, -19,  -9,  -1,   9,  10, -20, -24,
 -18,   8,  11,  22,  22,  22,   3, -17,
 -18,   4,  17,  16,  25,  16,  -6, -18,
 -22, -20,  -3,  10,  15,  -1,  -3, -23,
 -44, -23, -20,  -2,  -5, -10, -20, -42,
 -64, -50, -18, -22, -15, -23, -51, -29
};
    readonly int[] mg_bishop_table = {
    -8,   7, -42, -25, -37, -82,   4, -29,
 -47,  18,  59,  30, -13, -18,  16, -26,
  -2,  37,  50,  35,  40,  43,  37, -16,
  -2,   7,  37,  37,  50,  19,   5,  -4,
   4,  10,  12,  34,  26,  13,  13,  -6,
  10,  18,  27,  14,  15,  15,  15,   0,
   1,  33,  21,   7,   0,  16,  15,   4,
 -21, -39, -12, -13, -21, -14,  -3, -33
};
    readonly int[] eg_bishop_table = {
    -24, -17,  -9,  -7,  -9,  -7, -21, -14,
-14,  -4, -13,  -3, -12,   7,  -4,  -8,
  4,   0,   6,  -2,  -1,   0,  -8,   2,
  2,   3,  10,  14,   9,  12,   9,  -3,
 -9,  -3,  10,   7,  19,  13,   3,  -6,
-15,  -7,   3,  13,  10,   8,  -3, -12,
-27, -15,  -9,   4,  -1,  -7, -18, -14,
-17,  -5, -16,  -9,  -5, -23,  -9, -23
};
    readonly int[] mg_rook_table = {
     43,  31,   9,  63,  51,  32,  42,  32,
 44,  26,  67,  80,  62,  58,  32,  27,
 16,  61,  45,  17,  36,  26,  19,  -5,
-20,  -8,  35,  24,  26,   7, -11, -24,
-23,   6,  -7,   9,  -1, -12, -26, -36,
-33,  -5,   0,   3, -17, -16, -25, -45,
-71,  -6,  11,  -1,  -9, -20, -16, -44,
-26, -37,   7,  16,  17,   1, -13, -19
};
    readonly int[] eg_rook_table = {
    5,   8,  12,  12,  15,  18,  10,  13,
  3,   8,   3,  -3,  11,  13,  13,  11,
 -3,  -5,   4,   7,   7,   7,   7,   7,
  2,  -1,   1,   2,   1,  13,   3,   4,
-11,  -8,  -6,  -5,   4,   8,   5,   3,
-16,  -8, -12,  -7,  -1,  -5,   0,  -4,
 -3, -11,  -9,  -9,   2,   0,  -6,  -6,
-20,   4, -13,  -5,  -1,   3,   2,  -9
};
    readonly int[] mg_queen_table = {
    45,  43,  44,  59,  12,  29,   0, -28,
 54,  28,  57, -16,   1,  -5, -39, -24,
 57,  47,  56,  29,   8,   7, -17, -13,
  1,  -2,  17,  -1, -16, -16, -27, -27,
 -3,   3,  -4,  -2, -10,  -9, -26,  -9,
  5,  14,   2,  -5,  -2, -11,   2, -14,
  1,  -3,  15,   8,   2,  11,  -8, -35,
-50, -31, -25, -15,  10,  -9, -18,  -1
};
    readonly int[] eg_queen_table = {
     20,  10,  19,  27,  27,  22,  22,  -9,
  0,  30,  25,  58,  41,  32,  20, -17,
  9,  19,  35,  47,  49,   9,   6, -20,
 36,  57,  40,  57,  45,  24,  22,   3,
 23,  39,  34,  31,  47,  19,  28, -18,
  5, 10, 17,   9,   6, 15, -27, -16,
-32, -36, -23, -16, -16, -30, -23, -22,
-41, -20, -32,  -5, -43, -22, -28, -33
};
    readonly int[] mg_king_table = {
13,   2, -34, -56, -15,  16,  23, -65,
-29, -38,  -4,  -8,  -7, -20,  -1,  29,
-22,  22,   6, -20, -16,   2,  24,  -9,
-36, -14, -25, -30, -27, -12, -20, -17,
-51, -33, -44, -46, -39, -27,  -1, -49,
-27, -15, -30, -44, -46, -22, -14, -14,
  8,   9, -16, -43, -64,  -8,   7,   1,
 14,  24, -28,   8, -54,  12,  36, -15
};
    readonly int[] eg_king_table = {
    -17,   4,  15,  11, -18, -18, -35, -74,
 11,  23,  38,  17,  17,  14,  17, -12,
 13,  44,  45,  20,  15,  23,  17,  10,
  3,  26,  33,  26,  27,  24,  22,  -8,
-11,   9,  23,  27,  24,  21,  -4, -18,
 -9,   7,  16,  23,  21,  11,  -3, -19,
-17,  -5,   4,  14,  13,   4, -11, -27,
-43, -24, -14, -28, -11, -21, -34, -53
};
    readonly int[] black_mg_pawn_table = {
      0,   0,   0,   0,   0,   0,  0,   0,
     98, 134,  61,  95,  68, 126, 34, -11,
     -6,   7,  26,  31,  65,  56, 25, -20,
    -14,  13,   6,  21,  23,  12, 17, -23,
    -27,  -2,  -5,  12,  17,   6, 10, -25,
    -26,  -4,  -4, -10,   3,   3, 33, -12,
    -35,  -1, -20, -23, -15,  24, 38, -22,
      0,   0,   0,   0,   0,   0,  0,   0,
};
    readonly int[] black_eg_pawn_table = {
      0,   0,   0,   0,   0,   0,   0,   0,
    178, 173, 158, 134, 147, 132, 165, 187,
     94, 100,  85,  67,  56,  53,  82,  84,
     32,  24,  13,   5,  -2,   4,  17,  17,
     13,   9,  -3,  -7,  -7,  -8,   3,  -1,
      4,   7,  -6,   1,   0,  -5,  -1,  -8,
     13,   8,   8,  10,  13,   0,   2,  -7,
      0,   0,   0,   0,   0,   0,   0,   0,
};
    readonly int[] black_mg_knight_table = {
    -167, -89, -34, -49,  61, -97, -15, -107,
     -73, -41,  72,  36,  23,  62,   7,  -17,
     -47,  60,  37,  65,  84, 129,  73,   44,
      -9,  17,  19,  53,  37,  69,  18,   22,
     -13,   4,  16,  13,  28,  19,  21,   -8,
     -23,  -9,  12,  10,  19,  17,  25,  -16,
     -29, -53, -12,  -3,  -1,  18, -14,  -19,
    -105, -21, -58, -33, -17, -28, -19,  -23,
};
    readonly int[] black_eg_knight_table = {
    -58, -38, -13, -28, -31, -27, -63, -99,
    -25,  -8, -25,  -2,  -9, -25, -24, -52,
    -24, -20,  10,   9,  -1,  -9, -19, -41,
    -17,   3,  22,  22,  22,  11,   8, -18,
    -18,  -6,  16,  25,  16,  17,   4, -18,
    -23,  -3,  -1,  15,  10,  -3, -20, -22,
    -42, -20, -10,  -5,  -2, -20, -23, -44,
    -29, -51, -23, -15, -22, -18, -50, -64,
};
    readonly int[] black_mg_bishop_table = {
    -29,   4, -82, -37, -25, -42,   7,  -8,
    -26,  16, -18, -13,  30,  59,  18, -47,
    -16,  37,  43,  40,  35,  50,  37,  -2,
     -4,   5,  19,  50,  37,  37,   7,  -2,
     -6,  13,  13,  26,  34,  12,  10,   4,
      0,  15,  15,  15,  14,  27,  18,  10,
      4,  15,  16,   0,   7,  21,  33,   1,
    -33,  -3, -14, -21, -13, -12, -39, -21,
};
    readonly int[] black_eg_bishop_table = {
    -14, -21, -11,  -8, -7,  -9, -17, -24,
     -8,  -4,   7, -12, -3, -13,  -4, -14,
      2,  -8,   0,  -1, -2,   6,   0,   4,
     -3,   9,  12,   9, 14,  10,   3,   2,
     -6,   3,  13,  19,  7,  10,  -3,  -9,
    -12,  -3,   8,  10, 13,   3,  -7, -15,
    -14, -18,  -7,  -1,  4,  -9, -15, -27,
    -23,  -9, -23,  -5, -9, -16,  -5, -17,
};
    readonly int[] black_mg_rook_table = {
     32,  42,  32,  51, 63,  9,  31,  43,
     27,  32,  58,  62, 80, 67,  26,  44,
     -5,  19,  26,  36, 17, 45,  61,  16,
    -24, -11,   7,  26, 24, 35,  -8, -20,
    -36, -26, -12,  -1,  9, -7,   6, -23,
    -45, -25, -16, -17,  3,  0,  -5, -33,
    -44, -16, -20,  -9, -1, 11,  -6, -71,
    -19, -13,   1,  17, 16,  7, -37, -26,
};
    readonly int[] black_eg_rook_table = {
    13, 10, 18, 15, 12,  12,   8,   5,
    11, 13, 13, 11, -3,   3,   8,   3,
     7,  7,  7,  5,  4,  -3,  -5,  -3,
     4,  3, 13,  1,  2,   1,  -1,   2,
     3,  5,  8,  4, -5,  -6,  -8, -11,
    -4,  0, -5, -1, -7, -12,  -8, -16,
    -6, -6,  0,  2, -9,  -9, -11,  -3,
    -9,  2,  3, -1, -5, -13,   4, -20,
};
    readonly int[] black_mg_queen_table = {
    -28,   0,  29,  12,  59,  44,  43,  45,
    -24, -39,  -5,   1, -16,  57,  28,  54,
    -13, -17,   7,   8,  29,  56,  47,  57,
    -27, -27, -16, -16,  -1,  17,  -2,   1,
     -9, -26,  -9, -10,  -2,  -4,   3,  -3,
    -14,   2, -11,  -2,  -5,   2,  14,   5,
    -35,  -8,  11,   2,   8,  15,  -3,   1,
     -1, -18,  -9,  10, -15, -25, -31, -50,
};
    readonly int[] black_eg_queen_table = {
     -9,  22,  22,  27,  27,  19,  10,  20,
    -17,  20,  32,  41,  58,  25,  30,   0,
    -20,   6,   9,  49,  47,  35,  19,   9,
      3,  22,  24,  45,  57,  40,  57,  36,
    -18,  28,  19,  47,  31,  34,  39,  23,
    -16, -27,  15,   6,   9,  17,  10,   5,
    -22, -23, -30, -16, -16, -23, -36, -32,
    -33, -28, -22, -43,  -5, -32, -20, -41,
};
    readonly int[] black_mg_king_table = {
    -65,  23,  16, -15, -56, -34,   2,  13,
     29,  -1, -20,  -7,  -8,  -4, -38, -29,
     -9,  24,   2, -16, -20,   6,  22, -22,
    -17, -20, -12, -27, -30, -25, -14, -36,
    -49,  -1, -27, -39, -46, -44, -33, -51,
    -14, -14, -22, -46, -44, -30, -15, -27,
      1,   7,  -8, -64, -43, -16,   9,   8,
    -15,  36,  12, -54,   8, -28,  24,  14,
};
    readonly int[] black_eg_king_table = {
    -74, -35, -18, -18, -11,  15,   4, -17,
    -12,  17,  14,  17,  17,  38,  23,  11,
     10,  17,  23,  15,  20,  45,  44,  13,
     -8,  22,  24,  27,  26,  33,  26,   3,
    -18,  -4,  21,  24,  27,  23,   9, -11,
    -19,  -3,  11,  21,  23,  16,   7,  -9,
    -27, -11,   4,  13,  14,   4,  -5, -17,
    -53, -34, -21, -11, -28, -14, -24, -43
};

    public Move Think(Board board, Timer timer)
    {
        int[][] white_mg_table = { Array.Empty<int>(), mg_pawn_table, mg_knight_table, mg_bishop_table, mg_rook_table, mg_queen_table, mg_king_table };
        int[][] black_mg_table = { Array.Empty<int>(), black_mg_pawn_table, black_mg_knight_table, black_mg_bishop_table, black_mg_rook_table, black_mg_queen_table, black_mg_king_table };
        int[][] white_eg_table = { Array.Empty<int>(), eg_pawn_table, eg_knight_table, eg_bishop_table, eg_rook_table, eg_queen_table, eg_king_table };
        int[][] black_eg_table = { Array.Empty<int>(), black_eg_pawn_table, black_eg_knight_table, black_eg_bishop_table, black_eg_rook_table, black_eg_queen_table, black_eg_king_table };
        /*int currentPhase = 24;
        currentPhase -= BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Knight, true));
        currentPhase -= BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Bishop, true));
        currentPhase -= BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Rook, true)) * 2;
        currentPhase -= BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Queen, true)) * 4;
        currentPhase -= BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Knight, false));
        currentPhase -= BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Bishop, false));
        currentPhase -= BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Rook, false)) * 2;
        currentPhase -= BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard(PieceType.Queen, false)) * 4;
        currentPhase = (currentPhase * 256 + 12) / 24;*/
        Move bestRootMove = new();
        int depthdepth = 1;
        int score = new();
        // alpha beta pruning
        int alphabeta(Board board, int depth, int alpha, int beta, bool root, int extension)
        {
            int extension2 = 0;
            int startingAlpha = alpha;
            int bestEval = -100000;
            ref Transposition tp = ref tpt[board.ZobristKey & 0x3FFFFFF];
            if (!root && tp.zobristHash == board.ZobristKey && tp.depth >= depth)
            {
                // check if theres a hit in transposition table
                if (tp.flag == 1 || tp.flag == 2 && tp.evaluation >= beta || tp.flag == 3 && tp.evaluation <= alpha)
                {
                    return tp.evaluation;
                }
            }
            if (depth == 0) return Quiesce(alpha, beta, board);
            bool bSearchPv = true;
            // null move pruning - doesnt work right now
            /*if (!board.IsInCheck() && lastMoveWasNotNull && depth >= 2 && currentPhase < 192)
            {
                board.TrySkipTurn();
                int nullSearchScore = -alphabeta(board, depth - 2, -beta, 1 - beta, false, false);
                board.UndoSkipTurn();
                if (nullSearchScore >= beta)
                {
                    //Console.WriteLine("pruned");
                    return nullSearchScore;
                }
            }*/
            int staticEval = evaluation(board, depth);
            if (!board.IsInCheck() && depth <= 6 && staticEval - 100 * depth >= beta)
            {
                return staticEval;
            }
            System.Span<Move> moves = stackalloc Move[256];
            board.GetLegalMovesNonAlloc(ref moves);
            foreach (Move move in moves.ToArray().OrderByDescending(move => (move == bestRootMove, move.CapturePieceType, move.PromotionPieceType - move.MovePieceType)))
            {
                board.MakeMove(move);
                if (board.IsInCheckmate())
                {
                    if (root)
                    {
                        // mate in 1
                        bestRootMove = move;
                        Console.WriteLine(bestRootMove);
                        Convert.ToUInt32(-1);
                    }
                    score = evaluation(board, depth - extension);
                }
                else if (board.IsDraw())
                {
                    score = 0;
                }
                else
                {
                    if (board.IsInCheck() && extension < 16)
                    {
                        extension2++;
                    }
                    if (bSearchPv)
                    {
                        score = -alphabeta(board, depth - 1 + extension2, -beta, -alpha, false, extension + extension2);
                    }
                    else
                    {
                        score = -alphabeta(board, depth - 1 + extension2, -alpha - 1, -alpha, false, extension + extension2);
                        if (score > alpha)
                        {
                            score = -alphabeta(board, depth - 1 + extension2, -beta, -alpha, false, extension + extension2);
                        }
                    }
                }
                extension2 = 0;
                board.UndoMove(move);
                if (score > bestEval) bestEval = score;
                if (score >= beta)
                {
                    return beta;
                }
                if (score > alpha)
                {
                    alpha = score;
                    bSearchPv = false;
                    if (root)
                    {
                        bestRootMove = move;
                        //Console.WriteLine(bestRootMove);
                    }
                }
                // time management (1/30 of remaining time)
                if (!bestRootMove.IsNull)
                    Convert.ToUInt32(timer.MillisecondsRemaining - 30 * timer.MillisecondsElapsedThisTurn);
            }
            // save position into transposition table
            tp.evaluation = bestEval;
            tp.zobristHash = board.ZobristKey;
            if (alpha <= startingAlpha)
                tp.flag = 3;
            else if (alpha >= beta)
                tp.flag = 2;
            else tp.flag = 1;
            tp.depth = (sbyte)depth;
            return alpha;
        }
        int evaluation(Board board, int depth)
        {
            int eval = 0;
            if (board.IsInCheckmate())
                eval = 10000 - (depthdepth - depth);
            else if (!board.IsDraw())
            {
                int opening = 0;
                int endgame = 0;
                int phase = 24;
                bool whiteKingAlone = true;
                bool blackKingAlone = true;
                int[] phase_weight = { 0, 0, 1, 1, 2, 4, 0 };
                for (int piece_type = 1; piece_type <= 6; piece_type++)
                {
                    ulong white_bb = board.GetPieceBitboard((PieceType)piece_type, true);
                    ulong black_bb = board.GetPieceBitboard((PieceType)piece_type, false);
                    while (white_bb > 0)
                    {
                        if (piece_type != 6) whiteKingAlone = false;
                        else if (piece_type == 6 && whiteKingAlone)
                        {
                            Square whiteKingSquare = board.GetKingSquare(true);
                            int whiteKingDstFromCenter = Math.Max(3 - whiteKingSquare.File, whiteKingSquare.File - 4) + Math.Max(3 - whiteKingSquare.Rank, whiteKingSquare.Rank - 4);
                            endgame -= whiteKingDstFromCenter * 300;
                        }
                        int sq = BitboardHelper.ClearAndGetIndexOfLSB(ref white_bb);
                        opening += mg_value[piece_type] + white_mg_table[piece_type][63 - sq];
                        endgame += eg_value[piece_type] + white_eg_table[piece_type][63 - sq];
                        phase -= phase_weight[piece_type];
                    }
                    while (black_bb > 0)
                    {
                        if (piece_type != 6) blackKingAlone = false;
                        else if (piece_type == 6 && blackKingAlone)
                        {
                            Square blackKingSquare = board.GetKingSquare(true);
                            int blackKingDstFromCenter = Math.Max(3 - blackKingSquare.File, blackKingSquare.File - 4) + Math.Max(3 - blackKingSquare.Rank, blackKingSquare.Rank - 4);
                            endgame -= blackKingDstFromCenter * 300;
                        }
                        int sq = BitboardHelper.ClearAndGetIndexOfLSB(ref black_bb);
                        opening -= mg_value[piece_type] + black_mg_table[piece_type][sq];
                        endgame -= eg_value[piece_type] + black_eg_table[piece_type][sq];
                        phase -= phase_weight[piece_type];
                    }
                }
                phase = (phase * 256 + 12) / 24;
                eval = ((opening * (256 - phase)) + (endgame * phase)) / 256;
                if (!board.IsWhiteToMove)
                    eval *= -1;
            }
            return eval;
        }
        int Quiesce(int alpha, int beta, Board board)
        {
            int stand_pat = evaluation(board, -1);
            if (stand_pat >= beta) return beta;
            if (stand_pat > alpha) alpha = stand_pat;
            System.Span<Move> moves = stackalloc Move[256];
            board.GetLegalMovesNonAlloc(ref moves, true);
            foreach (Move move in moves.ToArray().OrderByDescending(move => (move.CapturePieceType - move.MovePieceType)))
            {
                if (stand_pat + pieceValue[(int)move.CapturePieceType] + 200 < alpha)
                {
                    continue;
                }
                board.MakeMove(move);
                int score = -Quiesce(-beta, -alpha, board);
                board.UndoMove(move);
                if (score >= beta) return beta;
                if (score > alpha) alpha = score;
            }
            return alpha;

        }
        int testeshiurawj = evaluation(board, 0); 
        if (!board.IsWhiteToMove)
            testeshiurawj *= -1;
        Console.WriteLine("evaluation: " + testeshiurawj);
        int currentEval = 0;
        Console.WriteLine("V1.17.1");
        try
        {
            for (; ; depthdepth++)
            {
                //iterative deepening
                currentEval = alphabeta(board, depthdepth, -100000, 100000, true, 0);
                if (!board.IsWhiteToMove)
                    currentEval *= -1;
                //Console.WriteLine(depthdepth);
                //Console.WriteLine(currentEval / 100f);
            }
        }
        catch
        {
            Console.WriteLine(currentEval / 100f);
            Console.WriteLine(depthdepth);
            return bestRootMove;
        }
    }
}