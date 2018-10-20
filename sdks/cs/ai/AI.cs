using Newtonsoft.Json.Linq.JArray;
using System'
using System.Collections.Generic;

namespace ai
{
    public static class AI
    {
        enum Player { us=1, other}; //TODO: Make sure player is right
        public static int[] NextMove(GameMessage gameMessage)
        {
            int[][] board = gameMessage.board;
            int player = gameMessage.player;
            int time = gameMessage.maxTurnTime;

            int[] nextMove = { 0, 0 };


            List<int[]> moves = GetPossibleMoves(board, player);


            return nextMove;
        }
      
        private int[] MiniMax( int[][] boardState, int depth, int alpha, int beta ) //TODO: POTENTIALLY STOPWATCH
        {
            if ( depth == 0 ) { //TODO: Add isGameOver
                return new MoveResult( new int[] {0, 0}, Evaluate( boardState ) );
            }

            int[] moveToMake = new int[2];
            int bestEval;
            bool isMyMove = true;

            if ( isMyMove ) {   // Max
                bestEval = int.MinValue;
                List<int[]> possibleMoves = GetPossibleMoves( boardState, Player.us );
                
                foreach( int[] possibleMove in possibleMoves ) {
                    int[][] newBoard = MakeMove( boardState, possibleMove);
                    int newEval = MiniMax( newBoard, depth - 1, alpha, beta );

                    if ( newEval > bestEval ) {
                        bestEval = newEval;
                        moveToMake = possibleMove;
                    }

                    alpha = Math.Max( alpha, bestEval );
                    if ( beta <= alpha) { break; }
                }
                return new MoveResult( moveToMake, bestEval );
            }
            else {              // Min
                bestEval = int.MaxValue;
                List<int[]> possibleMoves = GetPossibleMoves( boardState, Player.other );
                
                foreach( int[] possibleMove in possibleMoves ) {
                    int[][] newBoard = MakeMove( boardState, possibleMove );
                    int newEval = MiniMax( newBoard, depth - 1, alpha, beta );

                    if ( newEval < bestEval ) {
                        bestEval = newEval;
                        moveToMake = possibleMove;
                    }

                    beta = Math.Min( beta, bestEval );
                    if ( beta <= alpha) { break; }
                }
                return new MoveResult( moveToMake, bestEval );
            }
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
                        for ( int deltaY = -1; deltaY < 2; deltaY++ )
                        {
                            for ( int deltaX = -1; deltaX < 2; deltaX++ )
                            {
                                if ( deltaX != 0 || deltaY != 0 )
                                {
                                    int newY = yPos + deltaY;
                                    int newX = xPos + deltaX;

                                    Console.WriteLine( newY + ", " + newX );

                                    // While the position is on the board and is an enemy tile
                                    while ( newY >= 0 && newY < board[yPos].Length
                                        && newX >= 0 && newX < board[xPos].Length
                                        && board[newY][newX] == enemyPiece )
                                    {
                                        newY += deltaY;
                                        newX += deltaX;
                                    }

                                    if ( board[newY][newX] == 0 )
                                    {
                                        int[] move = new int[] { newY, newX };
                                        if ( !allMoves.Contains( move ) )
                                        {
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

        private int Evaluate( int[][] boardState ) {
            float peiceDiff = GetPeiceDiff( boardState, Player.us ) / 100 ;
            float movesToMake = GetPossibleMoves( boardState, Player.us ).Length as float;
            float cornersDiff = GetCorners( boardState );
            return peiceDiff + movesToMake + cornersDiff;
        }

        private float GetPeiceDiff( int[][] boardState, int us ) {
            int them = (us == 1) ? 2 : 1;
            float difference = 0.0;
            for ( int column = 0; column < boardState.Length; column++ ) {
                for ( int row = 0; row < boardState[column].Length; row++ ) {
                    switch ( boardState[column][row] ) {
                        case us: difference++; break;
                        case them: difference--; break;
                        default: break;
                    }
                }
            }
            return difference;
        }

        private float GetCorners( int[][] boardState, int us) {
            int them = (us == 1) ? 2 : 1;
            float difference = 0.0;
            switch ( boardState[0][0] ) {
                case 0: break;
                case us: difference += 10;
                case them: difference -= 10;
            }

            switch ( boardState[0][7] ) {
                case 0: break;
                case us: difference += 10;
                case them: difference -= 10;
            }

            switch ( boardState[7][0] ) {
                case 0: break;
                case us: difference += 10;
                case them: difference -= 10;
            }

            switch ( boardState[7][7] ) {
                case 0: break;
                case us: difference += 10;
                case them: difference -= 10;
            }

            return difference;
        }

        private bool CheckForThreshold( int[][] boardState, int thresh ) {
            int freeSpaces = 64;
            bool result = false;

            for ( int column = 0; column < boardState.Length; column++ ) {
                for ( int row = 0; row < boardState[column].Length; row++ ) {
                    if ( freeSpaces < thresh ) { 
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
            public int eval;

            public MoveResult(int[] moveArr, int moveEval) {
                move = moveArr;
                eval = moveEval;
            }
        }
    }
}
