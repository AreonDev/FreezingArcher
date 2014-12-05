//
//  DummyGraphicsResource.cs
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
using System.Linq;
using FurryLana.Engine.Graphics.Interfaces;
using FurryLana.Engine.Model.Interfaces;
using FurryLana.Engine.Model;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics.Shader;
using Pencil.Gaming;

namespace FurryLana.Engine.Graphics
{
    /// <summary>
    /// Dummy graphics resource.
    /// </summary>
    public class DummyGraphicsResource : IGraphicsResource
    {
        /// <summary>
        /// The model.
        /// </summary>
        protected IModel model;

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {
            Loaded = false;
            //model = new AssimpModel ("Model/Data/Stone.obj");
            //model.Init ();
            t = new FinDreieck ();
            NeedsLoad ((Action) this.Load, null);
        }

        FinDreieck t;

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
            //model.Load ();
            t.Init ();
            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">The NeedsLoad event handler.</param>
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
            Loaded = false;
            //model.Destroy ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Graphics.DummyGraphicsResource"/>
        /// is loaded.
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
            if (model != null && model.Loaded)
                model.FrameSyncedUpdate (deltaTime);
        }

        #endregion

        #region IUpdate implementation

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="deltaTime">Time delta in miliseconds.</param>
        public void Update (int deltaTime)
        {
            if (model != null && model.Loaded)
                model.Update (deltaTime);
        }

        #endregion

        #region IDrawable implementation

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {
            t.Update ();
            /*
            Vector4[] triangle = new Vector4[3];
            triangle[0].X = -1.0f;
            triangle[0].Y = -1.0f;
            triangle[1].X = 1.0f;
            triangle[1].Y =-1.0f;
            triangle[2].X = -1.0f;
            triangle[2].Y = 1.0f;

            Shader.Shader vsh = new Shader.Shader (FurryLana.Engine.Graphics.Shader.ShaderType.VertexShader,
                                                   "Graphics/Shader/RenderTarget/stdmodel.vsh");
            Shader.Shader fsh = new Shader.Shader (FurryLana.Engine.Graphics.Shader.ShaderType.FragmentShader,
                                                   "Graphics/Shader/RenderTarget/stdmodel.fsh");
            
            ShaderProgram shp = new ShaderProgram (vsh, fsh);
            
            shp.Load ();
            shp.Link ();

            using (var foo = shp.Use ())
            {
                int vaoID = GL.GenVertexArray ();
                GL.BindVertexArray (vaoID);

                int vboID = GL.GenBuffer ();
                GL.BindBuffer (BufferTarget.ArrayBuffer, vboID);

                //GL.BufferData (BufferTarget.ArrayBuffer, new IntPtr (triangle.Length * sizeof (float) * 3), trptr, BufferUsageHint.StaticDraw);
                GL.BufferData<Vector4> (BufferTarget.ArrayBuffer, new IntPtr (Marshal.SizeOf(typeof(Vector4))), triangle, BufferUsageHint.StaticDraw);

                GL.EnableVertexAttribArray (0);
                GL.VertexAttribPointer (0, 4, VertexAttribPointerType.Float);

                GL.BindVertexArray(vaoID);

                GL.DrawArrays (BeginMode.Triangles, 0, 3);
            }*/

            /*
            if (model != null && model.Loaded)
            {
                model.Draw ();
            }*/
        }

