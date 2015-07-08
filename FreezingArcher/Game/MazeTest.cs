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
using FreezingArcher.Output;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Content;
using FreezingArcher.Renderer;

namespace FreezingArcher.Game
{
    /// <summary>
    /// Maze test.
    /// </summary>
    public class MazeTest : IMessageConsumer
    {
        double f = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.MazeTest"/> class.
        /// </summary>
        /// <param name="messageProvider">The message provider for this instance.</param>
        /// <param name="objmnr">The object manager for this instance.</param>
        /// <param name="rendererContext">The renderer context for the maze scenes.</param>
        /// <param name="game">The game the maze should be generated in.</param>
        public MazeTest (MessageProvider messageProvider, ObjectManager objmnr, RendererContext rendererContext,
            Content.Game game)
        {
            ValidMessages = new[] { (int) MessageId.Input };
            messageProvider += this;
            mazeGenerator = new MazeGenerator (objmnr);
            this.game = game;

            game.AddGameState("maze_overworld", Content.Environment.Default, null);
            var state = game.GetGameState("maze_overworld");
            state.Scene = new CoreScene(rendererContext, state.MessageProxy);
            state.Scene.BackgroundColor = Color4.Crimson;

            player = EntityFactory.Instance.CreateWith ("player", state.MessageProxy, systems: new[] {
                typeof (MovementSystem),
                typeof (KeyboardControllerSystem),
                typeof (MouseControllerSystem),
                typeof (SkyboxSystem)
            });

            // embed new maze into game state logic and create a MoveEntityToScene
            SkyboxSystem.CreateSkybox(state.Scene, player);
            player.GetComponent<TransformComponent>().Position = new Vector3(0, 1.85f, 0);
            state.Scene.CameraManager.AddCam (new BaseCamera (player, state.MessageProxy), "player");

            int seed = new Random().Next();
            var rand = new Random(seed);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MazeTest", "Seed: {0}", seed);
            maze[0] = mazeGenerator.CreateMaze(rand.Next(), state.MessageProxy, state.PhysicsManager, player);

            game.AddGameState("maze_underworld", Content.Environment.Default, null);
            state = game.GetGameState("maze_underworld");
            state.Scene = new CoreScene(rendererContext, state.MessageProxy);
            state.Scene.BackgroundColor = Color4.AliceBlue;
            maze[1] = mazeGenerator.CreateMaze(rand.Next(), state.MessageProxy, state.PhysicsManager, player);

            game.SwitchToGameState("maze_overworld");
        }

        readonly MazeGenerator mazeGenerator;

        readonly Maze.Maze[] maze = new Maze.Maze[2];

        readonly Entity player;

        readonly Content.Game game;

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
                        maze[0].Generate(game.CurrentGameState);
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
