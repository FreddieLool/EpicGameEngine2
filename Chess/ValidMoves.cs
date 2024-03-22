using EpicTileEngine;
public class ValidMoves
{
    public void MoveChessPiece(ChessPiece piece, Position movementDirection, Tile[,] board)
    {
        Position currentPosition = piece.CurrentTile.Position;
        Position newPosition = currentPosition + movementDirection;

        if (IsValidMove(newPosition, piece, board))
        {
            Tile newTile = board[newPosition.X, newPosition.Y];
            piece.CurrentTile.Occupant = null; // Remove piece from current tile
            newTile.SetOccupant(piece); // Place piece on new tile
            piece.SetTile(newTile); // Update piece's current tile
        }
    }

    private bool IsValidMove(Position newPosition, ChessPiece piece, Tile[,] board)
    {
        return true;
    }

    // get valid moves for a specific chess piece
    public IEnumerable<Position> GetValidMoves(ChessPiece piece, Tile[,] board)
    {
        switch (piece.Type)
        {
            case PieceType.Pawn:
                return GetPawnMoves(piece, board);
            case PieceType.Rook:
                return GetRookMoves(piece, board);
            case PieceType.Knight:
                return GetKnightMoves(piece, board);
            // more..
            default:
                return new List<Position>();
        }
    }

    private IEnumerable<Position> GetPawnMoves(ChessPiece piece, Tile[,] board)
    {
        List<Position> validMoves = new List<Position>();
        // logic ... considering piece.Color (actor?) and piece.CurrentTile.Position
        return validMoves;
    }

    private IEnumerable<Position> GetRookMoves(ChessPiece piece, Tile[,] board)
    {
        List<Position> validMoves = new List<Position>();
        // logic here...
        return validMoves;
    }

    private IEnumerable<Position> GetKnightMoves(ChessPiece piece, Tile[,] board)
    {
        List<Position> validMoves = new List<Position>();
        // logic here...
        return validMoves;
    }
}