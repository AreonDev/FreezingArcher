//
//  CompositorNodeTextureAlphaMerger.cs
//
//  Author:
//       dboeg <>
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

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorNodeTextureAlphaMerger : CompositorNode
    {
        private Texture2D OutputTexture;

        public Math.Vector2 uShift { get; set;}

        public CompositorNodeTextureAlphaMerger(RendererContext rc, Messaging.MessageProvider mp) : base("NodeTextureAlphaMerger", rc, mp)
        {

        }

        public override void InitOtherStuff()
        {
            OutputTexture = PrivateRendererContext.CreateTexture2D("Merged_Output_Texture_" + DateTime.Now.Ticks, PrivateRendererContext.ViewportSize.X,
                PrivateRendererContext.ViewportSize.Y, false, IntPtr.Zero, false, true);
        }

        public override void Draw()
        {
            PrivateRendererContext.EnableDepthTest(false);

            if (InputSlots[0].SlotTexture == null)
                return;

            if (InputSlots[1].SlotTexture.Width != OutputTexture.Width || InputSlots[1].SlotTexture.Height != OutputTexture.Height)
            {
                OutputTexture.Resize(InputSlots[1].SlotTexture.Width, InputSlots[1].SlotTexture.Height);
            }

            PrivateRendererContext.Clear(Math.Color4.Black);

            Sprite spr1 = new Sprite();
            spr1.Init(InputSlots[0].SlotTexture);

            Sprite spr2 = new Sprite();
            spr2.Init(InputSlots[1].SlotTexture);

            PrivateRendererContext.DrawSpriteAbsolute(spr1);
            PrivateRendererContext.DrawSpriteAbsolute(spr2);

            PrivateRendererContext.EnableDepthTest(true);
        }

        #region implemented abstract members of CompositorNode

        public override void ConfigureSlots()
        {
            InputSlots = new CompositorInputSlot[2];
            InputSlots[0] = new CompositorInputSlot("Input1", CompositorInputSlotImportance.Required, 0, CompositorSlotType.Texture);
            InputSlots[1] = new CompositorInputSlot("Input2", CompositorInputSlotImportance.Required, 0, CompositorSlotType.Texture);
             
            OutputSlots = new CompositorOutputSlot[1];
            OutputSlots[0] = new CompositorOutputSlot("Output", 0, OutputTexture, CompositorSlotType.Texture);
        }

        public override void LoadEffect()
        {
            NodeEffect = null;
        }

        #endregion
    }
}
