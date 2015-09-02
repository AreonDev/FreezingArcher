//
//  CompositorNodeOutput.cs
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
    public class CompositorNodeOutput : CompositorNode
    {
        public CompositorNodeOutput(RendererContext rc, Messaging.MessageProvider mp) : base("NodeOutput", rc, mp)
        {
            EnableUI = true;
        }

        public bool EnableUI{ get; set;}

        public Texture2D OutputTexture { get; private set;}

        #region implemented abstract members of CompositorNode

        public override void ConfigureSlots()
        {
            InputSlots = new CompositorInputSlot[1];
            InputSlots[0] = new CompositorInputSlot("Result Input", CompositorInputSlotImportance.Required, 0, CompositorSlotType.Texture);
        }

        public override void LoadEffect()
        {
            //No need to load any effect...
        }

        public override void Draw()
        {
            PrivateRendererContext.EnableDepthTest(false);

            PrivateRendererContext.Clear(Math.Color4.Black);

            if (InputSlots[0].SlotTexture != null && InputSlots[0].SlotTexture.Created)
            {
                Sprite spr = new Sprite();
                spr.Init(InputSlots[0].SlotTexture);

                spr.AbsolutePosition = new FreezingArcher.Math.Vector2(0, 0);
                spr.CustomEffect = false;

                PrivateRendererContext.DrawSpriteAbsolute(spr);

                OutputTexture = InputSlots [0].SlotTexture;
            }

            PrivateRendererContext.EnableDepthTest(true);

            if (EnableUI)
                PrivateRendererContext.Canvas.RenderCanvas();
        }

        #endregion
    }
}

