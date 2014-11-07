//
//  FirstPersonCamera.cs
//
//  Author:
//       Paul Stang <>
//
//  Copyright (c) 2014 Paul Stang
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

using FurryLana.Engine.Camera.Interfaces;
using Pencil.Gaming.MathUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurryLana.Engine.Camera
{
    public class FirstPersonCamera : ICamera
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Engine.Camera.FirstPersonCamera"/> class.
        /// </summary>
        /// <param name="name">The camera name used to identify it inside a camera manager.</param>
        /// <param name="character">The character the camera is tracked to (must not be null).</param>
        public FirstPersonCamera(string name)
        {

            Name = name;
        }


        public Pencil.Gaming.MathUtils.Matrix ViewMatrix
        {
            get
            {
                return ViewMatrix;
//                throw new NotImplementedException();
            }
            set;
        }

        public Pencil.Gaming.MathUtils.Vector3 Position
        {
            get { throw new NotImplementedException(); }
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public void Update(int deltaTime)
        {
            throw new NotImplementedException();
        }

        public void FrameSyncedUpdate(float deltaTime)
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public bool Loaded
        {
            get { throw new NotImplementedException(); }
        }

        public EventHandler NeedsLoad
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Enable()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
