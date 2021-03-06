﻿//
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

using FreezingArcher.Renderer.Compositor;
using FreezingArcher.Renderer;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;

namespace FreezingArcher.Renderer.Scene
{
    public class CoreScene
    {
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

        public float DistanceFogIntensity{ get; set;}
        public FreezingArcher.Math.Color4 DistanceFogColor { get; set;}

        public float MaxRenderingDistance { get; set;}

        private List<SceneObject> Objects;
        public List<CoreScene> SubScenes;

        public event Action PostSceneRendering;

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
                if (obj.IsAddedToScene)
                {
                    Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", FreezingArcher.Core.Status.BadArgument,
                        "Object was already added to Scene!");
                    
                    return;
                }

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

                obj.Scene = this;
                obj.IsAddedToScene = true;
            }
            else
                Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", FreezingArcher.Core.Status.AKittenDies, "Scene is not initialized!"); 

        }

        public void RemoveObject(SceneObject obj)
        {
            if (!Objects.Remove(obj))
            {
                Output.Logger.Log.AddLogEntry(FreezingArcher.Output.LogLevel.Error, "CoreScene", "Could not remove Object");
                return;
            }

            obj.Scene = null;
            obj.IsAddedToScene = false;
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

        public void AddLight(Light light)
        {
            Lights.Add (light);
            light.Scene = this;
        }

        public void RemoveLight(Light light)
        {
            light.Scene = null;
            Lights.Remove (light);
        }

        public Color4 AmbientColor { get; set;}
        public float  AmbientIntensity { get; set;}

        public List<Light> Lights { get; private set;}
        public Color4 BackgroundColor{ get; set;}
        public string SceneName{ get; set;}
        public CameraManager CameraManager{ get; set;}

        public CoreScene(RendererContext rc, MessageProvider messageProvider)
        {
            CameraManager = new CameraManager(messageProvider);
            Objects = new List<SceneObject>();
            Lights = new List<Light>();
            SubScenes = new List<CoreScene>();

            SceneName = "CoreScene";

            Active = true;

            PostSceneRendering = null;

            DistanceFogIntensity = 0.08f;
            DistanceFogColor = FreezingArcher.Math.Color4.Black;

            MaxRenderingDistance = 1000.0f;

            ValidMessages = new int[0];
            //messageProvider += this;

            Init(rc);
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

        public void DoPostSceneRendering()
        {
            if (PostSceneRendering != null)
                PostSceneRendering ();
        }

        #region IMessageConsumer implementation

        public int[] ValidMessages
        {
            get;
            private set;
        }

        #endregion
    }
}

