//
//  PointerType.cs
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
    /// Represents the data type of the poitner
    /// </summary>
    public enum PointerType
    {
        /// <summary>The byte</summary>
        Byte = 5120,
        /// <summary>The unsigned byte</summary>
        /// <remarks>Use for color/attrib only!</remarks>
        UnsignedByte = 5121,
        /// <summary>The short</summary>
        Short = 5122,
        /// <summary>The unsigned short</summary>
        /// <remarks>Use for color/attrib only!</remarks>
        UnsignedShort = 5123,
        /// <summary>The int</summary>
        Int = 5124,
        /// <summary>The unsigned int</summary>
        /// <remarks>Use for color/attrib only!</remarks>
        UnsignedInt = 5125,
        /// <summary>The float</summary>
        Float = 5126,
        /// <summary>The double</summary>
        Double = 5130,
        /// <summary>The half float</summary>
        HalfFloat = 5131,
    }
}
