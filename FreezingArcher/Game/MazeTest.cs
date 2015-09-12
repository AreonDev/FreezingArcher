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
using FreezingArcher.Audio;
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
using FreezingArcher.Game.Particles;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.UI.Input;

namespace FreezingArcher.Game
{
    /// <summary>
    /// Maze test.
    /// </summary>
    public sealed class MazeTest : IMessageConsumer, IMessageCreator
    {
        const int OverworldScobisCount = 2;
        const int OverworldCaligoCount = 1;
        const int OverworldPassusCount = 2;
        const int OverworldViridionCount = 2;
        const int OverworldGhostCount = 3;
        const int UnderworldCaligoCount = 2;
        const int UnderworldPassusCount = 4;
        const int UnderworldRoachesCount = 5;
        const int UnderworldFenFireCount = 5;

        Source switchMazeSound;
        Source playerDamagedSound;
        Source playerDiedSound;
        Source playerNoStamina;
        Source playerDrinked;
        Source playerEaten;
        Source playerFlashlightTrigger;

        Source backgroundMusic;

        Texture2D PortalWarpTexture;
        Texture2D DefaultWarpingTexture;

        EndScreen     endScreen;
        LoadingScreen loadingScreen;
        Gwen.ControlInternal.Text FPS_Text;

        BasicCompositor Compositor;
         
        PauseMenu PauseMenu;

