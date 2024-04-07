namespace EpicTileEngine
{
    public interface ITileActionManager
    {
        // for custom movement validation logic
        Func<TileObject, Position, Tilemap, bool> ValidateMove { get; set; }

        // for handling interactions between tile obj
        Action<TileObject, TileObject> OnInteract { get; set; }

        // for custom logic after a move is executed
        Action<TileObject, Tile> OnMove { get; set; }

        // move to a new position
        bool TryMove(TileObject mover, Position targetPosition, Tilemap board);
    }
}
