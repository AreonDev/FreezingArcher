//
//  CompositorInputSlot.cs
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

using FreezingArcher.Renderer;
using FreezingArcher.Output;

namespace FreezingArcher.Renderer.Compositor
{
    public enum CompositorInputSlotImportance
    {
        Optional,
        Parameter,
        Required
    }

    public class CompositorInputSlot
    {
        public string Name { get; private set;}
        public CompositorInputSlotImportance SlotImportance { get; private set;}
        public int SlotNumber { get; private set;}
        public Texture2D SlotTexture { get; private set;}
        public CompositorSlotType SlotType { get; private set;}

        bool IsConnected {get; set;}

        public CompositorInputSlot(string name, CompositorInputSlotImportance importance, int number, Texture2D texture, CompositorSlotType type)
        {
            Name = name;
            SlotImportance = importance;
            SlotNumber = number;

            SlotTexture = texture;
            SlotType = type;

            if ((SlotType == CompositorSlotType.Texture || SlotType == CompositorSlotType.ValueTexture) && SlotTexture == null)
                Logger.Log.AddLogEntry(LogLevel.Error, "CompositorInputSlot: " + Name, FreezingArcher.Core.Status.BadArgument);

            IsConnected = false;
        }
    }
}

