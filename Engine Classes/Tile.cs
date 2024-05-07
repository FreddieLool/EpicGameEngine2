namespace EpicTileEngine
{
    public enum TileType
    {
        Normal, // Standard tile where objects can stop
        Cloud,  // Can be flown over, but not stopped on
        Water   // Can be passed if the object is dashing or flying
    }

    public class Tile : ICloneable
    {
        public TileObject? Occupant { get; set; }
        public Position Position { get; set; }
        public bool IsPassable { get; protected set; }

        public TileType Type { get; set; }

        public event Action<TileObject, Tile> OnEnter; // Triggered when a TileObject lands on this tile
        public event Action<TileObject, Tile> OnExit;  // Triggered when a TileObject exits this tile
        public event Action<TileObject, Tile> OnPass; // Triggered when a TileObject passes over this tile

        public Tile(bool isPassable = true, TileType type = TileType.Normal)
        {
            IsPassable = isPassable;
            Type = type;
        }

        public object Clone()
        {
            return new Tile(this.IsPassable, this.Type) { Occupant = this.Occupant?.Clone() as TileObject };
        }

        public void RemoveOccupant()
        {
            if(Occupant == null) return;

            OnExit?.Invoke(Occupant, this);
            Occupant = null;
        }

        public void SetOccupant(TileObject newOccupant, bool isPassing = false)
        {
            if (isPassing)
            {
                // Notify listeners that a TileObject is passing through
                OnPass?.Invoke(newOccupant, this);
            }
            else
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

        // Method to safely trigger the OnPass event
        public void TriggerOnPass(TileObject mover)
        {
            OnPass?.Invoke(mover, this);
        }
    }
}