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
    [Serializable]
    [StructLayout (LayoutKind.Sequential)]
    public struct Vertex
    {
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
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float PositionW;

        public float NormalX;
        public float NormalY;
        public float NormalZ;

        public float TexCoordX;
        public float TexCoordY;
#else
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
#endif
    }
}
