using FurryLana.Engine.Game.Interfaces;
using FurryLana.Engine.Input.Interfaces;
using FurryLana.Engine.Model;
using FurryLana.Engine.Model.Interfaces;
using FurryLana.Engine.Texture;
using FurryLana.Engine.Texture.Interfaces;
using System;
using System.Collections.Generic;

namespace FurryLana.Engine.Game
{

    /// <summary>
    /// ResourceManager implementation
    /// </summary>
    public class ResourceManager : IResourceManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager"/> class.
        /// </summary>
        public ResourceManager() {
//            InputManager = new InputManager();
            TextureManager = new TextureManager();
            ModelManager = new AssimpModelManager();
        }

        /// <summary>
        /// Gets the input manager.
        /// </summary>
        /// <value>
        /// The input manager.
        /// </value>
        public IInputManager InputManager { get; protected set; }

        /// <summary>
        /// Gets the texture manager.
        /// </summary>
        /// <value>
        /// The texture manager.
        /// </value>
        public ITextureManager TextureManager { get; protected set; }

        /// <summary>
        /// Gets the model manager.
        /// </summary>
        /// <value>
        /// The model manager.
        /// </value>
        public IModelManager ModelManager { get; protected set; }

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init()
        {}

        /// <summary>
        /// Gets the initialize jobs.
        /// </summary>
        /// <param name="list">The list.</param>
        public List<Action> GetInitJobs(List<Action> list)
        {
            list.Add(Init);
            list = TextureManager.GetLoadJobs (list);
            list = ModelManager.GetLoadJobs (list);
            list = InputManager.GetLoadJobs (list);
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
        /// <param name="list">The list.</param>
        /// <param name="reloader">The NeedsLoad event handler.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            list.Add(Load);
            NeedsLoad = reloader;
            list = TextureManager.GetLoadJobs (list, reloader);
            list = ModelManager.GetLoadJobs (list, reloader);
            list = InputManager.GetLoadJobs (list, reloader);
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy()
        {
            TextureManager.Destroy();
            ModelManager.Destroy();
            InputManager.Destroy();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Game.ResourceManager"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
        public EventHandler NeedsLoad { get; set; }
    }
}
