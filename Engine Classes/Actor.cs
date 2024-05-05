using System.Numerics;

namespace EpicTileEngine
{
    internal class Actor
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public List<TileObject> TileObjects { get; protected set; }

        public Actor(int id, string name)
        {
            Id = id;
            Name = name;
            TileObjects = new List<TileObject>();
        }

        public void AddTileObject(TileObject tileObject)
        {
            if (!TileObjects.Contains(tileObject))
            {
                TileObjects.Add(tileObject);
                tileObject.ActorId = this.Id;
            }
        }
    }
}