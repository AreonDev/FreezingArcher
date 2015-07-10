//
//  PhysicsTest.cs
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
using Henge3D;
using Henge3D.Physics;
using FreezingArcher.Renderer;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Output;
using FreezingArcher.Content;
using FreezingArcher.Core;
using System.Collections.Generic;

namespace FreezingArcher.Game
{
    /// <summary>
    /// Physics test.
    /// </summary>
    public class PhysicsTest : IMessageConsumer
    {
        List<Entity> grounds;
        List<Entity> walls;

        Entity wall_to_throw;

        GameState state;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.PhysicsTest"/> class.
        /// </summary>
        /// <param name="app">App.</param>
        public PhysicsTest(Application application, MessageProvider messageProvider)
        {
            application.Game.AddGameState ("PhysicsScene", Content.Environment.Default);
            state = application.Game.GetGameState ("PhysicsScene");

            state.Scene = new CoreScene (application.RendererContext, state.MessageProxy);
            state.Scene.BackgroundColor = Color4.Aqua;

            state.PhysicsManager.Gravity = new Vector3 (0.0f, -1.0f, 0.0f);

            Entity player = EntityFactory.Instance.CreateWith ("player", state.MessageProxy, systems: new [] {
                typeof(MovementSystem),
                typeof(KeyboardControllerSystem),
                typeof(MouseControllerSystem),
                typeof(SkyboxSystem)});

            state.Scene.CameraManager.AddCamera (new BaseCamera (player, state.MessageProxy), "playerCamera");

            SkyboxSystem.CreateSkybox (state.Scene, player);

            grounds = new List<Entity> ();
            walls = new List<Entity> ();

            application.Game.SwitchToGameState ("PhysicsScene");

            ValidMessages = new[] { (int)MessageId.Input, (int)MessageId.Update };
            messageProvider += this;

//            Entity wall = EntityFactory.Instance.CreateWith("wall", messageProvider, null,
//                new[] { typeof (ModelSystem), typeof (PhysicsSystem) });
//
//            var wallModel = new ModelSceneObject("lib/Renderer/TestGraphics/Wall/wall.xml");
//            wall.GetComponent<ModelComponent>().Model = wallModel;
//            app.RendererContext.Scene.AddObject(wallModel);
//
//            Vector3 centerofmass;
//
//            var wallRigidBody = new RigidBody();
//            wall.GetComponent<PhysicsComponent>().RigidBody = wallRigidBody;
//            Vector3 p1 = new Vector3(0f, 0f, 1), p2 = new Vector3(0f, 0f, -1);
//            wallRigidBody.MassProperties = MassProperties.FromTriMesh (50.0f, wallModel.Model.Meshes [0].Vertices,
//                wallModel.Model.Meshes [0].Indices, out centerofmass);
//            wallRigidBody.Freeze ();
//
//            Henge3D.Pipeline.CompiledMesh bla = new Henge3D.Pipeline.CompiledMesh (wallModel.Model.Meshes [0].Vertices, 
//                wallModel.Model.Meshes [0].Indices);
//
//            wallRigidBody.Skin.Add(new MeshPart(bla), new Henge3D.Physics.Material(0.0f, 0.000000001f));
//            wallRigidBody.SetWorld(new Vector3(0, 0, 0));
//
//            Entity ground = EntityFactory.Instance.CreateWith("ground", messageProvider, null,
//                new[] { typeof (ModelSystem), typeof (PhysicsSystem) });
//
//            var groundModel = new ModelSceneObject("lib/Renderer/TestGraphics/Ground/ground.xml");
//            ground.GetComponent<ModelComponent>().Model = groundModel;
//            app.RendererContext.Scene.AddObject(groundModel);
//
//            groundModel.WaitTillInitialized ();
//
//            groundRigidBody = new RigidBody();
//            ground.GetComponent<PhysicsComponent>().RigidBody = groundRigidBody;
//            groundRigidBody.MassProperties = new MassProperties (0.001f, Matrix.Identity);
//            //groundRigidBody.Freeze ();
//            groundRigidBody.Skin.DefaultMaterial = new Henge3D.Physics.Material(1f, 0.1f);
//            groundRigidBody.Skin.Add(new MeshPart(groundModel.Model.Meshes [0].Vertices, groundModel.Model.Meshes [0].Indices));
//            groundRigidBody.SetWorld (new Vector3 (0, 10, 0));
//
//            //app.RendererContext.Scene.CameraManager.ActiveCamera.MoveTo(new Vector3(-1, 1, 0));
//
//            app.Game.CurrentGameState.PhysicsManager.Add(groundRigidBody);
//            app.Game.CurrentGameState.PhysicsManager.Add(wallRigidBody);
        }

//        RigidBody wallRigidBody;
//        RigidBody groundRigidBody;

