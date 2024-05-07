using EpicTileEngine;

internal class ChessDemo : Tilemap/*, IRenderer*/
{
    public delegate void GameResetHandler();
    public event GameResetHandler OnGameReset;

    private Actor whitePlayer;
    private Actor blackPlayer;

    public ChessDemo(int width, int height) : base(width, height)
    {
        // Ensure dimensions are properly set
        if (width != 8 || height != 8)
        {
            throw new ArgumentException("Chess board must be 8x8.");
        }

        InitializeGame();
    }

    private void InitializeGame()
    {
        whitePlayer = new Actor(1, "White");
        blackPlayer = new Actor(2, "Black");
        InitializeChessPieces();
    }

    public void ResetGame()
    {
        // Clear the board
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                this[new Position(x, y)].RemoveOccupant();
            }
        }

        // Reinitialize da chess pieces
        InitializeChessPieces();

        // Trigger the event
        OnGameReset?.Invoke();

        CommandHandler.DisplayNotificationMessage("Game has been restarted.");
    }

    private void InitializeChessPieces()
    {
        // Black pieces
        PlaceMajorPieces(0, Color.Black, blackPlayer);
        PlacePawns(1, Color.Black, blackPlayer);

        // White pieces
        PlaceMajorPieces(Height - 1, Color.White, whitePlayer);  // Should be row 7 if Height is 8
        PlacePawns(Height - 2, Color.White, whitePlayer);       // Should be row 6 if Height is 8

    }

    public void Render(Tilemap tilemap, List<Position> highlightedPositions)
    {
        int boardWidth = tilemap.Width * 3;  // Each tile is 3 chars wide
        int boardHeight = tilemap.Height;

        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;

        // Center the board, adjusting startX for row numbers
        int startX = (consoleWidth - boardWidth) / 2 + 4; // Added 4 spaces for row numbers
        int startY = (consoleHeight - boardHeight) / 2;

        // Ensures startY is non-negative
        startY = Math.Max(startY, 0);

        // Render the row numbers
        for (int y = 0; y < tilemap.Height; y++)
        {
            Console.SetCursorPosition(startX - 2, startY + y); // Move cursor left from the start of the row
            ConsoleRGB.Write(8 - y, ConsoleColor.DarkGray);  // Chess rows go from 8 at the top to 1 at the bottom
        }

        // Render the board
        for (int y = 0; y < tilemap.Height; y++)
        {
            Console.SetCursorPosition(startX, startY + y);
            for (int x = 0; x < tilemap.Width; x++)
            {
                Position currentPosition = new Position(x, y);
                ConsoleColor backgroundColor = highlightedPositions.Contains(currentPosition) ? ConsoleColor.Green : Console.BackgroundColor;

                var tile = tilemap[currentPosition];
                if (tile.Occupant != null)
                {
                    ConsoleColor pieceColor = tile.Occupant.ActorId == 1 ? ConsoleColor.Yellow : ConsoleColor.Gray;
                    backgroundColor = highlightedPositions.Contains(currentPosition) ? ConsoleColor.Green : backgroundColor;
                    Console.Write($"[");
                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = pieceColor;
                    Console.Write($"{tile.Occupant.Symbol}");
                    Console.ResetColor();
                    Console.Write($"]");
                }
                else
                {
                    Console.ForegroundColor = backgroundColor;
                    Console.Write("[");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write("-");
                    Console.ForegroundColor = backgroundColor;
                    Console.Write("]");
                    Console.ResetColor();
                }
            }
        }

        // Render the column labels
        Console.SetCursorPosition(startX, startY + tilemap.Height);
        for (int x = 0; x < tilemap.Width; x++)
        {
            if (x > 0) Console.Write(" "); // adjust spacing
            ConsoleRGB.Write(" " + (char)('A' + x), ConsoleColor.DarkGray);  // A to H
        }
    }

    private void PlaceMajorPieces(int row, Color color, Actor player)
    {
        if (row < 0 || row >= Height)
        {
            throw new ArgumentOutOfRangeException(nameof(row), "Row index is out of the bounds of the Tilemap.");
        }

        ChessPiece[] pieces = new[]
        {
        new ChessPiece(PieceType.Rook, color, player.Id),
        new ChessPiece(PieceType.Knight, color, player.Id),
        new ChessPiece(PieceType.Bishop, color, player.Id),
        new ChessPiece(PieceType.Queen, color, player.Id),
        new ChessPiece(PieceType.King, color, player.Id),
        new ChessPiece(PieceType.Bishop, color, player.Id),
        new ChessPiece(PieceType.Knight, color, player.Id),
        new ChessPiece(PieceType.Rook, color, player.Id)
        };

        for (int i = 0; i < pieces.Length; i++)
        {
            if (i < 0 || i >= Width)
            {
                continue; // Skip iteration if the column index is out of bounds
            }
            this[new Position(i, row)].SetOccupant(pieces[i]);
            player.AddTileObject(pieces[i]);
        }
    }


    private void PlacePawns(int row, Color color, Actor player)
    {
        // Place pawns and assign them to the actor
        for (int col = 0; col < Width; col++)
        {
            var pawn = new ChessPiece(PieceType.Pawn, color, player.Id);
            this[new Position(col, row)].SetOccupant(pawn);
            player.AddTileObject(pawn);
        }
    }
}