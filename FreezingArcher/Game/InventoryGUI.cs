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

namespace FreezingArcher.Game
{
    public class InventoryGUI : IMessageConsumer
    {
        public InventoryGUI (Inventory inventory, Base parent)
        {
            this.inventory = inventory;

            var window = new WindowControl(parent, "Inventory");
            window.DisableResizing();
            window.SetSize (400, 400);
            window.SetPosition (100, 100);
            window.Show ();
            var table = new Table(window);
            table.ColumnCount = inventory.Size.X;
            var row = new TableRow(table);
            var item = new ImagePanel(row);
            item.ImageName = "lib/command.png";
            item.SizeToContents();
            row.ColumnCount = inventory.Size.Y;
            row.SetCellContents(0, item, true);
            table.AddRow(row);
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
