namespace EpicTileEngine
{
    public class ConsoleRenderer : IRenderer
    {
        public virtual void Render(Tilemap tilemap)
        {
            for (int y = 0; y < tilemap.Height; y++)
            {
                for (int x = 0; x < tilemap.Width; x++)
                {
                    var tile = tilemap[new Position(x, y)];
                    if (tile.Occupant != null)
                    {
                        Console.Write($"[{tile.Occupant.Symbol}]");
                    }
                    else
                    {
                        Console.Write("[");
                        ConsoleRGB.Write("-", ConsoleColor.DarkGray);
                        Console.Write("]");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
