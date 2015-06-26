//
//  ECSTest.cs
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
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Messaging;
using FreezingArcher.Math;
using FreezingArcher.Content;
using FreezingArcher.Renderer.Scene.SceneObjects;

namespace FreezingArcher.Game
{
    public class ECSTest
    {
        public ECSTest (MessageManager msgmnr, CoreScene scene)
        {
            scene.CameraManager.AddCam (
                new FirstPersonCamera ("Player", msgmnr, default(Vector3), default(Vector3), 0.1f, 200));
            BaseCamera test = scene.CameraManager.GetCam ("Player");
            scene.CameraManager.ActiveCamera = test;

            //Skybox
            skybox = EntityFactory.Instance.CreateWith ("skybox", systems: new[] { typeof(ModelSystem) });
            ModelSceneObject skyboxModel = new ModelSceneObject ("lib/Renderer/TestGraphics/Skybox/skybox.xml");
            skybox.GetComponent<TransformComponent>().Scale = 100.0f * Vector3.One;
            scene.AddObject (skyboxModel);
            skyboxModel.WaitTillInitialized ();
            skyboxModel.Model.EnableDepthTest = false;
            skyboxModel.Model.EnableLighting = false;
            skybox.GetComponent<ModelComponent> ().Model = skyboxModel;

            player = EntityFactory.Instance.CreateWith ("player",
                new[] { typeof (TransformComponent) },
                new[] { typeof (MovementSystem), typeof (KeyboardControllerSystem), typeof (MouseControllerSystem), typeof (ModelSystem) });

            ModelSceneObject cube = new ModelSceneObject ("lib/Renderer/TestGraphics/Wall/wall.xml");
            scene.AddObject(cube);
            cube.WaitTillInitialized();
            player.GetComponent<ModelComponent>().Model = cube;
        }

        Entity player;

        Entity skybox;

        CoreScene scene;
    }
}
