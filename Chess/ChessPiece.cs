using EpicTileEngine;

public class ChessPiece : TileObject
{
    public PieceType Type { get; private set; }
    public Color Color { get; private set; }

    public ChessPiece(PieceType type, Color color, int actorId) : base($"ChessPiece {type}", actorId, GetSymbolForPiece(type))
    {
        Type = type;
        Color = color;
    }


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

    public void Promote(PieceType newType)
    {
        if (Type == PieceType.Pawn)
        {
            Type = newType;
        }
    }

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