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

            Entity item3 = EntityFactory.Instance.CreateWith("C", new[] { typeof(ItemComponent) });
            var item3comp = item3.GetComponent<ItemComponent>();
            item3comp.Location = ItemLocation.Inventory;
            item3comp.Size = new Vector2i(2, 3);

            Entity item4 = EntityFactory.Instance.CreateWith("D", new[] { typeof(ItemComponent) });
            var item4comp = item4.GetComponent<ItemComponent>();
            item4comp.Location = ItemLocation.Inventory;
            item4comp.Size = new Vector2i(1, 3);

            Entity item5 = EntityFactory.Instance.CreateWith("E", new[] { typeof(ItemComponent) });
            var item5comp = item5.GetComponent<ItemComponent>();
            item5comp.Location = ItemLocation.Inventory;
            item5comp.Size = new Vector2i(1, 1);

            Entity item6 = EntityFactory.Instance.CreateWith("F", new[] { typeof(ItemComponent) });
            var item6comp = item6.GetComponent<ItemComponent>();
            item6comp.Location = ItemLocation.Inventory;
            item6comp.Size = new Vector2i(1, 1);

            Entity item7 = EntityFactory.Instance.CreateWith("G", new[] { typeof(ItemComponent) });
            var item7comp = item7.GetComponent<ItemComponent>();
            item7comp.Location = ItemLocation.Inventory;
            item7comp.Size = new Vector2i(2, 1);

            Entity item8 = EntityFactory.Instance.CreateWith("H", new[] { typeof(ItemComponent) });
            var item8comp = item8.GetComponent<ItemComponent>();
            item8comp.Location = ItemLocation.Inventory;
            item8comp.Size = new Vector2i(1, 3);

            Entity item9 = EntityFactory.Instance.CreateWith("I", new[] { typeof(ItemComponent) });
            var item9comp = item9.GetComponent<ItemComponent>();
            item9comp.Location = ItemLocation.Inventory;
            item9comp.Size = new Vector2i(1, 2);

            Inventory inv = new Inventory(5, 5);
            inv.Insert(item1comp);
            inv.Insert(item3comp);
            inv.Insert(item4comp);
            inv.Insert(item2comp);
            inv.Insert(item5comp);
            inv.Insert(item6comp);
            inv.Insert(item7comp);
            inv.Insert(item8comp);
            inv.Insert(item9comp);

            PrintInventory(inv);

            inv.TakeOut(3, 2);

            PrintInventory(inv);
        }

        public void PrintInventory(Inventory inv)
        {
            string s = "";
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
    }
}
