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

            NeededComponents = new[] { typeof (TransformComponent), typeof (LightComponent) };

            onPositionChangedHandler = pos => {
                var light = Entity.GetComponent<LightComponent>().Light;
                if (light != null)
                {
                    light.PointLightPosition = pos + light.DirectionalLightDirection * 0.20f;
                }
            };

            onRotationChangedHandler = rot => {
                var light = Entity.GetComponent<LightComponent>().Light;
                if (light != null)
                {
                    light.DirectionalLightDirection = Vector3.Normalize(Vector3.Transform (Vector3.UnitY, rot));        
                }
            };

            internalValidMessages = new int[0];
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
        {}

        public override void Destroy ()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged -= onPositionChangedHandler;
            transform.OnRotationChanged -= onRotationChangedHandler;
            base.Destroy ();
        }
    }
}
