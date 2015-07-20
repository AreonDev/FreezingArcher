//
//  InventoryGUI.cs
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
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Content;
using FreezingArcher.Messaging;
using FreezingArcher.Math;
using Gwen.Control;
using Gwen.Control.Layout;
using Gwen.DragDrop;
using FreezingArcher.Output;
using System.Drawing;
using FreezingArcher.DataStructures;

namespace FreezingArcher.Game
{
    public class InventorySpace : Button
    {
        public InventorySpace(Base parent, int boxSize, Inventory inventory) : base (parent)
        {
            DrawDebugOutlines = true;
            foreach(var child in Children)
                child.DrawDebugOutlines = false;
            ShouldDrawBackground = false;
            BoundsOutlineColor = parent.GetCanvas().Skin.Colors.Label.Dark;
            DragAndDrop_SetPackage(true, "item_drag");
            this.boxSize = boxSize;
            this.inventory = inventory;
        }

        readonly int boxSize;
        readonly Inventory inventory;

        public override bool DragAndDrop_CanAcceptPackage (Package p)
        {
            return true;
        }

        public override bool DragAndDrop_Draggable ()
        {
            return false;
        }

        public override void DragAndDrop_HoverEnter (Package p, int x, int y)
        {
            ShouldDrawBackground = true;
        }

        public override void DragAndDrop_HoverLeave (Package p)
        {
            ShouldDrawBackground = false;
        }

        public override bool DragAndDrop_HandleDrop (Package p, int x, int y)
        {
            InventoryGUI.PrintInventory("Inventory:", inventory);
            var item = p.UserData as Tuple<int, int, InventoryButton, ItemComponent>;

            int pos_x = X / boxSize - item.Item1;
            int pos_y = Y / boxSize - item.Item2;
            int pos_w = item.Item3.Width / boxSize;
            int pos_h = item.Item3.Height / boxSize;

            if (pos_x < 0 || pos_x + pos_w >= inventory.Size.X || pos_y < 0 || pos_y + pos_h >= inventory.Size.Y)
                return false;

            if (inventory.Insert(item.Item4, new Vector2i(pos_x, pos_y)))
            {
                item.Item3.X = pos_x * boxSize + 1;
                item.Item3.Y = pos_y * boxSize + 1;
                InventoryGUI.PrintInventory("Inventory:", inventory);
                return true;
            }
            return false;
        }
    }

    public class InventoryButton : Button
    {
        public InventoryButton(Base parent, Inventory inventory, ItemComponent item, Vector2i position, int boxSize) :
        base (parent)
        {
            this.boxSize = boxSize;
            this.IsToggle = true;
            this.inventory = inventory;
            this.item = item;
            X = position.X * boxSize + 1;
            Y = position.Y * boxSize + 1;
            Width = item.Size.X * boxSize - 1;
            Height = item.Size.Y * boxSize - 1;
            DragAndDrop_SetPackage(true, "item_drag");
        }

        readonly Inventory inventory;
        readonly ItemComponent item;

        readonly int boxSize;

        public override bool DragAndDrop_CanAcceptPackage (Package p)
        {
            return false;
        }

        public override bool DragAndDrop_Draggable ()
        {
            return true;
        }

        public override void DragAndDrop_StartDragging(Package package, int x, int y)
        {
            InventoryGUI.PrintInventory("Inventory:", inventory);
            var loc = CanvasPosToLocal(new Point(x, y));
            int pos_x = loc.X / boxSize;
            int pos_y = loc.Y / boxSize;
            var tmp = inventory.TakeOut(item);
            package.UserData = new Tuple<int, int, InventoryButton, ItemComponent>(pos_x, pos_y, this, item);
            base.DragAndDrop_StartDragging(package, x, y);
            InventoryGUI.PrintInventory("Inventory:", inventory);
        }
    }

    public class InventoryGUI : IMessageConsumer
    {
        public InventoryGUI (Inventory inventory, MessageProvider messageProvider, Base parent)
        {
            this.inventory = inventory;

            var spaces = new InventorySpace[inventory.Size.X,inventory.Size.Y];

            var window = new WindowControl(parent, "Inventory");
            window.DisableResizing();

            var itemGridFrame = new Base(window);
            itemGridFrame.SetSize((BoxSize + 1) * inventory.Size.X, (BoxSize + 1) * inventory.Size.Y);

            var itemInfoFrame = new Base(window);
            itemInfoFrame.SetSize(300, itemGridFrame.Height);
            itemGridFrame.X += itemInfoFrame.Width + 4;

            window.SetSize (itemGridFrame.Width + itemInfoFrame.Width + 4 + 8, itemGridFrame.Height + 28);
            window.SetPosition (100, 100);
            window.Show ();

            int w = 0, h = 0;

            for (int y = 0; y < inventory.Size.Y; y++)
            {
                for (int x = 0; x < inventory.Size.X; x++)
                {
                    spaces[x,y] = new InventorySpace(itemGridFrame, BoxSize, inventory);
                    spaces[x,y].X = w;
                    spaces[x,y].Y = h;
                    spaces[x,y].Width = BoxSize + 1;
                    spaces[x,y].Height = BoxSize + 1;

                    w += BoxSize;
                }
                h += BoxSize;
                w = 0;
            }

            Entity entity1 = EntityFactory.Instance.CreateWith("A", messageProvider,
                new[] { typeof(ItemComponent) });
            var item1 = entity1.GetComponent<ItemComponent>();
            item1.Size = new Vector2i(2, 1);
            inventory.Insert(item1);
            var btn1 = new InventoryButton(itemGridFrame, inventory, item1,
                inventory.GetPositionOfItem(item1), BoxSize);

            Entity entity2 = EntityFactory.Instance.CreateWith("B", messageProvider,
                new[] { typeof(ItemComponent) });
            var item2 = entity2.GetComponent<ItemComponent>();
            item2.Size = new Vector2i(1, 2);
            inventory.Insert(item2);
            var btn2 = new InventoryButton(itemGridFrame, inventory, item2,
                inventory.GetPositionOfItem(item2), BoxSize);

            Entity entity3 = EntityFactory.Instance.CreateWith("C", messageProvider,
                new[] { typeof(ItemComponent) });
            var item3 = entity3.GetComponent<ItemComponent>();
            item3.Size = new Vector2i(2, 2);
            inventory.Insert(item3);
            var btn3 = new InventoryButton(itemGridFrame, inventory, item3,
                inventory.GetPositionOfItem(item3), BoxSize);

            PrintInventory("Inventory:", inventory);
        }

        public static void PrintInventory(string message, Inventory inv)
        {
            string s = message;
            for (int i = 0; i < inv.Size.Y; i++)
            {
                s += "\n";
                for (int k = 0; k < inv.Size.X; k++)
                {
                    var item = inv.GetItemAt(k, i);
                    if (item != null)
                    {
                        s += item.Entity.Name;
                    }
                    else
                    {
                        s += "#";
                    }
                }
            }

            Logger.Log.AddLogEntry(LogLevel.Debug, "InventoryTest", s);
        }

        public static readonly int BoxSize = 64;

        Inventory inventory;

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
        }

        public int[] ValidMessages { get; private set; }
        #endregion
    }
}
