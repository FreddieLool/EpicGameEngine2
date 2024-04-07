using EpicTileEngine;

public class MovementManager : ITileActionManager
{
    public Func<TileObject, Position, Tilemap, bool> ValidateMove { get; set; }
    public Action<TileObject, TileObject> OnInteract { get; set; }
    public Action<TileObject, Tile> OnMove { get; set; }

    // init methods
    public MovementManager()
    {
        ValidateMove = (_mover, _targetPosition, _board) => IsValidMove(_targetPosition, (ChessPiece)_mover, _board);
        OnInteract = (_mover, _occupant) => HandleInteraction((ChessPiece)_mover, _occupant);
        OnMove = (_mover, _newTile) => AfterMoveActions((ChessPiece)_mover, _newTile);
    }

    private bool IsValidMove(Position newPosition, ChessPiece piece, Tilemap board)
    {
        // move validation logic here
        // calling GetValidMoves and checking if newPosition is in the list
        return true; 
    }

    private void HandleInteraction(ChessPiece mover, TileObject occupant)
    {
        // capturing logic, etc.
    }

    private void AfterMoveActions(ChessPiece piece, Tile newTile)
    {
        // Post-move logic?
    }

    public bool TryMove(TileObject mover, Position targetPosition, Tilemap board)
    {
        // is move valid?
        if (!ValidateMove(mover, targetPosition, board))
        {
            return false;
        }


        Tile targetTile = board.GetTile(targetPosition);
        Tile currentTile = mover.CurrentTile;

        // 0ccupied? handle interactions
        if (targetTile.Occupant != null)
        {
            OnInteract(mover, targetTile.Occupant);
        }

        // Move the piece
        currentTile?.RemoveOccupant();
        targetTile.SetOccupant(mover);
        mover.SetTile(targetTile);

        OnMove(mover, targetTile);

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
            case PieceType.King:
                return GetKingMoves(piece, board);
            case PieceType.Queen:
                return GetQueenMoves(piece, board);
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

    private IEnumerable<Position> GetKingMoves(ChessPiece piece, Tile[,] board)
    {
        List<Position> validMoves = new List<Position>();
        // logic here...
        return validMoves;
    }

    private IEnumerable<Position> GetQueenMoves(ChessPiece piece, Tile[,] board)
    {
        List<Position> validMoves = new List<Position>();
        // logic here...
        return validMoves;
    }
}
