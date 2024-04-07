namespace EpicTileEngine
{
    public abstract class TileObject : ITileObject, ICloneable
    {
        public int ActorId { get; set; }
        public Tile? CurrentTile { get; set; }
        public string Name { get; set; }
        public char Symbol { get; set; }

        public TileObject(string name, int actorId, char symbol)
        {
            Name = name;
            ActorId = actorId;
            Symbol = symbol;
        }

        // Deep copy method
        public abstract object Clone();

        public void SetTile(Tile tile)
        {
            CurrentTile.Exit(this);
            CurrentTile = tile;
            CurrentTile.Enter(this);
        }
    }
}
