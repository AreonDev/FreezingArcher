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
    /// <summary>
    /// First person camera.
    /// </summary>
    public class FirstPersonCamera : FreeCamera, IMessageConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Renderer.Scene.FirstPersonCamera"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="mssgmngr">Mssgmngr.</param>
        /// <param name="cameraPosition">Camera position.</param>
        /// <param name="currentRotation">Current rotation.</param>
        /// <param name="near">Near.</param>
        /// <param name="far">Far.</param>
        /// <param name="fov">Fov.</param>
        public FirstPersonCamera(string name, MessageManager mssgmngr, Vector3 cameraPosition = default(Vector3),
                                  Vector3 currentRotation = default(Vector3), float near = 0.1f, float far = 1000.0f,
                                  float fov = MathHelper.PiOver4)
            : base(name, mssgmngr, cameraPosition, currentRotation, near, far, fov)
        {
            ValidMessages = new int[] { (int)MessageId.Input, (int)MessageId.WindowResizeMessage };
            mssgmngr += this;
        }

        /// <summary>
        /// Moves the x.
        /// </summary>
        /// <param name="position">Position.</param>
        public override void MoveX (float position)
        {
            CameraPosition += position * new Vector3(ViewMatrix.Column2.X,0,ViewMatrix.Column2.Z);
            UpdateCamera ();
        }

        float bobbing = 0;

        public override void ConsumeMessage(IMessage msg)
        {
            base.ConsumeMessage(msg);

            if(msg.MessageId == (int) MessageId.Input)
            {
                var im = msg as InputMessage;

                if (im.IsActionDown("forward") || im.IsActionDown("backward"))
                {
                    float bobbX = (float)System.Math.Sin(bobbing * 2);
                    float bobbY = (float)System.Math.Sin(bobbing);
                    bobbX *= 0.03f;
                    bobbY *= 0.05f;
                    MoveY(bobbX);
                    MoveZ(bobbY);
                    bobbing += 0.05f;
                }
            }
        }
    }
}

