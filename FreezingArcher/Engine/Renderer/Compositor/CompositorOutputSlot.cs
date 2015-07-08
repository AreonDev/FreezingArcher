//
//  CompositorOutputSlot.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
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

using FreezingArcher.Output;
using FreezingArcher.Renderer;

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorOutputSlot
    {
        public string Name { get; private set;}
        public int SlotNumber { get; private set;}
        public Texture2D SlotTexture;
        public CompositorSlotType SlotType { get; private set;}

        bool IsConnected {get; set;}

        public CompositorOutputSlot(string name, int number, Texture2D output, CompositorSlotType type)
        {
            Name = name;
            SlotNumber = number;
            SlotTexture = output;
            SlotType = type;

            if ((SlotType == CompositorSlotType.Texture || SlotType == CompositorSlotType.ValueTexture) && SlotTexture == null)
                Logger.Log.AddLogEntry(LogLevel.Error, "CompositorInputSlot: " + Name, FreezingArcher.Core.Status.BadArgument);

            IsConnected = false;
        }
    }
}
