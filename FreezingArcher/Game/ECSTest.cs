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
using FreezingArcher.Content;
using FreezingArcher.Math;
using FreezingArcher.Messaging;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;

namespace FreezingArcher.Game
{
    /// <summary>
    /// Entity Component System Tests.
    /// </summary>
    public class ECSTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.ECSTest"/> class.
        /// </summary>
        /// <param name="messageProvider">The message provider for this instance.</param>
        /// <param name="scene">Scene.</param>
        public ECSTest (MessageProvider messageProvider, CoreScene scene)
        {
            Entity player = EntityFactory.Instance.CreateWith ("player", messageProvider, systems: new[] {
                typeof (MovementSystem),
                typeof (KeyboardControllerSystem),
                typeof (MouseControllerSystem)
            });

            scene.CameraManager.AddCamera (new BaseCamera (player, messageProvider), "player");

            Entity skybox = EntityFactory.Instance.CreateWith ("skybox", messageProvider, systems: new[] { typeof(ModelSystem) });
            ModelSceneObject skyboxModel = new ModelSceneObject ("lib/Renderer/TestGraphics/Skybox/skybox.xml");
            skybox.GetComponent<TransformComponent>().Scale = 100.0f * Vector3.One;
            scene.AddObject (skyboxModel);
            skyboxModel.WaitTillInitialized ();
            skyboxModel.Model.EnableDepthTest = false;
            skyboxModel.Model.EnableLighting = false;
            skybox.GetComponent<ModelComponent> ().Model = skyboxModel;
            // as we do not update skybox position the player is able to leave the skybox ... i dont wanna fix it cause its just a test...
        }
    }
}
