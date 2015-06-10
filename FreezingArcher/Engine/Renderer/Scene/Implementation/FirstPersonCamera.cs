//
//  FirstPersonCam.cs
//
//  Author:
//       wfailla <>
//
//  Copyright (c) 2015 wfailla
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
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Math;

namespace FreezingArcher.Renderer.Scene
{
    public class FirstPersonCamera : FreeCamera, IMessageConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Renderer.Scene.Implementation.FirstPersonCam"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="mssgmngr">Mssgmngr.</param>
        /// <param name="_cameraPosition">Camera position.</param>
        /// <param name="_currentRotation">Current rotation.</param>
        /// <param name="near">Near.</param>
        /// <param name="far">Far.</param>
        /// <param name="fov">Fov.</param>
        public FirstPersonCamera (string name, MessageManager mssgmngr, Vector3 _cameraPosition = default(Vector3),
            Vector3 _currentRotation = default(Vector3), float near = 0.1f, float far = 100.0f,
            float fov = MathHelper.PiOver4) : base (name, mssgmngr, _cameraPosition,
                _currentRotation, near, far, fov)
        {
            ValidMessages = new int[] { (int)MessageId.Input, (int) MessageId.WindowResizeMessage };
            mssgmngr += this;
        }

        /// <summary>
        /// Moves the x.
        /// </summary>
        /// <param name="_position">Posotion.</param>
        public override void MoveX (float _position)
        {
            cameraPosition += _position * new Vector3(ViewMatrix.Column2.X,0,ViewMatrix.Column2.Z);
            UpdateCamera ();
        }

    }
}

