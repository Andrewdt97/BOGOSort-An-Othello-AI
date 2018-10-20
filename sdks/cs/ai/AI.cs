
using System.Collections.Generic;

namespace ai
{
    public static class AI
    {
        public static int[] NextMove(GameMessage gameMessage)
        {
            int[][] board = gameMessage.board;
            int player = gameMessage.player;
            int time = gameMessage.maxTurnTime;

            int[] nextMove = { 0, 0 };


            List<int[]> moves = GetPossibleMoves(board, player);


            return nextMove;
        }

        public static List<int[]> GetPossibleMoves(int[][] board, int player)
        {
            List<int[]> allMoves = new List<int[]>();

            int ourPiece = player;
            int enemyPiece = (player == 1) ? 2 : 1;

            for (int yPos = 0; yPos < board.Length; yPos++)
            {
                for (int xPos = 0; xPos < board[yPos].Length; j++)
                {
                    // If the cell contains one of our pieces, look for chains
                    if (board[yPos][xPos] == player)
                    {
                        for (int deltaY = -1; deltaY < 2; deltaY++)
                        {
                            for (int deltaX = -1; deltaX < 2; deltaX++)
                            {
                                if (deltaX != 0 && deltaY != 0)
                                {
                                    int newY = yPos + deltaY;
                                    int newX = xPos + deltaX;

                                    // While the position is on the board and is an enemy tile
                                    while (newY >= 0 && newY < board[xPos].Length
                                        && newX >= 0 && newX < board[xPos].Length
                                        && board[newY][newX] == enemyPiece)
                                    {
                                        newY = yPos + deltaY;
                                        newX = xPos + deltaX;
                                    }

                                    if (board[newY][newX] == 0)
                                    {
                                        int[] move = new int[] { newY, newX };
                                        if (!allMoves.Contains(move))
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

    }
}
