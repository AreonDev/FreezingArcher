//
//  ThirdPersonCamera.cs
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
using FurryLana.Engine.Camera.Interfaces;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Interaction;
using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Graphics;
using FurryLana.Math;

namespace FurryLana.Engine.Camera
{
    /// <summary>
    /// Third person camera.
    /// </summary>
    public class ThirdPersonCamera : ICamera, IRotateable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Camera.ThirdPersonCamera"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="boundTo">Bound to.</param>
        /// <param name="radius">Radius.</param>
        /// <param name="mouseSpeed">Mouse speed.</param>
        /// <param name="zoomSpeed">Zoom speed.</param>
        public ThirdPersonCamera (string name, IEntity boundTo, float radius, float mouseSpeed, float zoomSpeed)
        {
            if (boundTo == null)
                throw new ArgumentNullException ("boundTo", "You must specify an entity to track!");

            BoundTo = boundTo;
            Rotation = Vector3.Zero;
            Position = Vector3.Zero;
            Radius = radius;
            MouseSpeed = mouseSpeed;
            ZoomSpeed = zoomSpeed;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the bound to.
        /// </summary>
        /// <value>The bound to.</value>
        public IEntity BoundTo { get; set; }
        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public float Radius { get; set; }
        /// <summary>
        /// Gets or sets the mouse speed.
        /// </summary>
        /// <value>The mouse speed.</value>
        public float MouseSpeed { get; set; }
        /// <summary>
        /// Gets or sets the zoom speed.
        /// </summary>
        /// <value>The zoom speed.</value>
        public float ZoomSpeed { get; set; }

        /// <summary>
        /// The last rotation.
        /// </summary>
        protected Vector3 lastRotation = Vector3.Zero;

        /// <summary>
        /// The last position.
        /// </summary>
        protected Vector3 lastPosition = Vector3.Zero;

        /// <summary>
        /// The last radius.
        /// </summary>
        protected float lastRadius;

        /// <summary>
        /// The y coordinate.
        /// </summary>
        protected float yCoord;

        #region ICameraManageable implementation

        /// <summary>
        /// This method is called when the camera manager switches to this subject.
        /// </summary>
        public void Enable ()
        {
            Rotation = new Vector3 (Rotation.X, BoundTo.Rotation.Y, Rotation.Z);
        }

        /// <summary>
        /// This method is called when the camera manager switches from this subject to another one.
        /// </summary>
        public void Disable ()
        {}

        #endregion

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {}

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            return list;
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {}

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">Reloader.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            NeedsLoad = reloader;
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy ()
        {}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Camera.ThirdPersonCamera"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }

        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        /// <summary>
        /// This update is called before every frame draw inside a gl context.
        /// </summary>
        /// <param name="deltaTime">Time delta.</param>
        public void FrameSyncedUpdate (float deltaTime)
        {   
            float smoothing = deltaTime * 10f;
            smoothing = smoothing.Clamp (0f, 1f);
            
            // smooth zooming
            float zdiff = Radius - lastRadius; // zoom diff
            float zm = lastRadius + zdiff * smoothing;
            lastRadius = zm;
            Vector3 rotVec = new Vector3 (0, 0, zm / 4f);
            
            // smooth the rotation
            Vector3 rdiff = Rotation - lastRotation;
            Vector3 rot = lastRotation + rdiff * smoothing;
            lastRotation = new Vector3 (rot);
            
            Matrix rotMat = Matrix.CreateRotationX (rot.X);
            rotMat *= Matrix.CreateRotationY (rot.Y);
            rotMat *= Matrix.CreateRotationZ (rot.Z);
            
            Vector3 transVec = Vector3.Transform (rotVec, rotMat);
            Vector3 pos = new Vector3 (BoundTo.SmoothedPosition);
            pos.Y += BoundTo.Height;
            
            Vector3 camPos = transVec + new Vector3 (pos.X, pos.Y, pos.Z);
            float posy = pos.Y;
            if (camPos.Y < yCoord)
            {
                posy = pos.Y + System.Math.Abs (camPos.Y - yCoord) / 4;
                camPos.Y = yCoord;
            }
            Position = camPos;
            ViewMatrix = Matrix.LookAt (camPos,
                                        new Vector3 (pos.X, posy, pos.Z),
                                        new Vector3 (0.0f, 1.0f, 0.0f));
        }

        #endregion

        /// <summary>
        /// The first mouse.
        /// </summary>
        protected bool firstMouse = true;

        #region IUpdate implementation

        /// <summary>
        /// This update is called in an extra thread which does not have a valid gl context.
        /// The updaterate might differ from the framerate.
        /// </summary>
        /// <param name="desc">Update description.</param>
        public void Update (UpdateDescription desc)
        {
            if (!Application.Application.Instance.Window.IsMouseCaptured ())
                return;

            // prevent mouse jump on first frame
            if (!firstMouse)
            {
                RotTo (new Vector3 (Rotation.X - Deg2Rad (desc.MouseMovement.Y * desc.DeltaTime * MouseSpeed * 10),
                                    Rotation.Y - Deg2Rad (desc.MouseMovement.X * desc.DeltaTime * MouseSpeed * 10),
                                    Rotation.Z), AngleEnum.Radian);
                //if (rotCharacter)
                //    Character.Rotation.Y -= Deg2Rad (e.XDelta * elapsedTime * MouseSpeed);
            }
            else
                firstMouse = false;

            Radius += desc.MouseScroll.Y * ZoomSpeed * desc.DeltaTime * 15f;
        }

        #endregion

        #region IDrawable implementation

        /// <summary>
        /// Draw this instance.
        /// </summary>
        public void Draw ()
        {}

        #endregion

        #region ICamera implementation

        /// <summary>
        /// Get or set the cameras view matrix
        /// </summary>
        /// <value>The view matrix</value>
        public Matrix ViewMatrix { get; set; }

        #endregion

        #region IManageable implementation

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        #endregion

        #region IPosition implementation

        /// <summary>
        /// Position in space
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get; set; }

