using System.Collections;

namespace EpicTileEngine
{
    public class Tilemap : IEnumerable<Tile>
    {
        private Tile[,] tileBoard;
        public int Width { get; private set; }
        public int Height { get; private set; }


        // usage: Tile tile = tilemap[pos];
        public Tile this[Position pos]
        {
            get
            {
                if (!IsPositionValid(pos))
                    throw new ArgumentOutOfRangeException(nameof(pos), "Coordinates are out of bounds.");
                return tileBoard[pos.X, pos.Y];
            }
        }

        // usage: Tile tile = tilemap[3, 2];
        public Tile this[int x, int y]
        {
            get
            {
                if (!IsPositionValid(x, y))
                    throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");
                return tileBoard[x, y];
            }
        }

        // Ctor & Initializes tilemap
        public Tilemap(int width, int height)
        {
            // Hard limit
            if (width > 100 || height > 100)
            {
                throw new ArgumentException("Maximum size for Tilemap is 100x100.");
            }

            Width = width;
            Height = height;
            InitializeMap();
        }

        protected virtual void InitializeMap()
        {
            tileBoard = new Tile[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tileBoard[x, y] = new Tile();
                    tileBoard[x,y].Position = new Position(x, y);
                }
            }
        }

        public bool IsPositionValid(Position position)
        {
            return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
        }

        public bool IsPositionValid(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        public virtual Tile GetTile(Position pos)
        {
            try
            {
                if (!IsPositionValid(pos))
                    throw new ArgumentOutOfRangeException(nameof(pos), "Coordinates are out of bounds.");

                return this[pos];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                CommandHandler.DisplayNotificationMessage(ex.Message, ConsoleColor.Red);
                return null;
            }
        }

        public virtual Tile GetTile(int x, int y)
        {
            try
            {
                if (!IsPositionValid(x, y))
                    throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");

                return this[x, y];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                CommandHandler.DisplayNotificationMessage(ex.Message, ConsoleColor.Red);
                return null;
            }
        }

        public IEnumerable<ChessPiece> GetAllPiecesByActorId(int actorId)
        {
            foreach (var tile in GetAllTiles())
            {
                if (tile.Occupant is ChessPiece piece && piece.ActorId == actorId)
                {
                    yield return piece;
                }
            }
        }


        public bool IsTileOccupied(Position pos)
        {
            return GetTile(pos).Occupant != null;
        }

        public bool IsTileOccupied(int x, int y)
        {
            return GetTile(x, y).Occupant != null;
        }

        public virtual IEnumerable<Tile> GetAllTiles()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    yield return tileBoard[x, y];
                }
            }
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    yield return tileBoard[x, y];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns the tiles from the Tilemap starting at a given position, expanding outward in a spiral pattern.
        /// </summary>
        /// <param name="startPosition">The starting position for the spiral traversal.</param>
        /// <returns>A collection of tiles in spiral order, starting from the specified position.</returns>
        public IEnumerable<Tile> GetTilesInSpiralOrder(Position startPosition)
        {
            int x = 0, y = 0;
            int dx = 1, dy = 0; // Start by moving to the right
            int segment_length = 1;

            // Current segment length and the number of steps taken in current direction
            int steps = 0, segment_passed = 0;

            for (int i = 0; i < Width * Height; i++)
            {
                // Calculate the actual x, y
                int actualX = startPosition.X + x;
                int actualY = startPosition.Y + y;

                if (actualX >= 0 && actualX < Width && actualY >= 0 && actualY < Height)
                {
                    yield return tileBoard[actualX, actualY];
                }

                // Move to the next cell
                steps++;
                if (steps == segment_length)
                {
                    // Change dir
                    steps = 0;
                    segment_passed++;

                    // Rotate direction clockwise: (dx, dy) -> (dy, -dx)
                    int temp = dy;
                    dy = -dx;
                    dx = temp;

                    // Increase segment length every two segments (full cycle around the spiral)
                    if (segment_passed % 2 == 0)
                    {
                        segment_length++;
                    }
                }

                x += dx;
                y += dy;
            }
        }



    }
}