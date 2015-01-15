//
//  GraphicsObject.cs
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
#define Vertex

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics.Interfaces;
using FurryLana.Engine.Graphics.Shader;
using FurryLana.Engine.Graphics.VertexBuffer;
using FurryLana.Engine.Texture.Interfaces;
using FurryLana.Engine.Interaction;
using ShaderType = FurryLana.Engine.Graphics.Shader.ShaderType;

namespace FurryLana.Engine.Graphics
{
    /// <summary>
    /// Graphics object.
    /// </summary>
    public class GraphicsObject : IGraphicsResource, IPosition, IRotation
    {
        void Create (ShaderProgram shader, Vertex[] vertices, int[] indices, ITexture[] textures)
        {
            Shader = shader;
#if Vertex
            VBO = new VertexBuffer<Vertex> (VertexBufferType.Static, vertices.Length,
                                            new VertexFormatInfo 
                                            { VertexParams = new VertexAttribParam[] {
                    new VertexAttribParam (0, 4, Marshal.SizeOf (typeof (Vertex)), 0),
                    new VertexAttribParam (1, 3, Marshal.SizeOf (typeof (Vertex)), 4 * sizeof (float)),
                    new VertexAttribParam (2, 2, Marshal.SizeOf (typeof (Vertex)), 7 * sizeof (float))
                }});

            VBO.LoadData (vertices);
#else
            VBO = new VertexBuffer<Vector4> (VertexBufferType.Static, vertices.Length,
                                             new VertexFormatInfo {
                VertexParams = new VertexAttribParam[] {
                    new VertexAttribParam (0, 4, 16, 0)
                }
            });

            Vector4[] vrt = vertices.Select (s => s.Position).ToArray ();
            VBO.LoadData (vrt);
#endif

            IBO = new VertexBuffer<int> (VertexBufferType.Static, indices.Length,
                                         new VertexFormatInfo (), VertexBufferTarget.IndiceBuffer);
            IBO.LoadData (indices);
            VAO = new VertexArrayObject ();

            if (textures.Length > 32)
                throw new IndexOutOfRangeException ("You must not bind more than 32 textures to one graphics object!");

            Textures = textures;
        }

        void LoadModel (string modelPath, out Vertex[] vertices, out int[] indices)
        {
            Vector4[] vrt;
            Vector3[] norm;
            Vector2[] texcoord;
            GL.Utils.LoadModel (modelPath, out vrt, out norm, out texcoord, out indices, false);

            if (norm.Length == 0 && texcoord.Length == 0)
                vertices = Enumerable.Select (vrt, (v, i) => new Vertex (v, new Vector3 (0, 0, 0), new Vector2 (0, 0)))
                    .ToArray ();
            else if (norm.Length == 0)
                vertices = Enumerable.Select (vrt, (v, i) => new Vertex (v, new Vector3 (0, 0, 0), texcoord[i]))
                    .ToArray ();
            else if (texcoord.Length == 0)
                vertices = Enumerable.Select (vrt, (v, i) => new Vertex (v, norm[i], new Vector2 (0, 0))).ToArray ();
            else
                vertices = Enumerable.Select (vrt, (v, i) => new Vertex (v, norm[i], texcoord[i])).ToArray ();
        }

