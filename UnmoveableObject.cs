using EpicGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    public class UnmoveableObject : TileObject
{
        public UnmoveableObject(Tile tile) : base(tile)
        {
        }

        public override void OnCollision(TileObject otherTileObj)
        {
            throw new NotImplementedException();
        }
    }
