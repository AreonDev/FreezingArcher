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
        public CoreScene     UIScene{ get; private set;}

        BasicCompositor compositor;

        CompositorNodeScene scenenode1;
        CompositorNodeScene scenenode2;
        CompositorNodeOutput outputnode;
        CompositorNodeDeferredShading deferredshadingnode;
        CompositorBlurNode blur;
        CompositorNodeTextureAlphaMerger merger;

        ParticleSceneObject particle;
        ParticleSceneObject particle_eye1;
        ParticleSceneObject particle_eye2;
        ParticleSceneObject particle_smoke;

        ScobisParticleEmitter paremitter;

        Light light;

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
            ValidMessages = new[] { (int) MessageId.Input, (int)MessageId.Update, (int)MessageId.Running, (int) MessageId.WindowClose };
            messageProvider += this;
            mazeGenerator = new MazeGenerator (objmnr);
            this.game = game;
            application = app;

            scenenode1 = new CompositorNodeScene (rendererContext, messageProvider);
            scenenode2 = new CompositorNodeScene (rendererContext, messageProvider);
            outputnode = new CompositorNodeOutput (rendererContext, messageProvider);
            deferredshadingnode = new CompositorNodeDeferredShading (rendererContext, messageProvider);
            blur = new CompositorBlurNode (rendererContext, messageProvider);
            merger = new CompositorNodeTextureAlphaMerger (rendererContext, messageProvider);

            game.SceneNode = scenenode1;

            game.AddGameState("maze_overworld", Content.Environment.Default, null);
            var state = game.GetGameState("maze_overworld");
            state.Scene = new CoreScene(rendererContext, messageProvider);
            state.Scene.BackgroundColor = Color4.Crimson;

            light = new Light (LightType.SpotLight);
            light.Color = new Color4 (0.3f, 0.3f, 0.3f, 1.0f);
            light.AmbientIntensity = 0.5f;
            light.AmbientColor = new Color4 (0.2f, 0.2f, 0.2f, 1.0f);
            //light.PointLightConstantAttenuation = 0.8f;
            light.PointLightLinearAttenuation = 0.2f;
            light.SpotLightConeAngle = Math.MathHelper.ToRadians (9.66f);
            //light.PointLightExponentialAttenuation = 0.000600f;

            state.Scene.Lights.Add (light);

            state.MessageProxy.StartProcessing();

            particle_eye1 = new ParticleSceneObject (10);
            particle_eye2 = new ParticleSceneObject (10);
            particle_smoke = new ParticleSceneObject (200);
            particle_eye1.Priority = 999;
            particle_eye2.Priority = 999;
            particle_smoke.Priority = 1000;

            state.Scene.AddObject (particle_eye1);
            state.Scene.AddObject (particle_eye2);
            state.Scene.AddObject (particle_smoke);

            paremitter = new ScobisParticleEmitter (particle_eye1, particle_eye2, particle_smoke);
            paremitter.SpawnPosition = new Vector3 (30.0f, 20.0f, 30.0f);

            particle = new ParticleSceneObject (paremitter.ParticleCount);
            particle.Priority = 998;
            state.Scene.AddObject (particle);

            paremitter.Init (particle, rendererContext);


            UIScene = new CoreScene (rendererContext, messageProvider);
            UIScene.BackgroundColor = Color4.Transparent;

            scenenode2.Scene = UIScene;
            UIScene.Active = false;

            loadingScreen = new LoadingScreen(application, messageProvider, "loading.png",
                "MazeLoadingScreen",
                to: new[] {new Tuple<string, GameStateTransition>(state.Name, new GameStateTransition(0))});

            game.SwitchToGameState("MazeLoadingScreen");

            compositor = new BasicCompositor (objmnr, rendererContext);

            compositor.AddNode (scenenode1);
            compositor.AddNode (scenenode2);
            compositor.AddNode (outputnode);
            compositor.AddNode (deferredshadingnode);
            compositor.AddNode (blur);
            compositor.AddNode (merger);

            compositor.AddConnection (scenenode1, deferredshadingnode, 0, 0);
            compositor.AddConnection (scenenode1, deferredshadingnode, 1, 1);
            compositor.AddConnection (scenenode1, deferredshadingnode, 2, 2);
            compositor.AddConnection (scenenode1, deferredshadingnode, 3, 3);
            compositor.AddConnection (scenenode1, deferredshadingnode, 4, 4);
            compositor.AddConnection (scenenode1, deferredshadingnode, 5, 5);

            compositor.AddConnection (deferredshadingnode, merger, 0, 0);

            compositor.AddConnection (merger, outputnode, 0, 0);
            //compositor.AddConnection (blur, outputnode, 0, 0);

            compositor.AddConnection (scenenode2, merger, 0, 1);

            deferredshadingnode.Active = false;

            rendererContext.Compositor = compositor;

            Player = EntityFactory.Instance.CreateWith ("player", state.MessageProxy, new[] {
                typeof (HealthComponent),
            }, new[] {
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
            Player.GetComponent<PhysicsComponent>().RigidBody = playerBody;
            Player.GetComponent<PhysicsComponent>().World = state.PhysicsManager.World;
            Player.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (playerBody);

            int seed = new Random().Next();
            var rand = new Random(seed);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MazeTest", "Seed: {0}", seed);
            maze[0] = mazeGenerator.CreateMaze(rand.Next(), state.MessageProxy, state.PhysicsManager, 30, 30);
            maze[0].PlayerPosition += Player.GetComponent<TransformComponent>().Position;

            game.AddGameState("maze_underworld", Content.Environment.Default,
                new[] { new Tuple<string, GameStateTransition>("maze_overworld", new GameStateTransition(0)) },
                new[] { new Tuple<string, GameStateTransition>("maze_overworld", new GameStateTransition(0)) });
            state = game.GetGameState("maze_underworld");
            state.Scene = new CoreScene(rendererContext, messageProvider);
            state.Scene.BackgroundColor = Color4.AliceBlue;
            state.Scene.CameraManager.AddCamera (new BaseCamera (Player, state.MessageProxy), "player");
            maze[1] = mazeGenerator.CreateMaze(rand.Next(), state.MessageProxy, state.PhysicsManager, 30, 30);
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
                    maze[0].ExportAsImage("overworld.png");
                    maze[1].ExportAsImage("underworld.png");

                    finishedLoading = true;

                    deferredshadingnode.Active = true;
                    UIScene.Active = true;

                    if (game.CurrentGameState.Name != "maze_overworld")
                        game.SwitchToGameState("maze_overworld");
                }

                paremitter.Update ((float)um.TimeStamp.TotalSeconds);

                if(game.CurrentGameState == game.GetGameState("maze_overworld") && maze[0].HasFinished)
                    game.CurrentGameState.PhysicsManager.Update(um.TimeStamp);
                
                if(game.CurrentGameState == game.GetGameState("maze_underworld") && maze[1].HasFinished)
                    game.CurrentGameState.PhysicsManager.Update(um.TimeStamp);

                //Update pointlight position and direction
                CoreScene scene = null;
                scene = game.CurrentGameState.Scene;

                if (scene != null) 
                {
                    BaseCamera cam = scene.CameraManager.ActiveCamera;

                    if (cam != null) 
                    {
                        light.PointLightPosition = Player.GetComponent<TransformComponent> ().Position;

                        Vector3 SpotLightDir = (light.PointLightPosition + cam.Direction * 75.0f) - light.PointLightPosition;
                        SpotLightDir.Normalize ();

                        light.DirectionalLightDirection = SpotLightDir;
                    }
                }
            }

            var im = msg as InputMessage;
            if (im != null)
            {
                if (im.IsActionPressedAndRepeated("frame"))
                {
                    SwitchMaze();
                }

                if (im.IsActionDown ("bla_unfug_links")) 
                {
                    paremitter.SpawnPosition += new Vector3 (-0.02f, 0, 0);
                }

                if (im.IsActionDown ("bla_unfug_rechts")) 
                {
                    paremitter.SpawnPosition += new Vector3 (0.02f, 0, 0);
                }

                if (im.IsActionDown ("bla_unfug_runter")) 
                {
                    paremitter.SpawnPosition += new Vector3 (0, -0.02f, 0);
                }

                if (im.IsActionDown ("bla_unfug_hoch")) 
                {
                    paremitter.SpawnPosition += new Vector3 (0, 0.02f, 0);
                }
            }

            if (msg.MessageId == (int)MessageId.Running)
            {
                Logger.Log.AddLogEntry (LogLevel.Debug, "MazeTest", "Generate Mazes....");

                maze[0].Generate(() => {
                    if (MessageCreated != null)
                        MessageCreated (new TransformMessage (Player, maze [0].PlayerPosition, Quaternion.Identity));
                    var state = game.GetGameState ("maze_underworld");
                    maze[1].Generate (() => {
                        if (maze[0].IsGenerated && !maze[0].AreFeaturesPlaced)
                            maze[0].SpawnFeatures(null, maze[1].graph);
                        if (maze[1].IsGenerated && !maze[1].AreFeaturesPlaced)
                            maze[1].SpawnFeatures(maze[0].graph);
                    }, state);
                },game.GetGameState("maze_overworld"));
            }

            if (msg.MessageId == (int) MessageId.WindowClose)
            {
                paremitter.Destroy();
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
