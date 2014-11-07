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
using System.Linq;

namespace FurryLana.Engine.Camera
{
    /// <summary>
    /// First person camera.
    /// </summary>
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

        /// <summary>
        /// Get or set the cameras view matrix
        /// </summary>
        /// <value>The view matrix</value>
        public Matrix ViewMatrix
        {
            get
            {
                return ViewMatrix;
            }
            set
            {
                ViewMatrix = value;
            }
        }

        /// <summary>
        /// Position in space
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="deltaTime">Time delta in miliseconds.</param>
        public void Update(int deltaTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate(float deltaTime)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Init this resource. Initialzes the resource within a valid gl context.
        /// 
        /// Why not use the constructor?:
        /// The constructor may not have a valid gl context to initialize gl components.
        /// </summary>
        public void Init()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FurryLana.Engine.Camera.FirstPersonCamera"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
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

        /// <summary>
        /// This method is called when the camera manager switches to this subject.
        /// </summary>
        public void Enable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is called when the camera manager switches from this subject to another one.
        /// </summary>
        public void Disable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }
}
