namespace EpicTileEngine
{
    public class Tile : ICloneable
    {
        public TileObject? Occupant { get; set; }
        public Position Position { get; set; }
        public bool IsPassable { get; protected set; }

        public event Action<TileObject, Tile> OnEnter;
        public event Action<TileObject, Tile> OnExit;

        public Tile(bool isPassable = true)
        {
            IsPassable = isPassable;
        }

        public object Clone()
        {
            Tile clone = new Tile(this.IsPassable);
            if (this.Occupant is ICloneable cloneableOccupant)
            {
                clone.Occupant = (TileObject)cloneableOccupant.Clone();
            }
            return clone;
        }

        public void RemoveOccupant()
        {
            if(Occupant == null) return;

            OnExit?.Invoke(Occupant, this);
            Occupant = null;
        }

        public void SetOccupant(TileObject newOccupant)
        {
            if (Occupant != null)
            {
                RemoveOccupant();
            }

            Occupant = newOccupant;

            if (newOccupant != null)
            {
                newOccupant.CurrentTile = this;
                OnEnter?.Invoke(newOccupant, this);
            }
        }
    }
}