//
//  VertexAttribParam.cs
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

namespace FurryLana.Engine.Graphics.VertexBuffer
{
    /// <summary>
    /// Vertex attrib parameter.
    /// </summary>
    public class VertexAttribParam
    {
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size { get; private set; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public PointerType Type { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FurryLana.Engine.Graphics.VertexBuffer.VertexAttribParam"/>
        /// is normalized.
        /// </summary>
        /// <value><c>true</c> if normalized; otherwise, <c>false</c>.</value>
        public bool Normalized { get; private set; }

        /// <summary>
        /// Gets the stride.
        /// </summary>
        /// <value>The stride.</value>
        public int Stride { get; private set; }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public int Offset { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.VertexBuffer.VertexAttribParam"/> class.
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="size">Size.</param>
        /// <param name="stride">Stride.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="normalized">If set to <c>true</c> normalized.</param>
        /// <param name="type">Type.</param>
        public VertexAttribParam (int index, int size, int stride, int offset, bool normalized = false,
                                  PointerType type = PointerType.Float)
        {
            Index = index;
            Size = size;
            Type = type;
            Normalized = normalized;
            Stride = stride;
            Offset = offset;
        }
    }
}
