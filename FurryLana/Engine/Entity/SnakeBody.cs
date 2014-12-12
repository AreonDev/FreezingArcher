using System;
using System.Collections.Generic;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Entity
{
    public class SnakeBody : IEntity
    {
        public SnakeBody ()
        {
            BBox = new BoundingBox (Vector3.Zero);
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            SmoothedPosition = Vector3.Zero;
            Height = 1;
            Name = "foo";
            stone = new DummyGraphicsResource ();
        }

        protected DummyGraphicsResource stone;

        public int ID
        {
            get { return 2; }
        }

        public Model.Interfaces.IModel Model { get; set; }

        public BoundingBox BBox { get; set; }

        public void Draw()
        {
            stone.Draw ();
        }

        public void FrameSyncedUpdate(float deltaTime)
        {
            stone.FrameSyncedUpdate (deltaTime);
        }

        public void Init()
        {}

        public List<Action> GetInitJobs(List<Action> list)
        {
            list.Add(Init);
            stone.GetInitJobs (list);
            return list;
        }

        public void Load()
        {
            Loaded = true;
        }

        public List<Action> GetLoadJobs(List<Action> list, EventHandler reloader)
        {
            list.Add(Load);
            NeedsLoad = reloader;
            stone.GetLoadJobs (list, reloader);
            return list;
        }

        public void Destroy()
        {
            stone.Destroy ();
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public string Name { get; set; }

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {
            stone.Update (desc);
        }

        #endregion

        #region IEntity implementation

        public float Height { get; set; }

        #endregion

        #region ISmoothedPosition implementation

        public Vector3 SmoothedPosition { get; set; }

        #endregion
    }
}
