namespace EpicTileEngine
{
    public class Tile : ITile, ICloneable
    {
        public TileObject Occupant { get; set; }
        public Position Position { get; set; }
        public bool IsPassable { get; private set; }

        // Callbacks (occupant tileobj, exited/entereD tileobj)
        public event Action<TileObject, Tile> OnEnter;
        public event Action<TileObject> OnExit;
        public event Action<TileObject, Tile> OnRemoved;
        // usage:
        // tile.OnAttemptEnter += (tileObject, tile) => Console.WriteLine($"TileObject {tileObject} entered the tile.");

        public Tile(Position position, bool isPassable = true)
        {
            Position = position;
            IsPassable = isPassable;
        }

        public object Clone()
        {
            Tile clone = new Tile(this.Position, this.IsPassable);
            if (this.Occupant is ICloneable cloneableOccupant)
            {
                clone.Occupant = (TileObject)cloneableOccupant.Clone();
            }
            return clone;
        }

        // Called when a TileObject moves onto this tile
        public void Enter(TileObject enteringObject)
        {
            OnEnter?.Invoke(enteringObject, this);

        }

        // Called when a TileObject moves away from this tile
        public void Exit(TileObject exitingObject)
        {
            OnExit?.Invoke(exitingObject);
        }

        // Set new tile occupant and invoke OnExitw
        public void SetOccupant(TileObject newOccupant)
        {
            Occupant = newOccupant;
            OnExit?.Invoke(Occupant);
        }

        public void RemoveOccupant()
        {
            var exitingObject = Occupant;
            Occupant = null;
            OnExit?.Invoke(exitingObject);
        }
    }
}
