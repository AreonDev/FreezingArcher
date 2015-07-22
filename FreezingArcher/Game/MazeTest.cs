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
using FreezingArcher.Renderer.Compositor;
using FreezingArcher.Content;
using FreezingArcher.Renderer;
using Jitter;
using Jitter.LinearMath;
using Jitter.Dynamics;
using Jitter.Collision;
using Jitter.Collision.Shapes;

namespace FreezingArcher.Game
{
    /// <summary>
    /// Maze test.
    /// </summary>
    public class MazeTest : IMessageConsumer, IMessageCreator
    {
        double f = 0;

        LoadingScreen loadingScreen;

        BasicCompositor compositor;

        CompositorNodeScene scenenode;
        CompositorNodeOutput outputnode;
        CompositorNodeDeferredShading deferredshadingnode;
        CompositorBlurNode blur;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.MazeTest"/> class.
        /// </summary>
        /// <param name="messageProvider">The message provider for this instance.</param>
        /// <param name="objmnr">The object manager for this instance.</param>
        /// <param name="rendererContext">The renderer context for the maze scenes.</param>
        /// <param name="game">The game the maze should be generated in.</param>
        public MazeTest (MessageProvider messageProvider, ObjectManager objmnr, RendererContext rendererContext,
            Content.Game game, Application app)
        {
            ValidMessages = new[] { (int) MessageId.Input, (int)MessageId.Update, (int)MessageId.Running };
            messageProvider += this;
            mazeGenerator = new MazeGenerator (objmnr);
            this.game = game;
            application = app;

            scenenode = new CompositorNodeScene (rendererContext, messageProvider);
            outputnode = new CompositorNodeOutput (rendererContext, messageProvider);
            deferredshadingnode = new CompositorNodeDeferredShading (rendererContext, messageProvider);
            blur = new CompositorBlurNode (rendererContext, messageProvider);

            game.SceneNode = scenenode;

            game.AddGameState("maze_overworld", Content.Environment.Default, null);
            var state = game.GetGameState("maze_overworld");
            state.Scene = new CoreScene(rendererContext, state.MessageProxy);
            state.Scene.BackgroundColor = Color4.Crimson;
            state.MessageProxy.StartProcessing();

            loadingScreen = new LoadingScreen(application, application.MessageManager, "loading.png",
                "MazeLoadingScreen",
                to: new[] {new Tuple<string, GameStateTransition>(state.Name, new GameStateTransition(0))});

            game.SwitchToGameState("MazeLoadingScreen");

            compositor = new BasicCompositor (objmnr, rendererContext);

            compositor.AddNode (scenenode);
            compositor.AddNode (outputnode);
            compositor.AddNode (deferredshadingnode);
            compositor.AddNode (blur);


            compositor.AddConnection (scenenode, deferredshadingnode, 0, 0);
            compositor.AddConnection (scenenode, deferredshadingnode, 1, 1);
            compositor.AddConnection (scenenode, deferredshadingnode, 2, 2);
            compositor.AddConnection (scenenode, deferredshadingnode, 3, 3);

            compositor.AddConnection (deferredshadingnode, blur, 0, 0);

            compositor.AddConnection (deferredshadingnode, outputnode, 0, 0);

            rendererContext.Compositor = compositor;

            Player = EntityFactory.Instance.CreateWith ("player", state.MessageProxy, systems: new[] {
                typeof (MovementSystem),
                typeof (KeyboardControllerSystem),
                typeof (MouseControllerSystem),
                typeof (SkyboxSystem),
                typeof (PhysicsSystem)
            });

            // embed new maze into game state logic and create a MoveEntityToScene
            SkyboxSystem.CreateSkybox(state.Scene, Player);
            Player.GetComponent<TransformComponent>().Position = new Vector3(0, 1.85f, 0);
            state.Scene.CameraManager.AddCamera (new BaseCamera (Player, state.MessageProxy), "player");

            RigidBody playerBody = new RigidBody (new SphereShape(1f));
            playerBody.Position = Player.GetComponent<TransformComponent> ().Position.ToJitterVector ();
            playerBody.AllowDeactivation = false;
            playerBody.Material.StaticFriction = 0f;
            playerBody.Material.KineticFriction = 0f;
            playerBody.Material.Restitution = 0.1f;
            //playerBody.Mass = 1000000.0f;
            playerBody.Update ();
          // playerBody.IsActive = false;
            Player.GetComponent<PhysicsComponent>().RigidBody = playerBody;
            Player.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (playerBody);

            int seed = new Random().Next();
            var rand = new Random(seed);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MazeTest", "Seed: {0}", seed);
            maze[0] = mazeGenerator.CreateMaze(rand.Next(), state.MessageProxy, state.PhysicsManager, 10, 10);
            maze[0].PlayerPosition += Player.GetComponent<TransformComponent>().Position;

            game.AddGameState("maze_underworld", Content.Environment.Default,
                new[] { new Tuple<string, GameStateTransition>("maze_overworld", new GameStateTransition(0)) },
                new[] { new Tuple<string, GameStateTransition>("maze_overworld", new GameStateTransition(0)) });
            state = game.GetGameState("maze_underworld");
            state.Scene = new CoreScene(rendererContext, state.MessageProxy);
            state.Scene.BackgroundColor = Color4.AliceBlue;
            state.Scene.CameraManager.AddCamera (new BaseCamera (Player, state.MessageProxy), "player");
            maze[1] = mazeGenerator.CreateMaze(rand.Next(), state.MessageProxy, state.PhysicsManager, 10, 10);
            maze[1].PlayerPosition += Player.GetComponent<TransformComponent>().Position;

            state.MessageProxy.StopProcessing();
            //game.SwitchToGameState("maze_overworld");
        }

