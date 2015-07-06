﻿//
//  UITest.cs
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
using Gwen.Control;
using System.Drawing;
using FreezingArcher.Output;
using FreezingArcher.Core;

namespace FreezingArcher.Game
{
    public class UITest
    {
        readonly Button[] buttons = new Button[6];

        public UITest ()
        {
            Gwen.Renderer.Base renderer = new Gwen.Renderer.FreezingArcherGwenRenderer (Application.Instance.RendererContext);
            var skin = new Gwen.Skin.TexturedBase(renderer, "DefaultSkin.png");
            var canvas = new Canvas(skin);
            //var input = new FreezingArcher.UI.Input.FreezingArcher(canvas); TODO
            canvas.SetSize(1024, 576);
            canvas.ShouldDrawBackground = false;

            var messages = new[] {
                "I'm a truely buttonshit!",
                "Don't click me, I'm scared!",
                "Press me hard, modafuzka!",
                "Jumpscare ahead...",
                "-.-",
                "__click__"
            };

            var recs = new[] {
                new Rectangle(10, 50, 200, 20),
                new Rectangle(220, 50, 200, 20),
                new Rectangle(430, 50, 200, 20),
                new Rectangle(10, 80, 200, 20),
                new Rectangle(220, 80, 100, 20),
                new Rectangle(330, 80, 150, 20)
            };

            for (int i = 0; i < messages.Length; i++)
            {
                buttons[i] = new Button(canvas);
                buttons[i].Text = messages[i];
                buttons[i].SetBounds(recs[i]);
                buttons[i].Pressed += onButtonPressed;
                buttons[i].Clicked += onButtonClicked;
                buttons[i].Released += onButtonReleased;
            }
        }

        void onButtonPressed(Base btn, EventArgs args)
        {
            Logger.Log.AddLogEntry(LogLevel.Debug, "UITest", "Button pressed");
        }

        void onButtonClicked(Base btn, EventArgs args)
        {
            Logger.Log.AddLogEntry(LogLevel.Debug, "UITest", "Button clicked");
        }

        void onButtonReleased(Base btn, EventArgs args)
        {
            Logger.Log.AddLogEntry(LogLevel.Debug, "UITest", "Button released");
        }
    }
}
