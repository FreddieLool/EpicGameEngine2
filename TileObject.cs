using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGameEngine
{
    public abstract class TileObject
    {
        public Tile Tile { get; private set; }

        public TileObject(Tile tile)
        {
            this.Tile = tile;
        }

        public abstract void OnCollision(TileObject otherTileObj)

    }


    public class ImmovableObject : TileObject
    {


    }

    public class MovableObject : TileObject
    {
        public override void OnCollision(TileObject otherTileObj)
        {
            if (this.Tile.Position.x == otherTileObj.Tile.Position.x && this.Tile.Position.y == otherTileObj.Tile.Position.y)
            {
                if (otherTileObj.GetType() == MovableObject)
                {
                    // Make an interaction between it and other tile object
                }
                else if (otherTileObj.GetType() == ImmovableObject)
                {
                    // Make an interaction based on this
                }
            }
        }
    }
}
