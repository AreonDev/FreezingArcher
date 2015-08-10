//
//  Scene.cs
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using FreezingArcher.Renderer;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Renderer.Scene
{
    public class CoreScene : IMessageConsumer
    {
        private class RCActionChangeFramebufferSize : RendererCore.RCAction
        {
            public CoreScene Scene;
            public RendererContext Renderer;

            public int Width;
            public int Height;

            public RCActionChangeFramebufferSize(RendererContext rc, CoreScene cs, int w, int h)
            {
                Scene = cs;
                Renderer = rc;

                Width = w;
                Height = h;
            }

            public RendererCore.RCActionDelegate Action
            {
                get
                {
                    return delegate()
                    {
                        Scene.ResizeTextures(Width, Height);
                    };
                }
            }
        }

        public class RCActionInitSceneObject : RendererCore.RCAction
        {
            public SceneObject Object;
            public RendererContext Context;

            public RCActionInitSceneObject(SceneObject toinit, RendererContext ctx) 
            {
                Object = toinit;
                Context = ctx;
            }

            public RendererCore.RCActionDelegate Action
            {
                get
                {
                    return delegate()
                    {
                        if(!Object.IsInitialized)
                            Object.Init(Context);
                    };
                }
            }
        }

        private class RCActionInitCoreScene : RendererCore.RCAction
        {
            public CoreScene Scene;
            public RendererContext Context;

            public RCActionInitCoreScene(CoreScene scene, RendererContext rc)
            {
                Scene = scene;
                Context = rc;
            }

            public RendererCore.RCActionDelegate Action
            {
                get
                {
                    return delegate()
                    {
                        if (!Scene.IsInitialized)
                            Scene.InitFromJob(Context);
                    };
                }
            }
        }

        private List<SceneObject> Objects;
        private List<CoreScene> SubScenes;

        private RendererContext PrivateRendererContext;
        public bool IsInitialized { get; private set;}

        public bool Active { get; set;}

        public void AddObject(SceneObject obj)
        {
            if(obj == null)
            {
                Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", FreezingArcher.Core.Status.BadArgument,
                    "SceneObject = null");
                return;
            }

            if (IsInitialized)
            {
                if (PrivateRendererContext.Application.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
                {
                    if (!obj.IsInitialized)
                    {
                        if (obj.Init(PrivateRendererContext))
                            Objects.Add(obj);
                        else
                            Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", 
                                FreezingArcher.Core.Status.AKittenDies, "Object could not be initialized!");
                    }
                }
                else
                {
                    if (!obj.IsInitialized)
                    {
                        PrivateRendererContext.AddRCActionJob(new RCActionInitSceneObject(obj, PrivateRendererContext));
                        obj.WaitTillInitialized();
                    }
                    Objects.Add(obj);
                }

                if (obj.Priority == -1)
                    obj.Priority = Objects.Count;
            }
            else
                Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", FreezingArcher.Core.Status.AKittenDies, "Scene is not initialized!"); 

        }

        public void RemoveObject(SceneObject obj)
        {
            if (!Objects.Remove(obj))
                Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", "Could not remove Object");
        }

        public List<string> GetObjectNames()
        {
            List<string> list = new List<string>();

            foreach (SceneObject obj in Objects)
            {
                bool name_in_new_list = false;

                foreach (string name in list)
                    if (name == obj.GetName())
                        name_in_new_list = true;

                if (!name_in_new_list)
                    list.Add(obj.GetName());
            }

            return list;
        }

        public List<SceneObject> GetObjects()
        {
            List<SceneObject> list = new List<SceneObject>(Objects.Count);

            foreach (SceneObject obj in Objects)
                list.Add(obj);

            return list;
        }

        public int GetCountOfObjectsWithName(string name)
        {
            int count = 0;

            foreach (SceneObject obj in Objects)
            {
                if (obj.GetName() == name)
                    count++;
            }

            return count;
        }

        public List<SceneObject> GetObjectsWithName(string name)
        {
            List<SceneObject> scnobj = new List<SceneObject>();

            foreach (SceneObject obj in Objects)
            {
                if (obj.GetName() == name)
                    scnobj.Add(obj);
            }

            return scnobj;
        }

        public IOrderedEnumerable<SceneObject> GetObjectsSorted()
        {
           return Objects.OrderBy(o => o.Priority);
        }

        public List<Light> Lights { get; set;}
        public Color4 BackgroundColor{ get; set;}
        public string SceneName{ get; set;}
        public CameraManager CameraManager{ get; set;}


        public FrameBuffer FrameBuffer{ get; private set;}
        public Texture2D   FrameBufferNormalTexture { get; private set;}
        public Texture2D   FrameBufferColorTexture{ get; private set;}
        public Texture2D   FrameBufferSpecularTexture { get; private set;}
        public Texture2D   FrameBufferDepthTexture { get; private set;}
        public TextureDepthStencil FrameBufferDepthStencilTexture { get; private set;}

        public CoreScene(RendererContext rc, MessageProvider messageProvider)
        {
            CameraManager = new CameraManager(messageProvider);
            Objects = new List<SceneObject>();
            Lights = new List<Light>();
            SubScenes = new List<CoreScene>();



            FrameBuffer = null;

            SceneName = "CoreScene";

            Active = true;

            ValidMessages = new [] { (int)MessageId.WindowResize };
            messageProvider += this;

            Init(rc);
        }

        public void ResizeTextures(int width, int height)
        {
            if (PrivateRendererContext.Application.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                FrameBufferNormalTexture.Resize(width, height);
                FrameBufferColorTexture.Resize(width, height);
                FrameBufferDepthTexture.Resize(width, height);
                FrameBufferSpecularTexture.Resize(width, height);
            

                FrameBufferDepthStencilTexture.Resize(width, height);
            }
            else
            {
                RCActionChangeFramebufferSize rcacfbs = new RCActionChangeFramebufferSize(PrivateRendererContext, this, width, height);
                PrivateRendererContext.AddRCActionJob(rcacfbs);
            }
        }

        public void Update()
        {
            //Init objects, which are not initialized yet
            /*if (ObjectsToInit.Count > 1)
            {
                foreach (SceneObject obj in ObjectsToInit)
                {
                    if (obj.Init(PrivateRendererContext))
                        Objects.Add(obj);
                    else
                        Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", 
                            FreezingArcher.Core.Status.AKittenDies, "Object could not be initialized!");
                }

                ObjectsToInit.Clear();
            }*/

            //IOrderedEnumerable<SceneObject> sorted = Objects.OrderBy(o => o.Priority);
        }

        internal bool InitFromJob(RendererContext rc)
        {
            long ticks = DateTime.Now.Ticks;

            FrameBuffer = rc.CreateFrameBuffer("CoreSceneFrameBuffer_" + ticks);

            FrameBufferNormalTexture = rc.CreateTexture2D("CoreSceneFrameBufferNormalTexture_"+ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false, true);

            FrameBufferColorTexture = rc.CreateTexture2D("CoreSceneFrameBufferColorTexture_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false, true);

            FrameBufferSpecularTexture = rc.CreateTexture2D("CoreSceneFrameBufferSpecularTexture_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false, true);

            FrameBufferDepthTexture = rc.CreateTexture2D("CoreSceneFrameBufferDepthTexture_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false, true);

            FrameBufferDepthStencilTexture = rc.CreateTextureDepthStencil("CoreSceneFrameBufferDepthStencil_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, IntPtr.Zero, false);

            //FrameBufferRenderedImamge = rc.CreateTexture2D("CoreSceneFrameBufferRendererImage_" + , rc.ViewportSize.Y, true,
            //    IntPtr.Zero, );

            FrameBuffer.BeginPrepare();

            FrameBuffer.AddTexture(FrameBufferNormalTexture, FrameBuffer.AttachmentUsage.Color0);
            FrameBuffer.AddTexture(FrameBufferColorTexture, FrameBuffer.AttachmentUsage.Color1);
            FrameBuffer.AddTexture(FrameBufferSpecularTexture, FrameBuffer.AttachmentUsage.Color2);
            FrameBuffer.AddTexture(FrameBufferDepthTexture, FrameBuffer.AttachmentUsage.Color3);

            FrameBuffer.AddTexture(FrameBufferDepthStencilTexture, FrameBuffer.AttachmentUsage.DepthStencil);

            FrameBuffer.EndPrepare();

            IsInitialized = true;

            return true;
        }

        internal bool Init(RendererContext rc)
        {
            PrivateRendererContext = rc;

            if (PrivateRendererContext.Application.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                InitFromJob(rc);
            }
            else
            {
                rc.AddRCActionJob(new RCActionInitCoreScene(this, rc));
            }

            while (!IsInitialized) System.Threading.Thread.Sleep(1);

            return true;
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage(IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.WindowResize)
            {
                WindowResizeMessage wrm = msg as WindowResizeMessage;

                ResizeTextures(wrm.Width, wrm.Height);
            }
        }

        public int[] ValidMessages
        {
            get;
            private set;
        }

        #endregion
    }
}

