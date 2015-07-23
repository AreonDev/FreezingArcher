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

            NeededComponents = new[] { typeof(ItemComponent) };

            internalValidMessages = new[] { (int) MessageId.ItemUse, (int) MessageId.ItemDropped };
            messageProvider += this;
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

                    if (body != null)
                    {
                        body.Position = (transform.Position + Vector3.Transform(new Vector3(0, 0, 2), transform.Rotation)).ToJitterVector();
                        var p = body.Position;
                        p.Y = 1;
                        body.Position = p;
                        body.Orientation = JMatrix.CreateFromQuaternion(transform.Rotation.ToJitterQuaternion());
                        Entity.GetComponent<PhysicsComponent>().RigidBody.IsStatic = false;
                    }
                }
            }

            if (msg.MessageId == (int) MessageId.ItemUse)
            {
                var ium = msg as ItemUseMessage;
                var itemcomp = Entity.GetComponent<ItemComponent>();

                if (ium.Item.Entity.Name != Entity.Name)
                    return;

                if (ium.Usage.HasFlag(ItemUsage.Eatable))
                {
                    if (itemcomp.Player == null || ium.Entity.Name != itemcomp.Player.Name)
                        return;

                    var playercomp = itemcomp.Player.GetComponent<HealthComponent>();

                    if (itemcomp.Usage <= 1)
                    {
                        playercomp.Health += itemcomp.HealthDelta;
                        playercomp.Health = playercomp.Health > playercomp.MaximumHealth ?
                            playercomp.MaximumHealth : playercomp.Health;
                        itemcomp.Usage = itemcomp.Usage <= (1 - itemcomp.UsageDeltaPerUsage) ?
                            itemcomp.Usage + itemcomp.UsageDeltaPerUsage : 1f;
                    }
                }
                if (ium.Usage.HasFlag(ItemUsage.Throwable))
                {
                    var physics = Entity.GetComponent<PhysicsComponent>();
                    physics.RigidBody.ApplyImpulse(JVector.Transform(new JVector(0, 0, 10), physics.RigidBody.Orientation));
                }
                if (ium.Usage.HasFlag(ItemUsage.Hitable))
                {
                    // TODO
                    var physics = Entity.GetComponent<PhysicsComponent>();
                    var transform = Entity.GetComponent<TransformComponent>();
                    RigidBody rb;
                    JVector n;
                    float f;
                    physics.World.CollisionSystem.Raycast(
                        transform.Position.ToJitterVector(),
                        Vector3.Transform(Vector3.UnitZ, ium.Scene.CameraManager.ActiveCamera.Rotation).ToJitterVector(),
                        new Jitter.Collision.RaycastCallback(((body, normal, fraction) => {
                            var entity = body.Tag as Entity;
                            if (entity != null && entity.Name.Contains("wall"))
                            {
                                var rigidBody = entity.GetComponent<PhysicsComponent>().RigidBody;
                                var health = entity.GetComponent<HealthComponent>();
                                health.Health = (health.Health - ium.Item.AttackStrength) < 0 ?
                                    0 : health.Health - ium.Item.AttackStrength;
                                var pos = rigidBody.Position;
                                pos.Y = -16 * (health.MaximumHealth - health.Health) / health.MaximumHealth;
                                rigidBody.Position = pos;
                                entity.GetComponent<ModelComponent>().Model.Position = rigidBody.Position.ToFreezingArcherVector();
                                return true;
                            }
                            return false;
                        })), out rb, out n, out f
                    );
                }
            }
        }
    }
}
