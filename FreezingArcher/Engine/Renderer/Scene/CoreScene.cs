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
using System.Collections.Generic;

using FreezingArcher.Renderer;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene.SceneObjects;

namespace FreezingArcher.Renderer.Scene
{
    public class CoreScene
    {
        private List<SceneObject> Objects;

        object ListLock = new object();

        private List<CoreScene> SubScenes;

        private RendererContext PrivateRendererContext;

        public void AddObject(SceneObject obj)
        {
            if (PrivateRendererContext != null)
            {
                if (obj.Init(PrivateRendererContext))
                    Objects.Add(obj);
                else
                    Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", FreezingArcher.Core.Status.ClimateChangeDrivenCatastrophicWeatherEvent);
            }
            else
                Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", FreezingArcher.Core.Status.AKittenDies, "Scene is not initialized!"); 
        }

        public void RemoveObject(SceneObject obj)
        {
            Objects.Remove(obj);
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

            lock (ListLock)
            {
                Objects.ForEach((item) =>
                    {
                        list.Add(item);
                    });
            }

            return list;
        }

        public int GetCountOfObjectsWithName(string name)
        {
            int count = 0;

            lock (ListLock)
            {
                Objects.ForEach((item) =>
                    {
                        if (item.GetName() == name)
                            count++;
                    });
            }

            return count;
        }

        public List<SceneObject> GetObjectsWithName(string name)
        {
            List<SceneObject> scnobj = new List<SceneObject>();

            Objects.ForEach((item) =>
                {
                    if(item.GetName() == name)
                        scnobj.Add(item);
                });

            return scnobj;
        }

        public Color4 BackgroundColor{ get; set;}
        public string SceneName{ get; set;}
        public CameraManager CamManager{ get; set;}


        public FrameBuffer FrameBuffer{ get; private set;}
        public Texture2D   FrameBufferNormalTexture { get; private set;}
        public Texture2D   FrameBufferColorTexture{ get; private set;}
        public Texture2D   FrameBufferSpecularTexture { get; private set;}
        public Texture2D   FrameBufferDepthTexture { get; private set;}
        public TextureDepthStencil FrameBufferDepthStencilTexture { get; private set;}

        public CoreScene()
        {
            CamManager = new CameraManager();
            Objects = new List<SceneObject>();
            SubScenes = new List<CoreScene>();

            FrameBuffer = null;

            SceneName = "CoreScene";
        }

        public void ResizeTextures(int width, int height)
        {
            FrameBufferNormalTexture.Resize(width, height);
            FrameBufferColorTexture.Resize(width, height);
            FrameBufferDepthTexture.Resize(width, height);
            FrameBufferSpecularTexture.Resize(width, height);

            FrameBufferDepthStencilTexture.Resize(width, height);
        }

        internal bool Init(RendererContext rc)
        {
            PrivateRendererContext = rc;

            //Init Framebuffer
            long ticks = DateTime.Now.Ticks;

            FrameBuffer = rc.CreateFrameBuffer("CoreSceneFrameBuffer_" + ticks);

            FrameBufferNormalTexture = rc.CreateTexture2D("CoreSceneFrameBufferNormalTexture_"+ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false);

            FrameBufferColorTexture = rc.CreateTexture2D("CoreSceneFrameBufferColorTexture_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false);

            FrameBufferSpecularTexture = rc.CreateTexture2D("CoreSceneFrameBufferSpecularTexture_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false);

            FrameBufferDepthTexture = rc.CreateTexture2D("CoreSceneFrameBufferDepthTexture_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, false, IntPtr.Zero, false);

            FrameBufferDepthStencilTexture = rc.CreateTextureDepthStencil("CoreSceneFrameBufferDepthStencil_" + ticks,
                rc.ViewportSize.X, rc.ViewportSize.Y, IntPtr.Zero, false);

            FrameBuffer.BeginPrepare();

            FrameBuffer.AddTexture(FrameBufferNormalTexture, FrameBuffer.AttachmentUsage.Color0);
            FrameBuffer.AddTexture(FrameBufferColorTexture, FrameBuffer.AttachmentUsage.Color1);
            FrameBuffer.AddTexture(FrameBufferSpecularTexture, FrameBuffer.AttachmentUsage.Color2);
            FrameBuffer.AddTexture(FrameBufferDepthTexture, FrameBuffer.AttachmentUsage.Color3);

            FrameBuffer.AddTexture(FrameBufferDepthStencilTexture, FrameBuffer.AttachmentUsage.DepthStencil);

            FrameBuffer.EndPrepare();

            return true;
        }
    }
}