        CompositorNodeScene MazeSceneNode;
        CompositorImageOverlayNode HealthOverlayNode;
        CompositorColorCorrectionNode ColorCorrectionNode;
        CompositorNodeOutput OutputNode;
        CompositorWarpingNode WarpingNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.MazeTest"/> class.
        /// </summary>
        /// <param name="messageProvider">The message provider for this instance.</param>
        /// <param name="objmnr">The object manager for this instance.</param>
        /// <param name="rendererContext">The renderer context for the maze scenes.</param>
        /// <param name="game">The game the maze should be generated in.</param>
        public MazeTest (MessageProvider messageProvider, ObjectManager objmnr, RendererContext rendererContext,
            Content.Game game, Application app, CompositorNodeScene sceneNode,
            CompositorImageOverlayNode healthOverlayNode, CompositorColorCorrectionNode colorCorrectionNode,
            CompositorNodeOutput outputNode, CompositorWarpingNode warpingNode)
        {
            ValidMessages = new[] {
                (int) MessageId.Input,
                (int) MessageId.Update,
                (int) MessageId.HealthChanged,
                (int) MessageId.CollisionDetected,
                (int) MessageId.StaminaChanged,
                (int) MessageId.ItemUse,
                (int)MessageId.FlashlightToggled
            };
            messageProvider += this;
            mazeGenerator = new MazeGenerator (objmnr);

            MazeSceneNode = sceneNode;
            HealthOverlayNode = healthOverlayNode;
            ColorCorrectionNode = colorCorrectionNode;
            OutputNode = outputNode;
            WarpingNode = warpingNode;
            this.game = game;
            application = app;

            FPS_Text = new Gwen.ControlInternal.Text (app.RendererContext.Canvas);
            FPS_Text.String = "0 FPS";
            FPS_Text.Font = new Gwen.Font (app.RendererContext.GwenRenderer);
            FPS_Text.SetPosition (5, 5);
            FPS_Text.Font.Size = 15;
            FPS_Text.Hide ();

            HealthOverlayNode.OverlayTexture = rendererContext.CreateTexture2D ("bloodsplatter", true, "Content/bloodsplatter.png");
            HealthOverlayNode.Factor = 0;
            HealthOverlayNode.Blending = OverlayBlendMode.Multiply;
            warpingNode.WarpTexture = rendererContext.CreateTexture2D ("warp", true, "Content/warp.jpg");

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
            state.Scene.MaxRenderingDistance = 1000.0f;

            state.AudioContext = new AudioContext (state.MessageProxy);

            state.MessageProxy.StartProcessing ();

            loadingScreen = new LoadingScreen (application, messageProvider, "Content/loading.png",
                "MazeLoadingScreen",
                new[] { new Tuple<string, GameStateTransition> ("main_menu", GameStateTransition.DefaultTransition) },
                new[] { new Tuple<string, GameStateTransition> (state.Name, new GameStateTransition (0)) });

            endScreen = new EndScreen (application, rendererContext, new Tuple<string, GameStateTransition>[] {
                new Tuple<string, GameStateTransition> ("maze_overworld", GameStateTransition.DefaultTransition),
                new Tuple<string, GameStateTransition> ("maze_underworld", GameStateTransition.DefaultTransition)
            });

            game.SwitchToGameState ("MazeLoadingScreen");

            Player = EntityFactory.Instance.CreateWith ("player", state.MessageProxy, new[] {
                typeof(HealthComponent),
                typeof(StaminaComponent)
            }, new[] {
                typeof(MovementSystem),
                typeof(KeyboardControllerSystem),
                typeof(MouseControllerSystem),
                typeof(SkyboxSystem),
                typeof(PhysicsSystem)
            });

            inventoryGui = new InventoryGUI (app, state, Player, messageProvider, warpingNode);
            var inventory = new Inventory (messageProvider, state, Player, new Vector2i (5, 7), 9);
            inventoryGui.Init (rendererContext.Canvas, inventory);

            PauseMenu = new PauseMenu (application, ColorCorrectionNode, rendererContext.Canvas,
                () => maze [currentMaze].AIManager.StartThinking (), () => maze [currentMaze].AIManager.StopThinking ());
            
            AddAudio (state);

            // embed new maze into game state logic and create a MoveEntityToScene
            SkyboxSystem.CreateSkybox (state.Scene, Player);
            Player.GetComponent<TransformComponent> ().Position = new Vector3 (0, 1.85f, 0);
            var maze_cam_entity = EntityFactory.Instance.CreateWith ("maze_cam_transform", state.MessageProxy, new[] { typeof(TransformComponent) });
            var maze_cam_transform = maze_cam_entity.GetComponent<TransformComponent> ();
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

            int seed = new FastRandom ().Next ();
            var rand = new FastRandom (seed);
            Logger.Log.AddLogEntry (LogLevel.Debug, "MazeTest", "Seed: {0}", seed);

            maze [0] = mazeGenerator.CreateMaze<OverworldMazeTheme> (rand.Next (), state.MessageProxy, state.PhysicsManager, app.AudioManager, 30, 30);
            maze [0].PlayerPosition += Player.GetComponent<TransformComponent> ().Position;
            maze [0].AIManager.RegisterEntity (Player);

            for (int i = 0; i < OverworldScobisCount; i++)
            {
                ScobisInstances.Add (new Scobis (state, maze [0].AIManager, rendererContext));
            }

            for (int i = 0; i < OverworldCaligoCount; i++)
            {
                CaligoInstances.Add (new Caligo (state, maze [0].AIManager, rendererContext, warpingNode));
            }

            for (int i = 0; i < OverworldViridionCount; i++)
            {
                ViridionInstances.Add (new Viridion (state, maze [0].AIManager, rendererContext, ColorCorrectionNode)); 
            }

            for (int i = 0; i < OverworldGhostCount; i++)
            {
                GhostInstances.Add (new Ghost (state, maze [0].AIManager, rendererContext, ColorCorrectionNode));
            }

            game.AddGameState ("maze_underworld", Content.Environment.Default,
                new[] { new Tuple<string, GameStateTransition> ("maze_overworld", new GameStateTransition (0)) },
                new[] { new Tuple<string, GameStateTransition> ("maze_overworld", new GameStateTransition (0)),
                    new Tuple<string, GameStateTransition> ("endscreen_state", new GameStateTransition (0))
                });
            state = game.GetGameState ("maze_underworld");
            state.Scene = new CoreScene (rendererContext, messageProvider);
            state.Scene.SceneName = "MazeUnderworld";
            state.Scene.Active = false;
            state.Scene.BackgroundColor = Color4.AliceBlue;
            state.Scene.DistanceFogIntensity = 0.07f;
            state.Scene.AmbientColor = Color4.White;
            state.Scene.AmbientIntensity = 0.3f;
            state.Scene.MaxRenderingDistance = 1000.0f;

            state.AudioContext = new AudioContext (state.MessageProxy);

            AddAudio (state);

            state.Scene.CameraManager.AddCamera (new BaseCamera (Player, state.MessageProxy), "player");
            maze [1] = mazeGenerator.CreateMaze<UnderworldMazeTheme> (rand.Next (), state.MessageProxy, state.PhysicsManager, app.AudioManager, 30, 30);
            maze [1].PlayerPosition += Player.GetComponent<TransformComponent> ().Position;
            maze [1].AIManager.RegisterEntity (Player);

            Func<int, int, bool> containsPortalFunc = (x, y) =>
            {
                foreach (var m in maze)
                {
                    var cell = m.entities [x, y].GetComponent<PhysicsComponent> ().RigidBody.Tag as MazeCell;
                    if (cell != null && cell.IsPortal)
                    {
                        return true;
                    }
                }
                return false;
            };

            mazeWallMover = new MazeWallMover (maze [0], maze [1], game.GetGameState ("maze_overworld"), containsPortalFunc);

            state.MessageProxy.StopProcessing ();
            //game.SwitchToGameState("maze_overworld");

            for (int i = 0; i < UnderworldCaligoCount; i++)
            {
                CaligoInstances.Add (new Caligo (state, maze [1].AIManager, rendererContext, warpingNode));
            }

            for (int i = 0; i < UnderworldPassusCount; i++)
            {
                PassusInstances.Add (new Passus (ColorCorrectionNode, state, maze [1].AIManager, rendererContext));
            }

            for (int i = 0; i < UnderworldRoachesCount; i++)
            {
                RoachesInstances.Add(new Roaches (state, maze [0].AIManager, rendererContext));
            }

            for (int i = 0; i < UnderworldFenFireCount; i++)
            {
                FenFireInstances.Add (new FenFire (state, maze [1].AIManager, rendererContext));
            }
        }

