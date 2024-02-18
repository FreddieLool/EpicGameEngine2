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

        public Tile(Vector2 position)
        {
            this.Position = position;
        }
    }
}
