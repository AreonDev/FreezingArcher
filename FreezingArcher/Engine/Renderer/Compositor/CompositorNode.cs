//
//  CompositorNode.cs
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
using System.Collections.Generic;

using FreezingArcher.Configuration;
using FreezingArcher.Renderer;
using FreezingArcher.DataStructures;

namespace FreezingArcher.Renderer.Compositor
{
    class RCActionCompositorNodeInit : RendererCore.RCAction
    {
        public RendererContext RendererContext;

        public CompositorNode Node;

        public bool WasCalled { get; private set;}

        public RCActionCompositorNodeInit(RendererContext rc, CompositorNode node)
        {
            RendererContext = rc;
            Node = node;

            WasCalled = false;
        }

        #region RCAction implementation

        public RendererCore.RCActionDelegate Action
        {
            get
            {
                return delegate ()
                {
                    Node.InitOtherStuff();
                    Node.ConfigureSlots();
                    Node.LoadEffect();
                  
                    Node.ConfigureSlots();
                    Node.InitFramebuffer();

                    WasCalled = true;
                };
            }
        }

        #endregion
    }

    public enum CompositorSlotType
    {
        Value,
        ValueTexture,
        Texture,
        NoTextureOrValueOutput
    }

    public abstract class CompositorNode
    {
        public CompositorInputSlot[] InputSlots { get; protected set;}
        public CompositorOutputSlot[] OutputSlots { get; protected set;}

        public Effect NodeEffect { get; protected set;}
        public string Name {get; private set;}
        public string ExtendedName {get; protected set;}

        public long ID { get; private set;}

        internal FrameBuffer OutputFramebuffer;
        private List<FrameBuffer.AttachmentUsage> Attachments;

        protected RendererContext PrivateRendererContext;
        protected Messaging.MessageProvider PrivateMessageProvider;

        public bool Rendered{ get; private set;}
        public bool Active {get; set;}

        public bool IsInitialized { get; private set;}

        Dictionary<string, Value> Settings;

        public CompositorNode(string name, RendererContext rc, Messaging.MessageProvider prov)
        {
            Name = name;
            ID = DateTime.Now.Ticks;

            PrivateRendererContext = rc;
            PrivateMessageProvider = prov;

            Init(rc);
        }
         
        private FrameBuffer.AttachmentUsage SlotNumberToAUsage(int number)
        {
            switch (number)
            {
                case 0:
                    return FrameBuffer.AttachmentUsage.Color0;
                case 1:
                    return FrameBuffer.AttachmentUsage.Color1;
                case 2:
                    return FrameBuffer.AttachmentUsage.Color2;
                case 3:
                    return FrameBuffer.AttachmentUsage.Color3;
                case 4:
                    return FrameBuffer.AttachmentUsage.Color4;
                case 5:
                    return FrameBuffer.AttachmentUsage.Color5;
                case 6:
                    return FrameBuffer.AttachmentUsage.Color6;
                case 7:
                    return FrameBuffer.AttachmentUsage.Color7;
                case 8:
                    return FrameBuffer.AttachmentUsage.Color8;
                case 9:
                    return FrameBuffer.AttachmentUsage.Color9;
                case 10:
                    return FrameBuffer.AttachmentUsage.Color10;
                case 11:
                    return FrameBuffer.AttachmentUsage.Color11;
                case 12:
                    return FrameBuffer.AttachmentUsage.Color12;
                case 13:
                    return FrameBuffer.AttachmentUsage.Color13;
                case 14:
                    return FrameBuffer.AttachmentUsage.Color14;
                case 15:
                    return FrameBuffer.AttachmentUsage.Color15;
            }

            return FrameBuffer.AttachmentUsage.Nothing;
        }

