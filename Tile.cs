using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGameEngine
{


    public class Tile
    {
        public Vector2 Position { get; set; }
        public bool IsWalkable { get => isWalkable;}
        public TileObject TileObject;
        bool isWalkable;
        char WhatToDisplay 
        {
            get 
            { 
                if (TileObject == null)
                {
                    return ' ';
                }
                else
                {
                    return TileObject.TileObjectIcon;
                }
            }
        }
        public Tile(Vector2 position)
        {
            this.Position = position;
        }

        public override string ToString()
        {
            return $" [ {WhatToDisplay} ] ";
        }



    }
}
