//
//  Inventory.cs
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
using FreezingArcher.Math;
using FreezingArcher.DataStructures;
using System.Collections.Generic;
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    public sealed class Inventory
    {
        public Inventory(int sizeX, int sizeY) : this(new Vector2i(sizeX, sizeY))
        {}

        public Inventory(Vector2i size)
        {
            Size = size;
            storage = new int?[Size.X, Size.Y];
        }

        public Vector2i Size { get; private set; }

        readonly int?[,] storage;

        SortedDictionary<int, ItemComponent> items = new SortedDictionary<int, ItemComponent>();

        public ItemComponent GetItemAt(Vector2i position)
        {
            return GetItemAt(position.X, position.Y);
        }

        public ItemComponent GetItemAt(int positionX, int positionY)
        {
            ItemComponent res = null;
            var id = storage[positionX, positionY];
            if (id != null)
                items.TryGetValue(id.Value, out res);
            return res;
        }

        public bool Insert(ItemComponent item)
        {
            bool result = false;
            Orientation orientation = Orientation.Horizontal;
            int x = 0, y = 0;
            for (; y < storage.GetLength(0); y++)
            {
                x = 0;
                for (; x < storage.GetLength(1); x++)
                {
                    if (fits(new Vector2i(x, y), item.Size))
                    {
                        result = true;
                        orientation = Orientation.Horizontal;
                        break;
                    }
                    if (fits(new Vector2i(x, y), item.Size.Yx))
                    {
                        result = true;
                        orientation = Orientation.Vertical;
                        break;
                    }
                }
                if (result)
                    break;
            }
            return result && Insert(item, new Vector2i(x, y), orientation);
        }

        public bool Insert(ItemComponent item, Vector2i position, Orientation orientation)
        {
            var size = orientation == Orientation.Vertical ? item.Size.Yx : item.Size;
            if (fits(position, size))
            {
                for (int i = 0; i < size.X; i++)
                {
                    for (int k = 0; k < size.Y; k++)
                    {
                        storage[position.X + i, position.Y + k] = item.GetHashCode();
                    }
                }

                items.Add(item.GetHashCode(), item);

                return true;
            }
            else
            {
                Logger.Log.AddLogEntry(LogLevel.Error, "Inventory",
                    "Failed to insert inventory item '{0}' at position <{1},{2}> - position already in use!",
                    item.Entity.Name, position.X, position.Y);
            }
            return false;
        }

        public ItemComponent TakeOut(Vector2i position)
        {
            return TakeOut(position.X, position.Y);
        }

        public ItemComponent TakeOut(int positionX, int positionY)
        {
            var id = storage[positionX, positionY];

            if (id == null)
                return null;

            int x = positionX, y = positionY;

            storage[x, y] = null;

            while (x - 1 >= 0 && storage[x - 1, y] == id)
            {
                x--;
                while (y - 1 >= 0 && storage[x, y - 1] == id)
                {
                    y--;
                    storage[x, y] = null;
                }
            }

            x = positionX;
            y = positionY;

            while (x + 1 < Size.X && storage[x + 1, y] == id)
            {
                x++;
                while (y + 1 < Size.Y && storage[x, y + 1] == id)
                {
                    y++;
                    storage[x, y] = null;
                }
            }

            ItemComponent item;
            items.TryGetValue(id.Value, out item);
            items.Remove(id.Value);

            return item;
        }

        private bool fits(Vector2i position, Vector2i size)
        {
            for (int i = 0; i < size.X; i++)
            {
                for (int k = 0; k < size.Y; k++)
                {
                    if (position.X + i >= Size.X || position.Y + k >= Size.Y ||
                        storage[position.X + i, position.Y + k] != null)
                        return false;
                }
            }
            return true;
        }
    }
}
