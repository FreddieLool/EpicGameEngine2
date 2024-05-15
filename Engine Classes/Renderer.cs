namespace EpicTileEngine
{
    public class Renderer : IRenderer
    {
        private ConsoleColor foregroundColor;
        private ConsoleColor backgroundColor;
        private ConsoleColor highlightedColor;
        private ConsoleColor selectedColor;
        private ConsoleColor actor1PieceColor, actor2PieceColor;
        private List<Position> highlightedPositions;

        public ConsoleColor ForegroundColor { get => foregroundColor; set => foregroundColor = value; }
        public ConsoleColor BackgroundColor { get => backgroundColor; set => backgroundColor = value; }
        public List<Position> HighlightedPositions { get => highlightedPositions; set => highlightedPositions = value; }
        public ConsoleColor HighlightedColor { get => highlightedColor; set => highlightedColor = value; }
        public ConsoleColor SelectedColor { get => selectedColor; set => selectedColor = value; }
        public ConsoleColor Actor1PieceColor { get => actor1PieceColor; set => actor1PieceColor = value; }
        public ConsoleColor Actor2PieceColor { get => actor2PieceColor; set => actor2PieceColor = value; }

        public void Render(Tilemap tilemap, TileObject? selectedObject = null, bool showValidMovesHighlighted = true)
        {
            int boardWidth = tilemap.Width * 3 + 5;  // Each tile is 3 chars wide + 5 to account for spacing, etc.. (fully centered board)
            int boardHeight = tilemap.Height;

            int consoleWidth = Console.WindowWidth;
            int consoleHeight = Console.WindowHeight;

            // Center the board, adjusting startX for row numbers
            int startX = (consoleWidth - boardWidth) / 2 + 4; // Added 4 spaces for row numbers
            int startY = (consoleHeight - boardHeight) / 2;

            // Ensures startY is non-negative
            startY = Math.Max(startY, 0);

            ConsoleColor bgColor;
            Position currentPosition;
            ConsoleColor pieceColor;

            // Render the row numbers
            for (int y = 0; y < tilemap.Height; y++)
            {
                Console.SetCursorPosition(startX - 2, startY + y); // Move cursor left from the start of the row
                ConsoleRGB.Write(8 - y, ConsoleColor.DarkGray);
            }

            // Render the board
            for (int y = 0; y < tilemap.Height; y++)
            {
                Console.SetCursorPosition(startX, startY + y);
                for (int x = 0; x < tilemap.Width; x++)
                {
                    currentPosition = new Position(x, y);
                    bgColor = (highlightedPositions.Contains(currentPosition) && showValidMovesHighlighted) ? highlightedColor : backgroundColor;

                    var tile = tilemap[currentPosition];

                    // Determine if the current tile has the selected piece
                    bool isSelected = (selectedObject != null && tile.Occupant == selectedObject);

                    // Apply selected color for brackets if selected, else normal background
                    ConsoleColor bracketColor = isSelected ? selectedColor : ConsoleColor.DarkGray;


                    if (tile.Occupant != null)
                    {
                        pieceColor = tile.Occupant.ActorId == 1 ? actor1PieceColor : ConsoleColor.Gray;

                        Console.ForegroundColor = bracketColor;
                        Console.Write($"[");
                        Console.BackgroundColor = bgColor;
                        Console.ForegroundColor = pieceColor;
                        Console.Write($"{tile.Occupant.Symbol}");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = bracketColor;
                        Console.Write($"]");
                        Console.ResetColor();
                    }
                    else
                    {
                        // Highlight valid moves [ ]
                        Console.ForegroundColor = bgColor;
                        Console.BackgroundColor = backgroundColor;
                        Console.Write("[");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("-");
                        Console.ForegroundColor = bgColor;
                        Console.Write("]");
                        Console.ResetColor();
                    }

                    // for spiral only
                    if (tile.IsHighlighted)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
            }

            // Render the column labels
            Console.SetCursorPosition(startX, startY + tilemap.Height);
            for (int x = 0; x < tilemap.Width; x++)
            {
                if (x > 0) Console.Write(" ");
                ConsoleRGB.Write(" " + (char)('a' + x), ConsoleColor.DarkGray);  // A to H
            }
        }
    }
}