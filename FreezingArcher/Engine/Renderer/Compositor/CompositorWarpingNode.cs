//
//  CompositorWarpingNode.cs
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
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;

namespace FreezingArcher.Renderer.Compositor
{
    public sealed class CompositorWarpingNode : CompositorNode, IMessageConsumer
    {
        private Texture2D OutputTexture;

        public CompositorWarpingNode (RendererContext rc, MessageProvider mp)
            : base ("NodeWarping", rc, mp)
        {
            WarpFactor = 0;
            ValidMessages = new [] { (int) MessageId.Update };
            mp += this;
        }

        public override void InitOtherStuff ()
        {
            OutputTexture = PrivateRendererContext.CreateTexture2D("Warping_Output_Texture_" + DateTime.Now.Ticks,
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

            if (WarpTexture != null)
                WarpTexture.Bind (1);

            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("input"), 0);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("warpFactor"), WarpFactor);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("timer"),
                timer / 10000);
            NodeEffect.PixelProgram.SetUniform(NodeEffect.PixelProgram.GetUniformLocation("warpTexture"), 1);

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

            NodeEffect = PrivateRendererContext.CreateEffect("Warping_Effect_" + ticks);

            NodeEffect.PixelProgram = PrivateRendererContext.CreateShaderProgramFromFile(
                "Color_Correction_PixelProgram_" + ticks, ShaderType.PixelShader,
                "lib/Renderer/Effects/Warping/warping.ps");
            
            NodeEffect.VertexProgram = PrivateRendererContext.RC2DEffect.VertexProgram;
        }

        float timer = 0;

        #endregion

        public float WarpFactor { get; set; }

        public Texture2D WarpTexture { get; set; }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Update && running)
            {
                var um = msg as UpdateMessage;
                timer += (float) um.TimeStamp.TotalMilliseconds;
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion

        bool running = true;

        public void Stop ()
        {
            running = false;
        }

        public void Start ()
        {
            running = true;
        }
    }
}

