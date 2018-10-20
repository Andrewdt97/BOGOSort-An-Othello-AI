using System;
using System.Collections.Generic;

namespace ai
{
    public static class AI
    {
        private static int us;

        public static int[] NextMove(GameMessage gameMessage)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            int[][] board = gameMessage.board;
            us = gameMessage.player;
            int time = gameMessage.maxTurnTime;

            int depth = 4;

            int[] nextMove = null;
            float score = float.MinValue;

            do
            {
                Console.WriteLine($"Runnig at depth {depth}");
                MoveResult result = MiniMax(board, depth, float.MinValue, float.MaxValue, us, ref sw, ref time);
                if (result.eval > score && sw.ElapsedMilliseconds < (time - 500))
                {
                    score = result.eval;
                    nextMove = result.move;
                }
                depth++;
            } while (sw.ElapsedMilliseconds < (time - 500));

            return nextMove;
        }
      
        private static MoveResult MiniMax( int[][] boardState, int depth, float alpha, float beta, int player, ref System.Diagnostics.Stopwatch sw, ref int time)
        {
            int them = (us == 1) ? 2 : 1;

            if (CheckForThreshold(boardState, 1))
            {
                return new MoveResult(new int[] { 0, 0 }, EvaluateEndGame(boardState));
            }
            if ( depth == 0 ) {
                return new MoveResult( new int[] {0, 0}, Evaluate( boardState ) );
            }

            int[] moveToMake = new int[] { 9, 9 };

            List<int[]> possibleMoves = GetPossibleMoves(boardState, player);

            bool noMovesFound = possibleMoves.Count == 0;

            if (noMovesFound)
            {
                return new MoveResult(new int[] { 0, 0 }, Evaluate(boardState));
            }

            float bestEval;

            if ( player == us ) {   // Max
                bestEval = float.MinValue;

                foreach ( int[] possibleMove in possibleMoves ) {
                    if (sw.ElapsedMilliseconds  >= (time - 500) )
                    {
                        return new MoveResult(new int[] { 0, 0 }, float.MinValue);
                    }

                    int[][] newBoard = CloneBoard(boardState);
                    MakeMove( newBoard, possibleMove, player);
                    float newEval = MiniMax( newBoard, depth - 1, alpha, beta,
                        (player == 1) ? 2 : 1, ref sw, ref time).eval;

                    if ( newEval > bestEval ) {
                        bestEval = newEval;
                        moveToMake = possibleMove;
                    }

                    alpha = Math.Max(alpha, bestEval);
                    if (beta <= alpha) { break; }
                }
            }
            else {              // Min
                bestEval = float.MaxValue;

                foreach ( int[] possibleMove in possibleMoves ) {
                    if (sw.ElapsedMilliseconds >= (time - 500) )
                    {
                        return new MoveResult(new int[] { 0, 0 }, float.MaxValue);
                    }

                    int[][] newBoard = CloneBoard(boardState);
                    MakeMove(newBoard, possibleMove, them);
                    float newEval = MiniMax( newBoard, depth - 1, alpha, beta, 
                        (player == 1) ? 2 : 1, ref sw, ref time).eval;

                    if ( newEval < bestEval ) {
                        bestEval = newEval;
                        moveToMake = possibleMove;
                    }

                    beta = Math.Min(beta, bestEval);
                    if (beta <= alpha) { break; }
                }
            }

            if (moveToMake[0] != 9)
            {
                return new MoveResult(moveToMake, bestEval);
            }

            return new MoveResult (possibleMoves[0], Evaluate(boardState));
        }
      
