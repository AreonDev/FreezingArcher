//
//  LightSystem.cs
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
using FreezingArcher.Core;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Math;

namespace FreezingArcher.Content
{
    public sealed class LightSystem : EntitySystem
    {
        public override void Init (MessageProvider messageProvider, Entity entity)
        {
            base.Init (messageProvider, entity);

            NeededComponents = new[] { typeof (TransformComponent), typeof (LightComponent), typeof (ItemComponent) };

            onPositionChangedHandler = pos => {
                var light = Entity.GetComponent<LightComponent>().Light;
                if (light != null)
                {
                    light.PointLightPosition = pos + 
                        ((light.Type == FreezingArcher.Renderer.Scene.LightType.SpotLight) ? light.DirectionalLightDirection * 0.19f : 
                            Vector3.Zero);
                }
            };

            onRotationChangedHandler = rot => {
                var light = Entity.GetComponent<LightComponent>().Light;
                if (light != null)
                {
                    light.DirectionalLightDirection = Vector3.Normalize(Vector3.Transform (Vector3.UnitY, rot));        
                }
            };

            internalValidMessages = new[] { (int) MessageId.Update };
            messageProvider += this;
        }

        Action<Vector3> onPositionChangedHandler;

        Action<Quaternion> onRotationChangedHandler;

        public override void PostInit ()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged += onPositionChangedHandler;
            transform.OnRotationChanged += onRotationChangedHandler;
        }

        public override void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Update && Entity != null && Entity.GetComponent<LightComponent>() != null)
            {
                var light = Entity.GetComponent<LightComponent>().Light;
                var item = Entity.GetComponent<ItemComponent> ();

                if (light != null)
                {
                    if (light.On)
                    {
                        if (item != null)
                            item.Usage += item.UsageDeltaPerUsage;
                        //else
                        //    light.PointLightPosition = Entity.GetComponent<PhysicsComponent> ().RigidBody.Position.ToFreezingArcherVector();
                    }
                }
            }
        }

        public override void Destroy ()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged -= onPositionChangedHandler;
            transform.OnRotationChanged -= onRotationChangedHandler;
            var light = Entity.GetComponent<LightComponent>().Light;
            light.On = false;
            light.Destroy();
            base.Destroy ();
        }
    }
}
