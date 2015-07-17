//
//  JitterTest.cs
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
using System.Linq;
using System.Collections.Generic;
using FreezingArcher.Content;
using FreezingArcher.Core;
using FreezingArcher.Math;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using System.Threading;

namespace FreezingArcher.Game
{
    public class JitterTest : IMessageConsumer
    {
        GameState state;
        List<Entity> grounds;
        List<Entity> walls;
        Entity wall_to_throw;
        Entity player;
        LoadingScreen loadingScreen;
        Application application;

        public JitterTest (Application application)
        {
            this.application = application;
            application.Game.AddGameState ("PhysicsScene", Content.Environment.Default);
            state = application.Game.GetGameState ("PhysicsScene");
            state.MessageProxy.StartProcessing();

            state.Scene = new CoreScene (application.RendererContext, state.MessageProxy);
            state.Scene.BackgroundColor = Color4.Aqua;

            loadingScreen = new LoadingScreen(application, application.MessageManager, "loading.png",
                "JitterTestLoadingScreen",
                to: new[] {new Tuple<string, GameStateTransition>(state.Name, new GameStateTransition(0))});

            player = EntityFactory.Instance.CreateWith ("player", state.MessageProxy,
                systems: new [] {
                typeof(MovementSystem),
                typeof(KeyboardControllerSystem),
                typeof(MouseControllerSystem),
                typeof(SkyboxSystem),
                typeof(PhysicsSystem)
            });

            state.Scene.CameraManager.AddCamera (new BaseCamera (player, state.MessageProxy), "playerCamera");

            SkyboxSystem.CreateSkybox (state.Scene, player);

            grounds = new List<Entity> ();
            walls = new List<Entity> ();

            ValidMessages = new[] { (int)MessageId.Running, (int)MessageId.Update, (int)MessageId.Input };
            application.MessageManager += this;

            RigidBody playerBody = new RigidBody(new CylinderShape(1.0f, 0.5f));
            playerBody.Position = new JVector (3.0f, 1.0f, 2.0f);
            playerBody.AllowDeactivation = false;
            playerBody.Material.StaticFriction = 0f;
            playerBody.Material.KineticFriction = 0f;
            playerBody.Material.Restitution = 0.1f;
            //playerBody.Mass = 1000000.0f;
            playerBody.Update ();
            playerBody.IsActive = false;
            player.GetComponent<PhysicsComponent>().RigidBody = playerBody;
            player.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody(playerBody);
        }

        void InitializeTest()
        {
            //Init grounds
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    Entity ground = EntityFactory.Instance.CreateWith ("ground." + i + "." + j, state.MessageProxy, null,
                        new[] { typeof(ModelSystem)});

                    var groundModel = new ModelSceneObject ("lib/Renderer/TestGraphics/Ground/ground.xml");
                    state.Scene.AddObject (groundModel);

                    ground.GetComponent<ModelComponent> ().Model = groundModel;

                    var tc = ground.GetComponent<TransformComponent> ();
                    tc.Position = new Vector3 (i * 2, 0, j * 2);

                    grounds.Add (ground);
                }
            }

            //Init Ground plane

            Entity groundplane = EntityFactory.Instance.CreateWith ("ground_plane", state.MessageProxy, null,
                                     new[] { typeof(PhysicsSystem) });

            groundplane.GetComponent<TransformComponent> ().Position = Vector3.Zero;

            // TODO add to physics
            var bodyplane = new RigidBody(new BoxShape(1000,0.01f,1000));
            bodyplane.Position = groundplane.GetComponent<TransformComponent> ().Position.ToJitterVector();
            state.PhysicsManager.World.AddBody(bodyplane);
            bodyplane.IsStatic = true;
            groundplane.GetComponent<PhysicsComponent>().RigidBody = bodyplane;

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

                // TODO add to physics

                List<JVector> vertices = new List<JVector>();
                wallModel.Model.Meshes[0].Vertices.ForEach(x => (vertices.Add(x.ToJitterVector())));

                List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();

