using EpicTileEngine;

internal class ChessDemo : Tilemap, IRenderer
{
    private Actor whitePlayer;
    private Actor blackPlayer;

    public ChessDemo(int width, int height) : base(width, height)
    {
        // Ensure dimensions are properly set
        if (width != 8 || height != 8)
        {
            throw new ArgumentException("Chess board must be 8x8.");
        }

        whitePlayer = new Actor(1, "White");
        blackPlayer = new Actor(2, "Black");

        InitializeChessPieces();
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

    public void Render(Tilemap tilemap)
    {
        int boardWidth = tilemap.Width * 3; // [-] each tile is 3 chars
        int boardHeight = tilemap.Height;

        int consoleWidth = Console.WindowWidth;
        int consoleHeight = Console.WindowHeight;

        // center the board
        int startX = (consoleWidth - boardWidth) / 2;
        int startY = (consoleHeight - boardHeight) / 2;

        // ensures startY is non-negative
        startY = Math.Max(startY, 0);

        for (int y = 0; y < tilemap.Height; y++)
        {
            // Set cursor position to start of each row centered horizontally
            Console.SetCursorPosition(startX, startY + y);

            for (int x = 0; x < tilemap.Width; x++)
            {
                var tile = tilemap[new Position(x, y)];
                if (tile.Occupant != null)
                {
                    Console.Write($"[");
                    if (tile.Occupant is ChessPiece piece)
                    {
                        ConsoleColor color = piece.Color == Color.White ? ConsoleColor.Yellow : ConsoleColor.Gray;
                        ConsoleRGB.Write($"{piece.Symbol}", color, Console.BackgroundColor);
                    }
                    Console.Write($"]");
                }
                else
                {
                    ConsoleRGB.Write("[-]", ConsoleColor.DarkGray, Console.BackgroundColor);
                }
            }
        }
    }


    private void PlaceMajorPieces(int row, Color color, Actor player)
    {
        Console.WriteLine($"Tilemap height is set to: {Height}");
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
                continue; // Skip this iteration if the column index is out of bounds
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