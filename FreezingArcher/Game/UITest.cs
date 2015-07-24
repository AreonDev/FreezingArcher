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
using System.Security.Cryptography;

namespace FreezingArcher.Game
{
    public class UITest : FreezingArcher.Messaging.Interfaces.IMessageConsumer
    {
        readonly Button[] buttons = new Button[6];
        readonly WindowControl window;
        readonly ScrollControl scroll;

        UISceneObject sceneobj;

        MessageProvider MessageProvider;

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

            var inventory = new Inventory(messageProvider, state, player, new Vector2i(5, 7), 9);

            flashlight = Inventory.CreateNewItem(messageProvider, state, player,
                "flashlight",
                "Content/Flashlight/thumb.png",
                "flashlight_description",
                "Content/Flashlight/flashlight.xml",
                new Vector2i(2, 1),
                new Vector3(-0.55f, -0.33f, 0.4f),
                Orientation.Horizontal,
                ItemLocation.Inventory,
                AttackClass.Object,
                ItemUsage.Throwable,
                ItemComponent.DefaultProtection,
                new Jitter.Dynamics.Material { KineticFriction = 10, StaticFriction = 10, Restitution = -100 },
                1,
                0,
                0.01f,
                0f,
                5f,
                0f
            );
            inventory.Insert(flashlight);

            soda_can = Inventory.CreateNewItem(messageProvider, state, player,
                "soda_can",
                "Content/SodaCan/thumb.png",
                "soda_can_description",
                "Content/SodaCan/soda_can.xml",
                new Vector2i(1, 1),
                new Vector3(-0.4f, -0.25f, 0.5f),
                Orientation.Horizontal,
                ItemLocation.Inventory,
                AttackClass.Object,
                ItemUsage.Eatable,
                ItemComponent.DefaultProtection,
                new Jitter.Dynamics.Material { KineticFriction = 10, StaticFriction = 10, Restitution = -100 },
                0.5f,
                20,
                0.2f,
                0,
                0.2f,
                0
            );
            inventory.Insert(soda_can);

            choco_milk = Inventory.CreateNewItem(messageProvider, state, player,
                "choco_milk",
                "Content/ChocoMilk/thumb.png",
                "choco_milk_description",
                "Content/ChocoMilk/choco_milk.xml",
                new Vector2i(1, 1),
                new Vector3(-0.4f, -0.25f, 0.5f),
                Orientation.Horizontal,
                ItemLocation.Inventory,
                AttackClass.Object,
                ItemUsage.Eatable,
                ItemComponent.DefaultProtection,
                new Jitter.Dynamics.Material { KineticFriction = 50, StaticFriction = 50, Restitution = -10 },
                0.5f,
                20,
                0.2f,
                0,
                0.001f,
                0
            );
            inventory.Insert(choco_milk);

            pickaxe = Inventory.CreateNewItem(messageProvider, state, player,
                "pickaxe",
                "Content/Pickaxe/thumb.png",
                "pickaxe_description",
                "Content/Pickaxe/pickaxe.xml",
                new Vector2i(2, 4),
                new Vector3(-0.4f, -0.3f, 0.5f),
                Orientation.Horizontal,
                ItemLocation.Inventory,
                AttackClass.Object,
                ItemUsage.Hitable,
                ItemComponent.DefaultProtection,
                new Jitter.Dynamics.Material { KineticFriction = 20, StaticFriction = 20, Restitution = -100 },
                2f,
                0,
                0.2f,
                25,
                5f,
                0
            );
            inventory.Insert(pickaxe);

            new InventoryGUI(app, state, player, inventory, messageProvider, sceneobj.Canvas);
        }

        ItemComponent soda_can;
        ItemComponent flashlight;
        ItemComponent pickaxe;
        ItemComponent choco_milk;
        Entity player;

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
