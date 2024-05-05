using System.Diagnostics;

namespace EpicTileEngine
{
    public class TileActionManager
    {
        // for custom movement validation logic
        public Func<TileObject, Position, Tilemap, bool> ValidateMove { get; set; }

        // for handling interactions between tile objects
        public Func<TileObject, TileObject, Tilemap, bool> OnInteract { get; set; }


        // for custom logic after a move is executed
        public Action<TileObject, Tile> OnMove { get; set; }

        // move to a new position
        public virtual bool TryMove(TileObject mover, Position targetPosition, Tilemap board)
        {
            Trace.WriteLine($"Attempt to move from {mover.CurrentTile.Position} to {targetPosition}");
            if (!board.IsPositionValid(targetPosition))
            {
                Trace.WriteLine("Move out of bounds.");
                return false;
            }

            Tile targetTile = board.GetTile(targetPosition);
            if (!targetTile.IsPassable || (targetTile.Occupant != null && targetTile.Occupant.ActorId == mover.ActorId))
            {
                Trace.WriteLine("Move blocked or occupied by a friendly piece.");
                return false;
            }

            // Additional debug information:
            Trace.WriteLine($"Target tile passable: {targetTile.IsPassable}, Occupied by self: {targetTile.Occupant?.ActorId == mover.ActorId}");

            Position currentPos = mover.CurrentTile.Position;
            Position moveVector = new Position(targetPosition.X - currentPos.X, targetPosition.Y - currentPos.Y);
            int moveDistance = Math.Max(Math.Abs(moveVector.X), Math.Abs(moveVector.Y));

            bool isMoveAllowed = false;
            foreach (var (direction, maxSteps) in mover.MovementCapabilities)
            {
                if (moveVector.X == direction.X * moveDistance && moveVector.Y == direction.Y * moveDistance && moveDistance <= maxSteps)
                {
                    isMoveAllowed = true;
                    break;
                }
            }

            Trace.WriteLine($"Is move allowed: {isMoveAllowed}");
            if (!isMoveAllowed)
                return false;

            if (ValidateMove != null && !ValidateMove(mover, targetPosition, board))
            {
                Trace.WriteLine("Move failed validation.");
                return false;
            }

            bool shouldMove = true;  // Default to moving unless interaction dictates otherwise

            if (targetTile.Occupant != null)
            {
                shouldMove = OnInteract.Invoke(mover, targetTile.Occupant, board);
            }

            if (shouldMove)
            {
                mover.CurrentTile.RemoveOccupant();
                targetTile.SetOccupant(mover);
                OnMove?.Invoke(mover, targetTile);
            }
            else
            {
                Trace.WriteLine("Move blocked by interaction logic.");
            }

            return shouldMove;
        }



    }
}