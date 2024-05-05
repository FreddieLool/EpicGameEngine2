using EpicTileEngine;
using System.Diagnostics;

public class MovementManager : TileActionManager
{
    public MovementManager()
    {
        ValidateMove = this.ValidateMoveMethod;
        OnInteract = this.HandleInteractionMethod;
        OnMove = this.AfterMoveActionsMethod;
    }

    private bool IsValidMove(Position newPosition, ChessPiece piece, Tilemap board)
    {
        var validMoves = GetValidMoves(piece, board).ToList();
        return validMoves.Contains(newPosition);
    }

    private bool ValidateMoveMethod(TileObject mover, Position targetPosition, Tilemap board)
    {
        Debug.WriteLine($"Validating move for {((ChessPiece)mover).Name} to {targetPosition}");
        return IsValidMove(targetPosition, (ChessPiece)mover, board);
    }

    private bool HandleInteractionMethod(TileObject mover, TileObject occupant, Tilemap board)
    {
        Debug.WriteLine($"Handling interaction between {((ChessPiece)mover).Name} and {occupant}");
        return HandleInteraction((ChessPiece)mover, occupant, board);
    }

    private bool HandleInteraction(ChessPiece mover, TileObject occupant, Tilemap board)
    {
        if (occupant is ChessPiece targetPiece)
        {
            if (targetPiece.ActorId != mover.ActorId)
            {
                bool isLegal = IsMoveLegalAfterInteraction(mover, targetPiece, board);
                Debug.WriteLine(isLegal ? "Interaction legal." : "Interaction results in illegal move.");
                if (!isLegal)
                {
                    return false; // Cannot capture because it would be illegal (self-check)
                }

                // Capture logic
                board[targetPiece.CurrentTile.Position].RemoveOccupant(); // Assume capture occurs here
                return true; // Capture is allowed
            }
        }
        return false; // No interaction possible (same player's piece)
    }
    private void AfterMoveActions(ChessPiece piece, Tile newTile)
    {
        // Check for pawn promotion
        if (piece.Type == PieceType.Pawn && (newTile.Position.Y == 0 || newTile.Position.Y == 7))
        {
            piece.Promote(PieceType.Queen);
        }
    }

    private void AfterMoveActionsMethod(TileObject mover, Tile newTile)
    {
        Debug.WriteLine($"Executing post-move actions for {((ChessPiece)mover).Name}");
        AfterMoveActions((ChessPiece)mover, newTile);
    }

    private bool IsMoveLegalAfterInteraction(ChessPiece mover, ChessPiece targetPiece, Tilemap board)
    {
        // Temporarily make the move
        var originalPosition = mover.CurrentTile;
        var targetPosition = targetPiece.CurrentTile;

        board[targetPosition.Position].SetOccupant(mover);
        originalPosition.RemoveOccupant();

        // Find the king for the current player
        ChessPiece king = FindKing(board, mover.ActorId);

        // Check if the king is in check after the move
        bool kingInCheck = CheckForThreats(king, board);

        // Undo the move
        originalPosition.SetOccupant(mover);
        targetPosition.SetOccupant(targetPiece);

        return !kingInCheck;
    }



    private bool CheckForThreats(ChessPiece king, Tilemap board)
    {
        Position kingPosition = king.CurrentTile.Position;
        foreach (var tile in board.GetAllTiles())
        {
            if (tile.Occupant is ChessPiece piece && piece.ActorId != king.ActorId)
            {
                // Assume GetValidMoves is designed to get moves without checking king's check status
                var threats = GetValidMoves(piece, board);
                if (threats.Contains(kingPosition))
                {
                    return true;  // King is in check
                }
            }
        }
        return false;
    }




    public bool TryMove(TileObject mover, Position targetPosition, Tilemap board)
    {
        if (!ValidateMove(mover, targetPosition, board))
            return false;

        Tile targetTile = board.GetTile(targetPosition);
        Tile currentTile = mover.CurrentTile;

        if (targetTile.Occupant != null)
        {
            if (!OnInteract(mover, targetTile.Occupant, board)) // Ensure you pass the board
                return false; // Interaction check failed
        }

        targetTile.SetOccupant(mover);

        OnMove(mover, targetTile); // Trigger any move-related actions

        return true;
    }


