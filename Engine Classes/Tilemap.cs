using System.Collections;

namespace EpicTileEngine
{
    /// <summary>
    /// Represents a two-dimensional grid of tiles with various utility methods to manage and manipulate them.
    /// </summary>
    public abstract class Tilemap : IEnumerable<Tile>
    {
        /// <summary>
        /// 2D array representing the tile grid
        /// </summary>
        private Tile[,] tileBoard;

        /// <summary>
        /// Gets the width of the tilemap.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets the height of the tilemap.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Indexer to retrieve a tile by its Position.
        /// </summary>
        /// <param name="pos">The position of the tile.</param>
        /// <returns>The tile at the specified position.</returns>
        public Tile this[Position pos]
        {
            get
            {
                if (!IsPositionValid(pos))
                    throw new ArgumentOutOfRangeException(nameof(pos), "Coordinates are out of bounds.");
                return tileBoard[pos.X, pos.Y];
            }
        }

        /// <summary>
        /// Indexer to retrieve a tile by its coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <returns>The tile at the specified coordinates.</returns>
        public Tile this[int x, int y]
        {
            get
            {
                if (!IsPositionValid(x, y))
                    throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");
                return tileBoard[x, y];
            }
        }

        /// <summary>
        /// Initializes a new instance of the Tilemap class with specified dimensions.
        /// </summary>
        /// <param name="width">The width of the tilemap.</param>
        /// <param name="height">The height of the tilemap.</param>
        public Tilemap(int width, int height)
        {
            // Set a hard limit for the size of the tilemap
            if (width > 100 || height > 100)
            {
                throw new ArgumentException("Maximum size for Tilemap is 100x100.");
            }

            Width = width;
            Height = height;
            InitializeMap();
        }

        /// <summary>
        /// Initializes the tilemap by creating Tile objects for each grid cell.
        /// </summary>
        protected virtual void InitializeMap()
        {
            tileBoard = new Tile[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    tileBoard[x, y] = new Tile();
                    tileBoard[x, y].Position = new Position(x, y);
                }
            }
        }

        /// <summary>
        /// Checks if a given position is valid within the tilemap.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the position is valid; otherwise, false.</returns>
        public bool IsPositionValid(Position position)
        {
            return position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
        }

        /// <summary>
        /// Checks if the given coordinates are valid within the tilemap.
        /// </summary>
        /// <param name="x">The x-coordinate to check.</param>
        /// <param name="y">The y-coordinate to check.</param>
        /// <returns>True if the coordinates are valid; otherwise, false.</returns>
        public bool IsPositionValid(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// Retrieves a tile by its Position.
        /// </summary>
        /// <param name="pos">The position of the tile.</param>
        /// <returns>The tile at the specified position.</returns>
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
                // Log or display the error message
                CommandHandler.DisplayNotification(ex.Message, ConsoleColor.Red);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a tile by its coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <returns>The tile at the specified coordinates.</returns>
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
                // Log or display the error message
                CommandHandler.DisplayNotification(ex.Message, ConsoleColor.Red);
                return null;
            }
        }

        /// <summary>
        /// Retrieves all chess pieces belonging to a specific actor ID.
        /// </summary>
        /// <param name="actorId">The actor ID to search for.</param>
        /// <returns>An enumerable collection of chess pieces belonging to the specified actor.</returns>
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

        /// <summary>
        /// Checks if a tile at a specific position is occupied.
        /// </summary>
        /// <param name="pos">The position to check.</param>
        /// <returns>True if the tile is occupied; otherwise, false.</returns>
        public bool IsTileOccupied(Position pos)
        {
            return GetTile(pos).Occupant != null;
        }

        /// <summary>
        /// Checks if a tile at specific coordinates is occupied.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <returns>True if the tile is occupied; otherwise, false.</returns>
        public bool IsTileOccupied(int x, int y)
        {
            return GetTile(x, y).Occupant != null;
        }

        /// <summary>
        /// Retrieves all tiles within the tilemap.
        /// </summary>
        /// <returns>An enumerable collection of all tiles in the tilemap.</returns>
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

        /// <summary>
        /// Gets an enumerator for the tiles in the tilemap.
        /// </summary>
        /// <returns>An enumerator of tiles in the tilemap.</returns>
        public IEnumerator<Tile> GetEnumerator()
        {
            // leveraging the existing method to avoid code duplication
            return GetAllTiles().GetEnumerator();
        }

        /// <summary>
        /// Returns a non-generic enumerator for the tiles in the tilemap.
        /// </summary>
        /// <returns>A non-generic enumerator of tiles.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetAllTiles().GetEnumerator();
        }

        /// <summary>
        /// Returns the tiles from the Tilemap starting at a given position, expanding outward in a spiral pattern.
        /// </summary>
        /// <param name="startPosition">The starting position for the spiral traversal.</param>
        /// <returns>A collection of tiles in spiral order, starting from the specified position.</returns>
        public IEnumerable<Tile> GetTilesInSpiralOrder(Position startPosition)
        {
            // Define the movement dir: right, down, left, up
            int[] dx = { 1, 0, -1, 0 }; // Change in x direction
            int[] dy = { 0, 1, 0, -1 }; // Change in y direction
            int direction = 0; // Start dir: right
            int segment_length = 1; // Init segment length
            int steps = 0; // steps taken in the current segment
            int segment_passed = 0; // No. of segments passed

            int x = startPosition.X;
            int y = startPosition.Y;

            // (center of the spiral)
            yield return tileBoard[x, y];

            for (int i = 1; i < Width * Height; i++)
            {
                // Move in current dir
                x += dx[direction];
                y += dy[direction];

                // valid?
                if (IsPositionValid(x, y))
                {
                    yield return tileBoard[x, y];
                }

                steps++; // +step count

                // If we completed the current seg
                if (steps == segment_length)
                {
                    steps = 0; // Reset
                    direction = (direction + 1) % 4; // Change dir (right -> down -> left -> up)
                    segment_passed++;

                    // Increase seg length after every two segments (a full turn)
                    if (segment_passed % 2 == 0)
                    {
                        segment_length++;
                    }
                }
            }
        }


    }
}