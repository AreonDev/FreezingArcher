//
//  EmptyClass.cs
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
using FreezingArcher.Messaging;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Math;

namespace FreezingArcher.Renderer.Compositor
{
    public class CompositorNodeScene : CompositorNode
    {
        private class RCActionCompositorNodeSceneUpdateScene : RendererCore.RCAction
        {
            CompositorNodeScene Node;
            RendererContext Renderer;

            public bool Ready {get; private set;}

            public RCActionCompositorNodeSceneUpdateScene(CompositorNodeScene node, RendererContext rend)
            {
                Node = node;
                Renderer = rend;

                Ready = false;
            }

            public RendererCore.RCActionDelegate Action
            {
                get
                {
                    return delegate()
                    {
                        Node.ConfigureSlots();
                        Node.InitFramebuffer();

                        Node.OutputFramebuffer.AddTexture(Node.Scene.FrameBufferDepthStencilTexture, FrameBuffer.AttachmentUsage.DepthStencil);

                        Ready = true;
                    };
                }
            }
        }

        private CoreScene PrivateScene;
        public CoreScene Scene 
        {   
            get 
            {
                return PrivateScene;
            }

            set
            {
                PrivateScene = value;

                RCActionCompositorNodeSceneUpdateScene rcacnsus = new RCActionCompositorNodeSceneUpdateScene(this, PrivateRendererContext);

                if (PrivateRendererContext.Application.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
                    rcacnsus.Action();
                else
                {
                    PrivateRendererContext.AddRCActionJob(rcacnsus);

                    while (!rcacnsus.Ready)
                        System.Threading.Thread.Sleep(1);
                }
            }
        }

        public CompositorNodeScene(RendererContext rc, MessageProvider prov) : base("NodeStart", rc, prov)
        {
            
        }

        public override void Begin()
        {
            base.Begin();
        }

        public override void Draw()
        {
            PrivateRendererContext.Clear(Color4.Black, 0);
            PrivateRendererContext.Clear(Color4.Black, 1);
            PrivateRendererContext.Clear(Color4.Black, 2);
            PrivateRendererContext.Clear(Color4.Black, 3);

            PrivateRendererContext.Clear(Color4.Black);

            PrivateRendererContext.Scene = Scene;
            PrivateRendererContext.DrawScene();

            OutputSlots[4].Value = Scene.Lights;
            OutputSlots[5].Value = Scene.CameraManager.ActiveCamera;
        }

        public override void End()
        {
            base.End();
        }

        public override void InitOtherStuff()
        {    
            PrivateScene = new CoreScene(PrivateRendererContext, PrivateMessageProvider);
            PrivateScene.Init(PrivateRendererContext);
            PrivateScene.BackgroundColor = new Math.Color4(1.0f, 0.0f, 0.0f, 0.3f);
            ExtendedName = "NodeScene";
        }

        #region implemented abstract members of CompositorNode

        public override void ConfigureSlots()
        {
            Active = true;

            InputSlots = null;
            OutputSlots = new CompositorOutputSlot[6];
           
            OutputSlots[0] = new CompositorOutputSlot("DiffuseColor", 0, Scene.FrameBufferColorTexture, CompositorSlotType.Texture);
            OutputSlots[1] = new CompositorOutputSlot("PositionColor", 1, Scene.FrameBufferDepthTexture, CompositorSlotType.Texture);
            OutputSlots[2] = new CompositorOutputSlot("NormalColor", 2, Scene.FrameBufferNormalTexture, CompositorSlotType.Texture);
            OutputSlots[3] = new CompositorOutputSlot("SpecularColor", 3, Scene.FrameBufferSpecularTexture, CompositorSlotType.Texture);
            OutputSlots[4] = new CompositorOutputSlot("LightInformation", 4, null);
            OutputSlots[5] = new CompositorOutputSlot("CameraInformation", 5, null);
        }

        public override void LoadEffect()
        {
            NodeEffect = null;
        }

        #endregion
    }
}

