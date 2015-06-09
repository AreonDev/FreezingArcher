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

namespace FreezingArcher.Renderer.Scene
{
    public class CameraManager
    {
        internal Tree<Pair<string, BaseCam>> CamTree;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.CameraManager"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="cam">Cam.</param>
        public CameraManager (string name = "root", BaseCam cam = null)
        {
            CamTree = new Tree<Pair<string, BaseCam>> (new Pair<string, BaseCam>(name, cam));
            CamTree.AddChild(new Pair<string, BaseCam>("ActiveCam", null));
        }

        /// <summary>
        /// Sets the active cam.
        /// </summary>
        /// <param name="cam">Cam.</param>
        public void setActiveCam(BaseCam cam){
            this.setCam("ActiveCam", cam);            
        }

        /// <summary>
        /// Gets the active cam.
        /// </summary>
        /// <param name="cam">Cam.</param>
        public BaseCam getActiveCam(){
            return this.getCam("ActiveCam");            
        }

        /// <summary>
        /// Sets the cam.
        /// </summary>
        /// <param name="camName">Cam name.</param>
        /// <param name="cam">Cam.</param>
        public void setCam(string camName, BaseCam cam){
            ((IEnumerable<Tree<Pair<string, BaseCam>>>) CamTree.LevelOrder).First(i => i.Data.A == camName).Data.B = cam;
        }

        /// <summary>
        /// Adds the group.
        /// </summary>
        /// <param name="name">Name.</param>
        public void AddGroup(string name){
            CamTree.AddChild (new Pair<string, BaseCam>(name, null));
        }

        /// <summary>
        /// Adds the cam.
        /// </summary>
        /// <param name="cam">Cam.</param>
        /// <param name="groupID">Group I.</param>
        public void AddCam(BaseCam cam, string groupID="root")
        {
            ((IEnumerable<Tree<Pair<string, BaseCam>>>) CamTree.LevelOrder).First(i => i.Data.A == groupID)
                .AddChild (new Pair<string, BaseCam>(cam.Name, cam));
        }

        /// <summary>
        /// Gets the cam.
        /// </summary>
        /// <returns>The cam.</returns>
        /// <param name="camName">Cam name.</param>
        public BaseCam getCam(string camName){
            return ((IEnumerable<Tree<Pair<string, BaseCam>>>) CamTree.LevelOrder).First(i => i.Data.A == camName).Data.B;
        }

        /// <summary>
        /// Toggles the cam.
        /// </summary>
        /// <param name="cam">Cam.</param>
        public BaseCam ToggleCam(){
            Tree<Pair<string, BaseCam>> TmpNode = ((IEnumerable<Tree<Pair<string, BaseCam>>>) CamTree.LevelOrder)
                .First(i => i.Data.A == getActiveCam().Name);

//            if (TmpNode.Parent.Parent == null)
//                return 1;
            var TmpGroup = TmpNode.Parent;

//            var Level = TmpGroup.Level+1;
//
//            var GroupList = from x in ((IEnumerable<Tree<Pair<string, BaseCam>>>)TmpGroup.LevelOrder)
//                                       where (x.Level == Level) && (x.Data.B == null)
//                                       select x;

            foreach(var node in (IEnumerable<Tree<Pair<string, BaseCam>>>) TmpGroup.DepthFirst)
                {
                if(node != TmpNode){
                    setActiveCam(node.Data.B);
                    return node.Data.B;
                }
            }
            return TmpNode.Data.B;
        }
    }
}

