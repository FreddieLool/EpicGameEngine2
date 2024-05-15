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
    public ChessPiece(PieceType type, Color color, int actorId, int boardWidth = 8, int boardHeight = 8) : base($"ChessPiece {type}", actorId, GetSymbolForPiece(type))
    {
        Type = type;
        Color = color;

        int direction = color == Color.White ? -1 : 1;

        // no max steps, dynamic for any board size
        switch (type)
        {
            case PieceType.Pawn:
                AddMovementCapability(new Position(0, direction), 2); // Forward movement for pawn
                AddMovementCapability(new Position(-1, direction), 1); // Capture movement for pawn (left diagonal)
                AddMovementCapability(new Position(1, direction), 1); // Capture movement for pawn (right diagonal)
                break;
            case PieceType.Rook:
                AddMovementCapability(new Position(1, 0), boardWidth - 1); // Horizontal right
                AddMovementCapability(new Position(-1, 0), boardWidth - 1); // Horizontal left
                AddMovementCapability(new Position(0, 1), boardHeight - 1); // Vertical up
                AddMovementCapability(new Position(0, -1), boardHeight - 1); // Vertical down
                break;
            case PieceType.Knight:
                AddMovementCapability(new Position(2, 1), 1); // L-shape movements
                AddMovementCapability(new Position(2, -1), 1);
                AddMovementCapability(new Position(-2, 1), 1);
                AddMovementCapability(new Position(-2, -1), 1);
                AddMovementCapability(new Position(1, 2), 1);
                AddMovementCapability(new Position(1, -2), 1);
                AddMovementCapability(new Position(-1, 2), 1);
                AddMovementCapability(new Position(-1, -2), 1);
                break;
            case PieceType.Bishop:
                AddMovementCapability(new Position(1, 1), Math.Max(boardWidth, boardHeight) - 1); // Diagonal movements
                AddMovementCapability(new Position(1, -1), Math.Max(boardWidth, boardHeight) - 1);
                AddMovementCapability(new Position(-1, 1), Math.Max(boardWidth, boardHeight) - 1);
                AddMovementCapability(new Position(-1, -1), Math.Max(boardWidth, boardHeight) - 1);
                break;
            case PieceType.Queen:
                AddMovementCapability(new Position(1, 0), boardWidth - 1); // Horizontal and vertical
                AddMovementCapability(new Position(-1, 0), boardWidth - 1);
                AddMovementCapability(new Position(0, 1), boardHeight - 1);
                AddMovementCapability(new Position(0, -1), boardHeight - 1);
                AddMovementCapability(new Position(1, 1), Math.Max(boardWidth, boardHeight) - 1); // Diagonal
                AddMovementCapability(new Position(1, -1), Math.Max(boardWidth, boardHeight) - 1);
                AddMovementCapability(new Position(-1, 1), Math.Max(boardWidth, boardHeight) - 1);
                AddMovementCapability(new Position(-1, -1), Math.Max(boardWidth, boardHeight) - 1);
                break;
            case PieceType.King:
                AddMovementCapability(new Position(1, 0), 1); // One step in all directions
                AddMovementCapability(new Position(-1, 0), 1);
                AddMovementCapability(new Position(0, 1), 1);
                AddMovementCapability(new Position(0, -1), 1);
                AddMovementCapability(new Position(1, 1), 1);
                AddMovementCapability(new Position(1, -1), 1);
                AddMovementCapability(new Position(-1, 1), 1);
                AddMovementCapability(new Position(-1, -1), 1);
                break;
        }
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