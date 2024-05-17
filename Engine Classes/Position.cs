namespace EpicTileEngine
{
    /// <summary>
    /// Represents a 2D position with X and Y coordinates.
    /// </summary>
    public readonly struct Position
    {
        /// <summary>
        /// Gets the X coordinate of the position.
        /// </summary>
        public readonly int X { get; }

        /// <summary>
        /// Gets the Y coordinate of the position.
        /// </summary>
        public readonly int Y { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> struct with specified X and Y coordinates.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Returns a string representation of the position.
        /// </summary>
        /// <returns>A string representing the position in the format "(X, Y)".</returns>
        public override string ToString() => $"({X}, {Y})";

        /// <summary>
        /// Determines whether this position is equal to another object.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if the specified object is a <see cref="Position"/> with the same coordinates; otherwise, false.</returns>
        public override bool Equals(object obj) => obj is Position other && X == other.X && Y == other.Y;

        /// <summary>
        /// Returns a hash code for the position.
        /// </summary>
        /// <returns>A hash code combining the X and Y coordinates.</returns>
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        /// Adds two positions together.
        /// </summary>
        /// <param name="a">The first position.</param>
        /// <param name="b">The second position.</param>
        /// <returns>A new <see cref="Position"/> that is the sum of the two positions.</returns>
        public static Position operator +(Position a, Position b) => new(a.X + b.X, a.Y + b.Y);

        /// <summary>
        /// Subtracts one position from another.
        /// </summary>
        /// <param name="a">The position to subtract from.</param>
        /// <param name="b">The position to subtract.</param>
        /// <returns>A new <see cref="Position"/> that is the difference of the two positions.</returns>
        public static Position operator -(Position a, Position b) => new(a.X - b.X, a.Y - b.Y);
    }
}