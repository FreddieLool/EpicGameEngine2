using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGameEngine
{
    internal class ChessDemo : Tilemap
    {
        public override int Height { get; set; }
        public override int Width { get; set; }
        public override Tile[,] Board { get; protected set ; }
        public override TileObject[,] tileObjects { get ; protected set; }


        public ChessDemo(int Height, int Width)
        {
            this.Height = Height;
            this.Width = Width;
            Board = new Tile[Width, Height];
            InitalizeMap();
        }
        public override void InitalizeMap()
        {
            base.InitalizeMap();
            PlaceTileOjects(1,2);

        }
        private void PlaceTileOjects(int firstRow,int lastRow)
        {
            for (int i = firstRow; i < lastRow; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Tile tile = Board[i, j];
                    if (tile != null)
                    {
                        SetTileObject(new Pawn(tile));

                    }
                }
            }
        }
        public override void SetTile(Vector2 position, Tile tile)
        {
            Board[position.x, position.y] = tile;

        }

      

       

    }
}
