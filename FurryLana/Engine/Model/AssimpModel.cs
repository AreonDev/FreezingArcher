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
using Pencil.Gaming.Graphics;

namespace FurryLana.Engine.Model
{
    /// <summary>
    /// Assimp model.
    /// </summary>
    public class AssimpModel : IModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Model.AssimpModel"/> class.
        /// </summary>
        /// <param name="location">The location of the model.</param>
        public AssimpModel (string location)
        {
            this.location = location;
            Loaded = false;
        }

        /// <summary>
        /// The model.
        /// </summary>
        protected Scene model;

        /// <summary>
        /// The location.
        /// </summary>
        protected string location;

        /// <summary>
        /// The assimp post process steps.
        /// </summary>
        protected PostProcessSteps AssimpPostProcessSteps;

        VertexFormatInfo       vfi; // vertex format info
        Vector3D[]             vrt; // vertices
        int[]                  idx; // indices
        VertexBuffer<Vector3D> vbo; // vertex buffer object
        VertexBuffer<int>      ibo; // index buffer object
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

            vsh = new Shader (FurryLana.Engine.Graphics.Shader.ShaderType.VertexShader,
                              "Graphics/Shader/RenderTarget/stdmodel.vsh");
            fsh = new Shader (FurryLana.Engine.Graphics.Shader.ShaderType.FragmentShader,
                              "Graphics/Shader/RenderTarget/stdmodel.fsh");

            shp = new ShaderProgram (vsh, fsh);

            shp.Load ();
            shp.Link ();

            shp["ProjMatrix"] = Matrix.CreatePerspectiveFieldOfView (1f, 16f/9f, 0.1f, 200f);
            shp["ViewMatrix"] = Matrix.LookAt (new Vector3 (0f, 1f, 0f),
                                               new Vector3 (0f, 0f, 0f),
                                               new Vector3 (0f, 1f, 0f));
	    shp["ModelMatrix"] = Matrix.Identity;

            vfi = new VertexFormatInfo ();

            vfi.VertexParams = new VertexAttribParam[]
            {
                new VertexAttribParam (0, 3, 3 * sizeof (float), 0)
            };

            List<Vector3D> lvrt = new List<Vector3D> ();
            List<int> lidx = new List<int> ();

            model.Meshes.ForEach (m => {
                lvrt.AddRange (m.Vertices);
                lidx.AddRange (m.GetIndices ());
            });

            vrt = lvrt.ToArray ();
            idx = lidx.ToArray ();

	    vbo = new VertexBuffer<Vector3D> (VertexBufferType.Static,
					      vrt.Length,
					      vfi,
					      VertexBufferTarget.DataBuffer);

	    ibo = new VertexBuffer<int> (VertexBufferType.Static,
                                         idx.Length,
                                         new VertexFormatInfo (),
                                         VertexBufferTarget.IndiceBuffer);

	    vao = new VertexArrayObject ();

	    vbo.LoadData (vrt);
	    ibo.LoadData (idx);
	    vbo.Load ();
	    ibo.Load ();
	    vao.Load ();
	    vao.AttachVBO (vbo);
	    vao.AttachVBO (ibo);

            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">The NeedLoad event handler.</param>
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
        {}

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
        {
	    if (!Loaded)
		return;

	    using (var shader = shp.Use ())
	    {
		vao.Bind ();
		GL.DrawElements (BeginMode.Triangles, idx.Length, DrawElementsType.UnsignedInt, 0);
	    }
	}

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
