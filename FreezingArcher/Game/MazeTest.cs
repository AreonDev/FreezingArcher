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
    public class MazeTest : IMessageConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.MazeTest"/> class.
        /// </summary>
        /// <param name="msgmnr">Msgmnr.</param>
        /// <param name="objmnr">Objmnr.</param>
        /// <param name="scene">Scene.</param>
        public MazeTest (MessageManager msgmnr, ObjectManager objmnr, CoreScene scene)
        {
            ValidMessages = new int[] { (int) MessageId.Input };
            msgmnr += this;
            mazeGenerator = new MazeGenerator (objmnr);
            this.scene = scene;
            int seed = new Random().Next();
            var rand = new Random(seed);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MazeTest", "Seed: {0}", seed);
            maze[0] = mazeGenerator.CreateMaze(rand.Next(), MazeColorTheme.Overworld);
            maze[1] = mazeGenerator.CreateMaze(rand.Next(), MazeColorTheme.Underworld);

            //rect = new RectangleSceneObject();
            //rect.Color = Color4.CadetBlue;
            //rect.Position = new Vector3(4, 4, 0);
            //rect.Scaling = new Vector3(50, 50, 1);
            //scene.AddObject(rect);
        }

        readonly MazeGenerator mazeGenerator;

        readonly Maze.Maze[] maze = new Maze.Maze[2];

        //RectangleSceneObject rect;

        CoreScene scene;

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
                if (im.IsActionPressed("jump"))
                {
                    if (!maze[0].IsGenerated)
                        maze[0].Generate(scene);
                    else if (!maze[1].IsGenerated)
                        maze[1].Generate();
                }
                if (im.IsActionPressed("run"))
                {
                    if (maze[0].IsGenerated && !maze[0].IsExitPathCalculated)
                        maze[0].CalculatePathToExit();
                    else if (maze[1].IsGenerated && !maze[1].IsExitPathCalculated)
                        maze[1].CalculatePathToExit();
                }
                if (im.IsActionPressed("sneek"))
                {
                    if (maze[0].IsGenerated && !maze[0].AreFeaturesPlaced)
                        maze[0].SpawnFeatures(null, maze[1].graph);
                    else if (maze[1].IsGenerated && !maze[1].AreFeaturesPlaced)
                        maze[1].SpawnFeatures(maze[0].graph);
                }
                /*var pos = rect.Position;
                const int fac = 10;
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
                if (im.IsActionPressed("frame"))
                {
                    Logger.Log.AddLogEntry(LogLevel.Debug, "Maze", "FPS: {0}", Application.Instance.FPSCounter);
                }
                rect.Position = pos;*/
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
