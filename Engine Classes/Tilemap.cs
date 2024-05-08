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
            int x = startPosition.X;
            int y = startPosition.Y;

            // Set initial direction upward (dx = 0, dy = -1)
            int dx = 0;
            int dy = -1;

            // Calculate the maximum number of iterations needed (total tiles in the tilemap)
            int maxI = Width * Height;

            // Determine the maximum spiral size (accommodating non-square grids)
            int size = Math.Max(Width, Height);

            // for debugging
            int steps = 1;

            // Iterate through all possible tile positions up to the total tile count
            for (int i = 0; i < maxI; i++)
            {
                // Check if the current coordinates lie within the maximum size boundary
                // The boundary is centered at the starting point and grows outward
                if (-size / 2 < x && x <= size / 2 && -size / 2 < y && y <= size / 2)
                {
                    // Translate coordinates back to the positive grid range
                    // and yield the tile if within the bounds of the Tilemap
                    int translateX = x + startPosition.X;
                    int translateY = y + startPosition.Y;
                    if (translateX >= 0 && translateX < Width && translateY >= 0 && translateY < Height)
                    {
                        yield return tileBoard[translateX, translateY];
                    }
                }

                // Adjust the direction of the spiral based on specific points in the pattern:
                // - When reaching the diagonal (x == y)
                // - The lower left diagonal (-x == y)
                // - The upper right diagonal (x == 1 - y)
                if (x == y || (x < 0 && x == -y) || (x > 0 && x == 1 - y))
                {
                    // Change direction in a circular manner
                    int temp = dx;
                    dx = -dy;
                    dy = temp;
                }

                // Move the coordinates forward in the current direction
                x += dx;
                y += dy;
            }
        }

    }
}