using EpicTileEngine;

internal class ChessDemo : Tilemap, IRenderer
{
    private Actor whitePlayer;
    private Actor blackPlayer;

    public ChessDemo(int width, int height) : base(width, height)
    {
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
        PlaceMajorPieces(Height - 1, Color.White, whitePlayer);
        PlacePawns(Height - 2, Color.White, whitePlayer);
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
        // Place major pieces and assign them to the actor
        ChessPiece[] pieces = new[]
        {
            new ChessPiece(PieceType.Rook, color),
            new ChessPiece(PieceType.Knight, color),
            new ChessPiece(PieceType.Bishop, color),
            new ChessPiece(PieceType.Queen, color),
            new ChessPiece(PieceType.King, color),
            new ChessPiece(PieceType.Bishop, color),
            new ChessPiece(PieceType.Knight, color),
            new ChessPiece(PieceType.Rook, color)
        };

        for (int i = 0; i < pieces.Length; i++)
        {
            this[new Position(i, row)].SetOccupant(pieces[i]);
            player.AddTileObject(pieces[i]);
        }
    }

    private void PlacePawns(int row, Color color, Actor player)
    {
        // Place pawns and assign them to the actor
        for (int col = 0; col < Width; col++)
        {
            var pawn = new ChessPiece(PieceType.Pawn, color);
            this[new Position(col, row)].SetOccupant(pawn);
            player.AddTileObject(pawn);
        }
    }
}