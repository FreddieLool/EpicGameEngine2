using EpicTileEngine;

internal class ChessDemo : Tilemap, IRenderer
{
    public ChessDemo(int width, int height) : base(width, height)
    {
        InitializeChessPieces();
    }

    private void InitializeChessPieces()
    {
        // Place Black pieces
        PlaceMajorPieces(0, Color.Black);
        PlacePawns(1, Color.Black);

        // Place White pieces
        PlaceMajorPieces(Height - 1, Color.White);
        PlacePawns(Height - 2, Color.White);
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


    private void PlaceMajorPieces(int row, Color color)
    {
        this[new Position(0, row)].SetOccupant(new ChessPiece(PieceType.Rook, color));
        this[new Position(1, row)].SetOccupant(new ChessPiece(PieceType.Knight, color));
        this[new Position(2, row)].SetOccupant(new ChessPiece(PieceType.Bishop, color));
        this[new Position(3, row)].SetOccupant(new ChessPiece(PieceType.Queen, color));
        this[new Position(4, row)].SetOccupant(new ChessPiece(PieceType.King, color));
        this[new Position(5, row)].SetOccupant(new ChessPiece(PieceType.Bishop, color));
        this[new Position(6, row)].SetOccupant(new ChessPiece(PieceType.Knight, color));
        this[new Position(7, row)].SetOccupant(new ChessPiece(PieceType.Rook, color));
    }

    private void PlacePawns(int row, Color color)
    {
        for (int col = 0; col < Width; col++)
        {
            this[new Position(col, row)].SetOccupant(new ChessPiece(PieceType.Pawn, color));
        }
    }
}