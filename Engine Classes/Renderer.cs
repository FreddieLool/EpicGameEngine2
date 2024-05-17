using System.Diagnostics;

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

        // NEW: 
        // Juicy & visually appealing colors
        // Highlights column & row labels for capture-able pieces for easier notation identification
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
                ConsoleColor rowColor = ConsoleColor.DarkGray;

                // Iterate through highlighted positions to find the capture-able piece
                foreach (var highlightedPos in highlightedPositions)
                {
                    if (tilemap[highlightedPos].Occupant != null)
                    {
                        if (highlightedPos.Y == y)
                        {
                            rowColor = ConsoleColor.White;
                            break;
                        }
                    }
                }
                ConsoleRGB.Write(8 - y, rowColor);
            }

            // Render the board
            for (int y = 0; y < tilemap.Height; y++)
            {
                Console.SetCursorPosition(startX, startY + y);
                for (int x = 0; x < tilemap.Width; x++)
                {
                    currentPosition = new Position(x, y);
                    var tile = tilemap[currentPosition];

                    bgColor = (highlightedPositions.Contains(currentPosition) && showValidMovesHighlighted) || tile.IsHighlighted ? highlightedColor : backgroundColor;


                    // Determine if the current tile has the selected piece
                    bool isSelected = (selectedObject != null && tile.Occupant == selectedObject);
                    if (tile.Occupant != null)
                    {
                        // Set piece color: Red for capture (green bg), actor1PieceColor for player 1, Gray for others
                        pieceColor = (bgColor == ConsoleColor.Green) ? ConsoleColor.Red : (tile.Occupant.ActorId == 1 ? actor1PieceColor : ConsoleColor.Gray);

                        // Set bracket color: White for selected, Red for capture (green bg), DarkGray for others
                        ConsoleColor bracketColor = isSelected ? ConsoleColor.White : (bgColor == ConsoleColor.Green) ? ConsoleColor.Red : ConsoleColor.DarkGray;

                        Console.ForegroundColor = bracketColor;
                        Console.Write("[");
                        Console.BackgroundColor = ConsoleColor.Black; // Ensure background is black
                        Console.ForegroundColor = pieceColor;
                        Console.Write($"{tile.Occupant.Symbol}");
                        Console.BackgroundColor = ConsoleColor.Black; // Ensure background remains black
                        Console.ForegroundColor = bracketColor;
                        Console.Write("]");
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

            // Render the column labels (with highlighting)
            Console.SetCursorPosition(startX, startY + tilemap.Height);
            for (int x = 0; x < tilemap.Width; x++)
            {
                if (x > 0) Console.Write(" ");
                ConsoleColor columnColor = ConsoleColor.DarkGray;

                // Iterate through highlighted positions to find the capture-able piece
                foreach (var highlightedPos in highlightedPositions)
                {
                    if (tilemap[highlightedPos].Occupant != null)
                    {
                        if (highlightedPos.X == x)
                        {
                            columnColor = ConsoleColor.White;
                            break;
                        }
                    }
                }
                ConsoleRGB.Write(" " + (char)('a' + x), columnColor);
            }
        }
    }
}