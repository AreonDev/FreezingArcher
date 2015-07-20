//
//  CompositorNodeStart.cs
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

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorNodeStart : CompositorNode
    {
        public CompositorNodeStart(RendererContext rc, Messaging.MessageProvider mp) : base("NodeStart", rc, mp)
        {
        }

        public override bool Init(RendererContext rc)
        {
            return base.Init(rc);
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override void Draw()
        {
            
        }

        public override void End()
        {
            base.End();
        }

        public override void ConfigureSlots()
        {
            Active = true;

            InputSlots = null;
            OutputSlots = new CompositorOutputSlot[1];
            OutputSlots[0] = new CompositorOutputSlot("Begin", 0, null, CompositorSlotType.NoTextureOrValueOutput);
        }

        public override void LoadEffect()
        {
            NodeEffect = null;
        }
    }
}

