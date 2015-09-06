//
//  RoachGroupSystem.cs
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
using FreezingArcher.Content;
using FreezingArcher.Math;

namespace FreezingArcher.Game.Ghosts
{
    public sealed class RoachGroupSystem : EntitySystem
    {
        public override void Init (FreezingArcher.Messaging.MessageProvider messageProvider, Entity entity)
        {
            base.Init (messageProvider, entity);

            NeededComponents = new[] { typeof (TransformComponent), typeof (RoachGroupComponent) };

            onPositionChangedHandler = pos => {
                var roachGroup = Entity.GetComponent<RoachGroupComponent>();
                if (roachGroup != null)
                    roachGroup.RoachGroup.Position = pos;
            };
        }

        Action<Vector3> onPositionChangedHandler;

        public override void PostInit ()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged += onPositionChangedHandler;
        }

        public override void ConsumeMessage (FreezingArcher.Messaging.Interfaces.IMessage msg)
        {
        }

        public override void Destroy ()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            transform.OnPositionChanged -= onPositionChangedHandler;
            base.Destroy ();
        }
    }
}