      public static List<int[]> GetPossibleMoves( int[][] board, int player )
        {
            List<int[]> allMoves = new List<int[]>();

            int ourPiece = player;
            int enemyPiece = ( player == 1 ) ? 2 : 1;

            for ( int yPos = 0; yPos < board.Length; yPos++ )
            {
                for ( int xPos = 0; xPos < board[yPos].Length; xPos++ )
                {
                    // If the cell contains one of our pieces, look for chains
                    if ( board[yPos][xPos] == player )
                    {
                        //Console.WriteLine($"Start: {xPos}, {yPos}");
                        for ( int deltaY = -1; deltaY < 2; deltaY++ )
                        {
                            for ( int deltaX = -1; deltaX < 2; deltaX++ )
                            {
                                if ( deltaX != 0 || deltaY != 0 )
                                {
                                    int newY = yPos + deltaY;
                                    int newX = xPos + deltaX;

                                    bool foundEnemy = false;
                                    // While the position is on the board and is an enemy tile
                                    while ( newY >= 0 && newY < board[yPos].Length
                                        && newX >= 0 && newX < board[xPos].Length
                                        && board[newY][newX] == enemyPiece )
                                    {
                                        newY += deltaY;
                                        newX += deltaX;
                                        foundEnemy = true;
                                    }

                                    if (foundEnemy
                                        && newY >= 0 && newY < board[yPos].Length
                                        && newX >= 0 && newX < board[xPos].Length
                                        && board[newY][newX] == 0 )
                                    {
                                        int[] move = new int[] { newY, newX };
                                        if ( !allMoves.Exists( x => CompareMoves(x, move) ) )
                                        {
                                            //Console.WriteLine($"StartX: {xPos}, Start Y:{yPos}\n{newX}, {newY}");
                                            allMoves.Add(move);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return allMoves;
        }

        private static int[][] CloneBoard(int[][] original)
        {
            int[][] ret = new int[8][];
            for (int r = 0; r < 8; r++)
            {
                ret[r] = new int[8];
                for (int c = 0; c < 8; c++)
                {
                    ret[r][c] = original[r][c];
                }
            }

            return ret;
        }

        public static bool CompareMoves(int[] a1, int[] a2)
        {
            bool result = false;
            if ( a1[0] == a2[0] 
                && a1[1] == a2[1] )
            {
                result = true;
            }
            return result;
        }

        public static int[][] MakeMove( int[][] board, int[] move, int player )
        {
            int enemy = ( player == 1 ) ? 2 : 1 ;

            int startY = move[0];
            int startX = move[1];

            board[startY][startX] = player;

            // Check for matches in all directions
            List<int[]> flips = new List<int[]>();

            for ( int deltaY = -1; deltaY < 2; deltaY++ )
            {
                for ( int deltaX = -1; deltaX < 2; deltaX++ )
                {
                    if ( deltaX != 0 || deltaY != 0)
                    {
                        List<int[]> tempFlips = new List<int[]>();

                        int newY = startY + deltaY;
                        int newX = startX + deltaX;

                        bool foundEnemy = false;

                        // While the position is on the board and is an enemy tile
                        while ( newY >= 0 && newY < board[startY].Length
                            && newX >= 0 && newX < board[startX].Length
                            && board[newY][newX] == enemy )
                        {
                            foundEnemy = true;
                            tempFlips.Add( new int[] { newY, newX } );
                            newY += deltaY;
                            newX += deltaX;
                        }

                        if ( foundEnemy 
                            && newY >= 0 && newY < board[startY].Length
                            && newX >= 0 && newX < board[startX].Length
                            && board[newY][newX] == player)
                        {
                            flips.AddRange( tempFlips );
                        }
                    }
                }
            }

            foreach ( int[] flip in flips )
            {
                int value = board[flip[0]][flip[1]];
                board[flip[0]][flip[1]] = (value == 1) ? 2 : 1 ;
            }

            return board;
        }

        private static float Evaluate( int[][] boardState ) {
            float peiceDiff = GetPeiceDiff( boardState, us ) / 100 ;
            float movesToMake = (float)GetPossibleMoves( boardState, us ).Count;
            float cornersDiff = GetCorners( boardState, us );
            return peiceDiff + movesToMake + cornersDiff;
        }

        private static float EvaluateEndGame( int[][] boardState )
        {
            float peiceDiff = GetPeiceDiff(boardState, us) * 128;
            return peiceDiff;
        }

        private static float GetPeiceDiff( int[][] boardState, int us ) {
            int them = (us == 1) ? 2 : 1;
            float difference = 0.0f;
            for ( int row = 0; row < boardState.Length; row++ ) {
                for ( int column = 0; column < boardState[row].Length; column++ ) {
                    if ( boardState[row][column] == us )
                    {
                        difference++;
                    } else if ( boardState[row][column] == them )
                    {
                        difference--;
                    }
                }
            }
            return difference;
        }

        private static float GetCorners( int[][] boardState, int us) {
            int them = (us == 1) ? 2 : 1;
            float difference = 0.0f;

            if ( boardState[0][0] == us )
            {
                difference += 10;
            } else if (boardState[0][0] == them )
            {
                difference -= 10;
            }

            if (boardState[7][0] == us)
            {
                difference += 10;
            }
            else if (boardState[7][0] == them)
            {
                difference -= 10;
            }

            if (boardState[0][7] == us)
            {
                difference += 10;
            }
            else if (boardState[0][7] == them)
            {
                difference -= 10;
            }

            if (boardState[7][7] == us)
            {
                difference += 10;
            }
            else if (boardState[7][7] == them)
            {
                difference -= 10;
            }

            return difference;
        }

        private static bool CheckForThreshold( int[][] boardState, int thresh ) {
            int freeSpaces = 64;
            bool result = false;

            for ( int column = 0; column < boardState.Length; column++ ) {
                for ( int row = 0; row < boardState[column].Length; row++ ) {
                    if ( freeSpaces == 0 ) { 
                        result = true;
                        break; }
                    if ( boardState[column][row] != 0 ) { freeSpaces--; }
                }
                if ( result ) { break; }
            }
            return result;
        }
      
        public class MoveResult {
            public int[] move;
            public float eval;

            public MoveResult(int[] moveArr, float moveEval) {
                move = moveArr;
                eval = moveEval;
            }
        }
    }
}
