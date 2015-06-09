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
using FreezingArcher.Input;
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
    abstract public class BaseCam : IMessageConsumer
    {
        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; protected set; }

        protected Vector3 cameraPosition { get; set; }

        protected Vector3 currentRotation { get; set; }

        protected float _WindowX;
        public float WindowX
        {
            get{ return this._WindowX; }
            set
            {
                this._WindowX = value;
                UpdateProjectionMatrix();
            }
        }

        protected float _WindowY;
        public float WindowY
        {
            get{ return this._WindowY; }
            set
            {
                this._WindowY = value;
                UpdateProjectionMatrix();
            }
        }

        protected float _zNear;
        public float zNear
        {
            get{ return this._zNear; }
            set
            {
                this._zNear = value;
                UpdateProjectionMatrix();
            }
        }

        protected float _zFar;
        public float zFar
        {
            get{ return this._zFar; }
            set
            {
                this._zFar = value;
                UpdateProjectionMatrix();
            }
        }

        protected float _fovY;
        public float fovY
        {
            get{ return this._fovY; }
            set
            {
                this._fovY = value;
                UpdateProjectionMatrix();
            }
        }

        protected const float fak = 0.1f;

        Vector3 cameraReference;
        Vector3 transformedReference = Vector3.UnitX;
        Vector3 cameraLookat;

        public string Name { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.SimpleCam"/> class.
        /// </summary>
        /// <param name="mssgmngr">Mssgmngr.</param>
        /// <param name="_cameraPosition">Camera position.</param>
        /// <param name="_currentRotation">Current rotation.</param>
        /// <param name="near">Near.</param>
        /// <param name="far">Far.</param>
        /// <param name="fov">Fov.</param>
        internal BaseCam (string _name,
            Vector3 _cameraPosition = default(Vector3),
            Vector3 _currentRotation = default(Vector3), float near = 0.1f, float far = 100.0f,
            float fov = MathHelper.PiOver4)
        {
            Name = _name;
            cameraPosition = _cameraPosition;
            currentRotation = _currentRotation;
            cameraReference = new Vector3 (0, 0, -1);
            _zNear = near;
            _zFar = far;
            _fovY = fov;



            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (_fovY,
                (float)Application.Instance.RendererContext.ViewportSize.X / (float)Application.Instance.RendererContext.ViewportSize.Y,
                _zNear, _zFar); 
            
            UpdateCamera ();
            Logger.Log.AddLogEntry (LogLevel.Debug, "CAM " + Name, Status.Computing);
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

        protected void UpdateCamera ()
        {
            float sinx = (float) System.Math.Sin (currentRotation.X);
            float cosx = (float) System.Math.Cos (currentRotation.X);
            float siny = (float) System.Math.Sin (currentRotation.Y);
            float cosy = (float) System.Math.Cos (currentRotation.Y);
            float sinz = (float) System.Math.Sin (currentRotation.Z);
            float cosz = (float) System.Math.Cos (currentRotation.Z);

            cameraLookat = cameraPosition + transformedReference;

            ViewMatrix = Matrix.LookAt (cameraPosition, cameraLookat, Vector3.UnitY);

            ViewMatrix *= Matrix.CreateFromQuaternion (new Quaternion (sinx, 0, 0, cosx) *
                new Quaternion (0, siny, 0, cosy) *
                new Quaternion (0, 0, sinz, cosz));
        }

        /// <summary>
        /// Rotates the x.
        /// </summary>
        /// <param name="_rotation">Rotation.</param>
        public void rotateX (float _rotation)
        {
            var tmp = currentRotation;
            tmp.X += _rotation;
            currentRotation = tmp;
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "Rotate X", Status.Computing);
        }

        /// <summary>
        /// Rotates the y.
        /// </summary>
        /// <param name="_rotation">Rotation.</param>
        public void rotateY (float _rotation)
        {
            var tmp = currentRotation;
            tmp.Y += _rotation;
            currentRotation = tmp;
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "Rotate Y", Status.Computing);
        }

        /// <summary>
        /// Rotates the z.
        /// </summary>
        /// <param name="_rotation">Rotation.</param>
        public void rotateZ (float _rotation)
        {
            var tmp = currentRotation;
            tmp.Z += _rotation;
            currentRotation = tmp;
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "Rotate Z", Status.Computing);
        }

        /// <summary>
        /// Moves the x.
        /// </summary>
        /// <param name="_position">Posotion.</param>
        public void moveX (float _position)
        {
//            var tmp = cameraPosition;
//            tmp.X += _position;
//            cameraPosition = tmp;
            cameraPosition += _position * new Vector3(ViewMatrix.Column2.X,0,ViewMatrix.Column2.Z);//ViewMatrix.Column2.Y
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "MoveX", Status.Computing);
        }

        public void MoveTo(Vector3 position)
        {
            cameraPosition = position;
            UpdateCamera();
        }

        /// <summary>
        /// Moves the y.
        /// </summary>
        /// <param name="_position">Posotion.</param>
        public void moveY (float _position)
        {
//            var tmp = cameraPosition;
//            tmp.Y += _position;
//            cameraPosition = tmp;
            cameraPosition += _position * new Vector3(ViewMatrix.Column1.X,ViewMatrix.Column1.Y,ViewMatrix.Column1.Z);
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "MoveY", Status.Computing);
        }

        /// <summary>
        /// Moves the z.
        /// </summary>
        /// <param name="_position">Posotion.</param>
        public void moveZ (float _position)
        {
//            var tmp = cameraPosition;
//            tmp.Z += _position;
//            cameraPosition = tmp;
            cameraPosition += _position * new Vector3(ViewMatrix.Column0.X,ViewMatrix.Column0.Y,ViewMatrix.Column0.Z);
            UpdateCamera ();
//            Logger.Log.AddLogEntry (LogLevel.Debug, "MoveZ", Status.Computing);
        }

        protected void UpdateProjectionMatrix ()
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView (_fovY, 
                (float)_WindowX / (float)_WindowY, _zNear, _zFar); 
        }

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

