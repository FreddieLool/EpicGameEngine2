using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGameEngine
{
    class Pawn : TileObject
    {
        public Pawn(Tile tile) : base(tile)
        {
            tileObjectIcon = '#';
        }

        public override void OnCollision(TileObject otherTileObj)
        {
            throw new NotImplementedException();
        }
    }
}
