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

            scnobjarr = new SceneObjectArray ("ModelSceneObject_lib/Renderer/TestGraphics/Rabbit/Rabbit.obj");
            scnobjarr.LayoutLocationOffset = 10;
            Application.Instance.RendererContext.Scene.AddObject (scnobjarr);

            //Models
            m300 = new ModelSceneObject[1];
            for (int i = 0; i < m300.Length; i++) 
            {
                if (i == 0) {
                    m300 [i] = new ModelSceneObject ("lib/Renderer/TestGraphics/Rabbit/Rabbit.obj");
                } else
                    m300 [i] = new ModelSceneObject ("lib/Renderer/TestGraphics/Rabbit/Rabbit.obj", false);

                m300 [i].Position = new Vector3 (i*1.0f, i*1.0f, 0.0f);
                m300 [i].Scaling = new Vector3 (1, 1, 1);

                scnobjarr.AddObject (m300 [i]);
            }
        }

        ModelSceneObject[] m300;
        SceneObjectArray scnobjarr;

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
                float fac = 100.0f;

                var pos = m300[0].Position;
                if (im.IsActionDown("right"))
                {
                    pos.X += (float) (im.DeltaTime.TotalMilliseconds / fac);
                }
                if (im.IsActionDown("left"))
                {
                    pos.X -= (float) (im.DeltaTime.TotalMilliseconds / fac);
                }
                if (im.IsActionDown("backward"))
                {
                    pos.Y += (float) (im.DeltaTime.TotalMilliseconds / fac);
                }
                if (im.IsActionDown("forward"))
                {
                    pos.Y -= (float) (im.DeltaTime.TotalMilliseconds / fac);
                }

                m300[0].Position = pos;
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
