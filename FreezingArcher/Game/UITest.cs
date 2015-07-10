//
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
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Messaging;
using FreezingArcher.Content;

namespace FreezingArcher.Game
{
    public class UITest
    {
        readonly Button[] buttons = new Button[6];
        readonly WindowControl window;
        readonly ScrollControl scroll;

        UISceneObject sceneobj;

        MessageProvider MessageProvider;

        public UITest (Content.Game game)
        {
            game.AddGameState ("UITestState", Content.Environment.Default);

            var state = game.GetGameState ("UITestState");
            state.Scene = new FreezingArcher.Renderer.Scene.CoreScene (Application.Instance.RendererContext, state.MessageProxy);
            state.Scene.BackgroundColor = FreezingArcher.Math.Color4.AliceBlue;

            game.SwitchToGameState ("UITestState");

            sceneobj = new UISceneObject ();
            state.Scene.AddObject (sceneobj);

            var input = new FreezingArcher.UI.Input.FreezingArcherInput(state.MessageProxy);
            input.Initialize (sceneobj.Canvas);

            sceneobj.Canvas.SetSize(1024, 576);
            sceneobj.Canvas.ShouldDrawBackground = false;

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

            window = new WindowControl (sceneobj.Canvas);
            window.SetSize (400, 400);
            window.SetPosition (30, 30);
            window.Show ();

            scroll = new ScrollControl (window);
            scroll.SetBounds (10, 10, 370, 350);
            scroll.EnableScroll (true, false);
            scroll.Dock = Gwen.Pos.Fill;

            for (int i = 0; i < messages.Length; i++)
            {
                buttons [i] = new Button (scroll);
                buttons[i].Text = messages[i];
                buttons[i].SetBounds(recs[i]);
                buttons[i].Pressed += onButtonPressed;
                buttons[i].Clicked += onButtonClicked;
                buttons[i].Released += onButtonReleased;
            }
                
            scroll.ScrollToLeft ();
            scroll.ScrollToLeft ();
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