    private ChessPiece FindKing(Tilemap board, int actorId)
    {
        foreach (var tile in board.GetAllTiles())
        {
            if (tile.Occupant is ChessPiece piece && piece.ActorId == actorId && piece.Type == PieceType.King)
            {
                return piece;
            }
        }
        throw new InvalidOperationException("King not found on the board, which should never happen.");
    }

    // get valid moves for a specific chess piece
    public IEnumerable<Position> GetValidMoves(ChessPiece piece, Tilemap board)
    {
        switch (piece.Type)
        {
            case PieceType.Pawn:
                return GetPawnMoves(piece, board);
            case PieceType.Rook:
                return GetRookMoves(piece, board);
            case PieceType.Knight:
                return GetKnightMoves(piece, board);
            case PieceType.Bishop:
                return GetBishopMoves(piece, board);
            case PieceType.King:
                return GetKingMoves(piece, board);
            case PieceType.Queen:
                return GetQueenMoves(piece, board);
            default:
                return [];
        }
    }
    private IEnumerable<Position> GetPawnMoves(ChessPiece piece, Tilemap board)
    {
        int direction = piece.Color == Color.White ? 1 : -1;
        Position currentPosition = piece.CurrentTile.Position;
        Position singleStepForward = new Position(currentPosition.X, currentPosition.Y + direction);

        // Single step forward
        if (board.IsPositionValid(singleStepForward) && !board.IsTileOccupied(singleStepForward))
        {
            yield return singleStepForward;
        }

        // Double step from start
        if ((piece.Color == Color.White && currentPosition.Y == 6) || (piece.Color == Color.Black && currentPosition.Y == 1))
        {
            Position doubleStepForward = new Position(currentPosition.X, currentPosition.Y + 2 * direction);
            if (board.IsPositionValid(doubleStepForward) && !board.IsTileOccupied(doubleStepForward) && !board.IsTileOccupied(singleStepForward))
            {
                yield return doubleStepForward;
            }
        }

        // Captures
        Position[] potentialCaptures = {
        new Position(currentPosition.X - 1, currentPosition.Y + direction),
        new Position(currentPosition.X + 1, currentPosition.Y + direction)
        };

        foreach (Position capturePos in potentialCaptures)
        {
            if (board.IsPositionValid(capturePos) && board.IsTileOccupied(capturePos) && board.GetTile(capturePos).Occupant.ActorId != piece.ActorId)
            {
                yield return capturePos;
            }
        }
    }


