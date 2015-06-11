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

namespace FreezingArcher.Renderer.Scene
{

    /*
	 * Dies wird deine allererste Kamera... 
	 * Hier sollst du dich mit dem aktuellen FreezingArcher auseinandersetzen,
	 * das heißt, du wirst UNSERE API benutzen und kennenlernen.
	 * 
	 * Die erste Aufgabe ist eigentlich ganz einfach... 
	 * Hier sollst du eine simple Kamera schreiben, die du nach links, rechts, oben, unten,
	 * hinten und vorne verschieben kannst. Dabei sollst du auf Tastendrücke reagieren, und diese
	 * richtig verarbeiten. Wie du das genau tust, dass erfährst du in den Docs.
	 * Fin hat da gute Arbeit geleistet
	 * 
	 * Hinweis: Größe des Fensters erhälst du auch über Fins MessagingAPI
	 * Achtung: Da gibt es einen fiesen gemeinen BUG, aber das muss sich fin angucken
	 */

    /// <summary>
    /// My first freezing archer cam.
    /// </summary>
    abstract public class BaseCamera : IMessageConsumer
    {
        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; protected set; }

        /// <summary>
        /// Gets or sets the camera position.
        /// </summary>
        /// <value>The camera position.</value>
        protected Vector3 CameraPosition { get; set; }

        /// <summary>
        /// Gets or sets the current rotation.
        /// </summary>
        /// <value>The current rotation.</value>
        protected Vector3 CurrentRotation { get; set; }

        /// <summary>
        /// The M window x.
        /// </summary>
        protected float MWindowX;
        /// <summary>
        /// Gets or sets the window x.
        /// </summary>
        /// <value>The window x.</value>
        public float WindowX
        {
            get{ return this.MWindowX; }
            set
            {
                this.MWindowX = value;
                UpdateProjectionMatrix();
            }
        }

        /// <summary>
        /// The M window y.
        /// </summary>
        protected float MWindowY;
        /// <summary>
        /// Gets or sets the window y.
        /// </summary>
        /// <value>The window y.</value>
        public float WindowY
        {
            get{ return this.MWindowY; }
            set
            {
                this.MWindowY = value;
                UpdateProjectionMatrix();
            }
        }

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
        /// The M up.
        /// </summary>
        protected Vector3 MUp;
        /// <summary>
        /// Gets or sets up.
        /// </summary>
        /// <value>Up.</value>
        public Vector3 Up
        {
            get{ return this.MUp; }
            set
            {
                this.MUp = value;
                UpdateProjectionMatrix();
            }
        }

        /// <summary>
        /// The fak.
        /// </summary>
        protected const float Fak = 0.1f;

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
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        //const Vector3 defaultUp = Vector3.UnitY;

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
        internal BaseCamera (string name,
            Vector3 cameraPosition = default(Vector3),
            Vector3 currentRotation = default(Vector3), float near = 0.1f, float far = 100.0f,
            float fov = MathHelper.PiOver4, Vector3 up = default(Vector3))
        {
            Name = name;
            CameraPosition = cameraPosition;
            CurrentRotation = currentRotation;
            cameraReference = new Vector3 (0, 0, -1);
            MZNear = near;
            MZFar = far;
            MFov = fov;
            MUp = up != Vector3.Zero ? up : Vector3.UnitY;



            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MFov,
                (float)Application.Instance.RendererContext.ViewportSize.X / (float)Application.Instance.RendererContext.ViewportSize.Y,
                MZNear, MZFar); 
            
            UpdateCamera ();
            Logger.Log.AddLogEntry (LogLevel.Debug, "BaseCamera", Status.Computing, "Creating new camera {0}", Name);
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
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; set; }

        /// <summary>
        /// Updates the camera.
        /// </summary>
        protected void UpdateCamera ()
        {
            float sinx = (float) System.Math.Sin (CurrentRotation.X);
            float cosx = (float) System.Math.Cos (CurrentRotation.X);
            float siny = (float) System.Math.Sin (CurrentRotation.Y);
            float cosy = (float) System.Math.Cos (CurrentRotation.Y);
            float sinz = (float) System.Math.Sin (CurrentRotation.Z);
            float cosz = (float) System.Math.Cos (CurrentRotation.Z);

            cameraLookat = CameraPosition + transformedReference;


            ViewMatrix = Matrix.LookAt(CameraPosition, cameraLookat, MUp);

            ViewMatrix *= Matrix.CreateFromQuaternion (new Quaternion (sinx, 0, 0, cosx) *
                new Quaternion (0, siny, 0, cosy) *
                new Quaternion (0, 0, sinz, cosz));
        }

        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="position">Position.</param>
        public void MoveTo(Vector3 position)
        {
            CameraPosition = position;
            UpdateCamera();
        }


        /// <summary>
        /// Rotates the x.
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        public void RotateX (float rotation)
        {
            var tmp = CurrentRotation;
            tmp.X += rotation;
            CurrentRotation = tmp;
            UpdateCamera ();
        }

        /// <summary>
        /// Rotates the y.
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        public void RotateY (float rotation)
        {
            var tmp = CurrentRotation;
            tmp.Y += rotation;
            CurrentRotation = tmp;
            UpdateCamera ();
        }

        /// <summary>
        /// Rotates the z.
        /// </summary>
        /// <param name="rotation">Rotation.</param>
        public void RotateZ (float rotation)
        {
            var tmp = CurrentRotation;
            tmp.Z += rotation;
            CurrentRotation = tmp;
            UpdateCamera ();
        }
            

        /// <summary>
        /// Moves the x.
        /// </summary>
        /// <param name="position">Position.</param>
        public virtual void MoveX (float position)
        {
            CameraPosition += position * new Vector3(ViewMatrix.Column2.X,ViewMatrix.Column2.Y,ViewMatrix.Column2.Z);
            UpdateCamera ();
        }

        /// <summary>
        /// Moves the y.
        /// </summary>
        /// <param name="position">Position.</param>
        public virtual void MoveY (float position)
        {
            CameraPosition += position * new Vector3(ViewMatrix.Column1.X,ViewMatrix.Column1.Y,ViewMatrix.Column1.Z);
            UpdateCamera ();
        }

        /// <summary>
        /// Moves the z.
        /// </summary>
        /// <param name="position">Position.</param>
        public virtual void MoveZ (float position)
        {
            CameraPosition += position * new Vector3(ViewMatrix.Column0.X,ViewMatrix.Column0.Y,ViewMatrix.Column0.Z);
            UpdateCamera ();
        }

        /// <summary>
        /// Updates the projection matrix.
        /// </summary>
        protected void UpdateProjectionMatrix ()
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (MFov, 
                (float)MWindowX / (float)MWindowY, MZNear, MZFar); 
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public virtual void ConsumeMessage (Messaging.Interfaces.IMessage msg)
        {
            WindowResizeMessage wrm = msg as WindowResizeMessage;
            if (wrm != null)
            {
                WindowX = wrm.Width;
                WindowY = wrm.Height;
            }
        }
    }
}