        #endregion
    }

    class FinDreieck
    {
        private static GlfwWindowPtr bla_window;
        
        private static int vertex_array_buffer = 0;
        private static int vertex_buffer_id = 0;
        private static float[] vertex_data = 
        {
            -1.0f, -1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
            0.0f, 1.0f, 0.0f
        };
        
        private static uint program_id = 0;
        private const string VertexShader_Path = "VertexShader.vs";
        private const string PixelShader_Path = "PixelShader.fs";
        Matrix ProjMatrix = Matrix.CreatePerspectiveFieldOfView (1, 16f / 9, 0.1f, 200);
        Matrix ViewMatrix = Matrix.LookAt (0,1,1,
                                        0,0,0,
                                        0,1,0);
        Matrix ModelMatrix = Matrix.Identity;

        public void Init ()
        {
            InitVertexBufferFoo();
            program_id = LoadShaders();
        }
        
        public void foomain(string[] bla)
        {
            #region Initialize
            try
            {
                if (!Glfw.Init())
                {
                    Console.WriteLine("Failed to initialize Glfw!");
                    Glfw.Terminate();
                }
            }catch
            {
                Console.WriteLine("Failed to initialize Glfw!");
                Glfw.Terminate();
            }
            
            try
            {
                Glfw.WindowHint(WindowHint.ContextVersionMajor, 4);
                Glfw.WindowHint(WindowHint.ContextVersionMinor, 0);
                Glfw.WindowHint(WindowHint.OpenGLDebugContext, 1);
                
                bla_window = Glfw.CreateWindow(500, 500, "Bla Window!", GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
                
                Glfw.MakeContextCurrent(bla_window);
                
                Glfw.SetWindowSizeCallback(bla_window, WindowResize);
                
                Glfw.SetTime(0.0);
            }
            catch 
            {
                Console.WriteLine("Failed to initialize Rest...");
                Glfw.DestroyWindow(bla_window);
                Glfw.Terminate();
            }
            #endregion
            
            InitVertexBufferFoo();
            program_id = LoadShaders();
            
            while(!Glfw.WindowShouldClose(bla_window))
            {
                Glfw.PollEvents();
                
                Update();
            }
            
            Glfw.DestroyWindow(bla_window);
            Glfw.Terminate();
        }
        
        private void InitVertexBufferFoo()
        {
            vertex_array_buffer = GL.GenVertexArray();
            
            GL.BindVertexArray(vertex_array_buffer);
            
            vertex_buffer_id = GL.GenBuffer();
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer_id);

            GL.Utils.LoadModel ("Model/Data/cube.obj", out vrt, out norm, out crds, out idx);

            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer, (IntPtr) (sizeof (float) * 4 * vrt.Length), vrt, BufferUsageHint.StaticDraw);
        }

        int[] idx;
        Vector4[] vrt;
        Vector2[] crds;
        Vector3[] norm;
        
        private uint LoadShaders()
        {
            uint VertexShaderID = GL.CreateShader(Pencil.Gaming.Graphics.ShaderType.VertexShader);
            uint PixelShaderID = GL.CreateShader(Pencil.Gaming.Graphics.ShaderType.FragmentShader);
            
            string vertex_text = System.IO.File.ReadAllText(FinDreieck.VertexShader_Path);
            string pixel_text = System.IO.File.ReadAllText(FinDreieck.PixelShader_Path);

            GL.ShaderSource(VertexShaderID, vertex_text);
            GL.CompileShader(VertexShaderID);
            
            string info = "";
            GL.GetShaderInfoLog((int)VertexShaderID, out info);
            Console.WriteLine("Messages: " + info);
            
            GL.ShaderSource(PixelShaderID, pixel_text);
            GL.CompileShader(PixelShaderID);
            
            GL.GetShaderInfoLog((int)PixelShaderID, out info);
            Console.WriteLine("Messages: " + info);
            
            uint Program = GL.CreateProgram();
            
            GL.AttachShader(Program, VertexShaderID);
            GL.AttachShader(Program, PixelShaderID);

            GL.BindAttribLocation (Program, 0, "ProjMatrix");
            GL.BindAttribLocation (Program, 1, "ViewMatrix");
            GL.BindAttribLocation (Program, 2, "ModelMatrix");

            ErrorCode ec = GL.GetError();
            if (ec != ErrorCode.NoError)
                Console.WriteLine("Error: " + ec.ToString());

            GL.UniformMatrix4 (0, false, ref ProjMatrix );//FIXME
            GL.UniformMatrix4 (1, false, ref ViewMatrix );//FIXME
            GL.UniformMatrix4 (2, false, ref ModelMatrix);//FIXME

            ec = GL.GetError();
            if (ec != ErrorCode.NoError)
                Console.WriteLine("Error: " + ec);

            GL.LinkProgram(Program);
            
            GL.GetProgramInfoLog((int)Program, out info);
            Console.WriteLine("Messages: " + info);
            
            GL.DeleteShader(VertexShaderID);
            GL.DeleteShader(PixelShaderID);
            
            GL.ShaderSource(PixelShaderID, pixel_text);
            
            return Program;
        }
        
        public void Update()
        {
            GL.Enable (EnableCap.DepthTest);
            GL.CullFace (CullFaceMode.FrontAndBack);
            GL.ClearColor(0.2f, 0.1f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            ErrorCode ec = GL.GetError();
            if (ec != ErrorCode.NoError)
                Console.WriteLine("Error: " + ec.ToString());
            
            GL.UseProgram(program_id);
            
            ec = GL.GetError();
            if (ec != ErrorCode.NoError)
                Console.WriteLine("Error: " + ec.ToString());
            
            GL.BindVertexArray(vertex_buffer_id);
            
            GL.EnableVertexAttribArray(0);
            
            ec = GL.GetError();
            if (ec != ErrorCode.NoError)
                Console.WriteLine("Error: " + ec.ToString());
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer_id);
            
            ec = GL.GetError();
            if (ec != ErrorCode.NoError)
                Console.WriteLine("Error: " + ec.ToString());
            
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 4, 0);
            
            ec = GL.GetError();
            if (ec != ErrorCode.NoError)
                Console.WriteLine("Error: " + ec.ToString());
            
            GL.DrawElements<int> (BeginMode.Triangles, idx.Length, DrawElementsType.UnsignedInt, idx);
            
            ec = GL.GetError();
            if (ec != ErrorCode.NoError)
                Console.WriteLine("Error: " + ec.ToString());
            
            GL.DisableVertexAttribArray(0);
            
            ec = GL.GetError();
            if (ec != ErrorCode.NoError)
                Console.WriteLine("Error: " + ec.ToString());
            
            //Glfw.SwapBuffers(bla_window);
            //System.Threading.Thread.Sleep(50);
        }
        
        private static void WindowResize(GlfwWindowPtr window, int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }
    }
}
