using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGameEngine
{
    public abstract class TileObject
    {
        public Tile Tile { get; protected set; }
        protected char tileObjectIcon;
        public char TileObjectIcon
        {
            get
            {
                return tileObjectIcon;
            }
        }
        public TileObject(Tile tile)
        {
            this.Tile = tile;
        }

        public abstract void OnCollision(TileObject otherTileObj);
    }


    public class ImmovableObject : TileObject
    {
        public ImmovableObject(Tile tile) : base(tile)
        {
        }

        public override void OnCollision(TileObject otherTileObj)
        {
            throw new NotImplementedException();
        }
    }

    public class MovableObject : TileObject
    {
        public MovableObject(Tile tile) : base(tile)
        {
        }

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
        public virtual void MoveTo(Tile tile)
        {
            this.Tile = tile;
            this.Tile.TileObject = this; 
            // Make TileObject move to destination Tile
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
}
