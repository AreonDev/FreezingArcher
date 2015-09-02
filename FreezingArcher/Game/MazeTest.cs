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
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using System.Collections.Generic;
using FreezingArcher.Game.Ghosts;

namespace FreezingArcher.Game
{
    /// <summary>
    /// Maze test.
    /// </summary>
    public sealed class MazeTest : IMessageConsumer, IMessageCreator
    {
        const int ScobisCount = 2;
        const int CaligoCount = 1;
        const int PassusCount = 2;
        const int ViridionCount = 2;
        const int GhostCount = 3;

        LoadingScreen loadingScreen;
        Gwen.ControlInternal.Text FPS_Text;

        BasicCompositor Compositor;

        CompositorNodeScene MazeSceneNode;
        CompositorImageOverlayNode HealthOverlayNode;
        CompositorColorCorrectionNode ColorCorrectionNode;
        CompositorNodeOutput OutputNode;
        CompositorWarpingNode warpingNode;

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
            ValidMessages = new[] {
                (int)MessageId.Input,
                (int)MessageId.Update,
                (int)MessageId.Running,
                (int)MessageId.HealthChanged
            };
            messageProvider += this;
            mazeGenerator = new MazeGenerator (objmnr);

            this.game = game;
            application = app;

            FPS_Text = new Gwen.ControlInternal.Text (app.RendererContext.Canvas);
            FPS_Text.String = "0 FPS";
            FPS_Text.Font = new Gwen.Font (app.RendererContext.GwenRenderer);
            FPS_Text.SetPosition (5, 5);
            FPS_Text.Font.Size = 15;

            MazeSceneNode = new CompositorNodeScene (rendererContext, messageProvider);
            OutputNode = new CompositorNodeOutput (rendererContext, messageProvider);
            //OutputNode.EnableUI = false;
            ColorCorrectionNode = new CompositorColorCorrectionNode (rendererContext, messageProvider);
            HealthOverlayNode = new CompositorImageOverlayNode (rendererContext, messageProvider);
            HealthOverlayNode.OverlayTexture = rendererContext.CreateTexture2D ("bloodsplatter", true, "Content/bloodsplatter.png");
            HealthOverlayNode.Factor = 0;
            HealthOverlayNode.Blending = OverlayBlendMode.Multiply;
            warpingNode = new CompositorWarpingNode (rendererContext, messageProvider);
            warpingNode.WarpTexture = rendererContext.CreateTexture2D("warp", true, "Content/warp.jpg");

            game.MazeSceneNode = MazeSceneNode;

            game.AddGameState ("maze_overworld", Content.Environment.Default, null);
            var state = game.GetGameState ("maze_overworld");
            state.Scene = new CoreScene (rendererContext, messageProvider);
            state.Scene.SceneName = "MazeOverworld";
            state.Scene.Active = false;
            state.Scene.BackgroundColor = Color4.Fuchsia;
            state.Scene.DistanceFogIntensity = 0.07f;
            state.Scene.AmbientColor = Color4.White;
            state.Scene.AmbientIntensity = 0.3f;
            state.Scene.MaxRenderingDistance = 200.0f;



            state.AudioContext = new AudioContext (messageProvider);


            state.MessageProxy.StartProcessing ();

            loadingScreen = new LoadingScreen (application, messageProvider, "loading.png",
                "MazeLoadingScreen",
                to: new[] { new Tuple<string, GameStateTransition> (state.Name, new GameStateTransition (0)) });

            game.SwitchToGameState ("MazeLoadingScreen");

            Compositor = new BasicCompositor (objmnr, rendererContext);

            Compositor.AddNode (MazeSceneNode);
            Compositor.AddNode (OutputNode);
            Compositor.AddNode (HealthOverlayNode);
            Compositor.AddNode (ColorCorrectionNode);
            Compositor.AddNode (warpingNode);

            Compositor.AddConnection (MazeSceneNode, HealthOverlayNode, 0, 0);
            Compositor.AddConnection (HealthOverlayNode, ColorCorrectionNode, 0, 0);
            Compositor.AddConnection (ColorCorrectionNode, warpingNode, 0, 0);
            Compositor.AddConnection (warpingNode, OutputNode, 0, 0);

