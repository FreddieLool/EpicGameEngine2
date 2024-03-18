namespace EpicGameEngine
{
    public abstract class Tilemap  /*IEnumerable<Tile>*/
    {
        public abstract int Height { get; set; }
        public abstract int Width { get; set; }
        public abstract Tile[,] Board { get; protected set; }
        public abstract TileObject[,] tileObjects { get; protected set; }
        
       


        
        public virtual void InitalizeMap()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Board[i, j] = new Tile(new Vector2(i, j));
                }
            }
        }




        public abstract void SetTile(Vector2 position, Tile tile);

        public virtual void SetTileObject(TileObject tileObject)
        {

            Tile tile = tileObject.Tile;
            tile.TileObject = tileObject;
            

        }
      

        public virtual Tile getTile(Vector2 position)
        {
            return Board[position.x, position.y];
        }
        //public IEnumerator<Tile> GetEnumerator()
        //{
        //    // Implement your custom logic for traversing the Tilemap
        //    for (int y = 0; y < Height; y++)
        //    {
        //        for (int x = 0; x < Width; x++)
        //        {
        //            yield return board[x, y];
        //        }
        //    }
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}


        }
        //public void SaveTilemap(char[,] tilemap, string filePath)
        //{
        //    try
        //    {
        //        // Convert the tilemap to a 2D array of strings
        //        string[,] stringTilemap = new string[tilemap.GetLength(0), tilemap.GetLength(1)];
        //        for (int y = 0; y < tilemap.GetLength(1); y++)
        //        {
        //            for (int x = 0; x < tilemap.GetLength(0); x++)
        //            {
        //                stringTilemap[x, y] = tilemap[x, y].ToString();
        //            }
        //        }

        //        // Convert the 2D array to a JSON string
        //        string json = JsonSerializer.Serialize(stringTilemap);

        //        // Write the JSON string to a file
        //        File.WriteAllText(filePath, json);

        //        Console.WriteLine("Tilemap saved successfully as JSON.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error saving tilemap as JSON: {ex.Message}");
        //    }
        //}

        //public char[,] LoadTilemap(string filePath)
        //{
        //    try
        //    {
        //        // Read all lines from the file
        //        string[] lines = File.ReadAllLines(filePath);

        //        // Get the dimensions of the tilemap
        //        int width = lines[0].Length;
        //        int height = lines.Length;

        //        // Create a 2D array to store the tilemap
        //        char[,] tilemap = new char[width, height];

        //        // Fill the array with tilemap data
        //        for (int y = 0; y < height; y++)
        //        {
        //            for (int x = 0; x < width; x++)
        //            {
        //                tilemap[x, y] = lines[y][x];
        //            }
        //        }

        //        return tilemap;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error loading tilemap: {ex.Message}");
        //        return null;
        //    }
        //}
    }
public struct Vector2
{
    public int x;
    public int y;
    public Vector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}


