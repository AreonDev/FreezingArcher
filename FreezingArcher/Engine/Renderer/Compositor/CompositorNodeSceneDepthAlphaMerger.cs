//
//  CompositorNodeSceneDepthAlphaMerger.cs
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

using FreezingArcher.Messaging;

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorNodeSceneDepthAlphaMerger : CompositorNode
    {
        private Texture2D OutputTexture;

        public CompositorNodeSceneDepthAlphaMerger(RendererContext rc, MessageProvider prov) : base("NodeDepthAlphaMerger", rc, prov)
        {
        }

        public override void InitOtherStuff()
        {
            OutputTexture = PrivateRendererContext.CreateTexture2D("SceneDepthAlphaMerger_Output_Texture_" + DateTime.Now.Ticks, PrivateRendererContext.ViewportSize.X,
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

            Sprite spr = new Sprite();
            spr.Init(InputSlots[0].SlotTexture);

            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Texture1"), 0);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("Texture2"), 1);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("TextureViewPosition1"), 2);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("TextureViewPosition2"), 3);

            spr.CustomEffect = true;

            PrivateRendererContext.DrawSpriteAbsolute(spr);

            PrivateRendererContext.EnableDepthTest(true);
        }

        #region implemented abstract members of CompositorNode

        public override void ConfigureSlots()
        {
            InputSlots = new CompositorInputSlot[4];
            InputSlots[0] = new CompositorInputSlot("InputTexture1", CompositorInputSlotImportance.Required, 0, CompositorSlotType.Texture);
            InputSlots[1] = new CompositorInputSlot("InputTexture2", CompositorInputSlotImportance.Required, 1, CompositorSlotType.Texture);

            InputSlots[2] = new CompositorInputSlot("InputView1", CompositorInputSlotImportance.Required, 2, CompositorSlotType.Texture);
            InputSlots[3] = new CompositorInputSlot("InputView2", CompositorInputSlotImportance.Required, 3, CompositorSlotType.Texture);

            OutputSlots = new CompositorOutputSlot[1];
            OutputSlots[0] = new CompositorOutputSlot("Output", 0, OutputTexture, CompositorSlotType.Texture);
        }

        public override void LoadEffect()
        {
            long ticks = DateTime.Now.Ticks;

            NodeEffect = PrivateRendererContext.CreateEffect("SceneDepthAlphaMerger_Effect_" + ticks);

            NodeEffect.PixelProgram = PrivateRendererContext.CreateShaderProgramFromFile("SceneDepthAlphaMerger_PixelProgram_" + ticks, ShaderType.PixelShader,
                "lib/Renderer/Effects/SceneDepthAlphaMerger/SceneDepthAlphaMerger.ps");

            NodeEffect.VertexProgram = PrivateRendererContext.RC2DEffect.VertexProgram;
        }

        #endregion
    }
}

