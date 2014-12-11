using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Map;
using FurryLana.Engine.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Entity
{
    public class Floor : Tile, IEntity
    {
        public Floor(TiledMap map, IModel model, bool walkable) : base (map,model,walkable) { }
        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public int ID
        {
            get { return 0; }
        }

        public BoundingBox BBox { get; set; }

        public void Draw()
        {
            Model.Draw();
        }

        public void Update(UpdateDescription desc)
        {

        }

        public void FrameSyncedUpdate(float deltaTime)
        {

        }

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init()
        {
            //Model.Load();

        }

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <param name="list"></param>
        /// <returns>
        /// The init jobs.
        /// </returns>
        public List<Action> GetInitJobs(List<Action> list)
        {
            list.Add(Init);
            return list;
        }

        public void Load()
        {
            Loaded = false;

        }

        public List<Action> GetLoadJobs(List<Action> list, EventHandler reloader)
        {
            list.Add(Load);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy()
        {
            Model.Destroy();
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 SmoothedPosition { get; set; }

        public float Height { get; set; }

        public Vector3 Rotation { get; set; }

        public string Name { get; set; }
    }
}