            rendererContext.Compositor = Compositor;

            Player = EntityFactory.Instance.CreateWith ("player", state.MessageProxy, new[] {
                typeof(HealthComponent),
            }, new[] {
                typeof(MovementSystem),
                typeof(KeyboardControllerSystem),
                typeof(MouseControllerSystem),
                typeof(SkyboxSystem),
                typeof(PhysicsSystem)
            });

            var input = new FreezingArcher.UI.Input.FreezingArcherInput(app, messageProvider);
            input.Initialize (rendererContext.Canvas);
            rendererContext.Canvas.SetSize(app.Window.Size.X, app.Window.Size.Y);
            rendererContext.Canvas.ShouldDrawBackground = false;

            inventoryGui = new InventoryGUI(app, state, Player, messageProvider);
            var inventory = new Inventory(messageProvider, state, Player, new Vector2i(5, 7), 9);
            inventoryGui.Init(rendererContext.Canvas, inventory);

            AddAudio (state);

            // embed new maze into game state logic and create a MoveEntityToScene
            SkyboxSystem.CreateSkybox (state.Scene, Player);
            Player.GetComponent<TransformComponent> ().Position = new Vector3 (0, 1.85f, 0);
            var maze_cam_entity = EntityFactory.Instance.CreateWith ("maze_cam_transform", state.MessageProxy, new[] {typeof (TransformComponent)});
            var maze_cam_transform = maze_cam_entity.GetComponent<TransformComponent>();
            var maze_cam = new BaseCamera (maze_cam_entity, state.MessageProxy, orthographic: true);
            state.Scene.CameraManager.AddCamera (maze_cam, "maze");
            maze_cam_transform.Position = new Vector3 (115, 240, 110);
            maze_cam_transform.Rotation = Quaternion.FromAxisAngle (Vector3.UnitX, MathHelper.PiOver2);
            state.Scene.CameraManager.AddCamera (new BaseCamera (Player, state.MessageProxy), "player");

            RigidBody playerBody = new RigidBody (new SphereShape (1f));
            playerBody.Position = Player.GetComponent<TransformComponent> ().Position.ToJitterVector ();
            playerBody.AllowDeactivation = false;
            playerBody.Material.StaticFriction = 0f;
            playerBody.Material.KineticFriction = 0f;
            playerBody.Material.Restitution = 0.1f;
            //playerBody.Mass = 1000000.0f;
            playerBody.Update ();
            Player.GetComponent<PhysicsComponent> ().RigidBody = playerBody;
            Player.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
            Player.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (playerBody);

            int seed = new Random().Next();
            var rand = new Random(seed);
            Logger.Log.AddLogEntry(LogLevel.Debug, "MazeTest", "Seed: {0}", seed);

            maze[0] = mazeGenerator.CreateMaze<OverworldMazeTheme> (rand.Next(), state.MessageProxy, state.PhysicsManager, app.AudioManager, 30, 30);
            maze[0].PlayerPosition += Player.GetComponent<TransformComponent>().Position;
            maze[0].AIManager.RegisterEntity (Player);

            for (int i = 0; i < ScobisCount; i++)
            {
                ScobisInstances.Add (new Scobis (state, maze[0].AIManager, rendererContext));
            }

            for (int i = 0; i < CaligoCount; i++)
            {
                CaligoInstances.Add (new Caligo (state, maze[0].AIManager, rendererContext, warpingNode));
            }

            for (int i = 0; i < ViridionCount; i++)
            {
                ViridionInstances.Add (new Viridion (state, maze[0].AIManager, rendererContext, ColorCorrectionNode)); 
            }

            for (int i = 0; i < GhostCount; i++)
            {
                GhostInstances.Add (new Ghost (state, maze[0].AIManager, rendererContext, ColorCorrectionNode));
            }

