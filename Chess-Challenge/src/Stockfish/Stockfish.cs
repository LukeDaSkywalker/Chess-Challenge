﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ChessChallenge.API;

public class Stockfish : IChessBot
{
    private Process stockfishProcess;

    private StreamWriter Ins() => stockfishProcess.StandardInput;

    private StreamReader Outs() => stockfishProcess.StandardOutput;

    /// <summary>
    /// The skill level of stockfish. Max is 20, min is 0.
    /// </summary>
    private const int SKILL_LEVEL = 20;

    public Stockfish()
    {
        var stockfishExe = "C:/Users/lukas/Documents/GitHub/Chess-Challenge/Chess-Challenge/src/Stockfish/stockfish-windows-x86-64-avx2/stockfish/stockfish-windows-x86-64-avx2.exe";

        stockfishProcess = new();
        stockfishProcess.StartInfo.RedirectStandardOutput = true;
        stockfishProcess.StartInfo.RedirectStandardInput = true;
        stockfishProcess.StartInfo.FileName = stockfishExe;
        stockfishProcess.Start();

        Ins().WriteLine("uci");
        string? line;
        var isOk = false;

        while ((line = Outs().ReadLine()) != null)
        {
            if (line == "uciok")
            {
                isOk = true;
                break;
            }
        }

        if (!isOk)
        {
            throw new Exception("Failed to communicate with stockfish");
        }

        Ins().WriteLine($"setoption name Skill Level value {SKILL_LEVEL}");
        Ins().WriteLine("ucinewgame");
    }

    public Move Think(Board board, Timer timer)
    {
        Ins().WriteLine($"position fen {board.GetFenString()}");

        string me = "w",
            other = "b";
        if (!board.IsWhiteToMove)
        {
            (me, other) = (other, me);
        }
        Ins()
            .WriteLine(
                $"go {me}time {timer.MillisecondsRemaining} {other}time {timer.OpponentMillisecondsRemaining}"
            );
        /* Ins().WriteLine($"go movetime 100"); */

        string? line;
        Move? move = null;

        while ((line = Outs().ReadLine()) != null)
        {
            if (line.StartsWith("bestmove"))
            {
                var moveStr = line.Split()[1];
                move = new Move(moveStr, board);

                break;
            }
        }

        if (move == null)
        {
            throw new Exception("Engine crashed");
        }

        return (Move)move;
    }
}