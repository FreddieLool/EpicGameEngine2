using EpicGameEngine;

class Renderer
{
    Tilemap currentTilemap;

    void Update()
    {
        for (int i = 0; i < currentTilemap.Width; i++)
        {
            for (int j = 0; j < currentTilemap.Height; j++)
            {
                Console.Write(currentTilemap.Board[j, i].ToString());
            }
        }
    }


}
