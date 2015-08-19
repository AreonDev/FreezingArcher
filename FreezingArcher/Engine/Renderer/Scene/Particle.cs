//
//  Particle.cs
//
//  Author:
//       dboeg <>
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
using FreezingArcher.Core;

namespace FreezingArcher.Renderer.Scene
{
    public class Particle
    {
        public Vector2 Size { get; set;}

        public Vector3 Position { get; set;}
        public Vector3 Velocity { get; set;}
        public float Mass{ get; set;}

        public Color4 Color{ get; set;}

        public float Age { get; private set;}
        public float LifeTime { get; set;}
        public float Life {get; set;}

        public Particle ()
        {
            Size = new Vector2 (1.0f, 1.0f);
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Color = Color4.Black;
            Age = 0.0f;

            Life = 0.0f;
            LifeTime = 0.0f;
        }

        public void Update(float time)
        {
            Position += Velocity * time;
            Age += time;

            if(Age >= LifeTime)
                Life = 0.0f;
        }

        public void Reset()
        {
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Color = Color4.Black;

            Age = 0.0f;

            Life = 0.0f;
            LifeTime = 0.0f;
        }
    }
}