        #endregion

        /// <summary>
        /// Degree to radian.
        /// </summary>
        /// <returns>The radian.</returns>
        /// <param name="deg">The degree.</param>
        protected float Deg2Rad (float deg)
        {
            const float degToRad = (float) System.Math.PI / 180.0f;
            return deg * degToRad;
        }

        /// <summary>
        /// Radian to degree.
        /// </summary>
        /// <returns>The degree.</returns>
        /// <param name="rad">The radian.</param>
        protected float Rad2Deg (float rad)
        {
            const float radToDeg = 180.0f / (float)System.Math.PI;
            return rad * radToDeg;
        }

        #region IRotateable implementation

        /// <summary>
        /// Rotate to given angles
        /// </summary>
        /// <param name="rotation">Rotation x,y,z</param>
        /// <param name="angle">Angle format</param>
        public void RotTo (Vector3 rotation, AngleEnum angle = AngleEnum.Degree)
        {
            double oldRotX = Rotation.X;
            if (angle == AngleEnum.Degree)
            {
                float x = Rotation.X;
                float y = Rotation.Y;
                float z = Rotation.Z;
                Rotation = new Vector3 (Deg2Rad (x), Deg2Rad (y), Deg2Rad (z));
            }
            else
            {
                Rotation = rotation;
            }
            
            // clamp cam
            float PiOver2 = (float) System.Math.PI / 2;
            float ThreePiOver2 = (float) System.Math.PI + PiOver2;
            float TwoPi = (float) System.Math.PI * 2;
            float d = 0.000001f;
            
            //FIXME
            float offs = ((int) (Rotation.X / TwoPi)) * TwoPi;
            while (Rotation.X - offs < 0)
                offs -= TwoPi;

            float rx = Rotation.X - offs < ThreePiOver2 + d && oldRotX - offs > ThreePiOver2 ? offs + ThreePiOver2 + d : Rotation.X;
            rx = Rotation.X - offs > PiOver2 - d && oldRotX - offs < PiOver2 ? offs + PiOver2 - d : Rotation.X;
            Rotation = new Vector3 (rx, Rotation.Y, Rotation.Z);
        }

        #endregion

        #region IRotation implementation

        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public Vector3 Rotation { get; set; }

        #endregion
    }
}

