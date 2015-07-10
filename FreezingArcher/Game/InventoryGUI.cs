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

namespace FreezingArcher.Game
{
    public class InventoryButton : Button
    {
        public InventoryButton(Base parent) : base (parent)
        {}

        public override bool DragAndDrop_ShouldStartDrag()
        {
            if (!base.DragAndDrop_ShouldStartDrag())
                return false;

            return true;
        }

        public override bool DragAndDrop_CanAcceptPackage (Gwen.DragDrop.Package p)
        {
            return true;
        }

        public override bool DragAndDrop_Draggable ()
        {
            return base.DragAndDrop_Draggable();
        }

        public override void DragAndDrop_EndDragging (bool success, int x, int y)
        {
            var package = DragAndDrop_GetPackage(x,y);
            var tuple = package.UserData as Tuple<string, string>;
            if (tuple != null)
            {
                SetImage(tuple.Item2);
                Text = tuple.Item1;
            }
            base.DragAndDrop_EndDragging (success, x, y);
        }

        public override Gwen.DragDrop.Package DragAndDrop_GetPackage (int x, int y)
        {
            return base.DragAndDrop_GetPackage (x, y);
        }

        public override bool DragAndDrop_HandleDrop (Gwen.DragDrop.Package p, int x, int y)
        {
            var tuple = p.UserData as Tuple<string, string>;
            if (tuple != null)
            {
                SetImage(tuple.Item2);
                Text = tuple.Item1;
            }
            return base.DragAndDrop_HandleDrop (p, x, y);
        }

        public override void DragAndDrop_Hover (Gwen.DragDrop.Package p, int x, int y)
        {
            base.DragAndDrop_Hover (p, x, y);
            Logger.Log.AddLogEntry(LogLevel.Debug, "InventoryGUI", "Hover");
        }

        public override void DragAndDrop_HoverEnter (Gwen.DragDrop.Package p, int x, int y)
        {
            base.DragAndDrop_HoverEnter (p, x, y);
            Logger.Log.AddLogEntry(LogLevel.Debug, "InventoryGUI", "Enter");
        }

        public override void DragAndDrop_HoverLeave (Gwen.DragDrop.Package p)
        {
            base.DragAndDrop_HoverLeave (p);
            Logger.Log.AddLogEntry(LogLevel.Debug, "InventoryGUI", "Leave");
        }

        public override void DragAndDrop_SetPackage (bool draggable, string name = "", object userData = null)
        {
            base.DragAndDrop_SetPackage (draggable, name, userData);
        }

        public override void DragAndDrop_StartDragging(Package package, int x, int y)
        {
            var image = m_Image != null ? m_Image.ImageName : "";
            package.UserData = new Tuple<string, string>(Text, image);
            base.DragAndDrop_StartDragging(package, x, y);
        }
    }

    public class InventoryGUI : IMessageConsumer
    {
        public InventoryGUI (Inventory inventory, Base parent)
        {
            this.inventory = inventory;

            string[][] items = new[] {
                new[] {"", "", "", "", "", "", "", "", "", ""},
                new[] {"", "", "command.png", "", "", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "", "", "", ""},
                new[] {"", "", "", "", "command.png", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "", "", "", ""},
                new[] {"", "", "", "command.png", "", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "command.png", "", "", ""},
                new[] {"", "command.png", "", "", "", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "", "", "", ""},
                new[] {"", "", "", "", "", "", "", "", "", ""},
            };

            InventoryButton[,] buttons = new InventoryButton[inventory.Size.X,inventory.Size.Y];

            var window = new WindowControl(parent, "Inventory");
            window.DisableResizing();
            window.SetSize (760, 402);
            window.SetPosition (100, 100);
            window.Show ();

            int w = 0, h = 0;

            for (int y = 0; y < inventory.Size.Y; y++)
            {
                for (int x = 0; x < inventory.Size.X; x++)
                {
                    buttons[x,y] = new InventoryButton(window);
                    buttons[x,y].SetPosition(w,h);
                    buttons[x,y].SetSize(64, 64);
                    buttons[x,y].Text = x + "." + y;
                    if (items[y][x].Length != 0)
                        buttons[x,y].SetImage(items[y][x], true);

                    buttons[x,y].DragAndDrop_SetPackage(true, "item_drag");

                    w += 76;
                }
                h += 76;
                w = 0;
            }
        }

        Inventory inventory;

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
        }

        public int[] ValidMessages { get; private set; }
        #endregion
    }
}
