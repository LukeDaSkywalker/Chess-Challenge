using ChessChallenge.API;

public class EvilBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] EveryMove = board.GetLegalMoves();
        System.Random rnd = new();
        foreach (Move move in EveryMove)
        {
            if (huntCheckmate(board, move))
            {
                return move;
            }

        }

        return EveryMove[0];


        bool huntCheckmate(Board board, Move moveToCheck)
        {
            board.MakeMove(moveToCheck);
            bool isMate = board.IsInCheckmate();
            /*if (board.IsInCheck() && depth < 4)
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
            return isMate;
        }


        /*float evaluation(Board board, Move moveToEvaluate)
        {
            board.MakeMove(moveToEvaluate);
            return;
        }*/
    }
}