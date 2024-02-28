using EpicGameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Queen : TileObject
{
    public Queen(Tile tile) : base(tile)
    {
        tileObjectIcon = 'Q';
    }

    public override void OnCollision(TileObject otherTileObj)
    {
        throw new NotImplementedException();
    }
}
