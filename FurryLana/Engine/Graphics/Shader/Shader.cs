//
//  Shader.cs
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
using System.Text.RegularExpressions;
using System.IO;
using Pencil.Gaming.Graphics;
using System.Text;

namespace FurryLana.Engine.Graphics.Shader
{
    /// <summary>
    /// Shader.
    /// </summary>
    public class Shader
    {
        static Regex import_match = new Regex("#include *\\\"{0,1}(?<file>.*)\\\"{0,1}", RegexOptions.Compiled | RegexOptions.Multiline);
        static Regex lines = new Regex(@"\r\n|\n\r|\r|\n", RegexOptions.Compiled | RegexOptions.Multiline);
        private string source;
        private ShaderType type;
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.Shader.Shader"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="source">Source.</param>
        public Shader(ShaderType type, string source)
        {
            this.source = Process(source);
            this.type = type;
            Loaded = false;
        }
        private static string Process(string source)
        {
            var Lines = lines.Split(source);
            foreach (var line in Lines)
            {
                var match = import_match.Match(line);
                if (match.Success)
                {
                    FileInfo file = new FileInfo(match.Groups["file"].Value);
                    if (!file.Exists) throw new FileNotFoundException(file.Name);
                    return Process(source.Replace(line, File.ReadAllText(file.FullName)));
                }
            }
            return source;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.Shader.Shader"/> class.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="f">F.</param>
        public Shader(ShaderType type, FileInfo f)
            : this(type, File.ReadAllText(f.FullName, Encoding.UTF8))
        { }
        private int shader_id;
        /// <summary>
        /// Gets the I.
        /// </summary>
        /// <value>The I.</value>
        public int ID
        {
            get
            {
                return shader_id;
            }
        }
        /// <summary>
        /// Compiles the shader.
        /// </summary>
        protected void CompileShader()
        {
            shader_id = (int) GL.CreateShader((Pencil.Gaming.Graphics.ShaderType)type);
            GL.ShaderSource((uint) ID, source);
            GL.CompileShader(ID);
            int status;
            GL.GetShader(ID, ShaderParameter.CompileStatus, out status);
            if (status == 0)
                throw new Exception(
                    String.Format("Error compiling {0} shader: {1}\n{2}", type, GL.GetShaderInfoLog(ID), source));
            Loaded = true;
        }
        #region IGraphicsResource Member
        /// <summary>
        /// Load this instance.
        /// </summary>
        public void Load()
        {
            CompileShader();
        }
        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void Destroy()
        {
            if (shader_id >= 0)
                GL.DeleteShader(ID);
            shader_id = -1;
            Loaded = false;
        }
        /// <summary>
        /// Gets a value indicating whether this <see cref="FurryLana.Engine.Graphics.Shader.Shader"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; private set; }
        #endregion
    }
}

