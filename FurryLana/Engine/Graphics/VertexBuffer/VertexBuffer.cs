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
    public class VertexBuffer<T> where T: struct
    {
        public VertexBufferTarget Target { get; private set; }
        public int Size { get; private set; }
        public int Count { get { return lastIndex; } }
        public VertexBufferType Usage { get; private set; }
        public int ID { get; private set; }
        private T[] data;
        private int lastIndex;
        private bool modified = false;
        public VertexFormatInfo VertexFormatInfo { get; private set; }
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
        public bool Loaded { get; private set; }
        #endregion
    }
}