        ITexture[] LoadTextures (string[] texturePaths)
        {
            ITexture[] textures = new ITexture[texturePaths.Length];
            int i = 0;
            foreach (var path in texturePaths)
            {
                Engine.Application.Application.Instance.ResourceManager.TextureManager.LoadFromLocation (path);
                textures[i++] = Engine.Application.Application.Instance.ResourceManager.TextureManager.GetByName (path);
            }

            return textures;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.GraphicsObject"/> class.
        /// </summary>
        /// <param name="shader">Shader.</param>
        /// <param name="vertices">Vertices.</param>
        /// <param name="indices">Indices.</param>
        /// <param name="textures">Textures.</param>
        public GraphicsObject (ShaderProgram shader, Vertex[] vertices, int[] indices, ITexture[] textures)
        {
            DoLoad = () => {
                Create (shader, vertices, indices, textures);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.GraphicsObject"/> class.
        /// </summary>
        /// <param name="shader">Shader.</param>
        /// <param name="modelPath">Model path.</param>
        /// <param name="texturePaths">Texture paths.</param>
        public GraphicsObject (ShaderProgram shader, string modelPath, string[] texturePaths)
        {
            DoLoad = () => {
                Vertex[] vertices;
                int[] indices;
                LoadModel (modelPath, out vertices, out indices);

                ITexture[] textures = LoadTextures (texturePaths);

                Create (shader, vertices, indices, textures);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.GraphicsObject"/> class.
        /// </summary>
        /// <param name="fragmentShader">Fragment shader.</param>
        /// <param name="vertexShader">Vertex shader.</param>
        /// <param name="vertices">Vertices.</param>
        /// <param name="indices">Indices.</param>
        /// <param name="texturePaths">Texture paths.</param>
        public GraphicsObject (string fragmentShader, string vertexShader, Vertex[] vertices, int[] indices,
                               string[] texturePaths)
        {
            DoLoad = () => {
                ShaderProgram shader = new ShaderProgram (new Shader.Shader (ShaderType.FragmentShader, fragmentShader),
                                                          new Shader.Shader (ShaderType.VertexShader, vertexShader));

                ITexture[] textures = LoadTextures (texturePaths);

                Create (shader, vertices, indices, textures);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.GraphicsObject"/> class.
        /// </summary>
        /// <param name="fragmentShader">Fragment shader.</param>
        /// <param name="vertexShader">Vertex shader.</param>
        /// <param name="modelPath">Model path.</param>
        /// <param name="textures">Textures.</param>
        public GraphicsObject (string fragmentShader, string vertexShader, string modelPath, ITexture[] textures)
        {
            DoLoad = () => {
                ShaderProgram shader = new ShaderProgram (new Shader.Shader (ShaderType.FragmentShader, fragmentShader),
                                                          new Shader.Shader (ShaderType.VertexShader, vertexShader));

                Vertex[] vertices;
                int[] indices;
                LoadModel (modelPath, out vertices, out indices);

                Create (shader, vertices, indices, textures);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.GraphicsObject"/> class.
        /// </summary>
        /// <param name="shader">Shader.</param>
        /// <param name="vertices">Vertices.</param>
        /// <param name="indices">Indices.</param>
        /// <param name="texturePaths">Texture paths.</param>
        public GraphicsObject (ShaderProgram shader, Vertex[] vertices, int[] indices, string[] texturePaths)
        {
            DoLoad = () => {
                ITexture[] textures = LoadTextures (texturePaths);

                Create (shader, vertices, indices, textures);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.GraphicsObject"/> class.
        /// </summary>
        /// <param name="fragmentShader">Fragment shader.</param>
        /// <param name="vertexShader">Vertex shader.</param>
        /// <param name="vertices">Vertices.</param>
        /// <param name="indices">Indices.</param>
        /// <param name="textures">Textures.</param>
        public GraphicsObject (string fragmentShader, string vertexShader, Vertex[] vertices, int[] indices, ITexture[] textures)
        {
            DoLoad = () => {
                ShaderProgram shader = new ShaderProgram (new Shader.Shader (ShaderType.FragmentShader, fragmentShader),
                                                          new Shader.Shader (ShaderType.VertexShader, vertexShader));

                Create (shader, vertices, indices, textures);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.GraphicsObject"/> class.
        /// </summary>
        /// <param name="shader">Shader.</param>
        /// <param name="modelPath">Model path.</param>
        /// <param name="textures">Textures.</param>
        public GraphicsObject (ShaderProgram shader, string modelPath, ITexture[] textures)
        {
            DoLoad = () => {
                Vertex[] vertices;
                int[] indices;
                LoadModel (modelPath, out vertices, out indices);

                Create (shader, vertices, indices, textures);
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.GraphicsObject"/> class.
        /// </summary>
        /// <param name="fragmentShader">Fragment shader.</param>
        /// <param name="vertexShader">Vertex shader.</param>
        /// <param name="modelPath">Model path.</param>
        /// <param name="texturePaths">Texture paths.</param>
        public GraphicsObject (string fragmentShader, string vertexShader, string modelPath, string[] texturePaths)
        {
            DoLoad = () => {
                ShaderProgram shader = new ShaderProgram (new Shader.Shader (ShaderType.FragmentShader, fragmentShader),
                                                          new Shader.Shader (ShaderType.VertexShader, vertexShader));

                Vertex[] vertices;
                int[] indices;
                LoadModel (modelPath, out vertices, out indices);

                ITexture[] textures = LoadTextures (texturePaths);

                Create (shader, vertices, indices, textures);
            };
        }

        /// <summary>
        /// Gets or sets the shader.
        /// </summary>
        /// <value>The shader.</value>
        public ShaderProgram     Shader         { get; protected set; }
        /// <summary>
        /// Gets or sets the VA.
        /// </summary>
        /// <value>The VA.</value>
        public VertexArrayObject VAO            { get; protected set; }
        /// <summary>
        /// Gets or sets the textures.
        /// </summary>
        /// <value>The textures.</value>
        public ITexture[]        Textures       { get; set; }
        /// <summary>
        /// Gets or sets the light direction.
        /// </summary>
        /// <value>The light direction.</value>
        public Vector3           LightDirection { get; set; }

#if Vertex
        /// <summary>
        /// The vertex buffer object.
        /// </summary>
        protected VertexBuffer<Vertex> VBO;
#else
        protected VertexBuffer<Vector4> VBO;
#endif
        /// <summary>
        /// The index buffer object.
        /// </summary>
        protected VertexBuffer<int> IBO;

        /// <summary>
        /// The do load action.
        /// </summary>
        protected Action DoLoad;

        /// <summary>
        /// The sampler.
        /// </summary>
        public int Sampler;

        #region IPosition implementation

        /// <summary>
        /// Position in space
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; set; }

        #endregion

        #region IRotation implementation

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public Vector3 Rotation { get; set; }

        #endregion

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {
            Loaded = false;
            LightDirection = new Vector3 (0.6f, 0.6f, 0.6f);
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

            DoLoad ();

            VBO.Load ();
            IBO.Load ();
            VAO.AttachVBO (VBO);
            VAO.AttachVBO (IBO);
            VAO.Load ();
            VertexArrayObject.UnbindVAO ();
            Shader.Load ();

            //Sampler = GL.GenSampler ();
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureWrapS, (int) TextureWrapMode.Repeat);
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureWrapT, (int) TextureWrapMode.Repeat);
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureWrapR, (int) TextureWrapMode.Repeat);
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureMinFilter, (int) TextureMinFilter.Linear);
            //GL.SamplerParameter (Sampler, SamplerParameter.TextureMagFilter, (int) TextureMagFilter.Linear);
            //GL.BindSampler (0, Sampler);

            Shader["Ambient"] = 0.2f;
            Shader["LightDirection"] = LightDirection;
            //Shader["DiffuseTexture"] = Sampler;

            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
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
            VAO.Destroy ();
            VBO.Destroy ();
            IBO.Destroy ();
            Shader.Destroy ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Graphics.GraphicsObject"/> is loaded.
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

        //TODO: Update really needed??????? FUCK YOU!

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate (float deltaTime)
        {
            Shader["ViewMatrix"] = Engine.Application.Application.Instance.GameManager.CurrentGame
                .LevelManager.CurrentLevel.CameraManager.ActiveCamera.ViewMatrix;
            Shader["ProjMatrix"] = Engine.Application.Application.Instance.GameManager.CurrentGame
                .LevelManager.CurrentLevel.ProjectionMatrix;
            Shader["ModelMatrix"] = Matrix.CreateTranslation (Position) *
                Matrix.CreateRotationX (Rotation.X) *
                Matrix.CreateRotationY (Rotation.Y) *
                Matrix.CreateRotationZ (Rotation.Z);
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
        }

        #endregion

        /// <summary>
        /// The texture unit names for shader texture samplers.
        /// </summary>
        protected string[] textureUnitNames = new string[]
        {
            "DiffuseTexture",
            "AmbientTexture",
            "NormalHeightTexture",
            "SpecularTexture",
            "GeneralTexture00",
            "GeneralTexture01",
            "GeneralTexture02",
            "GeneralTexture03",
            "GeneralTexture04",
            "GeneralTexture05",
            "GeneralTexture06",
            "GeneralTexture07",
            "GeneralTexture08",
            "GeneralTexture09",
            "GeneralTexture10",
            "GeneralTexture11",
            "GeneralTexture12",
            "GeneralTexture13",
            "GeneralTexture14",
            "GeneralTexture15",
            "GeneralTexture16",
            "GeneralTexture17",
            "GeneralTexture18",
            "GeneralTexture19",
            "GeneralTexture20",
            "GeneralTexture21",
            "GeneralTexture22",
            "GeneralTexture23",
            "GeneralTexture24",
            "GeneralTexture25",
            "GeneralTexture26",
            "GeneralTexture27"
        };

        /// <summary>
        /// Gets the texture unit names.
        /// </summary>
        /// <value>The texture unit names.</value>
        public string[] TextureUnitNames
        {
            get
            {
                return textureUnitNames;
            }
        }

        #region IDrawable implementation

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {
            for (int i = 0; i < Textures.Length; i++)
            {
                TextureUnit unit = (TextureUnit) ((int) TextureUnit.Texture0 + i);
                GL.ActiveTexture (unit);
                Textures[i].Bind ();
                Shader[TextureUnitNames[i]] = i;
            }

            VAO.Bind ();
            using (var handle = Shader.Use ())
            {
                GL.DrawElements (BeginMode.Triangles, IBO.Count, DrawElementsType.UnsignedInt, 0);
            }
            VertexArrayObject.UnbindVAO ();
        }

        #endregion
    }
}
