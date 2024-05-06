namespace EpicTileEngine
{
    public class Tilemap
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
            if (!IsPositionValid(pos))
                throw new ArgumentOutOfRangeException(nameof(pos), "Coordinates are out of bounds.");

            return this[pos];
        }

        public virtual Tile GetTile(int x, int y)
        {
            if (!IsPositionValid(x, y))
                throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");

            return this[x, y];
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
    }
}