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

namespace FreezingArcher.Game
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
	public class MyFirstFreezingArcherCam : FreezingArcher.Renderer.Scene.ICamera
	{
		Vector3 cameraPosition { get; set;}
		Vector3 currentRotation { get; set;}
		Vector3 cameraReference;
		Vector3 transformedReference;
		Vector3 cameraLookat;

		/// <summary>
		/// Initializes a new instance of the <see cref="FreezingArcher.Game.MyFirstFreezingArcherCam"/> class.
		/// </summary>
		public MyFirstFreezingArcherCam(){
			cameraPosition = new Vector3(0,0,30);
			currentRotation = new Vector3(0,0,0);
			cameraReference = new Vector3(0, 0, -1);
			UpdateCamera ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreezingArcher.Game.MyFirstFreezingArcherCam"/> class.
		/// </summary>
		/// <param name="_cameraPosition">Camera position.</param>
		public MyFirstFreezingArcherCam(Vector3 _cameraPosition){
			cameraPosition = _cameraPosition;
			currentRotation = new Vector3(0,0,0);
			cameraReference = new Vector3(1, 0, 0);
			UpdateCamera ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FreezingArcher.Game.MyFirstFreezingArcherCam"/> class.
		/// </summary>
		/// <param name="_cameraPosition">Camera position.</param>
		/// <param name="_currentRotation">Current rotation.</param>
		public MyFirstFreezingArcherCam(Vector3 _cameraPosition, Vector3 _currentRotation){
			cameraPosition = _cameraPosition;
			currentRotation = _currentRotation;
			cameraReference = new Vector3(0, 0, -1);
			UpdateCamera ();
		}

		/// <summary>
		/// Gets or sets the projection matrix.
		/// </summary>
		/// <value>The projection matrix.</value>
		public Matrix ProjectionMatrix { get; protected set;}

		/// <summary>
		/// Gets or sets the view matrix.
		/// </summary>
		/// <value>The view matrix.</value>
		public Matrix ViewMatrix { get; protected set;}

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

		private void UpdateCamera(){
			Matrix rotationPosition =
				Matrix.CreateRotationX (currentRotation.X)
				* Matrix.CreateRotationY (currentRotation.Y)
				* Matrix.CreateRotationZ (currentRotation.Z);

			transformedReference = Vector3.Transform (cameraReference, rotationPosition);

			cameraLookat = cameraPosition + transformedReference;

			ViewMatrix = Matrix.LookAt (cameraPosition, cameraLookat, new Vector3(0.0f, 1.0f, 0.0f));
		}

		/// <summary>
		/// Rotates the x.
		/// </summary>
		/// <param name="_rotation">Rotation.</param>
		public void rotateX(float _rotation){
			var tmp = currentRotation;
			tmp.X += _rotation;
			currentRotation = tmp;
			UpdateCamera ();
		}

		/// <summary>
		/// Rotates the y.
		/// </summary>
		/// <param name="_rotation">Rotation.</param>
		public void rotateY(float _rotation){
			var tmp = currentRotation;
			tmp.Y += _rotation;
			currentRotation = tmp;
			UpdateCamera ();
		}

		/// <summary>
		/// Rotates the z.
		/// </summary>
		/// <param name="_rotation">Rotation.</param>
		public void rotateZ(float _rotation){
			var tmp = currentRotation;
			tmp.Z += _rotation;
			currentRotation = tmp;
			UpdateCamera ();
		}

		/// <summary>
		/// Moves the x.
		/// </summary>
		/// <param name="_position">Posotion.</param>
		public void moveX(float _position){
			var tmp = cameraPosition;
			tmp.X += _position;
			cameraPosition=tmp;
			UpdateCamera ();
		}

		/// <summary>
		/// Moves the y.
		/// </summary>
		/// <param name="_position">Posotion.</param>
		public void moveY(float _position){
			var tmp = cameraPosition;
			tmp.Y += _position;
			cameraPosition=tmp;
			UpdateCamera ();
		}

		/// <summary>
		/// Moves the z.
		/// </summary>
		/// <param name="_position">Posotion.</param>
		public void moveZ(float _position){
			var tmp = cameraPosition;
			tmp.Z += _position;
			cameraPosition=tmp;
			UpdateCamera ();
		}

		/// <summary>
		/// Consumes the message.
		/// </summary>
		/// <param name="msg">Message.</param>
		public virtual void ConsumeMessage(Messaging.Interfaces.IMessage msg)
		{
			InputMessage im = msg as InputMessage;
			if (im != null) 
			{
				if (im.IsKeyPressed ("up")) 
				{
					moveX (1);
				}

				if (im.IsKeyPressed ("down")) 
				{
					moveX (-1);
				}

				if (im.IsKeyPressed ("forward")) 
				{
					moveZ (1);
				}

				if (im.IsKeyPressed ("backword")) 
				{
					moveZ (-1);
				}

				if (im.IsKeyPressed ("left")) 
				{
					moveY (1);
				}

				if (im.IsKeyPressed ("right")) 
				{
					moveY (-1);
				}
			}
		}
	}
}