        public void Generate ()
        {
            Logger.Log.AddLogEntry (LogLevel.Debug, "MazeTest", "Generating mazes....");

            maze [0].Generate (loadingScreen.UpdateProgress, () =>
            {
                if (MessageCreated != null)
                    MessageCreated (new TransformMessage (Player, maze [0].PlayerPosition, Quaternion.Identity));
                var _state = game.GetGameState ("maze_underworld");
                maze [1].Generate (loadingScreen.UpdateProgress, () =>
                {
                    if (maze [0].IsGenerated && !maze [0].AreFeaturesPlaced)
                        maze [0].SpawnFeatures (null, maze [1].graph);
                    if (maze [1].IsGenerated && !maze [1].AreFeaturesPlaced)
                        maze [1].SpawnFeatures (maze [0].graph);
                    maze[0].AIManager.CalculateSpawnPositions(maze [0].PlayerPosition);
                    maze[1].AIManager.CalculateSpawnPositions(maze [0].PlayerPosition);

                    application.AudioManager.LoadSound ("background_music_Sound", "Content/Audio/background_music.ogg");
                    backgroundMusic = application.AudioManager.CreateSource ("background_music_SoundSource", "background_music_Sound");
                    backgroundMusic.Gain = 1.0f;
                    backgroundMusic.Loop = true;

                    AddAudioToGhosts ();

                    //Load SwitchMaze sound
                    application.AudioManager.LoadSound ("portal_Sound", "Content/Audio/portal.ogg");
                    switchMazeSound = application.AudioManager.CreateSource ("portal_SoundSource", "portal_Sound");

                    //Load some player sounds
                    application.AudioManager.LoadSound ("player_damage_Sound", "Content/Audio/player_damage.ogg");
                    playerDamagedSound = application.AudioManager.CreateSource ("player_damage_SoundSource", "player_damage_Sound");
                    playerDamagedSound.Gain = 0.3f;

                    application.AudioManager.LoadSound ("player_died_Sound", "Content/Audio/player_died.ogg");
                    playerDiedSound = application.AudioManager.CreateSource ("player_died_SoundSource", "player_died_Sound");

                    application.AudioManager.LoadSound ("player_no_stamina_Sound", "Content/Audio/player_no_stamina.ogg");
                    playerNoStamina = application.AudioManager.CreateSource ("player_no_stamina_SoundSource", "player_no_stamina_Sound");
                    playerNoStamina.Gain = 0.2f;

                    application.AudioManager.LoadSound ("player_drinked_Sound", "Content/Audio/player_drinked.ogg");
                    playerDrinked = application.AudioManager.CreateSource ("player_drinked_SoundSource", "player_drinked_Sound");

                    application.AudioManager.LoadSound ("player_eaten_Sound", "Content/Audio/player_eaten.ogg");
                    playerEaten = application.AudioManager.CreateSource ("player_eaten_SoundSource", "player_eaten_Sound");

                    application.AudioManager.LoadSound ("player_flashlight_Sound", "Content/Audio/flashlight_trigger.ogg");
                    playerFlashlightTrigger = application.AudioManager.CreateSource ("player_flashlight_SoundSource", "player_flashlight_Sound");
                    playerFlashlightTrigger.Gain = 0.15f;

                    var healthcomp = Player.GetComponent<HealthComponent>();
                    healthcomp.Health = healthcomp.MaximumHealth;
                    inventoryGui.CreateInitialFlashlight ();

                    StartTime = DateTime.Now;
                }, _state);
            }, game.GetGameState ("maze_overworld"));            
        }

