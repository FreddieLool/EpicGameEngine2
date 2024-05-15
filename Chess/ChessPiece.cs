using EpicTileEngine;

public class ChessPiece : TileObject
{
    public PieceType Type { get; private set; }
    public Color Color { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ChessPiece class.
    /// </summary>
    /// <param name="type">The type of the chess piece.</param>
    /// <param name="color">The color of the chess piece.</param>
    /// <param name="actorId">The ID of the actor to whom the piece belongs.</param>
    public ChessPiece(PieceType type, Color color, int actorId) : base($"ChessPiece {type}", actorId, GetSymbolForPiece(type))
    {
        Type = type;
        Color = color;
    }

    /// <summary>
    /// Gets the symbol for the given piece type.
    /// </summary>
    /// <param name="type">The type of the chess piece.</param>
    /// <returns>The character symbol representing the piece.</returns>
    public static char GetSymbolForPiece(PieceType type)
    {
        switch (type)
        {
            case PieceType.Pawn:
                return 'P';
            case PieceType.Rook:
                return 'R';
            case PieceType.Knight:
                return 'N';
            case PieceType.Bishop:
                return 'B';
            case PieceType.Queen:
                return 'Q';
            case PieceType.King:
                return 'K';
            default:
                throw new ArgumentException("Unknown piece type");
        }
    }

    /// <summary>
    /// Promotes a pawn to a new piece type.
    /// </summary>
    /// <param name="newType">The new type to promote the pawn to.</param>
    public void Promote(PieceType newType)
    {
        if (Type == PieceType.Pawn)
        {
            Type = newType;
        }
    }

    /// <summary>
    /// Creates a clone of the chess piece.
    /// </summary>
    /// <returns>A new ChessPiece instance that is a copy of the current instance.</returns>
    public override object Clone()
    {
        ChessPiece clone = new(this.Type, this.Color, this.ActorId)
        {
            CurrentTile = this.CurrentTile != null ? (Tile)this.CurrentTile.Clone() : null
        };
        return clone;
    }
}

public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum Color { White, Black }