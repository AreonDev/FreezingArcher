using FurryLana.Engine.Game.Interfaces;
using FurryLana.Engine.Input.Interfaces;
using FurryLana.Engine.Model;
using FurryLana.Engine.Model.Interfaces;
using FurryLana.Engine.Texture;
using FurryLana.Engine.Texture.Interfaces;
using System;

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
        {
            TextureManager.Init();
            ModelManager.Init();
            InputManager.Init();
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load()
        {
            Loaded = false;
            TextureManager.Load();
            ModelManager.Load();
            InputManager.Load();
            Loaded = true;
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

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }
    }
}