            game.AddGameState("maze_underworld", Content.Environment.Default,
                new[] { new Tuple<string, GameStateTransition>("maze_overworld", new GameStateTransition(0)) },
                new[] { new Tuple<string, GameStateTransition>("maze_overworld", new GameStateTransition(0)) });
            state = game.GetGameState("maze_underworld");
            state.Scene = new CoreScene(rendererContext, messageProvider);
            state.Scene.SceneName = "MazeUnderworld";
            state.Scene.Active = false;
            state.Scene.BackgroundColor = Color4.AliceBlue;
            state.Scene.DistanceFogIntensity = 0.07f;
            state.Scene.AmbientColor = Color4.White;
            state.Scene.AmbientIntensity = 0.3f;
            state.Scene.MaxRenderingDistance = 200.0f;

            state.AudioContext = new AudioContext (messageProvider);

            AddAudio (state);

            state.Scene.CameraManager.AddCamera (new BaseCamera (Player, state.MessageProxy), "player");
            maze [1] = mazeGenerator.CreateMaze<UnderworldMazeTheme> (rand.Next (), state.MessageProxy, state.PhysicsManager, app.AudioManager, 30, 30);
            maze [1].PlayerPosition += Player.GetComponent<TransformComponent> ().Position;
            maze [1].AIManager.RegisterEntity (Player);

            Func<int, int, bool> containsPortalFunc = (x, y) => {
                foreach (var m in maze)
                {
                    var cell = m.entities [x, y].GetComponent<PhysicsComponent>().RigidBody.Tag as MazeCell;
                    if (cell != null && cell.IsPortal)
                    {
                        return true;
                    }
                }
                return false;
            };

            mazeWallMover = new MazeWallMover(maze[0], maze[1], game.GetGameState("maze_overworld"), containsPortalFunc);

            state.MessageProxy.StopProcessing ();
            //game.SwitchToGameState("maze_overworld");

            for (int i = 0; i < CaligoCount; i++)
            {
                CaligoInstances.Add (new Caligo (state, maze[1].AIManager, rendererContext, warpingNode));
            }

            for (int i = 0; i < PassusCount; i++)
            {
                PassusInstances.Add (new Passus (ColorCorrectionNode, state, maze[1].AIManager, rendererContext));
            }
        }

        readonly MazeWallMover mazeWallMover;

        readonly MazeGenerator mazeGenerator;

        readonly Maze.Maze[] maze = new Maze.Maze[2];

        public Entity Player { get; private set; }

        List<Scobis> ScobisInstances = new List<Scobis>();
        List<Caligo> CaligoInstances = new List<Caligo>();
        List<Passus> PassusInstances = new List<Passus>();
        List<Viridion> ViridionInstances = new List<Viridion>();
        List<Ghost> GhostInstances = new List<Ghost>();

        readonly InventoryGUI inventoryGui;
        readonly Content.Game game;

        readonly Application application;

        int currentMaze;

        void AddAudio(GameState state)
        {
            //Load walking sound
            Audio.Source src = application.AudioManager.GetSource("footstep_SoundSource");

            if (src == null)
            {
                application.AudioManager.LoadSound ("footstep_Sound", "Content/Audio/footstep.wav");
                src = application.AudioManager.CreateSource ("footstep_SoundSource", "footstep_Sound");
            }

            src.Gain = 1.0f;
            src.Loop = false;

            state.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.PlayerMove,
                new SoundSourceDescription (src, SoundAction.Play, Player));

            //Set listener Position from PlayerPosition
            TransformComponent tfc = Player.GetComponent<TransformComponent>();
            tfc.OnPositionChanged += (Vector3 obj) => application.AudioManager.Listener.Position = obj;

            //Item collect sound
            src = application.AudioManager.GetSource("item_collected_SoundSource");

            if (src == null)
            {
                application.AudioManager.LoadSound ("item_collected_Sound", "Content/Audio/item_collected.wav");
                src = application.AudioManager.CreateSource ("item_collected_SoundSource", "item_collected_Sound");
            }

            src.Gain = 1.0f;
            src.Loop = false;

