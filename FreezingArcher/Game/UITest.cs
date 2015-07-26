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
using FreezingArcher.DataStructures;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer;
using Jitter.LinearMath;

namespace FreezingArcher.Game
{
    public class UITest : FreezingArcher.Messaging.Interfaces.IMessageConsumer
    {
        readonly UISceneObject sceneobj;

        public UITest (Application app, MessageProvider messageProvider, Entity player, CoreScene scene)
        {
            ValidMessages = new[] { (int) MessageId.WindowResize };
            messageProvider += this;

            var state = app.Game.GetGameState ("maze_overworld");

            sceneobj = new UISceneObject ();
            sceneobj.Priority = 999;
            scene.BackgroundColor = Color4.Transparent;
            scene.AddObject (sceneobj);
            this.player = player;

            var input = new FreezingArcher.UI.Input.FreezingArcherInput(app, state.MessageProxy);
            input.Initialize (sceneobj.Canvas);
            sceneobj.Canvas.SetSize(app.Window.Size.X, app.Window.Size.Y);
            sceneobj.Canvas.ShouldDrawBackground = false;

            inventoryGui = new InventoryGUI(app, state, player, messageProvider);
            var inventory = new Inventory(messageProvider, state, player, new Vector2i(5, 7), 9);
            inventoryGui.Init(sceneobj.Canvas, inventory);
        }

        Entity player;
        InventoryGUI inventoryGui;

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
