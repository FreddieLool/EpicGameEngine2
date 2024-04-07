namespace EpicTileEngine
{
    public class Tilemap : ITilemap
    {
        private Tile[,] tileGrid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        // Ctor & Initializes tilemap
        public Tilemap(int width, int height)
        {
            Width = width;
            Height = height;
            InitializeMap(width, height);
        }

        protected virtual void InitializeMap(int width, int height)
        {
            tileGrid = new Tile[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileGrid[x, y] = new Tile(new Position(x, y));
                }
            }
        }

        // indexer based on Position
        // usage: Tile tile = tilemap[pos];
        public Tile this[Position position]
        {
            get => tileGrid[position.X, position.Y];
            private set => tileGrid[position.X, position.Y] = value;
        }

        // indexer based on x, y
        // usage: Tile tile = tilemap[3, 2];
        public Tile this[int x, int y]
        {
            get => tileGrid[x, y];
            set => tileGrid[x, y] = value;
        }

        public virtual Tile GetTile(Position position)
        {
            if (position.X < 0 || position.X >= Width || position.Y < 0 || position.Y >= Height)
                throw new ArgumentOutOfRangeException(nameof(position), "Coordinates are out of bounds.");

            return this[position];
        }

        public virtual void SetTile(Position position, Tile tile)
        {
            if (position.X < 0 || position.X >= Width || position.Y < 0 || position.Y >= Height)
                throw new ArgumentOutOfRangeException(nameof(position), "Coordinates are out of bounds.");

            this[position] = tile;
        }

        public bool IsTileOccupied(Position position)
        {
            return GetTile(position).Occupant != null;
        }

        public virtual IEnumerable<Tile> GetAllTiles()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    yield return tileGrid[x, y];
                }
            }
        }
    }
}