        readonly MazeGenerator mazeGenerator;

        readonly Maze.Maze[] maze = new Maze.Maze[2];

        public Entity Player { get; private set; }

        readonly Content.Game game;

        readonly Application application;

        int currentMaze;

        void SwitchMaze()
        {
            if (currentMaze == 0)
            {
                maze[0].PlayerPosition = Player.GetComponent<TransformComponent>().Position;
                game.MoveEntityToGameState(Player, game.GetGameState("maze_overworld"), game.GetGameState("maze_underworld"));
                game.SwitchToGameState("maze_underworld");
                if (MessageCreated != null)
                    MessageCreated (new TransformMessage (Player, maze [1].PlayerPosition, Quaternion.Identity));
                //player.GetComponent<PhysicsComponent>().RigidBody.Position = maze[1].PlayerPosition;
                currentMaze = 1;
            }
            else if (currentMaze == 1)
            {
                maze[1].PlayerPosition = Player.GetComponent<TransformComponent>().Position;
                game.MoveEntityToGameState(Player, game.GetGameState("maze_underworld"), game.GetGameState("maze_overworld"));
                game.SwitchToGameState("maze_overworld");

                if (MessageCreated != null)
                    MessageCreated (new TransformMessage (Player, maze [0].PlayerPosition, Quaternion.Identity));

                //player.GetComponent<TransformComponent>().Position = maze[0].PlayerPosition;
                currentMaze = 0;
            }
        }

        #region IMessageConsumer implementation

        bool finishedLoading = false;

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.Update) 
            {
                var um = msg as UpdateMessage;

                if (maze[0].HasFinished && maze[1].HasFinished && !finishedLoading)
                {
                    finishedLoading = true;
                    if (game.CurrentGameState.Name != "maze_overworld")
                        game.SwitchToGameState("maze_overworld");
                }

                if(game.CurrentGameState == game.GetGameState("maze_overworld") && maze[0].HasFinished)
                    game.CurrentGameState.PhysicsManager.Update(um.TimeStamp);
                
                if(game.CurrentGameState == game.GetGameState("maze_underworld") && maze[1].HasFinished)
                    game.CurrentGameState.PhysicsManager.Update(um.TimeStamp);
            }

            var im = msg as InputMessage;
            if (im != null)
            {
                if (im.IsActionPressedAndRepeated("run"))
                {
                    if (maze[0].IsGenerated && !maze[0].IsExitPathCalculated)
                        maze[0].CalculatePathToExit();
                    else if (maze[1].IsGenerated && !maze[1].IsExitPathCalculated)
                        maze[1].CalculatePathToExit();
                }
                if (im.IsActionPressedAndRepeated("sneek"))
                {
                    if (maze[0].IsGenerated && !maze[0].AreFeaturesPlaced)
                        maze[0].SpawnFeatures(null, maze[1].graph);
                    else if (maze[1].IsGenerated && !maze[1].AreFeaturesPlaced)
                        maze[1].SpawnFeatures(maze[0].graph);
                }
                if (im.IsActionPressedAndRepeated("frame"))
                {
                    SwitchMaze();
                }
            }

            if (msg.MessageId == (int)MessageId.Running)
            {
                Logger.Log.AddLogEntry (LogLevel.Debug, "MazeTest", "Generate Mazes....");

                maze[0].Generate(
                    () => {
                        if (MessageCreated != null)
                            MessageCreated (new TransformMessage (Player, maze [0].PlayerPosition, Quaternion.Identity));
                        var state = game.GetGameState ("maze_underworld");
                        maze[1].Generate (state: state);
                    },
                    state: game.GetGameState("maze_overworld"));
            }
        }

        /// <summary>
        /// Gets the valid messages which can be used in the ConsumeMessage method
        /// </summary>
        /// <value>The valid messages</value>
        public int[] ValidMessages { get; private set; }

        #endregion

        #region IMessageCreator implementation

        public event MessageEvent MessageCreated;

        #endregion
    }
}
