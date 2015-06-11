//
//  FreeCamera.cs
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

namespace FreezingArcher.Renderer.Scene
{
    /*
	 * Du sollst hier eine Kamera implementieren, die sich komplett frei im Raum bewegen kann
	 * Sie kann um ALLE ihre Achsen rotieren!
	 * Mit Tastatur und Maus ist sie zu steuern.
	 * Hinweis: Damit das Rotieren um alle Achsen funktioniert, würde ich mir an deiner Stelle
	 * nochmal das Konzept des Up-Vektors einer Kamera angucken
	 * 
	 * Hinweis2: Gimbal-Locks sind böse ;)
	 */

    public class FreeCamera : BaseCamera, IMessageConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.FreeCamera"/> class.
        /// </summary>
        /// <param name="mssgmngr">Mssgmngr.</param>
        /// <param name="_cameraPosition">Camera position.</param>
        /// <param name="_currentRotation">Current rotation.</param>
        /// <param name="near">Near.</param>
        /// <param name="far">Far.</param>
        /// <param name="fov">Fov.</param>
        public FreeCamera (string name, MessageManager mssgmngr, Vector3 _cameraPosition = default(Vector3),
                                   Vector3 _currentRotation = default(Vector3), float near = 0.1f, float far = 1000.0f,
                                   float fov = (float)System.Math.PI / 4.0f) : base (name, _cameraPosition,
                                                                           _currentRotation, near, far, fov)
        {
            ValidMessages = new int[] { (int)MessageId.Input, (int) MessageId.WindowResizeMessage };
            mssgmngr += this;
        }

        /// <summary>
        /// Consumes the message.
        /// </summary>
        /// <param name="msg">Message.</param>
        public override void ConsumeMessage (Messaging.Interfaces.IMessage msg)
        {
            base.ConsumeMessage(msg);

            InputMessage im = msg as InputMessage;
            if (im != null) {
                if (im.IsActionDown ("forward")) {
                    MoveX (-1 * fak);
                }

                if (im.IsActionDown ("backward")) {
                    MoveX (1 * fak);
                }

                if (im.IsActionDown ("left")) {
                    MoveZ (-1 * fak);
                }

                if (im.IsActionDown ("right")) {
                    MoveZ (1 * fak);
                }

                if (im.IsActionDown ("inventory")) {
                    MoveY (1 * fak);
                }

                if (im.IsActionDown ("drop")) {
                    MoveY (-1 * fak);
                }
                if (im.MouseMovement.X != 0)
                {
                    rotateY(im.MouseMovement.X * 0.001f);
                }
                if (im.MouseMovement.Y != 0)
                {
                    rotateX(im.MouseMovement.Y * 0.001f);
                }
            }
        }
    }
}
