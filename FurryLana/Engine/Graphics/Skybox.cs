//
//  Skybox.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;
using System.Collections.Generic;
using System.Drawing;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics.Interfaces;
using FurryLana.Engine.Interaction;
using FurryLana.Engine.Texture;

namespace FurryLana.Engine.Graphics
{
    /// <summary>
    /// Skybox.
    /// </summary>
    public class Skybox : IGraphicsResource, IPosition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.Skybox"/> class.
        /// </summary>
        /// <param name="size">Size.</param>
        /// <param name="fragmentShader">Fragment shader.</param>
        /// <param name="vertexShader">Vertex shader.</param>
        /// <param name="xPos">Positive X.</param>
        /// <param name="xNeg">Negative X.</param>
        /// <param name="yPos">Positive Y.</param>
        /// <param name="yNeg">Negative Y.</param>
        /// <param name="zPos">Positive Z.</param>
        /// <param name="zNeg">Negative Z.</param>
        public Skybox (float size, string fragmentShader, string vertexShader,
                       string xPos, string xNeg,
                       string yPos, string yNeg,
                       string zPos, string zNeg)
        {
            Vector4[] verts = new Vector4[]
            {
                new Vector4 (-size,  size,  size, 1),
                new Vector4 ( size,  size,  size, 1),
                new Vector4 ( size,  size, -size, 1),
                new Vector4 (-size,  size, -size, 1),
                new Vector4 (-size, -size,  size, 1),
                new Vector4 ( size, -size,  size, 1),
                new Vector4 ( size, -size, -size, 1),
                new Vector4 (-size, -size, -size, 1)
            };

            Vertex[] vertices = new Vertex[verts.Length];
            for (int i = 0; i < verts.Length; i++)
            {
                vertices[i] = new Vertex (verts[i], new Vector3 (), new Vector2 ());
            }

            int[] indices = new int[]
            {
                7, 3, 0,  4, 7, 0, // front
                5, 1, 2,  6, 5, 2, // back
                4, 0, 1,  5, 4, 1, // left
                6, 2, 3,  7, 6, 3, // right
                2, 1, 0,  3, 2, 0, // top
                4, 5, 6,  7, 4, 6  // down
            };

            Graphics = new GraphicsObject (fragmentShader, vertexShader, vertices, indices, new string[0]);

            CubeTexture = new CubeTexture (new Bitmap (xPos), new Bitmap (xNeg),
                                           new Bitmap (yPos), new Bitmap (yNeg),
                                           new Bitmap (zPos), new Bitmap (zNeg), xPos);
        }

        /// <summary>
        /// The internal graphics object.
        /// </summary>
        protected GraphicsObject Graphics;

        /// <summary>
        /// The cube texture for the skybox.
        /// </summary>
        protected CubeTexture CubeTexture;

        #region IPosition implementation

        /// <summary>
        /// Position in space
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; set; }

        #endregion

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {
            Engine.Application.Application.Instance.ResourceManager.TextureManager.Add (CubeTexture);
        }

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            list.Add (Init);
            Graphics.GetInitJobs (list);
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {}

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            Graphics.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
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
        public void Destroy ()
        {
            Graphics.Destroy ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Graphics.Skybox"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate (float deltaTime)
        {
            Graphics.FrameSyncedUpdate (deltaTime);
        }

        #endregion

        #region IUpdate implementation

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="desc">Update description.</param>
        public void Update (UpdateDescription desc)
        {
            Graphics.Update (desc);
        }

        #endregion

        #region IDrawable implementation

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {
            GL.Disable (EnableCap.DepthTest);
            CubeTexture.Bind ();
            Graphics.Draw ();
            GL.Enable (EnableCap.DepthTest);
        }

        #endregion
    }
}
