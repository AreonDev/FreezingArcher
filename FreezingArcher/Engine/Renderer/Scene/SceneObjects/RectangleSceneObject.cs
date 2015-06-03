//
//  RectangleSceneObject.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
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
using FreezingArcher.Math;

namespace FreezingArcher.Renderer.Scene.SceneObjects
{
    public class RectangleSceneObject : ISceneObject
    {
        public Vector3 Position{get; set;}
        public Vector3 Rotation{ get; set;}

        /// <summary>
        /// Size of the Rectangle
        /// </summary>
        /// <value>The scaling.</value>
        public Vector3 Scaling{ get; set;}

        public float LineWidth{ get; set;}
        public bool Filled{ get; set;}
        public Color4 Color{ get; set;}

        public RectangleSceneObject()
        {
        }

        public void Draw(RendererContext rc)
        {
            Vector2 pos = Position.Xy;
            Vector2 size = Scaling.Xy;
            Color4 col = Color;

            if (Filled)
                rc.DrawRectangleAbsolute(ref pos, ref size, LineWidth, ref col);
            else
                rc.DrawFilledRectangleAbsolute(ref pos, ref size, ref col);
        }

        public string GetName() {return "RectangleSceneObject";}
    }
}

