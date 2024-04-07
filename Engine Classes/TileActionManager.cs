namespace EpicTileEngine
{
    public class TileActionManager : ITileActionManager
    {
        // for custom movement validation logic
        public Func<TileObject, Position, Tilemap, bool> ValidateMove { get; set; }

        // for handling interactions between tile objects
        public Action<TileObject, TileObject> OnInteract { get; set; }

        // for custom logic after a move is executed
        public Action<TileObject, Tile> OnMove { get; set; }

        // move to a new position
        public virtual bool TryMove(TileObject mover, Position targetPosition, Tilemap board)
        {
            // is not valid move?
            if (ValidateMove != null && !ValidateMove(mover, targetPosition, board))
            {
                return false;
            }

            // p0s out of bounds?
            if (targetPosition.X < 0 || targetPosition.X >= board.Width ||
                targetPosition.Y < 0 || targetPosition.Y >= board.Height)
            {
                return false;
            }

            Tile targetTile = board.GetTile(targetPosition);

            // target tile = occupied? > handle interaction
            if (targetTile.Occupant != null)
            {
                OnInteract?.Invoke(mover, targetTile.Occupant);
            }

            // update the mover tile
            // assuming every interaction means the mover tileobj vacates his tile to interacted with tile.
            mover.SetTile(targetTile);

            OnMove?.Invoke(mover, targetTile);

            return true;
        }
    }
}
