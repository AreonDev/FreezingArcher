//
//  ItemSystem.cs
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
using FreezingArcher.Core;
using FreezingArcher.Math;
using Jitter.LinearMath;
using Jitter.Dynamics;
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    /// <summary>
    /// This system handles item usage on an item entity.
    /// </summary>
    public sealed class ItemSystem : EntitySystem
    {
        /// <summary>
        /// Initialize this system. This may be used as a constructor replacement.
        /// </summary>
        /// <param name="messageProvider">The message provider for this system instance.</param>
        /// <param name="entity">Entity.</param>
        public override void Init(MessageProvider messageProvider, Entity entity)
        {
            base.Init(messageProvider, entity);

            NeededComponents = new[] { typeof(ItemComponent), typeof(ModelComponent),
                typeof (TransformComponent), typeof (PhysicsComponent) };

            internalValidMessages = new[] { (int) MessageId.ItemUse, (int) MessageId.ItemDropped,
                (int) MessageId.ItemCollected };
            messageProvider += this;
        }

        void updatePosition(Vector3 position)
        {
            var item = Entity.GetComponent<ItemComponent>();
            var transform = Entity.GetComponent<TransformComponent>();
            if (item != null && transform != null && item.Player != null)
            {
                var player_transform = item.Player.GetComponent<TransformComponent>();
                transform.Position = position + Vector3.Transform(item.PositionOffset, player_transform.Rotation);
                transform.Rotation = player_transform.Rotation;
            }
        }

        void updateRotation(Quaternion rotation)
        {
            var item = Entity.GetComponent<ItemComponent>();
            var transform = Entity.GetComponent<TransformComponent>();
            if (item != null && item.Player != null)
            {
                var player_transform = item.Player.GetComponent<TransformComponent>();
                transform.Position = player_transform.Position + Vector3.Transform(item.PositionOffset, rotation);
                transform.Rotation = rotation;
            }
        }

        /// <summary>
        /// Processes the incoming message
        /// </summary>
        /// <param name="msg">Message to process</param>
        public override void ConsumeMessage(IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.ItemDropped)
            {
                var idm = msg as ItemDroppedMessage;

                if (idm.Item.Entity.Name == Entity.Name)
                {
                    var body = Entity.GetComponent<PhysicsComponent>().RigidBody;
                    var transform = Entity.GetComponent<TransformComponent>();
                    Entity.GetComponent<ModelComponent>().Model.Enabled = true;

                    if (idm.Item.Player != null)
                    {
                        var player_transform = idm.Item.Player.GetComponent<TransformComponent>();
                        player_transform.OnPositionChanged -= updatePosition;
                        player_transform.OnRotationChanged -= updateRotation;
                    }

                    idm.Item.Player = null;

                    if (body != null)
                    {
                        var view_direction = Vector3.Transform(new Vector3(0, 0, 2), transform.Rotation).ToJitterVector();
                        body.Position = transform.Position.ToJitterVector() + view_direction;
                        var p = body.Position;
                        p.Y = 1;
                        body.Position = p;
                        body.Orientation = JMatrix.CreateFromQuaternion(transform.Rotation.ToJitterQuaternion());
                        body.IsStatic = false;
                        body.IsActive = true;
                        body.ApplyImpulse(view_direction);
                    }
                }
            }

            if (msg.MessageId == (int) MessageId.ItemCollected)
            {
                var icm = msg as ItemCollectedMessage;

                if (icm.Item.Entity.Name == Entity.Name)
                {
                    Entity.GetComponent<PhysicsComponent>().RigidBody.IsStatic = true;
                    Entity.GetComponent<ModelComponent>().Model.Enabled = false;
                    Entity.GetComponent<PhysicsComponent>().RigidBody.Position = JVector.One * -1;

                    var player_transform = icm.Item.Player.GetComponent<TransformComponent>();
                    var transform = Entity.GetComponent<TransformComponent>();
                    player_transform.OnPositionChanged += updatePosition;
                    player_transform.OnRotationChanged += updateRotation;
                }
            }

            if (msg.MessageId == (int) MessageId.ItemUse)
            {
                var ium = msg as ItemUseMessage;
                var itemcomp = Entity.GetComponent<ItemComponent>();

                if (ium.Item.Entity.Name != Entity.Name || itemcomp == null || itemcomp.ItemUsageHandler == null)
                    return;

                if (ium.Item.ItemUsages.HasFlag(ItemUsage.Eatable) && ium.Usage.HasFlag(ItemUsage.Eatable))
                {
                    if (itemcomp.Player == null || ium.Entity.Name != itemcomp.Player.Name)
                        return;

                    itemcomp.ItemUsageHandler.Eat(itemcomp);
                }
                if (ium.Item.ItemUsages.HasFlag(ItemUsage.Throwable) && ium.Usage.HasFlag(ItemUsage.Throwable))
                {
                    itemcomp.ItemUsageHandler.Throw(itemcomp);
                }
                if (ium.Item.ItemUsages.HasFlag(ItemUsage.Hitable) && ium.Usage.HasFlag(ItemUsage.Hitable))
                {
                    var physics = Entity.GetComponent<PhysicsComponent>();
                    var transform = Entity.GetComponent<TransformComponent>();
                    RigidBody rb;
                    JVector n;
                    float f;
                    physics.World.CollisionSystem.Raycast(
                        transform.Position.ToJitterVector(),
                        Vector3.Transform(Vector3.UnitZ, ium.Scene.CameraManager.ActiveCamera.Rotation).ToJitterVector() * 20,
                        new Jitter.Collision.RaycastCallback((body, normal, fraction) =>
                            itemcomp.ItemUsageHandler.IsHit(body, normal.ToFreezingArcherVector(), fraction)),
                            out rb, out n, out f);
                    if (rb != null)
                        itemcomp.ItemUsageHandler.Hit(itemcomp, rb, n.ToFreezingArcherVector(), f);
                }
            }
        }
    }
}
