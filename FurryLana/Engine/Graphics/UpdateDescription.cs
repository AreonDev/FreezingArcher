//
//  UpdateDescription.cs
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
using FurryLana.Engine.Input;
using Pencil.Gaming.MathUtils;

namespace FurryLana.Engine.Graphics
{
    public class UpdateDescription
    {
        public UpdateDescription (List<KeyboardInput> keys, List<MouseInput> mouse,
                                  Vector2 mouseMovement, Vector2 mouseScroll, float deltaTime)
        {
            Keys = keys;
            Mouse = mouse;
            MouseMovement = mouseMovement;
            MouseScroll = mouseScroll;
            DeltaTime = deltaTime;
        }

        public List<KeyboardInput> Keys { get; protected set; }
        public List<MouseInput> Mouse { get; protected set; }
        public Vector2 MouseMovement { get; protected set; }
        public Vector2 MouseScroll { get; protected set; }
        public float DeltaTime { get; protected set; }
    }
}
