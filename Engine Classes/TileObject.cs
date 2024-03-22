namespace EpicTileEngine
{
    public abstract class TileObject : ICloneable
    {
        public int ActorId { get; private set; }
        private Tile _currentTile;
        public string Name { get; private set; } // Pawn, King, etc.
        public char Symbol { get; private set; } // K, Q, ...

        public TileObject(string name, int actorId, char symbol)
        {
            Name = name;
            ActorId = actorId;
            Symbol = symbol;
        }

        // Deep copy method
        public abstract object Clone();

        public Tile CurrentTile
        {
            get => _currentTile;
            protected set => _currentTile = value;
        }

        public void SetTile(Tile tile)
        {
            _currentTile.Exit(this);
            _currentTile = tile;
            _currentTile.Enter(this);
        }

        // derived classes implement their own movement logic // Change to Movement Class
        public abstract void MoveTo(Tile destinationTile);

        // determine if interaction with another tile object is allowed
        public abstract bool CanInteractWith(TileObject occupant);
        ///
        // to movement class
        protected virtual bool ValidateMove(Tile destinationTile)
        {
            // Check if the destination tile is passable & either unoccupied or interactable.
            return destinationTile.IsPassable &&
                   (destinationTile.Occupant == null || CanInteractWith(destinationTile.Occupant));
        }
    }
}
