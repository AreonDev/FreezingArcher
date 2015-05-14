//
//  InputManager.cs
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
using System.Collections.Generic;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using FreezingArcher.Output;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using System;
using System.Diagnostics;

namespace FreezingArcher.Input
{
    /// <summary>
    /// Input manager.
    /// </summary>
    public class InputManager : IMessageCreator
    {
        #region IMessageCreator implementation

        /// <summary>
        /// Occurs when a new message is created an is ready for processing
        /// </summary>
        public event MessageEvent MessageCreated;

        #endregion

        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "InputManager";

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Input.InputManager"/> class.
        /// </summary>
        internal InputManager (MessageManager messageManager)
        {
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Creating new input manager");
            Keys = new List<KeyboardInput> ();
            Mouse = new List<MouseInput> ();
            MouseMovement = Vector2.Zero;
            MouseScroll = Vector2.Zero;
            OldMousePosition = Vector2.Zero;
            KeyRegistry.Instance = new KeyRegistry (messageManager);
            Stopwatch = new Stopwatch();
            messageManager += this;
        }

        /// <summary>
        /// The keys.
        /// </summary>
        protected List<KeyboardInput> Keys;

        /// <summary>
        /// The mouse button inputs.
        /// </summary>
        protected List<MouseInput> Mouse;

        /// <summary>
        /// The mouse movement.
        /// </summary>
        protected Vector2 MouseMovement;

        /// <summary>
        /// The mouse scroll.
        /// </summary>
        protected Vector2 MouseScroll;

        /// <summary>
        /// The old mouse position.
        /// </summary>
        protected Vector2 OldMousePosition;

        /// <summary>
        /// The stopwatch for input update.
        /// </summary>
        protected Stopwatch Stopwatch;

        /// <summary>
        /// Handles the keyboard input.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="key">Key.</param>
        /// <param name="scancode">Scancode.</param>
        /// <param name="action">Action.</param>
        /// <param name="modifier">Modifier.</param>
        internal void HandleKeyboardInput (GlfwWindowPtr window, Key key, int scancode,
                                         KeyAction action, KeyModifiers modifier)
        {
            Keys.Add (new KeyboardInput (key, scancode, action, modifier));
        }

        /// <summary>
        /// Handles the mouse button.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="button">Button.</param>
        /// <param name="action">Action.</param>
        internal void HandleMouseButton (GlfwWindowPtr window, MouseButton button, KeyAction action)
        {
            Mouse.Add (new MouseInput (button, action));
        }

        /// <summary>
        /// Handles the mouse move.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        internal void HandleMouseMove (GlfwWindowPtr window, double x, double y)
        {
            MouseMovement += new Vector2 ((float) x - OldMousePosition.X, (float) y - OldMousePosition.Y);
            OldMousePosition = new Vector2 ((float) x, (float) y);
        }

        /// <summary>
        /// Handles the mouse scroll.
        /// </summary>
        /// <param name="window">Window.</param>
        /// <param name="xoffs">Xoffs.</param>
        /// <param name="yoffs">Yoffs.</param>
        internal void HandleMouseScroll (GlfwWindowPtr window, double xoffs, double yoffs)
        {
            MouseScroll += new Vector2 ((float) xoffs, (float) yoffs);
        }

        /// <summary>
        /// Generates the input message.
        /// </summary>
        internal void GenerateInputMessage ()
        {
            InputMessage id =
                KeyRegistry.Instance.GenerateInputMessage (
                    new List<KeyboardInput>(Keys), new List<MouseInput>(Mouse),
                    MouseMovement, MouseScroll, Stopwatch.Elapsed);
            Keys.Clear ();
            Mouse.Clear ();
            MouseMovement = Vector2.Zero;
            MouseScroll = Vector2.Zero;
            if (MessageCreated != null)
                MessageCreated (id);
            Stopwatch.Start();
        }

        /// <summary>
        /// Internal use only. Do not modify.
        /// </summary>
        internal List<string> CurrentlyDownKeys = new List<string>();
    }
}
