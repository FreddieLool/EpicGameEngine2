namespace EpicTileEngine
{
    /// <summary>
    /// This abstract class represents a generic tile object within the game engine.
    /// Specific tile objects like players, enemies, or items will inherit from this class.
    /// </summary>
    public abstract class TileObject : ICloneable
    {
        /// <summary>
        /// Actor identifier for the tile object.
        /// </summary>
        public int ActorId { get; set; }

        /// <summary>
        /// Reference to the tile the object currently occupies on the game board.
        /// Can be null if the object is not positioned on the board.
        /// </summary>
        public Tile? CurrentTile { get; set; } = null;

        /// <summary>
        /// Name of the tile object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Character symbol used to represent the object visually on the board.
        /// </summary>
        public char Symbol { get; set; }

        /// <summary>
        /// Indicates if the object can fly over certain types of tiles.
        /// </summary>
        public bool CanFly { get; set; }

        /// <summary>
        /// Indicates if the object has a dash ability, allowing it to pass over certain tiles.
        /// </summary>
        public bool IsDashing { get; set; }

        /// <summary>
        /// List of possible movement capabilities for the object.
        /// Each element is a tuple containing the movement direction (Position) and maximum steps allowed in that direction.
        /// </summary>
        public List<(Position direction, int maxSteps)> MovementCapabilities { get; set; }

        /// <summary>
        /// Constructor for the TileObject class.
        /// </summary>
        /// <param name="name">Name of the tile object.</param>
        /// <param name="actorId">Actor identifier for the tile object.</param>
        /// <param name="symbol">Character symbol used for visual representation.</param>
        /// <param name="canFly">Optional flag indicating flying ability (defaults to false).</param>
        /// <param name="isDashing">Optional flag indicating dashing ability (defaults to false).</param>
        public TileObject(string name, int actorId, char symbol, bool canFly = false, bool isDashing = false)
        {
            Name = name;
            ActorId = actorId;
            Symbol = symbol;
            CanFly = canFly;
            IsDashing = isDashing;
            MovementCapabilities = new List<(Position direction, int maxSteps)>();
        }

        /// <summary>
        /// Adds a movement capability for the tile object in a controlled manner.
        /// </summary>
        /// <param name="direction">Direction of movement (Position).</param>
        /// <param name="maxSteps">Maximum number of steps allowed in the specified direction.</param>
        public void AddMovementCapability(Position direction, int maxSteps)
        {
            MovementCapabilities.Add((direction, maxSteps));
        }

        /// <summary>
        /// Abstract method requiring subclasses to implement their specific cloning behavior.
        /// This allows for deep copying of the tile object and its properties.
        /// </summary>
        /// <returns>A deep copy of the current TileObject.</returns>
        public abstract object Clone();
    }
}