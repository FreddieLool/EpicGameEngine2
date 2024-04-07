namespace EpicTileEngine
{
    public readonly struct Position : IPosition
    {
        public readonly int X { get; }
        public readonly int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"({X}, {Y})";

        public override bool Equals(object obj) => obj is Position other && X == other.X && Y == other.Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static Position operator +(Position a, Position b) => new Position(a.X + b.X, a.Y + b.Y);

        public static Position operator -(Position a, Position b) => new Position(a.X - b.X, a.Y - b.Y);
    }
}