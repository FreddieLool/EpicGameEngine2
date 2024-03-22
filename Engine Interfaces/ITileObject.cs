namespace EpicTileEngine
{
    public interface ITileObject : ICloneable
    {
        int ActorId { get; set; }
        Tile CurrentTile { get; set; }
        string Name { get; set; }
        char Symbol { get; set; }

        void MoveTo(Tile destinationTile);
    }
}
