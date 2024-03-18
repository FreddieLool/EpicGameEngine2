using EpicGameEngine;

class Renderer
{
    Tilemap _currentTilemap;

    public Renderer(Tilemap currentTileMap)
    {
        _currentTilemap = currentTileMap;
    }
    public void Update()
    {
        for (int i = 0; i < _currentTilemap.Width; i++)
        {
            for (int j = 0; j < _currentTilemap.Height; j++)
            {
             Tile tile =  _currentTilemap.getTile(new Vector2(i,j));
                if(tile != null)
                {
                    if(tile.TileObject != null)
                    {

                        Console.Write(tile.TileObject.TileObjectIcon);
                        Console.Write(' ');
                        
                    }
                    else
                    {
                        Console.Write(_currentTilemap.Board[i, j].ToString());
                    }
                }
               
               
            }
            Console.WriteLine();
        }
    }


}
