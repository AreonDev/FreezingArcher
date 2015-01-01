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
            Rotation = new Vector3 (Rotation.X, BoundTo.Rotation.Y, Rotation.Z);
        }

        public void Disable ()
        {}

        #endregion

        #region IResource implementation

        public void Init ()
        {}

        public List<Action> GetInitJobs (List<Action> list)
        {
            return list;
        }

        public void Load ()
        {}

        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
            NeedsLoad = reloader;
            return list;
        }

        public void Destroy ()
        {}

        public bool Loaded { get; protected set; }

        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IFrameSyncedUpdate implementation

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

        protected bool firstMouse = true;

        #region IUpdate implementation

        public void Update (UpdateDescription desc)
        {
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

        public void Draw ()
        {}

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

        protected float Deg2Rad (float deg)
        {
            const float degToRad = (float) System.Math.PI / 180.0f;
            return deg * degToRad;
        }

        protected float Rad2Deg (float rad)
        {
            const float radToDeg = 180.0f / (float)System.Math.PI;
            return rad * radToDeg;
        }

        #region IRotateable implementation

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

        public Vector3 Rotation { get; set; }

        #endregion
    }
}

