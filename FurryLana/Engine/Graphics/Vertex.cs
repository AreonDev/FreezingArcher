//
//  Vertex.cs
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
#define BASE_TYPES
using System;
using System.Runtime.InteropServices;
using Pencil.Gaming.MathUtils;

namespace FurryLana.Engine.Graphics
{
    /// <summary>
    /// Vertex.
    /// </summary>
    [Serializable]
    [StructLayout (LayoutKind.Sequential)]
    public struct Vertex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Graphics.Vertex"/> struct.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="normal">Normal.</param>
        /// <param name="texCoord">Tex coordinate.</param>
        public Vertex (Vector4 position, Vector3 normal, Vector2 texCoord)
        {
#if BASE_TYPES
            PositionX = position.X;
            PositionY = position.Y;
            PositionZ = position.Z;
            PositionW = position.W;

            NormalX = normal.X;
            NormalY = normal.Y;
            NormalZ = normal.Z;

            TexCoordX = texCoord.X;
            TexCoordY = texCoord.Y;
#else
            Position = position;
            Normal = normal;
            TexCoord = texCoord;
#endif
        }

#if BASE_TYPES
        /// <summary>
        /// The position x.
        /// </summary>
        public float PositionX;
        /// <summary>
        /// The position y.
        /// </summary>
        public float PositionY;
        /// <summary>
        /// The position z.
        /// </summary>
        public float PositionZ;
        /// <summary>
        /// The position w.
        /// </summary>
        public float PositionW;

        /// <summary>
        /// The normal x.
        /// </summary>
        public float NormalX;
        /// <summary>
        /// The normal y.
        /// </summary>
        public float NormalY;
        /// <summary>
        /// The normal z.
        /// </summary>
        public float NormalZ;

        /// <summary>
        /// The tex coordinate x.
        /// </summary>
        public float TexCoordX;
        /// <summary>
        /// The tex coordinate y.
        /// </summary>
        public float TexCoordY;
#else
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
#endif
    }
}
