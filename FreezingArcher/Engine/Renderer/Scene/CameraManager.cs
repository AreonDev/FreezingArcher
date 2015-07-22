//
//  EmptyClass.cs
//
//  Author:
//       wfailla <>
//
//  Copyright (c) 2015 wfailla
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
using FreezingArcher.DataStructures.Trees;
using System.Collections.Generic;
using System.Collections;
using FreezingArcher.Core;
using System.Linq;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using System.IO;

namespace FreezingArcher.Renderer.Scene
{
    /// <summary>
    /// Camera manager.
    /// </summary>
    public class CameraManager : IMessageConsumer
    {
        /// <summary>
        /// The cam tree.
        /// </summary>
        internal Tree<Pair<string, BaseCamera>> CamTree;
        internal Tree<Pair<string, BaseCamera>> ActiveCameraNode { get; set;}
        internal List<string> TouchedCameras;

        public BaseCamera ActiveCamera
        {
            get
            {
                return ActiveCameraNode.Data.B;
            }
        }
            

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Renderer.Scene.CameraManager"/> class.
        /// </summary>
        /// <param name="messageProvider">The message provider for this camera manager instance.</param>
        /// <param name="name">Name.</param>
        /// <param name="cam">Cam.</param>
        public CameraManager (MessageProvider messageProvider, string name = "firstCam", BaseCamera cam = null)
        {
            CamTree = new Tree<Pair<string, BaseCamera>> (new Pair<string, BaseCamera>("root", null));
            ActiveCameraNode = new Tree<Pair<string, BaseCamera>> (new Pair<string, BaseCamera>(name, cam));
            ValidMessages = new int[] { (int)MessageId.Input };
            messageProvider += this;
        }

        /// <summary>
        /// Sets the cam.
        /// </summary>
        /// <param name="camName">Cam name.</param>
        /// <param name="cam">Cam.</param>
        public void SetCam(string camName, BaseCamera cam){
            var test = ((IEnumerable<Tree<Pair<string, BaseCamera>>>) CamTree.LevelOrder).First(i => i.Data.A == camName).Data;
            ((IEnumerable<Tree<Pair<string, BaseCamera>>>) CamTree.LevelOrder).First(i => i.Data.A == camName).Data.B = cam;
        }

        /// <summary>
        /// Adds the group.
        /// </summary>
        /// <param name="name">Name.</param>
        public void AddGroup(string name){
            CamTree.AddChild (new Pair<string, BaseCamera>(name, null));
        }

        /// <summary>
        /// Adds the cam.
        /// </summary>
        /// <param name="cam">Cam.</param>
        /// <param name="groupID">Group I.</param>
        public void AddCamera(BaseCamera cam, string name, string groupID="root")
        {
            ((IEnumerable<Tree<Pair<string, BaseCamera>>>) CamTree.LevelOrder).First(i => i.Data.A == groupID)
                .AddChild (new Pair<string, BaseCamera>(name, cam));
        }

        /// <summary>
        /// Gets the cam.
        /// </summary>
        /// <returns>The cam.</returns>
        /// <param name="camName">Cam name.</param>
        public BaseCamera GetCam(string camName){
            return ((IEnumerable<Tree<Pair<string, BaseCamera>>>) CamTree.LevelOrder).First(i => i.Data.A == camName).Data.B;
        }

        /// <summary>
        /// Toggles the camera.
        /// </summary>
        /// <returns>The camera.</returns>
        public void ToggleCamera(){
            if (CamTree.Children.Count <= 0)
                return;
            
            if(TouchedCameras == null) TouchedCameras = new List<string>();

            foreach(var node in (IEnumerable<Tree<Pair<string, BaseCamera>>>) CamTree.DepthFirst)
                {
                if (TouchedCameras.Count != 0 && TouchedCameras.Contains(node.Data.A))
                    continue;
                if(null != node.Data.B){
                    TouchedCameras.Add(node.Data.A);
                    ActiveCameraNode = node;
                    return;
                }
            }
            TouchedCameras.Clear();
            ToggleCamera();
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage (Messaging.Interfaces.IMessage msg)
        {
            InputMessage im = msg as InputMessage;
            if (im != null)
            {
                if (im.IsActionPressedAndRepeated("camera"))
                {
                    ToggleCamera();
                }
            }
        }
    }
}