    private IEnumerable<Position> GetRookMoves(ChessPiece piece, Tilemap board)
    {
        List<Position> validMoves = [];
        Position currentPosition = piece.CurrentTile.Position;

        // Directions a rook can move: vertical and horizontal
        (Position, Position)[] directions =
        {
            (new (1, 0), new (-1, 0)),  // Right and Left
            (new (0, 1), new (0, -1))  // Up and Down
        };

        foreach (var (positive, negative) in directions)
        {
            // Check positive direction (e.g., right or up)
            for (int i = 1; i < board.Width; i++)
            {
                Position nextPosition = new Position(currentPosition.X + positive.X * i, currentPosition.Y + positive.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant.ActorId != piece.ActorId)
                    {
                        validMoves.Add(nextPosition);
                    }
                    break;  // Stop if a piece is encountered
                }
                validMoves.Add(nextPosition);
            }

            // Check negative direction (e.g., left or down)
            for (int i = 1; i < board.Width; i++)
            {
                Position nextPosition = new Position(currentPosition.X + negative.X * i, currentPosition.Y + negative.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant.ActorId != piece.ActorId)
                    {
                        validMoves.Add(nextPosition);
                    }
                    break;  // Stop if a piece is encountered
                }
                validMoves.Add(nextPosition);
            }
        }

        return validMoves;

    }

    private IEnumerable<Position> GetKnightMoves(ChessPiece piece, Tilemap board)
    {
        List<Position> validMoves = new List<Position>();
        Position currentPosition = piece.CurrentTile.Position;

        // Possible knight moves in "L" shapes
        Position[] moves =
        {
            new (2, 1),     new (2, -1),   // Right + Up/Down
            new (-2, 1),    new (-2, -1), // Left + Up/Down
            new (1, 2),     new (1, -2),   // Up + Right/Left
            new (-1, 2),    new (-1, -2)  // Down + Right/Left
        };

        foreach (Position move in moves)
        {
            Position nextPosition = new Position(currentPosition.X + move.X, currentPosition.Y + move.Y);
            if (board.IsPositionValid(nextPosition))  // Check if the position is on the board
            {
                if (!board.IsTileOccupied(nextPosition) || (board.GetTile(nextPosition).Occupant.ActorId != piece.ActorId))
                {
                    // Add the position if it is not occupied or is occupied by an opponent's piece
                    validMoves.Add(nextPosition);
                }
            }
        }

        return validMoves;

    }

    private IEnumerable<Position> GetBishopMoves(ChessPiece piece, Tilemap board)
    {
        List<Position> validMoves = new List<Position>();
        Position currentPosition = piece.CurrentTile.Position;

        // Directions a bishop can move: all four diagonals
        (Position, Position)[] directions =
        {
            (new (1, 1),    new (-1, -1)),  // Diagonal right up and left down
            (new (1, -1),   new (-1, 1))   // Diagonal right down and left up
        };

        foreach (var (positive, negative) in directions)
        {
            // Check positive direction (e.g., right up)
            for (int i = 1; i < Math.Max(board.Width, board.Height); i++)
            {
                Position nextPosition = new Position(currentPosition.X + positive.X * i, currentPosition.Y + positive.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant.ActorId != piece.ActorId)
                    {
                        validMoves.Add(nextPosition);
                    }
                    break;  // Stop if a piece is encountered
                }
                validMoves.Add(nextPosition);
            }

            // Check negative direction (e.g., left down)
            for (int i = 1; i < Math.Max(board.Width, board.Height); i++)
            {
                Position nextPosition = new Position(currentPosition.X + negative.X * i, currentPosition.Y + negative.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant.ActorId != piece.ActorId)
                    {
                        validMoves.Add(nextPosition);
                    }
                    break;  // Stop if a piece is encountered
                }
                validMoves.Add(nextPosition);
            }
        }
        return validMoves;
    }

    private IEnumerable<Position> GetKingMoves(ChessPiece piece, Tilemap board)
    {
        List<Position> validMoves = new List<Position>();
        Position currentPosition = piece.CurrentTile.Position;

        // Possible directions for the king's movement
        Position[] moves =
        {
            new (1, 0),     new (-1, 0),   // Right, Left
            new (0, 1),     new (0, -1),   // Up, Down
            new (1, 1),     new (-1, -1),  // Diagonal right up, left down
            new (1, -1),    new (-1, 1)   // Diagonal right down, left up
        };

        foreach (Position move in moves)
        {
            Position nextPosition = new Position(currentPosition.X + move.X, currentPosition.Y + move.Y);
            if (board.IsPositionValid(nextPosition))  // Check if the position is on the board
            {
                if (!board.IsTileOccupied(nextPosition) || (board.GetTile(nextPosition).Occupant.ActorId != piece.ActorId))
                {
                    // Add the position if it is not occupied or is occupied by an opponent's piece
                    validMoves.Add(nextPosition);
                }
            }
        }

        return validMoves;

    }

    private IEnumerable<Position> GetQueenMoves(ChessPiece piece, Tilemap board)
    {
        List<Position> validMoves = new List<Position>();
        Position currentPosition = piece.CurrentTile.Position;

        // Directions the queen can move: vertical, horizontal, and diagonal
        (Position, Position)[] directions =
        {
            (new (1, 0),    new (-1, 0)),   // Right and Left
            (new (0, 1),    new (0, -1)),   // Up and Down
            (new (1, 1),    new (-1, -1)),  // Diagonal right up and left down
            (new (1, -1),   new (-1, 1))   // Diagonal right down and left up
        };

        foreach (var (positive, negative) in directions)
        {
            // Check positive directions
            for (int i = 1; i < Math.Max(board.Width, board.Height); i++)
            {
                Position nextPosition = new Position(currentPosition.X + positive.X * i, currentPosition.Y + positive.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant.ActorId != piece.ActorId)
                    {
                        validMoves.Add(nextPosition);
                    }
                    break;  // Stop if a piece is encountered
                }
                validMoves.Add(nextPosition);
            }

            // Check negative directions
            for (int i = 1; i < Math.Max(board.Width, board.Height); i++)
            {
                Position nextPosition = new Position(currentPosition.X + negative.X * i, currentPosition.Y + negative.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant.ActorId != piece.ActorId)
                    {
                        validMoves.Add(nextPosition);
                    }
                    break;  // Stop if a piece is encountered
                }
                validMoves.Add(nextPosition);
            }
        }
        return validMoves;
    }
}
