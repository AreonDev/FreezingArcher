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

namespace FurryLana.Engine.Camera
{
    public class ThirdPersonCamera : ICamera, IRotateable
    {
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

        public IEntity BoundTo { get; set; }
        public float Radius { get; set; }
        public float MouseSpeed { get; set; }
        public float ZoomSpeed { get; set; }

        protected Vector3 lastRotation = Vector3.Zero;
        protected Vector3 lastPosition = Vector3.Zero;
        protected float lastRadius;
        protected float yCoord;

        #region ICameraManageable implementation

        public void Enable ()
        {
            throw new NotImplementedException ();
        }

        public void Disable ()
        {
            throw new NotImplementedException ();
        }

        #endregion

        #region IResource implementation

        public void Init ()
        {
            throw new NotImplementedException ();
        }

        public List<Action> GetInitJobs (List<Action> list)
        {
            throw new NotImplementedException ();
        }

        public void Load ()
        {
            throw new NotImplementedException ();
        }

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            throw new NotImplementedException ();
        }

        public void Destroy ()
        {
            throw new NotImplementedException ();
        }

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

        public void FrameSyncedUpdate (float deltaTime)
        {   
            float smoothing = deltaTime * 0.5f;
            smoothing = smoothing.Clamp (0f, 1f);
            
            // smooth zooming
            float zdiff = Radius - lastRadius; // zoom diff
            float zm = lastRadius + zdiff * smoothing;
            lastRadius = zm;
            Vector3 rotVec = new Vector3 (0, 0, zm / 4f);
            
            // smooth the rotation
            Vector3 rdiff = Rotation - lastRotation;
            Vector3 rot = new Vector3 (Rotation);
            rot = lastRotation + rdiff * smoothing;
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

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {}

        #endregion

        #region IDrawable implementation

        public void Draw ()
        {
            throw new NotImplementedException ();
        }

        #endregion

        #region ICamera implementation

        public Matrix ViewMatrix { get; set; }

        #endregion

        #region IManageable implementation

        public string Name { get; set; }

        #endregion

        #region IPosition implementation

        public Vector3 Position { get; set; }

        #endregion

        #region IRotateable implementation

        public void RotTo (Vector3 rotation, FurryLana.Math.AngleEnum angle = (FurryLana.Math.AngleEnum)0)
        {
            throw new NotImplementedException ();
        }

        #endregion

        #region IRotation implementation

        public Vector3 Rotation {
            get {
                throw new NotImplementedException ();
            }
            set {
                throw new NotImplementedException ();
            }
        }

        #endregion
    }
}

