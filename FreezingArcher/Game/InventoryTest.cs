//
//  InventoryTest.cs
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
using FreezingArcher.Content;
using FreezingArcher.Math;
using FreezingArcher.DataStructures;
using FreezingArcher.Output;

namespace FreezingArcher.Game
{
    public class InventoryTest
    {
        public InventoryTest ()
        {
            Entity item1 = EntityFactory.Instance.CreateWith("A", new[] { typeof(ItemComponent) });
            var item1comp = item1.GetComponent<ItemComponent>();
            item1comp.AttackClasses = AttackClass.Enemy;
            item1comp.AttackStrength = 2.5f;
            item1comp.HealthDelta = 0f;
            item1comp.ItemUsages = ItemUsage.Throwable | ItemUsage.Usable;
            item1comp.Location = ItemLocation.Inventory;
            item1comp.Protection.Cold = 0.1f;
            item1comp.Protection.Heat = 0.3f;
            item1comp.Protection.Hit = 2f;
            item1comp.Size = new Vector2i(2, 1);
            item1comp.ThrowPower = 5f;
            item1comp.Usage = 0;

            Entity item2 = EntityFactory.Instance.CreateWith("B", new[] { typeof(ItemComponent) });
            var item2comp = item2.GetComponent<ItemComponent>();
            item2comp.AttackClasses = AttackClass.Nothing;
            item2comp.AttackStrength = 0f;
            item2comp.HealthDelta = 15f;
            item2comp.ItemUsages = ItemUsage.Eatable;
            item2comp.Location = ItemLocation.Inventory;
            item2comp.Protection.Cold = 0.5f;
            item2comp.Protection.Heat = 0.1f;
            item2comp.Protection.Hit = 0f;
            item2comp.Size = new Vector2i(5, 1);
            item2comp.ThrowPower = 1f;
            item2comp.Usage = 0;

            Inventory inv = new Inventory(10, 20);
            inv.Insert(item1comp, new Vector2i(0, 3), Orientation.Horizontal);
            inv.Insert(item2comp);

            string s = "";
            for (int i = 0; i < inv.Size.X; i++)
            {
                s += "\n";
                for (int k = 0; k < inv.Size.Y; k++)
                {
                    var item = inv.GetItemAt(i, k);
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
    }
}