                for(int j = 0; j < wallModel.Model.Meshes[0].Indices.Length; j+= 3)
                {
                    int i0 = wallModel.Model.Meshes[0].Indices[j+0];
                    int i1 = wallModel.Model.Meshes[0].Indices[j+1];
                    int i2 = wallModel.Model.Meshes[0].Indices[j+2];

                    indices.Add(new TriangleVertexIndices(i0, i1, i2));
                }

                //var body = new RigidBody(new TriangleMeshShape(new Octree(vertices, indices)));
                var body = new RigidBody(new BoxShape(2, 4, 2));
                body.Material.KineticFriction = 10.01f;
                body.Material.StaticFriction = 10.01f;
                body.Position = tc.Position.ToJitterVector () + JVector.Up*2;
                state.PhysicsManager.World.AddBody(body);
                body.IsStatic = true;
                wall.GetComponent<PhysicsComponent>().RigidBody = body;
            }

            InitStupidWall ();

            player.GetComponent<PhysicsComponent>().RigidBody.IsActive = true;
            application.Game.SwitchToGameState ("PhysicsScene");
        }

        void InitStupidWall()
        {
            wall_to_throw = EntityFactory.Instance.CreateWith ("wall_throw", state.MessageProxy, null,
                new[] { typeof(ModelSystem), typeof(PhysicsSystem) });

            var wallModel2 = new ModelSceneObject("lib/Renderer/TestGraphics/Monkey/monkey.xml");
            state.Scene.AddObject (wallModel2);

            wall_to_throw.GetComponent<ModelComponent> ().Model = wallModel2;

            var tc2 = wall_to_throw.GetComponent<TransformComponent>();
            tc2.Position = new Vector3 (10.0f, 20.0f, 10.0f);
            tc2.Rotation = Quaternion.FromAxisAngle(new Vector3(1,1,0), MathHelper.PiOver4);

            // TODO add to physics
            /*
            var body = new RigidBody(new BoxShape(2,4,2));
            body.Position = new JVector(tc2.Position.X, tc2.Position.Y, tc2.Position.Z);
            body.Orientation = JMatrix.CreateFromAxisAngle(new JVector(1,1,0), MathHelper.PiOver4);
            world.AddBody(body);
            wall_to_throw.GetComponent<JitterComponent>().RigidBody = body;*/

            List<JVector> vertices = new List<JVector>();
            wallModel2.Model.Meshes[0].Vertices.ForEach(x => (vertices.Add(x.ToJitterVector())));
       
            List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();

            for(int i = 0; i < wallModel2.Model.Meshes[0].Indices.Length; i+= 3)
            {
                int i0 = wallModel2.Model.Meshes[0].Indices[i+0];
                int i1 = wallModel2.Model.Meshes[0].Indices[i+1];
                int i2 = wallModel2.Model.Meshes[0].Indices[i+2];

                indices.Add(new TriangleVertexIndices(i0, i1, i2));
            }

            var triangleMeshShape = new TriangleMeshShape(new Octree(vertices, indices));
          
            var body = new RigidBody(triangleMeshShape);
            body.Position = tc2.Position.ToJitterVector ();
            body.Orientation = JMatrix.CreateFromAxisAngle(new JVector(1,1,0), MathHelper.PiOver4);
            body.AllowDeactivation = false;

            state.PhysicsManager.World.AddBody(body);
            wall_to_throw.GetComponent<PhysicsComponent>().RigidBody = body;
            wall_to_throw.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Orientation | AffectedByPhysics.Position;
        }

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int)MessageId.Update) 
            {
                var um = msg as UpdateMessage;

                state.PhysicsManager.Update(um.TimeStamp);
            }

            if (msg.MessageId == (int)MessageId.Input) 
            {
                var im = msg as InputMessage;

                if (im.Keys.Any(k => k.Action == Pencil.Gaming.KeyAction.Press && k.KeyAction == "jump"))
                    player.GetComponent<PhysicsComponent> ().RigidBody.AddForce (JVector.Up * 250);
            }

            if (msg.MessageId == (int)MessageId.Running)
            {
                application.Game.SwitchToGameState("JitterTestLoadingScreen");
                new Thread(InitializeTest).Start();
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }
}
