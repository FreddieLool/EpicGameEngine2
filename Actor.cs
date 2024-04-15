namespace EpicTileEngine
{
    internal class Actor
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<TileObject> TileObjects { get; private set; }

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
