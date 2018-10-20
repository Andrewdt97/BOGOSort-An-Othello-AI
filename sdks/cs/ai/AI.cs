using Newtonsoft.Json.Linq.JArray;
using System'

namespace ai
{
    public static class AI
    {
        enum Player { us=1, other}; //TODO: Make sure player is right
        public static int[] NextMove(GameMessage gameMessage)
        {
            var nextMove = new[] {1, 1};
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
