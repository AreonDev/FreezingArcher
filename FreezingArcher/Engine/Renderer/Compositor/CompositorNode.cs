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
    public enum CompositorSlotType
    {
        Value,
        ValueTexture,
        Texture
    }

    public abstract class CompositorNode
    {
        public CompositorInputSlot[] InputSlots { get; protected set;}
        public CompositorOutputSlot[] OutputSlots { get; protected set;}
        public Effect NodeEffect { get; private set;}
        public string Name {get; private set;}

        protected FrameBuffer OutputFramebuffer;
        protected RendererContext PrivateRendererContext;

        public bool Active {get; set;}

        public bool IsInitialized { get; private set;}

        Dictionary<string, Value> Settings;

        public CompositorNode(string name)
        {
            Name = name;
        }
            
        public virtual bool Init(RendererContext rc)
        {
            PrivateRendererContext = rc;

            IsInitialized = false;

            return true;
        }

        public virtual void Begin(RendererContext rc)
        {
            if (!IsInitialized)
            {
                OutputFramebuffer.Bind(FrameBuffer.FrameBufferTarget.Draw);

                NodeEffect.BindPipeline();
            }
        }

        public virtual void Draw(RendererContext rc)
        {
            Sprite spr = new Sprite();


        }

        public virtual void End(RendererContext rc)
        {
            if (!IsInitialized)
            {
                NodeEffect.UnbindPipeline();

                OutputFramebuffer.Unbind();
            }
        }
    }
}

