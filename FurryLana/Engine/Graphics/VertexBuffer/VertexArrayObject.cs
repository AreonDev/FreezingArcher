//
//  VertexArrayObject.cs
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
using Pencil.Gaming.Graphics;
using System.Collections.Generic;
using FurryLana.Math;

namespace FurryLana.Engine.Graphics.VertexBuffer
{
    /// <summary>
    /// Vertex array object.
    /// </summary>
    public class VertexArrayObject
    {
        /// <summary>
        /// Unbinds the vertex array object.
        /// </summary>
        public static void UnbindVAO()
        {
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.VertexBuffer.VertexArrayObject"/> class.
        /// </summary>
        public VertexArrayObject()
        {
            ID = -1;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The I.</value>
        public int ID { get; private set; }
        //Map of Integer (VBOId vs. VertexFormatInfo)
        private Dictionary<int, Pair<VertexBufferTarget, VertexFormatInfo>> vertexStructs = new Dictionary<int, Pair<VertexBufferTarget, VertexFormatInfo>>();
        private bool needsReload;

        /// <summary>
        /// Attachs the VBO.
        /// </summary>
        /// <param name="info">Info.</param>
        /// <param name="VBOId">VBO identifier.</param>
        /// <param name="idTarget">Identifier target.</param>
        [Obsolete("Prefer generic method")]
        public void AttachVBO(VertexFormatInfo info, int VBOId, VertexBufferTarget idTarget = VertexBufferTarget.DataBuffer)
        {
            vertexStructs.Add(VBOId, new Pair<VertexBufferTarget, VertexFormatInfo>(idTarget, info));
            needsReload = true;
        }

        /// <summary>
        /// Attachs the VBO.
        /// </summary>
        /// <param name="buf">Buffer.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void AttachVBO<T>(VertexBuffer<T> buf) where T:struct
        {
            vertexStructs.Add(buf.ID, new Pair<VertexBufferTarget, VertexFormatInfo>(buf.Target, buf.VertexFormatInfo));
            needsReload = true;
        }

        /// <summary>
        /// Detachs the VBO.
        /// </summary>
        /// <param name="VBOId">VBO identifier.</param>
        [Obsolete("Prefer generic method")]
        public void DetachVBO(int VBOId)
        {
            vertexStructs.Remove(VBOId);
            needsReload = true;
        }

        /// <summary>
        /// Detachs the VBO.
        /// </summary>
        /// <param name="buf">Buffer.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void DetachVBO<T>(VertexBuffer<T> buf) where T:struct
        {
            vertexStructs.Remove(buf.ID);
            needsReload = true;
        }

        /// <summary>
        /// Bind this instance.
        /// </summary>
        public void Bind()
        {
            GL.BindVertexArray(ID);
            if (needsReload)
            {
                foreach (var item in vertexStructs)
                {
                    GL.BindBuffer ((BufferTarget)item.Value.A, item.Key);
                    var i = item.Value.B;
                    foreach (var attrib in i.VertexParams)
                    {
                        GL.EnableVertexAttribArray(attrib.Index);
                        GL.VertexAttribPointer(attrib.Index, attrib.Size, (VertexAttribPointerType)attrib.Type, attrib.Normalized, attrib.Stride, attrib.Offset);
                    }
                }
                needsReload = false;
            }
        }

        /// <summary>
        /// Load this instance.
        /// </summary>
        public void Load()
        {
            if (Loaded) return;
            if (ID == -1)
            {
                int id;
                GL.GenVertexArrays(1, out id);
                ID = id;
            }
            Loaded = true;
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public void Destroy()
        {
            if (!Loaded) return;
            if (ID != -1)
            {
                int id = ID;
                GL.DeleteVertexArrays(1, ref id);
                ID = -1;
            }
            Loaded = false;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FurryLana.Engine.Graphics.VertexBuffer.VertexArrayObject"/>
        /// is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; private set; }
    }
}
