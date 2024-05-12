namespace EpicTileEngine
{
    /// <summary>
    /// Represents the type of a tile.
    /// </summary>
    public enum TileType
    {
        /// <summary>
        /// A standard tile that objects can stop on.
        /// </summary>
        Normal,

        /// <summary>
        /// A cloud tile that can be flown over but cannot be stopped on.
        /// </summary>
        Cloud,

        /// <summary>
        /// A water tile that can be passed only if the object is dashing or flying.
        /// </summary>
        Water
    }

    /// <summary>
    /// Represents a tile on the game board.
    /// </summary>
    public class Tile : ICloneable
    {
        /// <summary>
        /// Gets or sets the object currently occupying the tile.
        /// </summary>
        public TileObject? Occupant { get; private set; }

        /// <summary>
        /// Gets or sets the position of the tile.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tile is passable.
        /// </summary>
        public bool IsPassable { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tile is highlighted.
        /// </summary>
        public bool IsHighlighted { get; set; }

        /// <summary>
        /// Gets or sets the type of the tile.
        /// </summary>
        public TileType Type { get; set; }

        /// <summary>
        /// Triggered when a TileObject lands on this tile.
        /// </summary>
        public event Action<TileObject, Tile>? OnEnter;

        /// <summary>
        /// Triggered when a TileObject exits this tile.
        /// </summary>
        public event Action<TileObject, Tile>? OnExit;

        /// <summary>
        /// Triggered when a TileObject passes over this tile.
        /// </summary>
        public event Action<TileObject, Tile>? OnPass;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class with the specified passability and type.
        /// </summary>
        /// <param name="isPassable">Indicates if the tile is passable.</param>
        /// <param name="type">The type of the tile.</param>
        public Tile(bool isPassable = true, TileType type = TileType.Normal)
        {
            IsPassable = isPassable;
            Type = type;
        }

        /// <summary>
        /// Creates a clone of this tile.
        /// </summary>
        /// <returns>A new <see cref="Tile"/> object with the same properties.</returns>
        public object Clone()
        {
            return new Tile(IsPassable, Type)
            {
                Occupant = Occupant?.Clone() as TileObject
            };
        }

        /// <summary>
        /// Removes the current occupant from this tile and triggers the <see cref="OnExit"/> event.
        /// </summary>
        public void RemoveOccupant()
        {
            if (Occupant == null) return;

            OnExit?.Invoke(Occupant, this);
            Occupant = null;
        }

        /// <summary>
        /// Sets a new occupant for this tile, optionally indicating that the new occupant is just passing over.
        /// </summary>
        /// <param name="newOccupant">The new occupant.</param>
        /// <param name="isPassing">True if the new occupant is passing over, false otherwise.</param>
        public void SetOccupant(TileObject newOccupant, bool isPassing = false)
        {
            if (isPassing)
            {
                // Notify listeners that a TileObject is passing through.
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

        /// <summary>
        /// Safely triggers the <see cref="OnPass"/> event.
        /// </summary>
        /// <param name="mover">The TileObject that is passing over.</param>
        public void TriggerOnPass(TileObject mover)
        {
            OnPass?.Invoke(mover, this);
        }
    }
}