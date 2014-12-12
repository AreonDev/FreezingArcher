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
using System;
using System.Collections.Generic;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Graphics;
using FurryLana.Engine.Input.Interfaces;

namespace FurryLana.Engine.Input
{
    public class InputManager : IInputManager
    {
        public InputManager ()
        {
            Keys = new List<KeyboardInput> ();
            Mouse = new List<MouseInput> ();
            MouseMovement = Vector2.Zero;
            MouseScroll = Vector2.Zero;
            OldMousePosition = Vector2.Zero;
        }

        protected List<KeyboardInput> Keys;
        protected List<MouseInput> Mouse;
        protected Vector2 MouseMovement;
        protected Vector2 MouseScroll;
        protected Vector2 OldMousePosition;

        #region IInputManager implementation

        public void HandleKeyboardInput (GlfwWindowPtr window, Key key, int scancode,
                                         KeyAction action, KeyModifiers modifier)
        {
            Keys.Add (new KeyboardInput (key, scancode, action, modifier));
        }

        public void HandleMouseButton (GlfwWindowPtr window, MouseButton button, KeyAction action)
        {
            Mouse.Add (new MouseInput (button, action));
        }

        public void HandleMouseMove (GlfwWindowPtr window, double x, double y)
        {
            MouseMovement += new Vector2 ((float) x - OldMousePosition.X, (float) y - OldMousePosition.Y);
            OldMousePosition = new Vector2 ((float) x, (float) y);
        }

        public void HandleMouseScroll (GlfwWindowPtr window, double xoffs, double yoffs)
        {
            MouseScroll += new Vector2 ((float) xoffs, (float) yoffs);
        }

        public UpdateDescription GenerateUpdateDescription (float deltaTime)
        {
            UpdateDescription ud = new UpdateDescription (Keys, Mouse, MouseMovement, MouseScroll, deltaTime);
            Keys.Clear ();
            Mouse.Clear ();
            MouseMovement = Vector2.Zero;
            MouseScroll = Vector2.Zero;
            return ud;
        }

        #endregion
    }
}

