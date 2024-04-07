namespace EpicTileEngine
{
    public interface ITileObject
    {
        int ActorId { get; set; }
        Tile CurrentTile { get; set; }
        string Name { get; set; }
        char Symbol { get; set; }
    }
}
