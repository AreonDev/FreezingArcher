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
//#define DEBUG_PERFORMANCE
using System;
using System.Collections.Generic;
using Pencil.Gaming.MathUtils;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Input;
using FreezingArcher.Output;
using FreezingArcher.Core;
using Pencil.Gaming;
using System.Linq;

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
        internal InputMessage (List<KeyboardInput> keys, List<MouseInput> mouse,
            Vector2 mouseMovement, Vector2 mousePosition, Vector2 mouseScroll, TimeSpan deltaTime, Application app)
        {
            Keys = keys;
            Mouse = mouse;
            MouseMovement = mouseMovement;
            MousePosition = mousePosition;
            MouseScroll = mouseScroll;
            DeltaTime = deltaTime;

            ApplicationInstance = app;

            #if DEBUG_PERFORMANCE
            if (Keys.Count > 0)
                Logger.Log.AddLogEntry (LogLevel.Warning, "InputMessage", Status.AKittenDies,
                    String.Concat (Keys.Select (k => k.KeyAction)));
            #endif
        }

        public static readonly string ModuleName = "InputMessage";

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
        /// Gets or sets the mouse position.
        /// </summary>
        /// <value>The mouse position.</value>
        public Vector2 MousePosition { get; protected set; }

        /// <summary>
        /// Gets or sets the mouse scroll.
        /// </summary>
        /// <value>The mouse scroll.</value>
        public Vector2 MouseScroll { get; protected set; }

        /// <summary>
        /// Gets or sets the delta time.
        /// </summary>
        /// <value>The delta time.</value>
        public TimeSpan DeltaTime { get; protected set; }

        public Application ApplicationInstance { get; protected set;}

        /// <summary>
        /// Determines whether a key is pressed and repeated.
        /// </summary>
        /// <returns><c>true</c> key is pressed; otherwise, <c>false</c>.</returns>
        /// <param name="action">The action name associated with the key.</param>
        public bool IsActionPressedAndRepeated (string action)
        {
            var key = Keys.Find (k => k.KeyAction == action);
            return key != null && (key.Action == KeyAction.Press || key.Action == KeyAction.Repeat);
        }

        public bool IsMouseButtonPressedAndRepeated(MouseButton btn)
        {
            var key = Mouse.Find(k => k.Button == btn);
            return key != null && (key.Action == KeyAction.Press || key.Action == KeyAction.Repeat);
        }

        public bool IsMouseButtonDown(MouseButton btn)
        {
            var keys = Mouse.Where(k => k.Button == btn);

            foreach (var key in keys)
            {
                if (key == null)
                {
                    ApplicationInstance.InputManager.CurrentlyDownMouseButtons.Contains(btn);
                }

                if (key.Action == KeyAction.Press &&
                    !ApplicationInstance.InputManager.CurrentlyDownMouseButtons.Contains(btn))
                {
                    ApplicationInstance.InputManager.CurrentlyDownMouseButtons.Add(btn);
                }

                if (key.Action == KeyAction.Release &&
                    ApplicationInstance.InputManager.CurrentlyDownMouseButtons.Contains(btn))
                {
                    ApplicationInstance.InputManager.CurrentlyDownMouseButtons.Remove(btn);
                }

                if (key.Action != KeyAction.Repeat && key.Action != KeyAction.Press &&
                    ApplicationInstance.InputManager.CurrentlyDownMouseButtons.Contains(btn))
                {
                    ApplicationInstance.InputManager.CurrentlyDownMouseButtons.Remove(btn);
                }
            }

            return ApplicationInstance.InputManager.CurrentlyDownMouseButtons.Contains(btn);
        }

        public bool IsActionPressed(string action)
        {
            return Keys.Any(k => k.Action == KeyAction.Press && k.KeyAction == action);
        }

        public bool IsMouseButtonPressed(MouseButton btn)
        {
            return Mouse.Any(m => m.Action == KeyAction.Press && m.Button == btn);
        }

        /// <summary>
        /// Is an action button currently in down state.
        /// </summary>
        /// <returns><c>true</c> if key is down; otherwise, <c>false</c>.</returns>
        /// <param name="action">Action name.</param>
        public bool IsActionDown (string action)
        {
            var keys = Keys.Where(k => k.KeyAction == action);

            foreach (var key in keys)
            {
                if (key == null)
                {
                    return ApplicationInstance.InputManager.CurrentlyDownKeys.Contains(action);
                }

                if (key.Action == KeyAction.Press && !ApplicationInstance.InputManager.CurrentlyDownKeys.Contains(action))
                {
                    ApplicationInstance.InputManager.CurrentlyDownKeys.Add(action);
                }

                if (key.Action == KeyAction.Release && ApplicationInstance.InputManager.CurrentlyDownKeys.Contains(action))
                {
                    ApplicationInstance.InputManager.CurrentlyDownKeys.RemoveAll(s => s == action);
                }

                if (key.Action != KeyAction.Repeat && key.Action != KeyAction.Press && ApplicationInstance.InputManager.CurrentlyDownKeys.Contains(action))
                {
                    ApplicationInstance.InputManager.CurrentlyDownKeys.RemoveAll(s => s == action);
                }
            }

            return ApplicationInstance.InputManager.CurrentlyDownKeys.Contains(action);
        }
    }
}
