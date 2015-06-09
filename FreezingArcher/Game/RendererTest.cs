//
//  MazeTest.cs
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
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Game.Maze;
using FreezingArcher.Core;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Output;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Math;

namespace FreezingArcher.Game
{
    /// <summary>
    /// Maze test.
    /// </summary>
    public class RendererTest : IMessageConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.MazeTest"/> class.
        /// </summary>
        /// <param name="msgmnr">Msgmnr.</param>
        /// <param name="objmnr">Objmnr.</param>
        /// <param name="scene">Scene.</param>
        public RendererTest (MessageManager msgmnr, ObjectManager objmnr, CoreScene scene)
        {
            ValidMessages = new int[] { (int) MessageId.Input };
            msgmnr += this;

            SceneObjectArrayWalls = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/Wall/wall.xml");
            SceneObjectArrayWalls.LayoutLocationOffset = 10;
            scene.AddObject (SceneObjectArrayWalls);

            SceneObjectArrayGrounds = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/Ground/ground.xml");
            SceneObjectArrayGrounds.LayoutLocationOffset = 10;
            scene.AddObject (SceneObjectArrayGrounds);

            scene.CamManager.AddCam (new FreeCamera ("BLa", msgmnr));
            scene.CamManager.SetActiveCam (Application.Instance.RendererContext.Scene.CamManager.GetCam ("BLa"));

            //Walls
            walls = new ModelSceneObject[2500];
            for (int i = 1; i < walls.Length; i++) 
            {
                walls [i] = new ModelSceneObject ("lib/Renderer/TestGraphics/Wall/wall.xml");
                walls [i].Position = new Vector3 ((i-100)*2.0f, 0f, 0.0f);
                walls [i].Scaling = new Vector3 (1, 1, 1);

                SceneObjectArrayWalls.AddObject (walls [i]);
            }

            //Grounds
            grounds = new ModelSceneObject[2500];
            for (int i = 1; i < grounds.Length; i++) 
            {
                grounds [i] = new ModelSceneObject ("lib/Renderer/TestGraphics/Ground/ground.xml");
                grounds [i].Position = new Vector3 ((i-100)*2.0f, 0f, 2.0f);
                grounds [i].Scaling = new Vector3 (1, 1, 1);

                SceneObjectArrayGrounds.AddObject (grounds [i]);
            }
        }

        ModelSceneObject[] walls;
        ModelSceneObject[] grounds;

        SceneObjectArray SceneObjectArrayWalls;
        SceneObjectArray SceneObjectArrayGrounds;

        #region IMessageConsumer implementation

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage (IMessage msg)
        {
            var im = msg as InputMessage;
            if (im != null)
            {
                //float fac = 100.0f;

                //var pos = m300[0].Position;
                if (im.IsActionDown("right"))
                {
                    //pos.X += (float) (im.DeltaTime.TotalMilliseconds / fac);
                }
                if (im.IsActionDown("left"))
                {
                    //pos.X -= (float) (im.DeltaTime.TotalMilliseconds / fac);
                }
                if (im.IsActionDown("backward"))
                {
                    //pos.Y += (float) (im.DeltaTime.TotalMilliseconds / fac);
                }
                if (im.IsActionDown("forward"))
                {
                    //pos.Y -= (float) (im.DeltaTime.TotalMilliseconds / fac);
                }

                //m300[0].Position = pos;
            }
        }

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; private set; }

        #endregion
    }
}
