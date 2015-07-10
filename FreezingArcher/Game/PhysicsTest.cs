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

namespace FreezingArcher.Game
{
    /// <summary>
    /// Physics test.
    /// </summary>
    public class PhysicsTest : IMessageConsumer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.PhysicsTest"/> class.
        /// </summary>
        /// <param name="app">App.</param>
        public PhysicsTest(Application app, MessageProvider messageProvider)
        {
            Entity wall = EntityFactory.Instance.CreateWith("wall", messageProvider, null,
                new[] { typeof (ModelSystem), typeof (PhysicsSystem) });

            var wallModel = new ModelSceneObject("lib/Renderer/TestGraphics/Wall/wall.xml");
            wall.GetComponent<ModelComponent>().Model = wallModel;
            app.RendererContext.Scene.AddObject(wallModel);

            Vector3 centerofmass;

            var wallRigidBody = new RigidBody();
            wall.GetComponent<PhysicsComponent>().RigidBody = wallRigidBody;
            Vector3 p1 = new Vector3(0f, 0f, 1), p2 = new Vector3(0f, 0f, -1);
            wallRigidBody.MassProperties = MassProperties.FromTriMesh (50.0f, wallModel.Model.Meshes [0].Vertices,
                wallModel.Model.Meshes [0].Indices, out centerofmass);
            wallRigidBody.Freeze ();

            Henge3D.Pipeline.CompiledMesh bla = new Henge3D.Pipeline.CompiledMesh (wallModel.Model.Meshes [0].Vertices, 
                wallModel.Model.Meshes [0].Indices);

            wallRigidBody.Skin.Add(new MeshPart(bla), new Henge3D.Physics.Material(0.0f, 0.000000001f));
            wallRigidBody.SetWorld(new Vector3(0, 0, 0));

            Entity ground = EntityFactory.Instance.CreateWith("ground", messageProvider, null,
                new[] { typeof (ModelSystem), typeof (PhysicsSystem) });

            var groundModel = new ModelSceneObject("lib/Renderer/TestGraphics/Ground/ground.xml");
            ground.GetComponent<ModelComponent>().Model = groundModel;
            app.RendererContext.Scene.AddObject(groundModel);

            groundModel.WaitTillInitialized ();

            groundRigidBody = new RigidBody();
            ground.GetComponent<PhysicsComponent>().RigidBody = groundRigidBody;
            groundRigidBody.MassProperties = new MassProperties (0.001f, Matrix.Identity);
            //groundRigidBody.Freeze ();
            groundRigidBody.Skin.DefaultMaterial = new Henge3D.Physics.Material(1f, 0.1f);
            groundRigidBody.Skin.Add(new MeshPart(groundModel.Model.Meshes [0].Vertices, groundModel.Model.Meshes [0].Indices));
            groundRigidBody.SetWorld (new Vector3 (0, 10, 0));

            //app.RendererContext.Scene.CameraManager.ActiveCamera.MoveTo(new Vector3(-1, 1, 0));

            app.Game.CurrentGameState.PhysicsManager.Add(groundRigidBody);
            app.Game.CurrentGameState.PhysicsManager.Add(wallRigidBody);
        }

        RigidBody wallRigidBody;
        RigidBody groundRigidBody;

        #region IMessageConsumer implementation

        public void ConsumeMessage (IMessage msg)
        {
            UpdateMessage um = msg as UpdateMessage;
            if (um != null) 
            {

            }
        }

        public int[] ValidMessages {
            get {
                return new[] { (int)MessageId.Update };
            }
        }

        #endregion
    }
}
