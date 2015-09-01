//
//  CompositorImageOverlayNode.cs
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
using FreezingArcher.Renderer;
using FreezingArcher.Renderer.Compositor;

namespace FreezingArcher.Renderer.Compositor
{
    public enum OverlayBlendMode
    {
        Add = 0,
        Subtract = 1,
        SubtractInverse = 2,
        Multiply = 3,
        Divide = 4,
        DivideInverse = 5
    }

    public sealed class CompositorImageOverlayNode : CompositorNode
    {
        private Texture2D OutputTexture;

        public CompositorImageOverlayNode (RendererContext rc, Messaging.MessageProvider mp) : base ("NodeOverlay", rc, mp)
        {
            Factor = 0.5f;
        }

        public override void InitOtherStuff ()
        {
            OutputTexture = PrivateRendererContext.CreateTexture2D("Overlay_Output_Texture_" + DateTime.Now.Ticks,
                PrivateRendererContext.ViewportSize.X, PrivateRendererContext.ViewportSize.Y, false, IntPtr.Zero,
                false, true);
        }

        public override void Draw ()
        {
            PrivateRendererContext.EnableDepthTest (false);

            if (InputSlots[0].SlotTexture == null)
                return;

            if (InputSlots[0].SlotTexture.Width != OutputTexture.Width ||
                InputSlots[0].SlotTexture.Height != OutputTexture.Height)
            {
                OutputTexture.Resize (InputSlots[0].SlotTexture.Width, InputSlots[0].SlotTexture.Height);
                //OutputTexture.Bind(0);
            }

            PrivateRendererContext.Clear (Math.Color4.Black);

            Sprite spr = new Sprite();
            spr.Init (InputSlots[0].SlotTexture);

            spr.CustomEffect = true;

            OverlayTexture.Bind (1);

            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("input"), 0);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("overlayTexture"), 1);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("overlayBlendMode"), (int) Blending);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("overlayFactor"), Factor);

            PrivateRendererContext.DrawSpriteAbsolute (spr);

            PrivateRendererContext.EnableDepthTest (true);
        }

        #region implemented abstract members of CompositorNode

        public override void ConfigureSlots ()
        {
            InputSlots = new CompositorInputSlot[1];
            InputSlots[0] = new CompositorInputSlot("Input", CompositorInputSlotImportance.Required, 0, CompositorSlotType.Texture);

            OutputSlots = new CompositorOutputSlot[1];
            OutputSlots[0] = new CompositorOutputSlot("Output", 0, OutputTexture, CompositorSlotType.Texture);
        }

        public override void LoadEffect ()
        {
            long ticks = DateTime.Now.Ticks;

            NodeEffect = PrivateRendererContext.CreateEffect("Overlay_Effect_" + ticks);

            NodeEffect.PixelProgram = PrivateRendererContext.CreateShaderProgramFromFile("Overlay_PixelProgram_" + ticks, ShaderType.PixelShader,
                "lib/Renderer/Effects/ImageOverlay/overlay.ps");
            
            NodeEffect.VertexProgram = PrivateRendererContext.RC2DEffect.VertexProgram;
        }

        #endregion

        public Texture2D OverlayTexture { get; set; }

        public OverlayBlendMode Blending { get; set; }

        public float Factor { get; set; }
    }
}

