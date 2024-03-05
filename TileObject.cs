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


}
