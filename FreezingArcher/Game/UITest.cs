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
using FreezingArcher.Math;
using FreezingArcher.Messaging;

namespace FreezingArcher.Game
{
    public class UITest : FreezingArcher.Messaging.Interfaces.IMessageConsumer
    {
        readonly Button[] buttons = new Button[6];
        readonly WindowControl window;
        readonly ScrollControl scroll;

        UISceneObject sceneobj;

        MessageProvider MessageProvider;

        public UITest (Application app, MessageProvider messageProvider)
        {
            ValidMessages = new[] { (int) MessageId.WindowResize };
            messageProvider += this;

            //game.AddGameState ("UITestState", Content.Environment.Default);

            var state = app.Game.GetGameState ("maze_overworld");
            //state.Scene = new FreezingArcher.Renderer.Scene.CoreScene (Application.Instance.RendererContext, state.MessageProxy);
            //state.Scene.BackgroundColor = FreezingArcher.Math.Color4.AliceBlue;

            //game.SwitchToGameState ("UITestState");

            sceneobj = new UISceneObject ();
            sceneobj.Priority = 999;
            state.Scene.AddObject (sceneobj);

            var input = new FreezingArcher.UI.Input.FreezingArcherInput(app, state.MessageProxy);
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

            new InventoryGUI(new Inventory(new Vector2i(5, 7), 6), messageProvider, sceneobj.Canvas);
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

        public void ConsumeMessage (FreezingArcher.Messaging.Interfaces.IMessage msg)
        {
            var wrm = msg as WindowResizeMessage;
            if (wrm != null)
            {
                sceneobj.Canvas.SetBounds (0, 0, wrm.Width, wrm.Height);
            }
        }

        public int[] ValidMessages { get; private set; }
    }
}