            state.AudioContext.RegisterSoundPlaybackOnMessage(MessageId.ItemCollected,
                new SoundSourceDescription(src, SoundAction.Play, Player));

            //Item dropped sound
            src = application.AudioManager.GetSource("item_dropped_SoundSource");

            if (src == null)
            {
                application.AudioManager.LoadSound ("item_dropped_Sound", "Content/Audio/item_dropped.wav");
                src = application.AudioManager.CreateSource ("item_dropped_SoundSource", "item_dropped_Sound");
            }

            src.Gain = 1.0f;
            src.Loop = false;

            state.AudioContext.RegisterSoundPlaybackOnMessage(MessageId.ItemDropped,
                new SoundSourceDescription(src, SoundAction.Play, Player));

            //Wall moving sound
            src = application.AudioManager.GetSource("moving_wall_SoundSource");

            if (src == null)
            {
                application.AudioManager.LoadSound ("moving_wall_Sound", "Content/Audio/moving_wall.wav");
                src = application.AudioManager.CreateSource ("moving_wall_SoundSource", "moving_wall_Sound");
            }

            src.Gain = 1.0f;
            src.Loop = false;

            state.AudioContext.RegisterSoundPlaybackOnMessage(MessageId.BeginWallMovement,
                new SoundSourceDescription(src, SoundAction.Play));

            //Stop wall moving sound on stop
            state.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.EndWallMovement,
                new SoundSourceDescription (src, SoundAction.Stop));
        }

        void SwitchMaze ()
        {
            game.CurrentGameState.Scene.Active = false;
            game.CurrentGameState.AudioContext.StopAllSounds();

            if (currentMaze == 0)
            {
                maze [0].PlayerPosition = Player.GetComponent<TransformComponent> ().Position;

                game.MoveEntityToGameState (Player, game.GetGameState ("maze_overworld"), game.GetGameState ("maze_underworld"));

                inventoryGui.SwitchGameState (game.GetGameState ("maze_overworld"), game.GetGameState ("maze_underworld"), game);

                game.SwitchToGameState ("maze_underworld");

                //if (MessageCreated != null)
                //    MessageCreated (new TransformMessage (Player, maze [1].PlayerPosition, Quaternion.Identity));

                Player.GetComponent<PhysicsComponent> ().RigidBody.Position = maze [1].PlayerPosition.ToJitterVector () + Jitter.LinearMath.JVector.Up;

                currentMaze = 1;
            }
            else
            if (currentMaze == 1)
            {
                maze [1].PlayerPosition = Player.GetComponent<TransformComponent> ().Position;
                game.MoveEntityToGameState (Player, game.GetGameState ("maze_underworld"), game.GetGameState ("maze_overworld"));

                inventoryGui.SwitchGameState (game.GetGameState ("maze_underworld"), game.GetGameState ("maze_overworld"), game);

                game.SwitchToGameState ("maze_overworld");

                //if (MessageCreated != null)
                //    MessageCreated (new TransformMessage (Player, maze [0].PlayerPosition, Quaternion.Identity));

                     
                Player.GetComponent<PhysicsComponent> ().RigidBody.Position = maze [0].PlayerPosition.ToJitterVector () + Jitter.LinearMath.JVector.Up;
                    
                currentMaze = 0;
            }
                    
            game.CurrentGameState.Scene.Active = true;
            mazeWallMover.SwitchGameState (game.CurrentGameState);
        }

