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
    public class Skybox : IGraphicsResource, IPosition
    {
        public Skybox (float size, string fragmentShader, string vertexShader,
                       string xPos, string xNeg,
                       string yPos, string yNeg,
                       string zPos, string zNeg)
        {
            Vector4[] verts = new Vector4[]
            {
                new Vector4 (-size,  size,  size, size),
                new Vector4 ( size,  size,  size, size),
                new Vector4 ( size,  size, -size, size),
                new Vector4 (-size,  size, -size, size),
                new Vector4 (-size, -size,  size, size),
                new Vector4 ( size, -size,  size, size),
                new Vector4 ( size, -size, -size, size),
                new Vector4 (-size, -size, -size, size)
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

        protected GraphicsObject Graphics;
        protected CubeTexture CubeTexture;

        #region IPosition implementation

        public Vector3 Position { get; set; }

        #endregion

        #region IResource implementation

        public void Init ()
        {
            Engine.Application.Application.Instance.ResourceManager.TextureManager.Add (CubeTexture);
        }

        public List<Action> GetInitJobs (List<Action> list)
        {
            list.Add (Init);
            Graphics.GetInitJobs (list);
            return list;
        }

        public void Load ()
        {}

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            Graphics.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy ()
        {
            Graphics.Destroy ();
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        public void FrameSyncedUpdate (float deltaTime)
        {
            Graphics.FrameSyncedUpdate (deltaTime);
        }

        #endregion

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {
            Graphics.Update (desc);
        }

        #endregion

        #region IDrawable implementation

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
