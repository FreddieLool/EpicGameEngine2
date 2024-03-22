using EpicTileEngine;

public class ChessPiece : TileObject
{
    public PieceType Type { get; private set; }
    public Color Color { get; private set; }

    public ChessPiece(PieceType type, Color color) : base(type.ToString(), (int)color, GetSymbol(type, color))
    {
        Type = type;
        Color = color;
    }

    private static char GetSymbol(PieceType type, Color color)
    {
        switch (type)
        {
            case PieceType.Pawn: return 'P';
            case PieceType.Rook: return 'R';
            case PieceType.Knight: return 'N';
            case PieceType.Bishop: return 'B';
            case PieceType.Queen: return 'Q';
            case PieceType.King: return 'K';
            default: return '?';
        }
    }

    public override object Clone()
    {
        ChessPiece clone = new ChessPiece(this.Type, this.Color)
        {
            CurrentTile = this.CurrentTile != null ? (Tile)this.CurrentTile.Clone() : null
        };
        return clone;
    }

    public override void MoveTo(Tile destinatiSonTile)
    {
        //
    }

    public override bool CanInteractWith(TileObject occupant)
    {
        return true;
    }
}

public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum Color { White, Black }