        void InitializeTest()
        {
            //Init grounds
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    Entity ground = EntityFactory.Instance.CreateWith ("ground." + i + "." + j, state.MessageProxy, null,
                                        new[] { typeof(ModelSystem), typeof(PhysicsSystem) });

                    var groundModel = new ModelSceneObject ("lib/Renderer/TestGraphics/Ground/ground.xml");
                    state.Scene.AddObject (groundModel);

                    ground.GetComponent<ModelComponent> ().Model = groundModel;

                    var tc = ground.GetComponent<TransformComponent> ();
                    tc.Position = new Vector3 (i * 2, 0, j * 2);

                    grounds.Add (ground);

                    //Physics
                    Vector3 centerofmass;

                    var groundRigidBody = new RigidBody ();
                    ground.GetComponent<PhysicsComponent> ().RigidBody = groundRigidBody;

                    groundRigidBody.MassProperties = MassProperties.Immovable/*MassProperties.FromTriMesh (100.0f, groundModel.Model.Meshes [0].Vertices,
                        groundModel.Model.Meshes [0].Indices, out centerofmass)*/;

                    Henge3D.Pipeline.CompiledMesh bla = new Henge3D.Pipeline.CompiledMesh (groundModel.Model.Meshes [0].Vertices,
                                                            groundModel.Model.Meshes [0].Indices);

                    groundRigidBody.Skin.Add (new MeshPart (groundModel.Model.Meshes [0].Vertices,
                        groundModel.Model.Meshes [0].Indices), new Henge3D.Physics.Material (0.0f, 0.001f));
                    groundRigidBody.SetWorld (tc.Position);
                   
                    state.PhysicsManager.Add (groundRigidBody);
                }
            }

            //Init walls
            for (int i = 0; i < 10; i++) {
                Entity wall = EntityFactory.Instance.CreateWith ("walls." + i, state.MessageProxy, null,
                                  new[] { typeof(ModelSystem), typeof(PhysicsSystem) });

                var wallModel = new ModelSceneObject ("lib/Renderer/TestGraphics/Wall/wall.xml");
                state.Scene.AddObject (wallModel);

                wall.GetComponent<ModelComponent> ().Model = wallModel;

                var tc = wall.GetComponent<TransformComponent> ();
                tc.Position = new Vector3 (i * 2, 0.0f, 20.0f);

                walls.Add (wall);

                //Physics
                Vector3 centerofmass;

                var wallRigidBody = new RigidBody ();
                wall.GetComponent<PhysicsComponent> ().RigidBody = wallRigidBody;

                wallRigidBody.MassProperties = MassProperties.Immovable/*MassProperties.FromTriMesh (50.0f, wallModel.Model.Meshes [0].Vertices,
                    wallModel.Model.Meshes [0].Indices, out centerofmass)*/;
                //wallRigidBody.Freeze ();
                    
                Henge3D.Pipeline.CompiledMesh bla = new Henge3D.Pipeline.CompiledMesh (wallModel.Model.Meshes [0].Vertices, 
                                                                        wallModel.Model.Meshes [0].Indices);
                    
                wallRigidBody.Skin.Add (new MeshPart (wallModel.Model.Meshes [0].Vertices,
                    wallModel.Model.Meshes [0].Indices), new Henge3D.Physics.Material (0.0f, 0.0001f));
                wallRigidBody.SetWorld (tc.Position);
    
                state.PhysicsManager.Add (wallRigidBody);
            }

            InitStupidWall ();
        }
           
        private void InitStupidWall()
        {
            wall_to_throw = EntityFactory.Instance.CreateWith ("wall_throw", state.MessageProxy, null,
                new[] { typeof(ModelSystem), typeof(PhysicsSystem) });

            var wallModel2 = new ModelSceneObject("lib/Renderer/TestGraphics/Wall/wall.xml");
            state.Scene.AddObject (wallModel2);

            wall_to_throw.GetComponent<ModelComponent> ().Model = wallModel2;

            var tc2 = wall_to_throw.GetComponent<TransformComponent>();
            tc2.Position = new Vector3 (10.0f, 20.0f, 10.0f);


            //Physics
            Vector3 centerofmass;

            var wallRigidBody = new RigidBody ();
            wall_to_throw.GetComponent<PhysicsComponent> ().RigidBody = wallRigidBody;

            wallRigidBody.MassProperties = MassProperties.FromTriMesh (50.0f, wallModel2.Model.Meshes [0].Vertices,
                wallModel2.Model.Meshes [0].Indices, out centerofmass);
            //wallRigidBody.Freeze ();

            /*
            Henge3D.Pipeline.CompiledMesh bla = new Henge3D.Pipeline.CompiledMesh (wallModel2.Model.Meshes [0].Vertices, 
                wallModel2.Model.Meshes [0].Indices);*/

            var hull = new ConvexHull3D (wallModel2.Model.Meshes [0].Vertices);
            CompiledPolyhedron meshPart = hull.ToPolyhedron ();

            wallRigidBody.Skin.Add (new PolyhedronPart(meshPart), new Henge3D.Physics.Material (0.0f, 0.0001f));
            wallRigidBody.SetWorld (tc2.Position);
            //wallRigidBody.Freeze ();

            state.PhysicsManager.Add (wallRigidBody);
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.Update) 
            {
                var um = msg as UpdateMessage;

                state.PhysicsManager.Update ((float)um.TimeStamp.TotalMilliseconds);
            }

            if (msg.MessageId == (int)MessageId.Input)
            {
                var im = msg as InputMessage;

                if (im.IsActionPressed ("jump"))
                    InitializeTest ();
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }
}
