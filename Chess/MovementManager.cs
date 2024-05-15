using EpicGameEngine;
using EpicTileEngine;
using System.Diagnostics;

public class MovementManager : TileActionManager
{
    private ChessTurnManager _chessTurnManager;

    // State variables for last move
    public bool LastMoveWasCapture = false;
    public bool LastMoveWasCheckmate = false;
    public ChessPiece? LastCapturedPiece = null;
    public Position LastCapturedPiecePosition;

    // Selection info
    public ChessPiece? SelectedPiece;
    public List<Position> HighlightedPositions = [];
    public bool ShowValidMovementsHighlighted = true;

    public MovementManager(ChessTurnManager chessTurnManager)
    {
        this._chessTurnManager = chessTurnManager;
        base.ValidateTileObjectMove = this.ValidateChessPieceMove;
        OnTileObjectInteract = this.HandleChessPieceInteraction;
        OnTileObjectMove = this.AfterMoveActions;
    }

    /// <summary>
    /// Checks if a move is valid based on the piece's possible moves.
    /// </summary>
    private bool IsValidMove(Position newPosition, ChessPiece piece, Tilemap board)
    {
        var validMoves = GetValidMoves(piece, board).ToList();
        return validMoves.Contains(newPosition);
    }

    /// <summary>
    /// Validates a move by temporarily performing it and checking for threats.
    /// </summary>
    private bool ValidateChessPieceMove(TileObject mover, Position targetPosition, Tilemap board)
    {
        ChessPiece? piece = mover as ChessPiece;
        if (IsValidMove(targetPosition, piece, board))
        {
            // Temporarily move to check for threats against the king
            Tile? originalTile = piece.CurrentTile;
            Tile targetTile = board.GetTile(targetPosition);
            TileObject? originalOccupant = targetTile.Occupant;

            // Perform temporary move
            targetTile.SetOccupant(mover);
            originalTile.RemoveOccupant();

            // Check for threats against the mover's king
            bool isMoveLegal = !IsKingInCheck(piece.ActorId, board);

            // Undo the temporary move
            originalTile.SetOccupant(mover);
            targetTile.SetOccupant(originalOccupant);

            return isMoveLegal;
        }
        return false;
    }

    /// <summary>
    /// Checks if the king of the given actor is in check.
    /// </summary>
    private bool IsKingInCheck(int actorId, Tilemap board)
    {
        ChessPiece king = FindKing(board, actorId);
        return CheckForThreats(king, board);
    }

    /// <summary>
    /// Handles interactions between pieces, primarily for captures.
    /// </summary>
    private bool HandleChessPieceInteraction(TileObject mover, TileObject occupant, Tilemap board)
    {
        ChessPiece? chessPiece = mover as ChessPiece; // Cast
        if (occupant is ChessPiece targetPiece)
        {
            if (targetPiece.ActorId != mover.ActorId)
            {
                bool isLegal = IsMoveLegal(chessPiece, targetPiece.CurrentTile.Position, board);
                Debug.WriteLine(isLegal ? "Interaction legal." : "Interaction results in illegal move.");
                if (!isLegal)
                {
                    return false; // Cannot capture because it would be illegal (self-check)
                }

                // Capture logic
                board[targetPiece.CurrentTile.Position].RemoveOccupant();
                LastCapturedPiece = targetPiece; // Update the last captured piece
                LastCapturedPiecePosition = targetPiece.CurrentTile.Position;
                LastMoveWasCapture = true; // Indicate that the last move was a capture
                Trace.WriteLine("Last move was capture");
                return true; // Capture is allowed
            }
        }
        return false; // No interaction possible (same player's piece)
    }

    /// <summary>
    /// Performs actions after a move, such as checking for pawn promotion.
    /// </summary>
    private void AfterMoveActions(TileObject piece, Tile newTile)
    {
        ChessPiece? chessPiece = piece as ChessPiece; // Cast
        if (chessPiece != null)
        {
            // Check for pawn promotion
            if (chessPiece.Type == PieceType.Pawn && (newTile.Position.Y == 0 || newTile.Position.Y == 7))
            {
                chessPiece.Promote(PieceType.Queen);
                chessPiece.Symbol = 'Q';
            }
        }
    }

