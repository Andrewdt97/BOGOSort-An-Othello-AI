# BOGOSort: An Othello AI
### By Andrew Thomas and Matthew Nykamp

## Background
BOGOSort was created as an entry into the 2018 Atomic Games hosted in Grand Rapids, MI.

## Explanation of the Code
The `othello` program (written by Atomic Object) connects via a network to game host. Once connected, it will receive a game board, the player whose turn it was, and the number of millisecons until timeout. The board is an 8x8 array with 0 in spaces without a piece, 1 in spaces with pieces belong to player 1, and 2 in spaces with pieces belonging to player 2. Once a game board is received, it is passed to our AI which returns the coordinates it wants to move.

The AI uses a minimax search with alpha-beta pruning to determine its moves. Further details of the algoritmn are present as code comments.

Basic unit tests are also present in the `tests` folder.
