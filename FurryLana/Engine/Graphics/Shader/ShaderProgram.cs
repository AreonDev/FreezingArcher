//
//  ShaderProgram.cs
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
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace FurryLana.Engine.Graphics.Shader
{
    /// <summary>
    /// Shader program.
    /// </summary>
    public class ShaderProgram
    {
        private List<Shader> unatt = new List<Shader>();
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.Shader.ShaderProgram"/> class.
        /// </summary>
        /// <param name="shaders">Shaders.</param>
        public ShaderProgram(params Shader[] shaders)
        {
            program_id = -1;
            this.shaders = new List<Shader>(shaders.Length);
            this.unatt = new List<Shader>(shaders);
            Loaded = false;
        }
        /// <summary>
        /// Use this instance.
        /// </summary>
        public IDisposable Use()
        {
            if (!linked) Link();
            IDisposable r = new ShaderProgram.Handle(curr_program);
            if (curr_program != ID)
            {
                GL.UseProgram(ID);
                curr_program = ID;
            }
            return r;
        }
        /// <summary>
        /// Clear this instance.
        /// </summary>
        public static void Clear()
        {
            GL.UseProgram(curr_program = 0);
        }
        /// <summary>
        /// Link this instance.
        /// </summary>
        public void Link()
        {
            unifmap = new Dictionary<string, int>();
            GL.LinkProgram(ID);
            linked = true;
            int status;
            GL.GetProgram(ID, ProgramParameter.LinkStatus, out status);
            if (status == 0)
                throw new Exception(
                    String.Format("Error linking program: {0}", GL.GetProgramInfoLog(ID)));
        }
        /// <summary>
        /// Binds the attrib location.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="name">Name.</param>
        public void BindAttribLocation(int index, string name)
        {
            GL.BindAttribLocation(ID, index, name);
        }
        /// <summary>
        /// Binds the attrib location.
        /// </summary>
        /// <param name="attribs">Attribs.</param>
        public void BindAttribLocation(params Tuple<int, string>[] attribs)
        {
            foreach (Tuple<int, string> t in attribs)
            {
                BindAttribLocation(t.Item1, t.Item2);
            }
        }
        /// <summary>
        /// Binds the frag data location.
        /// </summary>
        /// <param name="color">Color.</param>
        /// <param name="name">Name.</param>
        public void BindFragDataLocation(int color, string name)
        {
            GL.BindFragDataLocation(ID, color, name);
        }
        /// <summary>
        /// Binds the frag data location.
        /// </summary>
        /// <param name="attribs">Attribs.</param>
        public void BindFragDataLocation(params Tuple<int, string>[] attribs)
        {
            foreach (Tuple<int, string> t in attribs)
            {
                BindFragDataLocation(t.Item1, t.Item2);
            }
        }
        /// <summary>
        /// Transforms the feedback varyings.
        /// </summary>
        /// <param name="mode">Mode.</param>
        /// <param name="varyings">Varyings.</param>
        public void TransformFeedbackVaryings(TransformFeedbackMode mode, params string[] varyings)
        {
            GL.TransformFeedbackVaryings(ID, varyings.Length, varyings, mode);
        }
        private void Detach(IEnumerable<Shader> shaders)
        {
            foreach (Shader s in shaders)
            {
                Shaders.Remove(s);
                GL.DetachShader(ID, s.ID);
            }
        }
        private void Attach(IEnumerable<Shader> shaders)
        {
            foreach (Shader s in shaders)
            {
                Shaders.Add(s);
                GL.AttachShader(ID, s.ID);
            }
        }
        private int program_id;
        /// <summary>
        /// Gets the I.
        /// </summary>
        /// <value>The I.</value>
        public int ID
        {
            get
            {
                return program_id;
            }
        }
        private List<Shader> shaders;
        /// <summary>
        /// Gets the shaders.
        /// </summary>
        /// <value>The shaders.</value>
        public List<Shader> Shaders
        {
            get
            {
                return shaders;
            }
        }
        private static int curr_program = 0;
        private Dictionary<string, int> unifmap;
        /// <summary>
        /// Sets the <see cref="FurryLana.Engine.Graphics.Shader.ShaderProgram"/> with the specified uniform.
        /// </summary>
        /// <param name="uniform">Uniform.</param>
        public unsafe object this[string uniform]
        {
            set
            {
                var p = Use();
                int uid;
                if (unifmap.ContainsKey(uniform))
                    uid = unifmap[uniform];
                else
                    uid = unifmap[uniform] = GL.GetUniformLocation(ID, uniform);
                int len = 0;
                Type t = value.GetType();
                if (t.IsArray)
                    len = ((Array)value).Length;
                if (t == typeof(int))
                    GL.Uniform1(uid, (int)value);
                else if (t == typeof(uint))
                    GL.Uniform1(uid, (uint)value);
                else if (t == typeof(float))
                    GL.Uniform1(uid, (float)value);
                else if (t == typeof(Vector2))
                    GL.Uniform2(uid, (Vector2)value);
                else if (t == typeof(Vector3))
                    GL.Uniform3(uid, (Vector3)value);
                else if (t == typeof(Vector4))
                    GL.Uniform4(uid, (Vector4)value);
                else if (t == typeof(Quaternion))
                    GL.Uniform4(uid, (Quaternion)value);
                else if (t == typeof(Color4))
                    GL.Uniform4(uid, (Color4)value);
                else if (t == typeof(Matrix))
                {
                    Matrix m = (Matrix) value;
                    GL.UniformMatrix4(uid, false, ref m);
                }
                else if (t == typeof(int[]))
                    GL.Uniform1(uid, len, (int[])value);
                else if (t == typeof(uint[]))
                    GL.Uniform1(uid, len, (uint[])value);
                else if (t == typeof(float[]))
                    GL.Uniform1(uid, len, (float[])value);
                else if (t == typeof(Color4))
                {
                    GL.Uniform4(uid, (Color4) value);
                }
                else
                    Console.Error.WriteLine("Unhandled type {0} for glUniform", t.FullName);
                p.Dispose ();
            }
        }
        private class Handle : IDisposable
        {
            private int prev;
            public Handle(int prev)
            {
                this.prev = prev;
            }
            public void Dispose()
            {
                if (ShaderProgram.curr_program != prev)
                {
                    ShaderProgram.curr_program = prev;
                    GL.UseProgram(prev);
                }
            }
        }
        #region IGraphicsResource Member
        private bool linked = false;
        /// <summary>
        /// Load this instance.
        /// </summary>
        public void Load()
        {
            if (Loaded) return;
            if(program_id == -1)
                program_id = (int) GL.CreateProgram();
            foreach (var item in unatt)
                if (!item.Loaded) item.Load();
            Attach(unatt);
            Loaded = true;
        }
        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void Destroy()
        {
            if (!Loaded) return;
            foreach (Shader s in Shaders)
                s.Destroy();
            Shaders.Clear();
            if (ID >= 0)
                GL.DeleteProgram(ID);
            program_id = -1;
            Loaded = false;
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="FurryLana.Engine.Graphics.Shader.ShaderProgram"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; private set; }
        #endregion
    }
}

