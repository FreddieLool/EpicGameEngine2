using EpicGameEngine;

class MoveableObject : TileObject
{
    public MoveableObject(Tile tile) : base(tile)
    {

    }

    public override void OnCollision(TileObject otherTileObj)
    {
        if (this.Tile.Position.x == otherTileObj.Tile.Position.x && this.Tile.Position.y == otherTileObj.Tile.Position.y)
        {
            if (otherTileObj.GetType() == typeof(MoveableObject))
            {
                // Make an interaction between it and other tile object
            }
            else if (otherTileObj.GetType() == typeof(UnmoveableObject))
            {
                // Make an interaction based on this
            }
        }
    }
    public virtual void MoveTo(Tile tile)
    {
        this.Tile = tile;
        this.Tile.TileObject = this;
        // Make TileObject move to destination Tile
        if (tile.TileObject != null)
        {
            // then it means there is already a tile object there
        }
    }
    public virtual bool IsTileAValidDest(Tile tile)
    {
        if (tile.IsWalkable == true)
        {
            return true;
        }
        return false;
    }
}
