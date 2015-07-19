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
                    Node.ConfigureSlots();
                    Node.LoadEffect();
                  
                    Node.InitFramebuffer(RendererContext);

                    Node.InitOtherStuff();

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

        public int ID { get; private set;}

        private FrameBuffer OutputFramebuffer;
        private List<FrameBuffer.AttachmentUsage> Attachments;

        protected RendererContext PrivateRendererContext;

        public bool Active {get; set;}

        public bool IsInitialized { get; private set;}

        Dictionary<string, Value> Settings;

        public CompositorNode(string name)
        {
            Name = name;
            ID = (int)DateTime.Now.Ticks;
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

        internal void InitFramebuffer(RendererContext rc)
        {
            if (OutputSlots != null && OutputSlots.Length > 0)
            {
                OutputFramebuffer = rc.CreateFrameBuffer("CompositorNode_" + Name + "_FrameBuffer_" + DateTime.Now.Ticks);

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
            PrivateRendererContext = rc;

            RCActionCompositorNodeInit init = new RCActionCompositorNodeInit(rc, this);

            //Inits all necessary things
            if (System.Threading.Thread.CurrentThread.ManagedThreadId == rc.Application.ManagedThreadId)
                init.Action();
            else
                rc.AddRCActionJob(init);

            while (!init.WasCalled)
                System.Threading.Thread.Sleep(1);

            return true;
        }

        public virtual void Begin(RendererContext rc)
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

        public virtual void Draw(RendererContext rc)
        {
            
        }

        public virtual void End(RendererContext rc)
        {
            if (IsInitialized)
            {
                if(NodeEffect != null)
                    NodeEffect.UnbindPipeline();

                if(OutputFramebuffer != null)
                    OutputFramebuffer.Unbind();
            }
        }

        public abstract void ConfigureSlots();
        public abstract void LoadEffect();
        public virtual void InitOtherStuff(){}
    }
}

