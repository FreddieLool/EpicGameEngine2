namespace EpicTileEngine
{
    public abstract class TileObject : ICloneable
    {
        public int ActorId { get; set; }
        public Tile? CurrentTile { get; set; } = null;
        public string Name { get; set; }
        public char Symbol { get; set; }
        public bool CanFly { get; set; }  // Ability to fly over certain tiles
        public bool IsDashing { get; set; }  // Dashing capability, bypass certain tiles


        // List of possible movements (dir & max steps in that dir)
        public List<(Position direction, int maxSteps)> MovementCapabilities { get; set; }

        public TileObject(string name, int actorId, char symbol, bool canFly = false, bool isDashing = false)
        {
            Name = name;
            ActorId = actorId;
            Symbol = symbol;
            CanFly = canFly;
            IsDashing = isDashing;
            MovementCapabilities = new List<(Position direction, int maxSteps)>();
        }

        // Add movement capabilities in a controlled manner
        public void AddMovementCapability(Position direction, int maxSteps)
        {
            MovementCapabilities.Add((direction, maxSteps));
        }

        // Deep copy method
        public abstract object Clone();

    }
}