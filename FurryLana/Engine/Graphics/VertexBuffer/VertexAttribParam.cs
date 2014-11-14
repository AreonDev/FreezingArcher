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
    public class VertexAttribParam
    {
        public int Index { get; private set; }
        public int Size { get; private set; }
        public PointerType Type { get; private set; }
        public bool Normalized { get; private set; }
        public int Stride { get; private set; }
        public int Offset { get; private set; }

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
