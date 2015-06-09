//
//  ThirdPersonCamera.cs
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
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Content;

namespace FreezingArcher.Renderer.Scene
{
    /*
	 * Diese Kamera soll den roten Würfel mitten auf der grün, blau karierten Fläche verfolgen und 
	 * mit der Maus soll man um diesen Würfel rotieren
	 * Auch hier wirst du unsere API benutzen und dich schonmal mit FreezingArcher.Math auseinandersetzen
	 * Diese Aufgabe ist etwas schwieriger ;)
	 */


    public class ThirdPersonCamera : BaseCam //, IMessageConsumer
    {
        Entity Person;
        Vector3 position;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.FreeCamera"/> class.
        /// </summary>
        /// <param name="mssgmngr">Mssgmngr.</param>
        /// <param name="_cameraPosition">Camera position.</param>
        /// <param name="_currentRotation">Current rotation.</param>
        /// <param name="near">Near.</param>
        /// <param name="far">Far.</param>
        /// <param name="fov">Fov.</param>
        public ThirdPersonCamera (Entity person, string name, MessageManager mssgmngr, Vector3 _cameraPosition = default(Vector3),
                                  Vector3 _currentRotation = default(Vector3), float near = 0.1f, float far = 100.0f,
                                  float fov = (float)System.Math.PI / 4.0f) : base (name, _cameraPosition,
                                                              _currentRotation, near, far, fov)
        {
            Person = person;
            ValidMessages = new int[] { (int)MessageId.Input, (int) MessageId.WindowResizeMessage };
            mssgmngr += this;
        }

        private void UpdateCamera(){
            currentRotation = Person.GetComponent<TransformComponent>().Position - cameraPosition;
            base.UpdateCamera();
        }

        /// <summary>
        /// Consumes the message.
        /// </summary>
        /// <param name="msg">Message.</param>
        public virtual void ConsumeMessage (Messaging.Interfaces.IMessage msg)
        {
            InputMessage im = msg as InputMessage;
            if (im != null) {
                if (im.IsActionDown ("forward")) {
                    moveX (1 * fak);
                }

                if (im.IsActionDown ("backward")) {
                    moveX (-1 * fak);
                }

                if (im.IsActionDown ("left")) {
                    moveZ (-1 * fak);
                }

                if (im.IsActionDown ("right")) {
                    moveZ (1 * fak);
                }

                if (im.IsActionDown ("inventory")) {
                    moveY (-1 * fak);
                }

                if (im.IsActionDown ("drop")) {
                    moveY (1 * fak);
                }
            }

            WindowResizeMessage wrm = msg as WindowResizeMessage;
            if (wrm != null) {
                UpdateProjectionMatrix (wrm);
            }
        }
    }
}
