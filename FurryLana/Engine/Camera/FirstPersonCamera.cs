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
using System.Collections.Generic;

namespace FurryLana.Engine.Camera
{
    /// <summary>
    /// First person camera.
    /// </summary>
    public class FirstPersonCamera : ICamera
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Camera.FirstPersonCamera"/> class.
        /// </summary>
        /// <param name="name">The camera name used to identify it inside a camera manager.</param>
        public FirstPersonCamera(string name)
        {
            Rotation = Vector3.Zero;
            Position = Vector3.Zero;
            Name = name;
        }

        /// <summary>
        /// Degree to radian function.
        /// </summary>
        protected Func<double, double> Deg2Rad = MathHelper.ToDegrees;

        /// <summary>
        /// Radian to degree function.
        /// </summary>
        protected Func<double, double> Rad2Deg = MathHelper.ToRadians;

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public Vector3 Rotation { get; protected set; }

        /// <summary>
        /// Rotate to a specific degree.
        /// </summary>
        /// <param name="degree">Degree.</param>
        /// <param name="angle">Angle.</param>
        public void RotTo(Vector3 degree , Math.AngleEnum angle)
        {
            if (angle == Math.AngleEnum.Degree)
            {
                Rotation = new Vector3((float)Deg2Rad(degree.X),
                                       (float)Deg2Rad(degree.Y),
                                       (float)Deg2Rad(degree.Z));
            }
            else
            {
                Rotation = degree;
            }
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
        public Vector3 Position{ get; set; }

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw(/*IRenderer renderer*/)
        {
            //renderer.SetViewMatrix(ViewMatix);
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
        /// The last update.
        /// </summary>
        protected int LastUpdate = 0;

        /// <summary>
        /// The elapsed time.
        /// </summary>
        protected float elapsedTime = 0;

        /// <summary>
        /// The last rotation.
        /// </summary>
        protected Vector3 lastRotation = Vector3.Zero;

        /// <summary>
        /// The last position.
        /// </summary>
        protected Vector3 lastPosition = Vector3.Zero;

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate(float deltaTime)
        {
            elapsedTime = deltaTime - LastUpdate;
            LastUpdate = (int)deltaTime;

            Position = new Vector3(Position.X,Position.Y,Position.Z);
            float magicSmoothing = (0.2f) * elapsedTime * 0.5f;
            Vector3 rotationdiff = Rotation -lastRotation;
            Vector3 rot = new Vector3(Rotation);
            rot = lastRotation + rotationdiff * magicSmoothing;

            float sinx = (float)System.Math.Sin(Position.X);
            float siny = (float)System.Math.Sin(Position.Y);
            float sinz = (float)System.Math.Sin(Position.Z);
            float cosx = (float)System.Math.Cos(Position.X);
            float cosy = (float)System.Math.Cos(Position.Y);
            float cosz = (float)System.Math.Cos(Position.Z);


            ViewMatrix = Matrix.CreateTranslation(new Vector3((float)-Position.X, (float)-Position.Y, (float)-Position.Z));
            ViewMatrix *= Matrix.CreateFromQuaternion(new Quaternion(sinx, 0, 0, cosx) *
                                                      new Quaternion(0, siny, 0, cosy) *
                                                      new Quaternion(0, 0, sinz, cosz));
        }

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init()
        {}

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load()
        {}

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

        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="FurryLana.Engine.Camera.FirstPersonCamera"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set;}

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
        { }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #region IResource implementation

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            list.Add (Init);
            return list;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">The NeedsLoad event handler.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            list.Add (Load);
            NeedsLoad = reloader;
            return list;
        }

        #endregion
    }
}