    /// <summary>
    /// Checks if the king is under threat from any opposing piece.
    /// </summary>
    public bool CheckForThreats(ChessPiece king, Tilemap board)
    {
        Position kingPosition = king.CurrentTile.Position;
        foreach (var tile in board.GetAllTiles())
        {
            if (tile.Occupant is ChessPiece piece && piece.ActorId != king.ActorId)
            {
                // GetValidMoves is designed to get moves without checking king's check status
                var threats = GetValidMoves(piece, board);
                if (threats.Contains(kingPosition))
                {
                    return true;  // King is in check
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the opponent is in checkmate.
    /// </summary>
    public bool CheckForCheckmate(int opponentActorId, Tilemap chessBoard)
    {
        bool isKingInCheck = false;
        foreach (var piece in chessBoard.GetAllPiecesByActorId(opponentActorId))
        {
            var validMoves = GetValidMoves(piece, chessBoard);
            foreach (var move in validMoves)
            {
                if (IsMoveLegal(piece, move, chessBoard))
                {
                    return false;  // Found a legal move that avoids check
                }
            }
            if (piece.Type == PieceType.King)
            {
                isKingInCheck = CheckForThreats(piece, chessBoard);  // Check if the king is in check
            }
        }
        return isKingInCheck;  // If king is in check and no moves prevent check, it's checkmate
    }

    /// <summary>
    /// Checks if a move is legal by temporarily performing it and checking for threats.
    /// </summary>
    private bool IsMoveLegal(ChessPiece piece, Position targetPosition, Tilemap board)
    {
        Tile? originalTile = piece.CurrentTile;
        Tile targetTile = board.GetTile(targetPosition);
        TileObject? originalOccupant = targetTile.Occupant;

        // Perform temporary move
        targetTile.SetOccupant(piece);
        originalTile.RemoveOccupant();

        // Check for threats against the mover's king
        bool isMoveLegal = !IsKingInCheck(piece.ActorId, board);

        // Undo the temporary move
        originalTile.SetOccupant(piece);
        targetTile.SetOccupant(originalOccupant);

        return isMoveLegal;
    }

    /// <summary>
    /// Attempts to move a piece to a new position.
    /// </summary>
    public override bool TryMove(TileObject mover, Position targetPosition, Tilemap board)
    {
        // Validate if the move is legal based on the rules of chess
        if (!base.ValidateTileObjectMove(mover, targetPosition, board))
            return false;

        // Get the target tile and the current tile of the piece
        Tile targetTile = board.GetTile(targetPosition);
        Tile? currentTile = mover.CurrentTile;

        // Handle interaction (e.g., capturing an opponent's piece) before performing the move
        if (targetTile.Occupant != null)
        {
            if (!OnTileObjectInteract.Invoke(mover, targetTile.Occupant, board))
                return false;
        }

        // Move the piece to the target tile and remove it from the current tile
        targetTile.SetOccupant(mover);
        currentTile?.RemoveOccupant();

        // Invoke the OnMove event to perform any additional actions after the move (promotion!)
        OnTileObjectMove.Invoke(mover, targetTile);

        // Get the current player and the opponent player
        Actor currentPlayer = _chessTurnManager.GetPlayingActor();
        Actor opponentPlayer = (currentPlayer == _chessTurnManager.whitePlayer) ? _chessTurnManager.blackPlayer : _chessTurnManager.whitePlayer;

        // Check if the opponent's king is in check after the move
        if (IsKingInCheck(opponentPlayer.Id, board))
        {
            // Check for checkm8 or check
            if (CheckForCheckmate(opponentPlayer.Id, board))
            {
                CommandHandler.DisplayCenteredNotification($"Game Over - {currentPlayer.Name} checkmates {opponentPlayer.Name}!\n {currentPlayer.Name} wins! Congratulations.\n 'restart' to start a new game.");
            }
            else
            {
                CommandHandler.DisplayCenteredNotification($"{currentPlayer.Name} checks {opponentPlayer.Name}!");
            }
        }

        return true;
    }

    /// <summary>
    /// Finds the king for the specified actor.
    /// </summary>
    public ChessPiece FindKing(Tilemap board, int actorId)
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

    /// <summary>
    /// Gets valid moves for a specific chess piece.
    /// </summary>
    public IEnumerable<Position> GetValidMoves(ChessPiece piece, Tilemap board)
    {
        foreach (var (direction, maxSteps) in piece.MovementCapabilities)
        {
            for (int step = 1; step <= maxSteps; step++)
            {
                Position nextPosition = new Position(piece.CurrentTile.Position.X + direction.X * step, piece.CurrentTile.Position.Y + direction.Y * step);

                if (!board.IsPositionValid(nextPosition))
                    break;

                // Special check for pawn (captures only diagonally, moves 2 only at beginning)
                if (piece.Type == PieceType.Pawn)
                {
                    // Dynamic for any board size
                    int startingRowWhite = board.Height - 2;
                    int startingRowBlack = 1;

                    if (direction.X == 0) // Forward move
                    {
                        if (!board.IsTileOccupied(nextPosition))
                        {
                            // Allow two steps from starting position
                            if (step == 2)
                            {
                                if ((piece.Color == Color.White && piece.CurrentTile.Position.Y == startingRowWhite) ||
                                    (piece.Color == Color.Black && piece.CurrentTile.Position.Y == startingRowBlack))
                                {
                                    yield return nextPosition;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                yield return nextPosition;
                            }
                        }
                        else
                        {
                            break; // Stop if a piece is encountered
                        }
                    }
                    else // Diagonal move for capture
                    {
                        if (board.IsTileOccupied(nextPosition) && board.GetTile(nextPosition).Occupant?.ActorId != piece.ActorId)
                        {
                            yield return nextPosition; // Capture move
                        }
                        break; // Stop if a piece is encountered
                    }
                }
                else
                {
                    if (board.IsTileOccupied(nextPosition))
                    {
                        if (board.GetTile(nextPosition).Occupant?.ActorId != piece.ActorId)
                        {
                            yield return nextPosition; // Capture move
                        }
                        break; // Stop if a piece is encountered
                    }
                    yield return nextPosition; // Normal move
                }
            }
        }
    }


    /// <summary>
    /// Resets the selection and highlights.
    /// </summary>
    public void ResetSelectionAndState()
    {
        SelectedPiece = null;
        HighlightedPositions.Clear();
    }


    #region
    //
    // Chess Piece valid moves section
    // 1. Define movement dir
    // 2. Validate moves (within boundary, not blocked by any pieces)
    // 3. Gather all valid positions into a list
    //
    // not used anymore, movement capabilities are now defined in ChessPiece
    // to make use of engine base class AddMovementCapability in TileObject

    /// <summary>
    /// Gets valid moves for a pawn.
    /// </summary>
    private IEnumerable<Position> GetPawnMoves(ChessPiece piece, Tilemap board)
    {
        int direction = piece.Color == Color.White ? -1 : 1;
        Position currentPosition = piece.CurrentTile.Position;
        Position singleStepForward = new(currentPosition.X, currentPosition.Y + direction);

        // Single step forward
        if (board.IsPositionValid(singleStepForward) && !board.IsTileOccupied(singleStepForward))
        {
            yield return singleStepForward;
        }

        // Double step from start
        if ((piece.Color == Color.White && currentPosition.Y == 6) || (piece.Color == Color.Black && currentPosition.Y == 1))
        {
            Position doubleStepForward = new(currentPosition.X, currentPosition.Y + 2 * direction);
            if (board.IsPositionValid(doubleStepForward) && !board.IsTileOccupied(doubleStepForward) && !board.IsTileOccupied(singleStepForward))
            {
                yield return doubleStepForward;
            }
        }

        // Captures
        Position[] potentialCaptures = {
        new(currentPosition.X - 1, currentPosition.Y + direction),
        new(currentPosition.X + 1, currentPosition.Y + direction)
        };

        foreach (Position capturePos in potentialCaptures)
        {
            if (board.IsPositionValid(capturePos) && board.IsTileOccupied(capturePos) && board.GetTile(capturePos).Occupant.ActorId != piece.ActorId)
            {
                yield return capturePos;
            }
        }
    }

    /// <summary>
    /// Gets valid moves for a rook.
    /// </summary>
    private List<Position> GetRookMoves(ChessPiece piece, Tilemap board)
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
                Position nextPosition = new(currentPosition.X + positive.X * i, currentPosition.Y + positive.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant?.ActorId != piece.ActorId)
                    {
                        validMoves.Add(nextPosition);
                    }
                    break;  // Stop if a piece is encountered
                }
                validMoves.Add(nextPosition);
            }

            // Check negative direction (left or down)
            for (int i = 1; i < board.Width; i++)
            {
                Position nextPosition = new(currentPosition.X + negative.X * i, currentPosition.Y + negative.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant?.ActorId != piece.ActorId)
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

    /// <summary>
    /// Gets valid moves for a knight.
    /// </summary>
    private List<Position> GetKnightMoves(ChessPiece piece, Tilemap board)
    {
        List<Position> validMoves = [];
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
            Position nextPosition = new(currentPosition.X + move.X, currentPosition.Y + move.Y);
            if (board.IsPositionValid(nextPosition))  // Check if the position is on the board
            {
                if (!board.IsTileOccupied(nextPosition) || (board.GetTile(nextPosition).Occupant?.ActorId != piece.ActorId))
                {
                    // Add the position if it is not occupied or is occupied by an opponent's piece
                    validMoves.Add(nextPosition);
                }
            }
        }
        return validMoves;
    }

    /// <summary>
    /// Gets valid moves for a bishop.
    /// </summary>
    private static List<Position> GetBishopMoves(ChessPiece piece, Tilemap board)
    {
        List<Position> validMoves = [];
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
                // calculates potential positions by adding a dir vector (positive/negative) to the current pos
                Position nextPosition = new(currentPosition.X + positive.X * i, currentPosition.Y + positive.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant?.ActorId != piece.ActorId)
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
                Position nextPosition = new(currentPosition.X + negative.X * i, currentPosition.Y + negative.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant?.ActorId != piece.ActorId)
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

    /// <summary>
    /// Gets valid moves for a king.
    /// </summary>
    private List<Position> GetKingMoves(ChessPiece piece, Tilemap board)
    {
        List<Position> validMoves = [];
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
            Position nextPosition = new(currentPosition.X + move.X, currentPosition.Y + move.Y);
            if (board.IsPositionValid(nextPosition))  // Check if the position is on the board
            {
                if (!board.IsTileOccupied(nextPosition) || (board.GetTile(nextPosition).Occupant?.ActorId != piece.ActorId))
                {
                    // Add the position if it is not occupied or is occupied by an opponent's piece
                    validMoves.Add(nextPosition);
                }
            }
        }

        return validMoves;

    }

    /// <summary>
    /// Gets valid moves for a queen.
    /// </summary>
    private List<Position> GetQueenMoves(ChessPiece piece, Tilemap board)
    {
        List<Position> validMoves = [];
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
                Position nextPosition = new(currentPosition.X + positive.X * i, currentPosition.Y + positive.Y * i);
                if (!board.IsPositionValid(nextPosition))
                    break;

                if (board.IsTileOccupied(nextPosition))
                {
                    if (board.GetTile(nextPosition).Occupant?.ActorId != piece.ActorId)
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
                Position nextPosition = new(currentPosition.X + negative.X * i, currentPosition.Y + negative.Y * i);
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

    #endregion
}
