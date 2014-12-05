using FurryLana.Engine.Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurryLana.Engine.Entity
{
    class Floor : IEntity
    {
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

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public Model.Interfaces.IModel Model { get; set; }

        public BoundingBox BBox { get; set; }

        public void Draw()
        {
            Model.Draw();
        }

        public void Update(int deltaTime)
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

        public EventHandler NeedsLoad
        { get; set; }

        public Pencil.Gaming.MathUtils.Vector3 Position { get; set; }

        public Pencil.Gaming.MathUtils.Vector3 Rotation
        {
            get { throw new NotImplementedException(); }
        }

        public string Name { get; set; }
    }
}