        readonly MazeWallMover mazeWallMover;

        readonly MazeGenerator mazeGenerator;

        readonly Maze.Maze[] maze = new Maze.Maze[2];

        public Entity Player { get; private set; }

        public List<Entity> Portals;

        List<Scobis> ScobisInstances = new List<Scobis>();
        List<Caligo> CaligoInstances = new List<Caligo>();
        List<Passus> PassusInstances = new List<Passus>();
        List<Viridion> ViridionInstances = new List<Viridion>();
        List<Ghost> GhostInstances = new List<Ghost>();
        List<Roaches> RoachesInstances = new List<Roaches>();
        List<FenFire> FenFireInstances = new List<FenFire>();

        readonly InventoryGUI inventoryGui;
        readonly Content.Game game;

        readonly Application application;

        int currentMaze;

        public static DateTime StartTime;

        void AddAudio(GameState state)
        {
            //Load walking sound
            Audio.Source src = application.AudioManager.GetSource("footstep_SoundSource");

            if (src == null)
            {
                application.AudioManager.LoadSound ("footstep_Sound", "Content/Audio/footstep.ogg");
                src = application.AudioManager.CreateSource ("footstep_SoundSource", "footstep_Sound");
            }

            src.Gain = 0.5f;
            src.Loop = false;

            state.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.PlayerMove,
                new SoundSourceDescription (src, SoundAction.Play, Player));

            state.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.PlayerRun,
                new SoundSourceDescription (src, SoundAction.Stop, Player));

            //Load running sound
            src = application.AudioManager.GetSource("footstep_run_SoundSource");

            if (src == null)
            {
                application.AudioManager.LoadSound ("footstep_run_Sound", "Content/Audio/footstep_run.ogg");
                src = application.AudioManager.CreateSource ("footstep_run_SoundSource", "footstep_run_Sound");
            }

            src.Loop = false;
            src.Gain = 0.5f;

