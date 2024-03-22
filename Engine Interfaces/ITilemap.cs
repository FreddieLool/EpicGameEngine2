namespace EpicTileEngine
{
    public interface ITilemap
    {
        int Width { get; }
        int Height { get; }

        Tile GetTile(Position position);
        void SetTile(Position position, Tile tile);
        bool IsTileOccupied(Position position);
        IEnumerable<Tile> GetAllTiles();
    }
}

