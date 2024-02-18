using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicGameEngine
{
    public abstract class Tilemap
    {
        public abstract int Height { get; set; }
        public abstract int Width { get; set; }
        private Tile[,] board ;
        

        public Tilemap(int Height,int Widht) { 
            this.Height = Height;
            this.Width = Widht;
            board = new Tile[Widht,Height];
            InitalizeMap();
        }
        public virtual void InitalizeMap()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    board[Width, Height] = new Tile(new Vector2(i,j));
                }
            }
        }
        public virtual void SetTile(Vector2 position,Tile tile)
        {
            board[position.x, position.y] = tile;
           
        }

    }
}

public struct Vector2 {
    public int x;
    public int y;
    public Vector2(int x,int y)
    {
        this.x = x;
        this.y = y;
    }

}