#region IMessageConsumer implementation

        bool finishedLoading = false;
        bool lighting = true;
        int count = 0;

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.Update)
            {
                var um = msg as UpdateMessage;

                if (maze [0].HasFinished && maze [1].HasFinished && !finishedLoading)
                {
                    maze [0].ExportAsImage ("overworld.png");
                    maze [1].ExportAsImage ("underworld.png");

                    finishedLoading = true;

                    loadingScreen.Ready ();

                    if (game.CurrentGameState.Name != "maze_overworld")
                    {
                        game.SwitchToGameState ("maze_overworld");

                        game.CurrentGameState.Scene.Active = true;
                    }
                }
                else if (!finishedLoading)
                    loadingScreen.BringToFront ();

                if (game.CurrentGameState == game.GetGameState ("maze_overworld") && maze [0].HasFinished)
                    game.CurrentGameState.PhysicsManager.Update (um.TimeStamp);

                if (game.CurrentGameState == game.GetGameState ("maze_underworld") && maze [1].HasFinished)
                    game.CurrentGameState.PhysicsManager.Update (um.TimeStamp);

                if (application.FPSCounter >= 50)
                   FPS_Text.TextColor = System.Drawing.Color.Green;
                else if (application.FPSCounter < 50 && application.FPSCounter > 30)
                    FPS_Text.TextColor = System.Drawing.Color.Yellow;
               else
                    FPS_Text.TextColor = System.Drawing.Color.Red;

                FPS_Text.String = application.FPSCounter + " FPS";

                if (HealthOverlayNode.Factor > 0.0001f)
                {
                    var factor = HealthOverlayNode.Factor - 0.01f;
                    HealthOverlayNode.Factor = factor < 0 ? 0 : factor;
                }
            }

            if (msg.MessageId == (int) MessageId.HealthChanged)
            {
                var hcm = msg as HealthChangedMessage;

                if (hcm.Entity == Player)
                {
                    if (hcm.HealthDelta < 0)
                    {
                        var factor = -hcm.HealthDelta / 20;
                        HealthOverlayNode.Factor = factor > 1 ? 1 : factor;
                    }

                    var healthComponent = Player.GetComponent<HealthComponent>();
                    var health = healthComponent.Health > 0 ? healthComponent.Health : 0;
                    ColorCorrectionNode.Saturation = -((healthComponent.MaximumHealth - health) / (healthComponent.MaximumHealth)) / 4;
                }
            }

            var im = msg as InputMessage;
            if (im != null)
            {
                if (im.IsActionPressed ("frame"))
                {
                    SwitchMaze();
                }

                if (im.IsActionPressed ("frame"))
                {
                    /*
                    if (lighting)
                    {
                        var state = game.CurrentGameState;
                        state.Scene.DistanceFogIntensity = 0;
                        state.Scene.AmbientIntensity = 1f;
                        //light1.PointLightLinearAttenuation = 99999;
                        lighting = false;
                    }
                    else
                    {
                        var state = game.CurrentGameState;
                        state.Scene.DistanceFogIntensity = 0.04f;
                        state.Scene.AmbientIntensity = 0.35f;
                        //light1.PointLightLinearAttenuation = 0.01f;
                        lighting = true;
                    }*/
                }

                if (im.IsActionPressed("damage"))
                {
                    var healthComponent = Player.GetComponent<HealthComponent>();
                    healthComponent.Health -= 10;
                }
            }

            if (msg.MessageId == (int)MessageId.Running)
            {
                Logger.Log.AddLogEntry (LogLevel.Debug, "MazeTest", "Generating mazes....");

                maze [0].Generate (() =>
                {
                    if (MessageCreated != null)
                        MessageCreated (new TransformMessage (Player, maze [0].PlayerPosition, Quaternion.Identity));
                    var state = game.GetGameState ("maze_underworld");
                    maze [1].Generate (() =>
                    {
                        if (maze [0].IsGenerated && !maze [0].AreFeaturesPlaced)
                            maze [0].SpawnFeatures (null, maze [1].graph);
                        if (maze [1].IsGenerated && !maze [1].AreFeaturesPlaced)
                            maze [1].SpawnFeatures (maze [0].graph);
                        maze[0].AIManager.CalculateSpawnPositions(maze [0].PlayerPosition);
                        maze[1].AIManager.CalculateSpawnPositions(maze [0].PlayerPosition);
                        inventoryGui.CreateInitialFlashlight ();
                        var healthcomp = Player.GetComponent<HealthComponent>();
                        healthcomp.Health = healthcomp.MaximumHealth;
                    }, state);
                }, game.GetGameState ("maze_overworld"));
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

        public void Destroy ()
        {
            maze[0].Destroy ();
            maze[1].Destroy ();
        }
    }
}
