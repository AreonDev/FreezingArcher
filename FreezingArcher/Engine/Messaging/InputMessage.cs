//
//  InputDescription.cs
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
#define DEBUG_PERFORMANCE
using System;
using System.Linq;
using System.Collections.Generic;
using Pencil.Gaming.MathUtils;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Input;
using FreezingArcher.Output;
using FreezingArcher.Core;

namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Update description.
    /// </summary>
    public class InputMessage : IMessage
    {
        #region IMessage implementation

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public object Source { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        /// <value>The destination.</value>
        public object Destination { get; set; }

        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        /// <value>The message identifier.</value>
        public int MessageId
        {
            get
            {
                return (int) Messaging.MessageId.Input;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Messaging.InputMessage"/> class.
        /// </summary>
        /// <param name="keys">Keys.</param>
        /// <param name="mouse">Mouse.</param>
        /// <param name="mouseMovement">Mouse movement.</param>
        /// <param name="mouseScroll">Mouse scroll.</param>
        /// <param name="deltaTime">Delta time.</param>
        public InputMessage (List<KeyboardInput> keys, List<MouseInput> mouse,
            Vector2 mouseMovement, Vector2 mouseScroll, double deltaTime)
        {
            Keys = keys;
            Mouse = mouse;
            MouseMovement = mouseMovement;
            MouseScroll = mouseScroll;
            DeltaTime = deltaTime;

            #if DEBUG_PERFORMANCE
            if (Keys.Count > 0)
                Logger.Log.AddLogEntry (LogLevel.Warning, "InputMessage", Status.AKittenDies,
                    String.Concat (Keys.Select (k => k.KeyAction)));
            #endif
        }

        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public List<KeyboardInput> Keys { get; protected set; }

        /// <summary>
        /// Gets or sets the mouse.
        /// </summary>
        /// <value>The mouse.</value>
        public List<MouseInput> Mouse { get; protected set; }

        /// <summary>
        /// Gets or sets the mouse movement.
        /// </summary>
        /// <value>The mouse movement.</value>
        public Vector2 MouseMovement { get; protected set; }

        /// <summary>
        /// Gets or sets the mouse scroll.
        /// </summary>
        /// <value>The mouse scroll.</value>
        public Vector2 MouseScroll { get; protected set; }

        /// <summary>
        /// Gets or sets the delta time.
        /// </summary>
        /// <value>The delta time.</value>
        public double DeltaTime { get; protected set; }
    }
}
