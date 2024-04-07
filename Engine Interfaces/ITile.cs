namespace EpicTileEngine
{
    public interface ITile
    {
        TileObject Occupant { get; set; }
        bool IsPassable { get; }
        Position Position {  get; protected set; }

        // for entering and exiting the tile
        void Enter(TileObject enteringObject);
        void Exit(TileObject exitingObject);

        void SetOccupant(TileObject newOccupant);
        void RemoveOccupant();

        // custom behavior on tile interaction
        event Action<TileObject, Tile> OnEnter;
        event Action<TileObject> OnExit;
        event Action<TileObject, Tile> OnRemoved;
    }
}
