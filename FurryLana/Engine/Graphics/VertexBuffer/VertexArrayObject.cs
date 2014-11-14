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
    public class VertexArrayObject
    {
        public static void UnbindVAO()
        {
            GL.BindVertexArray(0);
        }
        public VertexArrayObject()
        {
            ID = -1;
        }
        public int ID { get; private set; }
        //Map of Integer (VBOId vs. VertexFormatInfo)
        private Dictionary<int, Pair<VertexBufferTarget, VertexFormatInfo>> vertexStructs = new Dictionary<int, Pair<VertexBufferTarget, VertexFormatInfo>>();
        private bool needsReload;
        [Obsolete("Prefer generic method")]
        public void AttachVBO(VertexFormatInfo info, int VBOId, VertexBufferTarget idTarget = VertexBufferTarget.DataBuffer)
        {
            vertexStructs.Add(VBOId, new Pair<VertexBufferTarget, VertexFormatInfo>(idTarget, info));
            needsReload = true;
        }
        public void AttachVBO<T>(VertexBuffer<T> buf) where T:struct
        {
            vertexStructs.Add(buf.ID, new Pair<VertexBufferTarget, VertexFormatInfo>(buf.Target, buf.VertexFormatInfo));
            needsReload = true;
        }
        [Obsolete("Prefer generic method")]
        public void DetachVBO(int VBOId)
        {
            vertexStructs.Remove(VBOId);
            needsReload = true;
        }
        public void DetachVBO<T>(VertexBuffer<T> buf) where T:struct
        {
            vertexStructs.Remove(buf.ID);
            needsReload = true;
        }
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
        public bool Loaded { get; private set; }
    }
}
