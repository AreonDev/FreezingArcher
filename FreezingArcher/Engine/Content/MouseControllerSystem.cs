﻿//
//  MouseControllerSystem.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2015 Fin Christensen
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
using FreezingArcher.Configuration;
using FreezingArcher.Core;
using System.Xml.Linq;

namespace FreezingArcher.Content
{
    /// <summary>
    /// Mouse controller system. This system converts mouse movement to rotation messages.
    /// </summary>
    public sealed class MouseControllerSystem : EntitySystem
    {
        /// <summary>
        /// Initialize this system. This may be used as a constructor replacement.
        /// </summary>
        /// <param name="messageProvider">Msgmnr.</param>
        /// <param name="entity">Entity.</param>
        public override void Init(MessageProvider messageProvider, Entity entity)
        {
            base.Init(messageProvider, entity);

            DoSmoothing = true;
            SmoothingFactor = 7;

            NeededComponents = new[] { typeof(TransformComponent) };

            internalValidMessages = new[] { (int) MessageId.Input };
            messageProvider += this;
        }

        public float SmoothingFactor { get; set; }

        public bool DoSmoothing { get; set; }

        readonly float movement =
            (float) ConfigManager.Instance ["freezing_archer"].GetDouble("general", "mouse_speed") * 0.0001f;

        readonly float clampTop    =  MathHelper.PiOver2 - 0.0001f;
        readonly float clampBottom = -MathHelper.PiOver2 + 0.0001f;

        float rotationX;
        float rotationY;
        float rotationZ;

        float last_x = 0;
        float last_y = 0;

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Input)
            {
                InputMessage im = msg as InputMessage;

                if (!im.ApplicationInstance.Window.IsMouseCaptured())
                    return;

                float x = im.MouseMovement.Y * movement * (float) im.DeltaTime.TotalMilliseconds;
                float y = im.MouseMovement.X * -movement * (float) im.DeltaTime.TotalMilliseconds;

                x = (SmoothingFactor * last_x + x) / (SmoothingFactor + 1);
                y = (SmoothingFactor * last_y + y) / (SmoothingFactor + 1);

                last_x = x;
                last_y = y;

                if (rotationX + x > clampTop)
                {
                    rotationX = clampTop;
                }
                else if (rotationX + x < clampBottom)
                {
                    rotationX = clampBottom;
                }
                else
                {
                    rotationX += x;
                }

                rotationY += y;

                var quaternionHelper = Quaternion.FromAxisAngle(Vector3.UnitY, rotationY);
                
                Quaternion rotation =
                    Quaternion.FromAxisAngle(Vector3.Transform(Vector3.UnitX, quaternionHelper), rotationX) *
                    Quaternion.FromAxisAngle(Vector3.Transform(Vector3.UnitZ, quaternionHelper), rotationZ) *
                    quaternionHelper;

                Entity.GetComponent<TransformComponent>().Rotation = rotation;
            }
        }
    }
}
