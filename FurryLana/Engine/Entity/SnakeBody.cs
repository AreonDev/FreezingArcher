using System;
using System.Collections.Generic;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Graphics;

namespace FurryLana.Engine.Entity
{
    /// <summary>
    /// Snake body.
    /// </summary>
    public class SnakeBody : IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Entity.SnakeBody"/> class.
        /// </summary>
        public SnakeBody ()
        {
            BBox = new BoundingBox (Vector3.Zero);
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            SmoothedPosition = Vector3.Zero;
            Height = 0.5f;
            Name = "SnakeBody";
            stone = new DummyGraphicsResource ();
        }

        /// <summary>
        /// The stone.
        /// </summary>
        protected DummyGraphicsResource stone;

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public Model.Interfaces.IModel Model { get; set; }

        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        /// <value>The B box.</value>
        public BoundingBox BBox { get; set; }

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw()
        {
            stone.Draw ();
        }

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate(float deltaTime)
        {
            stone.FrameSyncedUpdate (deltaTime);
        }

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init()
        {}

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs(List<Action> list)
        {
            list.Add(Init);
            stone.GetInitJobs (list);
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load()
        {
            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs(List<Action> list, EventHandler reloader)
        {
            list.Add(Load);
            NeedsLoad = reloader;
            stone.GetLoadJobs (list, reloader);
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy()
        {
            stone.Destroy ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Entity.SnakeBody"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
        public EventHandler NeedsLoad { get; set; }

        /// <summary>
        /// Position in space
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public Vector3 Rotation { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #region IUpdate implementation

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="desc">Update description.</param>
        public void Update (UpdateDescription desc)
        {
            stone.Update (desc);
            stone.Position = Position;
            stone.Rotation = Rotation;
        }

        #endregion

        #region IEntity implementation

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height { get; set; }

        #endregion

        #region ISmoothedPosition implementation

        /// <summary>
        /// Gets or sets the smoothed position.
        /// </summary>
        /// <value>The smoothed position.</value>
        public Vector3 SmoothedPosition { get; set; }

        #endregion
    }
}
