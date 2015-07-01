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

namespace FreezingArcher.Content
{
    public sealed class Inventory
    {
        public Inventory(int sizeX, int sizeY) : this(new Vector2i(sizeX, sizeY));

        public Inventory(Vector2i size)
        {
            Size = size;
            Storage = new int?[Size.X, Size.Y];
        }

        public Vector2i Size { get; private set; }

        public int?[,] Storage { get; private set; }

        SortedDictionary<int, ItemComponent> items = new SortedDictionary<int, ItemComponent>();

        public bool Insert(ItemComponent item)
        {
            bool result = false;
            Orientation orientation = Orientation.Horizontal;
            int x = 0, y = 0;
            for (; x < Storage.GetLength(0); x++)
            {
                for (; y < Storage.GetLength(1); y++)
                {
                    if (fits(new Vector2i(x, y), item.Size))
                    {
                        result = true;
                        orientation = Orientation.Horizontal;
                        break;
                    }
                    if (fits(new Vector2i(y, x), item.Size))
                    {
                        result = true;
                        orientation = Orientation.Vertical;
                        break;
                    }
                }
                if (result)
                    break;
            }
            return Insert(item, new Vector2i(x, y), orientation);
        }

        public bool Insert(ItemComponent item, Vector2i position, Orientation orientation)
        {
            var pos = orientation == Orientation.Vertical ? new Vector2i(position.Y, position.X) : position;
            if (fits(pos, item.Size))
            {
                for (int i = 0; i < Size.X; i++)
                {
                    for (int k = 0; k < Size.Y; k++)
                    {
                        Storage[pos.X + i, pos.Y + k] = item.GetHashCode();
                    }
                }

                items.Add(item.GetHashCode(), item);

                return true;
            }
            return false;
        }

        private bool fits(Vector2i position, Vector2i size)
        {
            for (int i = 0; i < size.X; i++)
            {
                for (int k = 0; k < size.Y; k++)
                {
                    if (Storage[position.X + i, position.Y + k] != null)
                        return false;
                }
            }
            return true;
        }
    }
}
