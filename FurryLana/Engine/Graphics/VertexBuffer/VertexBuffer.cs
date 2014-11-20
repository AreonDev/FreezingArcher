//
//  VertexBuffer.cs
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
using System.Runtime.InteropServices;
using Pencil.Gaming.Graphics;

namespace FurryLana.Engine.Graphics.VertexBuffer
{
    /// <summary>
    /// Vertex buffer.
    /// </summary>
    public class VertexBuffer<T> where T: struct
    {
        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public VertexBufferTarget Target { get; private set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; private set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get { return lastIndex; } }

        /// <summary>
        /// Gets the usage.
        /// </summary>
        /// <value>The usage.</value>
        public VertexBufferType Usage { get; private set; }
        /// <summary>
        /// Gets the I.
        /// </summary>
        /// <value>The I.</value>
        public int ID { get; private set; }
        private T[] data;
        private int lastIndex;
        private bool modified = false;
        /// <summary>
        /// Gets the vertex format info.
        /// </summary>
        /// <value>The vertex format info.</value>
        public VertexFormatInfo VertexFormatInfo { get; private set; }
        /// <summary>
        /// Initializes a new instance of the VertexBuffer.
        /// </summary>
        /// <param name="usage">Usage.</param>
        /// <param name="maxCount">Max count.</param>
        /// <param name="vertexInfo">Vertex info.</param>
        /// <param name="target">Target.</param>
        public VertexBuffer(VertexBufferType usage, int maxCount, VertexFormatInfo vertexInfo, VertexBufferTarget target = VertexBufferTarget.DataBuffer)
        {
            this.Target = target;
            ID = -1;
            VertexFormatInfo= vertexInfo;
            data = new T[maxCount];
            Size = Marshal.SizeOf(data[0]);
            Usage = usage;
        }
        #region IGraphicsResource Member
        private bool dataLoaded = false;
        /// <summary>
        /// Loads the data.
        /// </summary>
        /// <returns>The data.</returns>
        /// <param name="d">D.</param>
        public int LoadData(IEnumerable<T> d)
        {
            if (Usage == VertexBufferType.Static && dataLoaded) throw new InvalidOperationException("Data is already loaded.");
            dataLoaded = true;
            lock (data)
            {
                int start = lastIndex;
                foreach (var item in d)
                {
                    data[lastIndex++] = item;
                }
                modified = true;
                return start;
            }
        }
        /// <summary>
        /// Bind this instance.
        /// </summary>
        public void Bind()
        {
            GL.BindBuffer((BufferTarget)Target, ID);
            if (modified)
            {
                lock (data)
                {
                    GL.BufferData((BufferTarget)Target, new IntPtr(lastIndex * Size), data, (BufferUsageHint)Usage);
                    modified = false;
                }
            }
        }
        /// <summary>
        /// Load this instance.
        /// </summary>
        public void Load()
        {
            if (ID == -1)
            {
                int id;
                GL.GenBuffers(1, out id);
                ID = id;
            }
            if (modified)
            {
                lock (data)
                {
                    GL.BindBuffer((BufferTarget)Target, ID);
                    GL.BufferData((BufferTarget)Target, new IntPtr(lastIndex * Size), data, (BufferUsageHint)Usage);
                    modified = false;
                }
            }
            //if (Usage == VertexBufferType.Static) data = null;
            Loaded = true;
        }
        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void Destroy()
        {
            if (ID != -1)
            {
                int id = ID;
                GL.DeleteBuffers(1, ref id);
                ID = -1;
            }
            Loaded = false;
        }
        /// <summary>
        /// Gets a value indicating whether this VertexBuffer is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; private set; }
        #endregion
    }
}