            state.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.PlayerRun, 
                new SoundSourceDescription (src, SoundAction.Play, Player));

            state.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.PlayerMove,
                new SoundSourceDescription (src, SoundAction.Stop, Player));

            //Load jump sound
            src = application.AudioManager.GetSource("player_jump_SoundSource");

            if (src == null)
            {
                application.AudioManager.LoadSound ("player_jump_Sound", "Content/Audio/player_jumped.ogg");
                src = application.AudioManager.CreateSource ("player_jump_SoundSource", "player_jump_Sound");
            }

            src.Gain = 0.5f;
            src.Loop = false;

            state.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.PlayerJump,
                new SoundSourceDescription (src, SoundAction.Play, Player));

            //Set listener Position from PlayerPosition
            TransformComponent tfc = Player.GetComponent<TransformComponent>();
            tfc.OnPositionChanged += (Vector3 obj) => application.AudioManager.Listener.Position = obj;

            //Item collect sound
            src = application.AudioManager.GetSource("item_collected_SoundSource");

            if (src == null)
            {
                application.AudioManager.LoadSound ("item_collected_Sound", "Content/Audio/item_collected.ogg");
                src = application.AudioManager.CreateSource ("item_collected_SoundSource", "item_collected_Sound");
            }

            src.Gain = 0.25f;
            src.Loop = false;

            state.AudioContext.RegisterSoundPlaybackOnMessage(MessageId.ItemCollected,
                new SoundSourceDescription(src, SoundAction.Play, Player));

            //Item dropped sound
            src = application.AudioManager.GetSource("item_dropped_SoundSource");

            if (src == null)
            {
                application.AudioManager.LoadSound ("item_dropped_Sound", "Content/Audio/item_dropped.ogg");
                src = application.AudioManager.CreateSource ("item_dropped_SoundSource", "item_dropped_Sound");
            }

            src.Gain = 0.5f;
            src.Loop = false;

            state.AudioContext.RegisterSoundPlaybackOnMessage(MessageId.ItemDropped,
                new SoundSourceDescription(src, SoundAction.Play, Player));

            //Wall moving sound
            src = application.AudioManager.GetSource("moving_wall_SoundSource");

            if (state.Name != "maze_underworld")
            {
                if (src == null)
                {
                    application.AudioManager.LoadSound ("moving_wall_Sound", "Content/Audio/moving_wall.ogg");
                    src = application.AudioManager.CreateSource ("moving_wall_SoundSource", "moving_wall_Sound");
                }

                src.Gain = 0.5f;
                src.Loop = false;
            }
            else
            {
                src = application.AudioManager.GetSource ("moving_wall_wood_SoundSource");

                if (src == null)
                {
                    //application.AudioManager.LoadSound ("moving_wall_Sound", "Content/Audio/moving_wall.ogg");
                    application.AudioManager.LoadSound ("moving_wall_wood_Sound", "Content/Audio/moving_wall_wood.ogg");

                    src = application.AudioManager.CreateSource ("moving_wall_wood_SoundSource", "moving_wall_wood_Sound");
                }

                src.Gain = 0.5f;
                src.Loop = false;
            }

            state.AudioContext.RegisterSoundPlaybackOnMessage(MessageId.BeginWallMovement,
                new SoundSourceDescription(src, SoundAction.Play));

            //Stop wall moving sound on stop
            state.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.EndWallMovement,
                new SoundSourceDescription (src, SoundAction.Stop));
        }

        void AddAudioToGhosts()
        {
            //Add Caligo sound
            application.AudioManager.LoadSound("caligo_attack_Sound", "Content/Audio/caligo_attack.ogg");
            Source src = application.AudioManager.CreateSource ("caligo_attack_SoundSource", "caligo_attack_Sound");

            src.Gain = 0.4f;

            foreach (Caligo cal in CaligoInstances)
            {
                cal.CaligoGameState.AudioContext.RegisterSoundPlaybackOnMessage(MessageId.AIAttack, 
                    new SoundSourceDescription(src, SoundAction.Play, cal.CaligoEntity));
            }

            //Add Passus sound
            application.AudioManager.LoadSound("passus_attack_Sound", "Content/Audio/passus_attack.ogg");
            src = application.AudioManager.CreateSource ("passus_attack_SoundSource", "passus_attack_Sound");

            src.Gain = 0.4f;

            foreach(Passus pass in PassusInstances)
            {
                pass.PassusGameState.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.AIAttack,
                    new SoundSourceDescription (src, SoundAction.Play, pass.PassusEntity));
            }

            //Add Scobis sound
            application.AudioManager.LoadSound("scobis_attack_Sound", "Content/Audio/scobis_attack.ogg");
            src = application.AudioManager.CreateSource ("scobis_attack_SoundSource", "scobis_attack_Sound");

            src.Gain = 0.1f;

            foreach (Scobis scob in ScobisInstances)
            {
                scob.ScobisGameState.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.AIAttack,
                    new SoundSourceDescription (src, SoundAction.Play, scob.ScobisEntity));
            }

            //Add white ghost sound
            application.AudioManager.LoadSound("ghost_attack_Sound", "Content/Audio/white_ghost_attack.ogg");
            src = application.AudioManager.CreateSource ("ghost_attack_SoundSource", "ghost_attack_Sound");

            src.Gain = 0.4f;

            foreach (Ghost gho in GhostInstances)
            {
                gho.GhostGameState.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.AIAttack,
                    new SoundSourceDescription (src, SoundAction.Play, gho.GhostEntity));
            }

            //Add viridion sound
            application.AudioManager.LoadSound("viridion_attack_Sound", "Content/Audio/viridion_attack.ogg");
            src = application.AudioManager.CreateSource ("viridion_attack_SoundSource", "viridion_attack_Sound");

            src.Gain = 0.4f;

            foreach (Viridion vir in ViridionInstances)
            {
                vir.ViridionGameState.AudioContext.RegisterSoundPlaybackOnMessage (MessageId.AIAttack,
                    new SoundSourceDescription (src, SoundAction.Play, vir.ViridionEntity));
            }
        }

        void SwitchMaze ()
        {
            game.CurrentGameState.Scene.Active = false;
            game.CurrentGameState.AudioContext.StopAllSounds();

            if (currentMaze == 0)
            {
                maze [0].PlayerPosition = Player.GetComponent<TransformComponent> ().Position;
                maze [1].PlayerPosition = Player.GetComponent<TransformComponent> ().Position;

                game.MoveEntityToGameState (Player, game.GetGameState ("maze_overworld"), game.GetGameState ("maze_underworld"));

                inventoryGui.SwitchGameState (game.GetGameState ("maze_overworld"), game.GetGameState ("maze_underworld"), game);

                game.SwitchToGameState ("maze_underworld");

                //if (MessageCreated != null)
                //    MessageCreated (new TransformMessage (Player, maze [1].PlayerPosition, Quaternion.Identity));

                Player.GetComponent<PhysicsComponent> ().RigidBody.Position = maze [1].PlayerPosition.ToJitterVector () + Jitter.LinearMath.JVector.Up*2.0f;

                currentMaze = 1;

                maze [0].AIManager.StopThinking ();
                maze [1].AIManager.StartThinking ();
            }
            else if (currentMaze == 1)
            {
                maze [1].PlayerPosition = Player.GetComponent<TransformComponent> ().Position;
                maze [0].PlayerPosition = Player.GetComponent<TransformComponent> ().Position;

                game.MoveEntityToGameState (Player, game.GetGameState ("maze_underworld"), game.GetGameState ("maze_overworld"));

                inventoryGui.SwitchGameState (game.GetGameState ("maze_underworld"), game.GetGameState ("maze_overworld"), game);

                game.SwitchToGameState ("maze_overworld");

                //if (MessageCreated != null)
                //    MessageCreated (new TransformMessage (Player, maze [0].PlayerPosition, Quaternion.Identity));
                     
                Player.GetComponent<PhysicsComponent> ().RigidBody.Position = maze [0].PlayerPosition.ToJitterVector () + Jitter.LinearMath.JVector.Up*2.0f;
                    
                currentMaze = 0;

                maze [1].AIManager.StopThinking ();
                maze [0].AIManager.StartThinking ();
            }
                    
            game.CurrentGameState.Scene.Active = true;
            mazeWallMover.SwitchGameState (game.CurrentGameState);
        }

