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
    public class UITest
    {
        public UITest (Application app, MessageProvider messageProvider, Entity player, RendererContext rc)
        {
            var state = app.Game.GetGameState ("maze_overworld");

            this.player = player;

            var input = new FreezingArcher.UI.Input.FreezingArcherInput(app, state.MessageProxy);
            input.Initialize (rc.Canvas);
            rc.Canvas.SetSize(app.Window.Size.X, app.Window.Size.Y);
            rc.Canvas.ShouldDrawBackground = false;

            inventoryGui = new InventoryGUI(app, state, player, messageProvider);
            var inventory = new Inventory(messageProvider, state, player, new Vector2i(5, 7), 9);
            inventoryGui.Init(rc.Canvas, inventory);
        }

        Entity player;
        InventoryGUI inventoryGui;
    }
}
