//
//  MyFirstFreezingArcherCam.cs
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
using FreezingArcher.Messaging;
using FreezingArcher.Core;
using FreezingArcher.Output;
using FreezingArcher.Messaging.Interfaces;
using Pencil.Gaming.Graphics;
using FreezingArcher.Content;

namespace FreezingArcher.Renderer.Scene
{
    /// <summary>
    /// My first freezing archer cam.
    /// </summary>
    public class BaseCamera : IMessageConsumer
    {
        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; protected set; }
        public System.Type[] NeededComponents { get; protected set; }

        protected Entity Entity;

        /// <summary>
        /// Gets or sets the camera position.
        /// </summary>
        /// <value>The camera position.</value>
        protected Vector3 MPosition;
        public Vector3 Position
        {
            get{ return this.MPosition; }
            set
            {
                this.MPosition = value;
                UpdateCamera();
            }
        }

        /// <summary>
        /// Gets or sets the current rotation.
        /// </summary>
        /// <value>The current rotation.</value>
        protected Quaternion MRotation;
        public Quaternion Rotation
        {
            get{ return this.MRotation; }
            set
            {
                this.MRotation = value;
                UpdateCamera();
            }
        }

        /// <summary>
        /// The M window x.
        /// </summary>
        protected float WindowX;

        /// <summary>
        /// The M window y.
        /// </summary>
        protected float WindowY;

        /// <summary>
        /// The MZ near.
        /// </summary>
        protected float MZNear;
        /// <summary>
        /// Gets or sets the Z near.
        /// </summary>
        /// <value>The Z near.</value>
        public float ZNear
        {
            get{ return this.MZNear; }
            set
            {
                this.MZNear = value;
                UpdateProjectionMatrix();
            }
        }

        /// <summary>
        /// The MZ far.
        /// </summary>
        protected float MZFar;
        /// <summary>
        /// Gets or sets the Z far.
        /// </summary>
        /// <value>The Z far.</value>
        public float ZFar
        {
            get{ return this.MZFar; }
            set
            {
                this.MZFar = value;
                UpdateProjectionMatrix();
            }
        }

        /// <summary>
        /// The M fov.
        /// </summary>
        protected float MFov;
        /// <summary>
        /// Gets or sets the fov.
        /// </summary>
        /// <value>The fov.</value>
        public float Fov
        {
            get{ return this.MFov; }
            set
            {
                this.MFov = value;
                UpdateProjectionMatrix();
            }
        }

        /// <summary>
        /// The fak.
        /// </summary>
        protected const float Fak = 0.15f;

        /// <summary>
        /// The camera reference.
        /// </summary>
        Vector3 cameraReference;
        /// <summary>
        /// The transformed reference.
        /// </summary>
        Vector3 transformedReference = Vector3.UnitX;
        /// <summary>
        /// The camera lookat.
        /// </summary>
        Vector3 cameraLookat;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Renderer.Scene.BaseCamera"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="cameraPosition">Camera position.</param>
        /// <param name="currentRotation">Current rotation.</param>
        /// <param name="near">Near.</param>
        /// <param name="far">Far.</param>
        /// <param name="fov">Fov.</param>
        /// <param name="up">Up.</param>
        public BaseCamera (Entity entity, MessageProvider messageProvider, Vector3 position = default(Vector3),
            Quaternion rotation = default(Quaternion), float near = 0.1f, float far = 400.0f,
            float fov = MathHelper.PiOver2)
        {
            MPosition = position;
            MRotation = rotation;
            cameraReference = new Vector3 (0, 0, -1);
            MZNear = near;
            MZFar = far;
            MFov = fov;
            ValidMessages = new int[] { (int)MessageId.WindowResize };
            NeededComponents = new[] { typeof(TransformComponent) };
            messageProvider += this;
            Entity = entity;

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MFov,
                (float)Application.Instance.RendererContext.ViewportSize.X / (float)Application.Instance.RendererContext.ViewportSize.Y,
                MZNear, MZFar); 
            
            UpdateCamera ();

            var transform = entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged += () => {
                Position = transform.Position;
            };
            transform.OnRotationChanged += () => {
                Rotation = transform.Rotation;
            };
        }

        /// <summary>
        /// Gets or sets the projection matrix.
        /// </summary>
        /// <value>The projection matrix.</value>
        public Matrix ProjectionMatrix { get; protected set; }

        /// <summary>
        /// Gets or sets the view matrix.
        /// </summary>
        /// <value>The view matrix.</value>
        public Matrix ViewMatrix { get; protected set; }

        /// <summary>
        /// Updates the camera.
        /// </summary>
        protected void UpdateCamera ()
        {
            ViewMatrix = Matrix.LookAt(MPosition, MPosition + Vector3.Transform(Vector3.UnitZ, MRotation),
                Vector3.Transform(Vector3.UnitY, MRotation));
        }

        /// <summary>
        /// Updates the projection matrix.
        /// </summary>
        protected void UpdateProjectionMatrix ()
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MFov, 
                (float)WindowX / (float)WindowY, MZNear, MZFar); 
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public virtual void ConsumeMessage (Messaging.Interfaces.IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.WindowResize)
            {
                WindowResizeMessage wrm = msg as WindowResizeMessage;
                WindowX = wrm.Width;
                WindowY = wrm.Height;
                UpdateProjectionMatrix();
            }
        }
    }
}