#region IMessageConsumer implementation

        bool finishedLoading = false;
        bool lighting = true;
        int count = 0;

        bool switch_maze = false;
        bool entered_portal = false;
        bool can_play_background = false;

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Update)
            {
                var um = msg as UpdateMessage;

                if (maze [0].HasFinished && maze [1].HasFinished && !finishedLoading)
                {
                    #if DEBUG
                    maze [0].ExportAsImage ("overworld.png");
                    maze [1].ExportAsImage ("underworld.png");
                    #endif

                    //Add Portals
                    Portals = new List<Entity> ();

                    foreach (var node in maze[0].graph.Nodes)
                    {
                        if (node.Data.IsPortal)
                        {
                            var portalEmitter = new PortalParticles ();
                            var particleSceneObject = new ParticleSceneObject (portalEmitter.ParticleCount);
                            particleSceneObject.Priority = 7000;
                            game.GetGameState ("maze_overworld").Scene.AddObject (particleSceneObject); 

                            portalEmitter.Init (particleSceneObject, Application.Instance.RendererContext);

                            var portalEntity = EntityFactory.Instance.CreateWith ("PortalEmitter " + DateTime.Now.Ticks, 
                                                   game.GetGameState ("maze_overworld").MessageProxy, systems: new[] { typeof(ParticleSystem) });

                            portalEntity.GetComponent<ParticleComponent> ().Emitter = portalEmitter;
                            portalEntity.GetComponent<ParticleComponent> ().Particle = particleSceneObject;

                            portalEntity.GetComponent<TransformComponent> ().Position = node.Data.WorldPosition;

                            Portals.Add (portalEntity);
                        }
                    }

                    foreach (var node in maze[1].graph.Nodes)
                    {
                        if (node.Data.IsPortal)
                        {
                            var portalEmitter = new PortalParticles ();
                            var particleSceneObject = new ParticleSceneObject (portalEmitter.ParticleCount);
                            particleSceneObject.Priority = 7000;
                            game.GetGameState ("maze_underworld").Scene.AddObject (particleSceneObject); 

                            portalEmitter.Init (particleSceneObject, Application.Instance.RendererContext);

                            var portalEntity = EntityFactory.Instance.CreateWith ("PortalEmitter " + DateTime.Now.Ticks, 
                                                   game.GetGameState ("maze_underworld").MessageProxy, systems: new[] { typeof(ParticleSystem) });

                            portalEntity.GetComponent<ParticleComponent> ().Emitter = portalEmitter;
                            portalEntity.GetComponent<ParticleComponent> ().Particle = particleSceneObject;

                            portalEntity.GetComponent<TransformComponent> ().Position = node.Data.WorldPosition;

                            Portals.Add (portalEntity);
                        }
                    }

                    finishedLoading = true;

                    application.Window.CaptureMouse ();

                    loadingScreen.Ready ();

                    if (game.CurrentGameState.Name != "maze_overworld")
                    {
                        game.SwitchToGameState ("maze_overworld");

                        game.CurrentGameState.Scene.Active = true;
                    }

                    backgroundMusic.Play ();
                    FurryLana.MainMenuMusic.Stop ();
                }

                if (!finishedLoading)
                    loadingScreen.BringToFront ();

                if (game.CurrentGameState == game.GetGameState ("maze_overworld") && maze [0].HasFinished)
                    game.CurrentGameState.PhysicsManager.Update (um.TimeStamp);

                if (game.CurrentGameState == game.GetGameState ("maze_underworld") && maze [1].HasFinished)
                    game.CurrentGameState.PhysicsManager.Update (um.TimeStamp);

                if (application.FPSCounter >= 50)
                    FPS_Text.TextColor = System.Drawing.Color.Green;
                else
                if (application.FPSCounter < 50 && application.FPSCounter > 30)
                    FPS_Text.TextColor = System.Drawing.Color.Yellow;
                else
                    FPS_Text.TextColor = System.Drawing.Color.Red;

                FPS_Text.String = application.FPSCounter + " FPS";

                if (HealthOverlayNode.Factor > 0.0001f)
                {
                    var factor = HealthOverlayNode.Factor - 0.01f;
                    HealthOverlayNode.Factor = factor < 0 ? 0 : factor;
                }

                if (switch_maze)
                {
                    if (entered_portal && ColorCorrectionNode.Contrast > 0.0f)
                    {
                        //ColorCorrectionNode.Brightness += (float) um.TimeStamp.TotalSeconds * 0.8f;
                        ColorCorrectionNode.Contrast -= (float) um.TimeStamp.TotalSeconds * 0.3f;
                        WarpingNode.WarpFactor = (1 - ColorCorrectionNode.Contrast) * 0.5f;
                    }
                    else if (entered_portal && ColorCorrectionNode.Contrast <= 0.0f)
                    {
                        SwitchMaze ();
                        entered_portal = false;
                    }
                    else if (!entered_portal && ColorCorrectionNode.Contrast < 1.0f)
                    {
                        //ColorCorrectionNode.Brightness -= (float) um.TimeStamp.TotalSeconds * 0.8f;
                        ColorCorrectionNode.Contrast += (float) um.TimeStamp.TotalSeconds * 0.3f;
                        WarpingNode.WarpFactor = (1 - ColorCorrectionNode.Contrast) * 0.5f;
                    }
                    else
                    {
                        ColorCorrectionNode.Contrast = 1.0f;
                        switch_maze = false;
                        Player.GetComponent<PhysicsComponent>().IsMoveable = true;

                        //WarpingNode.WarpTexture = DefaultWarpingTexture;
                        WarpingNode.WarpFactor = 0;
                    }
                }

                if (Player.GetComponent<TransformComponent> ().Position.Y <= -10.0f && 
                    finishedLoading && game.CurrentGameState.Name != "MazeLoadingScreen")
                {
                    Player.GetComponent<HealthComponent> ().Health = 0.0f;
                }
            }

            if (msg.MessageId == (int) MessageId.CollisionDetected)
            {
                CollisionDetectedMessage cdm = msg as CollisionDetectedMessage;

                if (cdm.Body1.Tag == null)
                {
                    if (cdm.Body2.Tag == null)
                        return;

                    MazeCell mc = cdm.Body2.Tag as MazeCell;
                    if (mc == null)
                        return;

                    if (mc.IsPortal)
                    {
                        if (!switch_maze)
                        {
                            entered_portal = true;
                            switch_maze = true;
                            Player.GetComponent<PhysicsComponent>().IsMoveable = false;
                            switchMazeSound.Play ();
                        }
                    }
                }
                else if (cdm.Body2.Tag == null)
                {
                    if (cdm.Body1.Tag == null)
                        return;

                    MazeCell mc = cdm.Body1.Tag as MazeCell;
                    if (mc == null)
                        return;

                    if (mc.IsPortal)
                    {
                        if (!switch_maze)
                        {
                            entered_portal = true;
                            switch_maze = true;
                            Player.GetComponent<PhysicsComponent>().IsMoveable = false;
                            switchMazeSound.Play ();
                        }
                    }
                }
            }

            if (msg.MessageId == (int) MessageId.HealthChanged && finishedLoading)
            {
                var hcm = msg as HealthChangedMessage;

                if (hcm.Entity == Player)
                {
                    if (hcm.HealthDelta < 0)
                    {
                        var factor = -hcm.HealthDelta / 20;
                        HealthOverlayNode.Factor = factor > 1 ? 1 : factor;

                        if (playerDamagedSound.GetState () != SourceState.Playing)
                            playerDamagedSound.Play ();
                    }

                    var healthComponent = Player.GetComponent<HealthComponent> ();
                    var health = healthComponent.Health > 0 ? healthComponent.Health : 0;
                    ColorCorrectionNode.Saturation = -((healthComponent.MaximumHealth - health) / (healthComponent.MaximumHealth)) / 4;

                    if (hcm.Health <= 0.0f && game.CurrentGameState.Name != "MazeLoadingScreen")
                    {
                        WarpingNode.Stop ();
                        endScreen.State.Scene = game.CurrentGameState.Scene;

                        game.SwitchToGameState ("endscreen_state");

                        if (playerDiedSound.GetState () != SourceState.Playing)
                            playerDiedSound.Play ();

                        if (MessageCreated != null)
                            MessageCreated (new GameEndedDiedMessage ());
                    }
                }
            }

            var im = msg as InputMessage;
            if (im != null)
            {
                #if RELEASE
                if (im.IsActionPressed ("frame"))
                {
                    if (FPS_Text.IsHidden)
                        FPS_Text.Show();
                    else
                        FPS_Text.Hide();
                }
                #else
                if (im.IsActionPressed ("drop"))
                    application.Window.CaptureMouse ();

                if (im.IsActionPressed ("frame"))
                {
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
                    }
                }

                if (im.IsActionPressed ("damage"))
                {
                    SwitchMaze ();
                }
                #endif
            }
           
            if (msg.MessageId == (int) MessageId.StaminaChanged && finishedLoading)
            {
                StaminaChangedMessage scm = msg as StaminaChangedMessage;

                if (scm.Stamina <= 0.1f)
                {
                    if (playerNoStamina.GetState () != SourceState.Playing)
                        playerNoStamina.Play ();
                }
            }

            if (msg.MessageId == (int) MessageId.ItemUse)
            {
                ItemUseMessage ium = msg as ItemUseMessage;
                if (ium.Usage.HasFlag (ItemUsage.Eatable) && ium.Item.ItemUsages.HasFlag (ItemUsage.Eatable))
                {
                    if (ium.Item.Entity.Name.Contains ("choco_milk") || ium.Item.Entity.Name.Contains ("soda_can") ||
                                           ium.Item.Entity.Name.Contains ("mate"))
                    {
                        if (playerDrinked.GetState () != SourceState.Playing)
                            playerDrinked.Play ();
                    }
                    else
                    if (ium.Item.Entity.Name.Contains ("apple") || ium.Item.Entity.Name.Contains ("toast"))
                    {
                        if (playerEaten.GetState () != SourceState.Playing)
                            playerEaten.Play ();
                    }
                }
            }

            if (msg.MessageId == (int) MessageId.FlashlightToggled)
            {
                playerFlashlightTrigger.Play ();
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