        internal void InitFramebuffer()
        {
            if (OutputFramebuffer != null)
            {
                OutputFramebuffer.BeginPrepare();
                foreach (CompositorOutputSlot slot in OutputSlots)
                {
                    OutputFramebuffer.DeleteTexture(SlotNumberToAUsage(slot.SlotNumber));

                    //Why should i delete all resources? Framebuffer is enough
                    //PrivateRendererContext.DeleteGraphicsResourceAsync(slot.SlotTexture);
                }
                OutputFramebuffer.EndPrepare();

                PrivateRendererContext.DeleteGraphicsResourceAsync(OutputFramebuffer);
            }

            if (OutputSlots != null && OutputSlots.Length > 0)
            {
                OutputFramebuffer = PrivateRendererContext.CreateFrameBuffer("CompositorNode_" + Name + "_FrameBuffer_" + DateTime.Now.Ticks);

                Attachments = new List<FrameBuffer.AttachmentUsage>();

                OutputFramebuffer.BeginPrepare();

                foreach (CompositorOutputSlot slot in OutputSlots)
                {
                    if (slot.SlotTexture != null && slot.SlotType != CompositorSlotType.NoTextureOrValueOutput)
                    {
                        OutputFramebuffer.AddTexture(slot.SlotTexture, SlotNumberToAUsage(slot.SlotNumber));
                        Attachments.Add(SlotNumberToAUsage(slot.SlotNumber));
                    }
                }



                OutputFramebuffer.EndPrepare();
            }
        }

        public virtual bool Init(RendererContext rc)
        {
            RCActionCompositorNodeInit init = new RCActionCompositorNodeInit(rc, this);

            //Inits all necessary things
            if (System.Threading.Thread.CurrentThread.ManagedThreadId == PrivateRendererContext.Application.ManagedThreadId)
                init.Action();
            else
                rc.AddRCActionJob(init);

            while (!init.WasCalled)
                System.Threading.Thread.Sleep(1);

            IsInitialized = true;

            return true;
        }

        public virtual void Begin()
        {
            if (IsInitialized)
            {
                if (OutputFramebuffer != null)
                {
                    OutputFramebuffer.UseAttachments(Attachments.ToArray());
                    OutputFramebuffer.Bind(FrameBuffer.FrameBufferTarget.Draw);
                }

                if (NodeEffect != null)
                {
                    NodeEffect.BindPipeline();

                    if (InputSlots != null && InputSlots.Length > 0)
                    {
                        foreach (CompositorInputSlot sl in InputSlots)
                        {
                            if (sl.SlotTexture != null)
                                sl.SlotTexture.Bind(sl.SlotNumber);
                        }
                    }
                }
            }
        }

        public virtual void Draw()
        {
            Sprite spr = new Sprite();
            spr.AbsolutePosition = new FreezingArcher.Math.Vector2(0, 0);
            spr.CustomEffect = false;

            //Just stub texture.... all others need to be binded too
            spr.Init(InputSlots[0].SlotTexture);

            foreach(CompositorInputSlot cis in InputSlots)
            {
                if (cis.SlotTexture != null)
                    cis.SlotTexture.Bind(cis.SlotNumber);
            }

            PrivateRendererContext.DrawSpriteAbsolute(spr);
        }

        public virtual void End()
        {
            if (IsInitialized)
            {
                if(NodeEffect != null)
                    NodeEffect.UnbindPipeline();

                if(OutputFramebuffer != null)
                    OutputFramebuffer.Unbind();
            }

            Rendered = true;
        }

        public virtual void Bypass()
        {
            Begin();
            Sprite spr = new Sprite();
            spr.AbsolutePosition = new FreezingArcher.Math.Vector2(0, 0);
            spr.CustomEffect = false;

            spr.Init(InputSlots[0].SlotTexture);

            PrivateRendererContext.DrawSpriteAbsolute(spr);

            End();

            Rendered = true;
        }

        public void Reset()
        {
            Rendered = false;
        }

        public abstract void ConfigureSlots();
        public abstract void LoadEffect();
        public virtual void InitOtherStuff(){}
    }
}

