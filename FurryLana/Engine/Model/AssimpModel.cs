//
//  AssimpModel.cs
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
using System.IO;
using System.Linq;
using Assimp;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics.Shader;
using FurryLana.Engine.Graphics.VertexBuffer;
using FurryLana.Engine.Model.Interfaces;

namespace FurryLana.Engine.Model
{
    public class AssimpModel : IModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Model.AssimpModel"/> class.
        /// </summary>
        /// <param name="model">Model.</param>
        public AssimpModel (string location)
        {
            this.location = location;
            Loaded = false;
        }

        protected Scene model;
        protected string location;
        protected PostProcessSteps AssimpPostProcessSteps;

        VertexFormatInfo       vfi; // vertex format info
        Vector3D[]             vrt; // vertices
        byte[]                 idx; // indices
        VertexBuffer<Vector3D> vbo; // vertex buffer object
        VertexBuffer<byte>     ibo; // index buffer object
        VertexArrayObject      vao; // vertex array object
        Shader                 vsh; // vertex shader
        Shader                 fsh; // fragment shader
        ShaderProgram          shp; // shader program

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {
            AssimpPostProcessSteps =
                PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateNormals |
                    PostProcessSteps.OptimizeMeshes |
                    PostProcessSteps.JoinIdenticalVertices |
                    PostProcessSteps.ImproveCacheLocality;

            using (AssimpContext importer = new AssimpContext ())
            {
                try
                {
                    model = importer.ImportFile (location, AssimpPostProcessSteps);
                }
                catch (FileNotFoundException e)
                {
                    throw new FileNotFoundException ("Model file \"" + location + "\" was not found!", location, e);
                }
                catch (AssimpException e)
                {
                    throw new AssimpException ("Error during model loading via Assimp (see inner exception)!", e);
                }
                catch (ObjectDisposedException e)
                {
                    throw new ObjectDisposedException ("Invalid Assimp context!", e);
                }
                catch (Exception e)
                {
                    throw new Exception ("Unknown error during loading file \"" + location + "\" via Assimp!", e);
                }
                
                if (model == null)
                {
                    throw new Exception ("Unknown error during loading file \"" + location + "\" via Assimp!");
                }
            }
        }

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            list.Add (Init);
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {
            Loaded = false;

            vsh = new Shader (ShaderType.VertexShader, "Graphics/Shader/RenderTarget/stdmodel.vsh");
            fsh = new Shader (ShaderType.FragmentShader, "Graphics/Shader/RenderTarget/stdmodel.fsh");

            shp = new ShaderProgram (vsh, fsh);

            shp.Load ();
            shp.Link ();

            shp["ProjMatrix"] = Matrix.CreatePerspectiveFieldOfView (1f, 16f/9f, 0.1f, 200f);
            shp["ViewMatrix"] = Matrix.LookAt (new Vector3 (0f, 0f, 0f),
                                               new Vector3 (0f, 0f, 0f),
                                               new Vector3 (0f, 1f, 0f));

            vfi = new VertexFormatInfo ();
            vfi.VertexParams = new VertexAttribParam[]
            {
                new VertexAttribParam (0, 3, 3 * sizeof (float), 0)
            };

            vrt = model.Meshes.Select (j => j.Vertices).JoinSequence ().ToArray ();

            foreach (var v in vrt)
                Console.WriteLine ("X: {0}, Y: {1}, Z: {2}", v.X, v.Y, v.Z);

            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            list.Add (Load);
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
            model.Clear ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Model.AssimpModel"/> is loaded.
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
            throw new NotImplementedException ();
        }

        #endregion

        #region IUpdate implementation

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="deltaTime">Time delta in miliseconds.</param>
        public void Update (int deltaTime)
        {}

        #endregion

        #region IDrawable implementation

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {}

        #endregion

        #region IModel implementation

        /// <summary>
        /// Gets the animations.
        /// </summary>
        /// <value>The animations.</value>
        public List<Animation> Animations
        {
            get
            {
                return model.Animations;
            }
        }

        /// <summary>
        /// Gets the cameras.
        /// </summary>
        /// <value>The cameras.</value>
        public List<Assimp.Camera> Cameras
        {
            get
            {
                return model.Cameras;
            }
        }

        /// <summary>
        /// Gets the lights.
        /// </summary>
        /// <value>The lights.</value>
        public List<Light> Lights
        {
            get
            {
                return model.Lights;
            }
        }

        /// <summary>
        /// Gets the materials.
        /// </summary>
        /// <value>The materials.</value>
        public List<Assimp.Material> Materials
        {
            get
            {
                return model.Materials;
            }
        }

        /// <summary>
        /// Gets the meshes.
        /// </summary>
        /// <value>The meshes.</value>
        public List<Mesh> Meshes
        {
            get
            {
                return model.Meshes;
            }
        }

        /// <summary>
        /// Gets the textures.
        /// </summary>
        /// <value>The textures.</value>
        public List<EmbeddedTexture> Textures
        {
            get
            {
                return model.Textures;
            }
        }

        #endregion

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion
    }
}
