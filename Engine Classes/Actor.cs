using System.Collections.Generic;
using System.Numerics;

namespace EpicTileEngine
{
    /// <summary>
    /// Represents an actor in the tile engine. An actor controls a set of tile objects and has an identity.
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// Gets the unique identifier of the actor.
        /// </summary>
        public int Id { get; protected set; }

        /// <summary>
        /// Gets the name of the actor.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the list of tile objects controlled by the actor.
        /// </summary>
        public List<TileObject> TileObjects { get; protected set; }

        /// <summary>
        /// Indicates whether the actor is actively playing or not.
        /// </summary>
        public bool IsPlaying { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class with the specified ID and name.
        /// </summary>
        /// <param name="id">The unique identifier of the actor.</param>
        /// <param name="name">The name of the actor.</param>
        public Actor(int id, string name)
        {
            Id = id;
            Name = name;
            TileObjects = new List<TileObject>();
        }

        /// <summary>
        /// Adds a tile object to the list of tile objects controlled by the actor.
        /// </summary>
        /// <param name="tileObject">The tile object to be added.</param>
        public void AddTileObject(TileObject tileObject)
        {
            if (!TileObjects.Contains(tileObject))
            {
                TileObjects.Add(tileObject);
                tileObject.ActorId = this.Id;
            }
        }

        /// <summary>
        /// Changes the playing status of the actor.
        /// </summary>
        /// <param name="status">True to mark the actor as playing; otherwise, false.</param>
        public void ChangePlayingStatus(bool status)
        {
            IsPlaying = status;
        }
    }
}