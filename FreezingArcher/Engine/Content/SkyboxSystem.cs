//
//  SkyboxSystem.cs
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
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Math;
using FreezingArcher.Renderer.Scene;

namespace FreezingArcher.Content
{
    public sealed class SkyboxSystem : EntitySystem
    {
        /// <summary>
        /// Initialize this system. This may be used as a constructor replacement.
        /// </summary>
        /// <param name="messageProvider">The message provider for this instance.</param>
        /// <param name="entity">Entity.</param>
        public override void Init(MessageProvider messageProvider, Entity entity)
        {
            base.Init(messageProvider, entity);

            //Added needed components
            NeededComponents = new[] { typeof(TransformComponent), typeof(SkyboxComponent) };

            //Needs more Initializing?
            //Scene does this for me, so no!
            internalValidMessages = new[] { (int)MessageId.PositionChanged };
            messageProvider += this;
        }

        /// <summary>
        /// Creates the skybox.
        /// </summary>
        /// <param name="scene">Scene.</param>
        /// <param name="path">Path to skybox xml.</param>
        public static void CreateSkybox(CoreScene scene, Entity player,
            string path = "lib/Renderer/TestGraphics/Skybox/skybox.xml")
        {
            ModelSceneObject skyboxModel = new ModelSceneObject (path);
            skyboxModel.Priority = 0;
            skyboxModel.Scaling = 100.0f * Vector3.One;
            scene.AddObject (skyboxModel);
            skyboxModel.Model.EnableDepthTest = false;
            skyboxModel.Model.EnableLighting = false;
            player.GetComponent<SkyboxComponent> ().Skybox = skyboxModel;
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            SkyboxComponent sc = Entity.GetComponent<SkyboxComponent>();
            TransformComponent tc = Entity.GetComponent<TransformComponent>();

            if (sc == null || sc.Skybox == null)
                return;

            if (msg.MessageId == (int)MessageId.PositionChanged)
            {
                PositionChangedMessage pcm = msg as PositionChangedMessage;
                if (pcm.Entity.Name == Entity.Name)
                    sc.Skybox.Position = tc.Position;
            }
        }
    }
}
