//
//  MazeItemUseHandler.cs
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
using Jitter.LinearMath;
using Jitter.Dynamics;
using FreezingArcher.Math;
using FreezingArcher.Output;

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Maze item use handler.
    /// </summary>
    public class MazeItemUseHandler : IItemUsageHandler
    {
        #region IItemUsageHandler implementation

        /// <summary>
        /// Eat the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Eat (ItemComponent item)
        {
            var playercomp = item.Player.GetComponent<HealthComponent>();

            if (item.Usage <= 1)
            {
                playercomp.Health += item.HealthDelta;
                playercomp.Health = playercomp.Health > playercomp.MaximumHealth ?
                    playercomp.MaximumHealth : playercomp.Health;
                item.Usage = item.Usage <= (1 - item.UsageDeltaPerUsage) ? item.Usage + item.UsageDeltaPerUsage : 1f;
            }
        }

        /// <summary>
        /// Throw the specified item.
        /// </summary>
        /// <param name="item">Item.</param>
        public void Throw (ItemComponent item)
        {
            var physics = item.Entity.GetComponent<PhysicsComponent>();
            physics.RigidBody.IsActive = true;
            physics.RigidBody.ApplyImpulse(JVector.Transform(new JVector(0, 0, 10), physics.RigidBody.Orientation));
        }

        /// <summary>
        /// Hit the specified item, rigidBody, normal and fraction.
        /// </summary>
        /// <param name="item">Item.</param>
        /// <param name="rigidBody">Rigid body.</param>
        /// <param name="normal">Normal.</param>
        /// <param name="fraction">Fraction.</param>
        public void Hit (ItemComponent item, RigidBody rigidBody, Vector3 normal, float fraction)
        {
            var entity = rigidBody.Tag as Entity;

            if (entity != null)
            {
                var model = entity.GetComponent<ModelComponent>().Model;
                var health = entity.GetComponent<HealthComponent>();
                var wallcomp = entity.GetComponent<WallComponent>();
                health.Health = (health.Health - item.AttackStrength) < 0 ? 0 : health.Health - item.AttackStrength;
                var tmp = (health.MaximumHealth - health.Health) / health.MaximumHealth;
                var pos = model.Position;
                pos.Y = 7.5f * tmp + 0.25f;

                if (wallcomp.IsOverworld)
                    pos.Y *= -2;

                model.Position = pos;
                var rbpos = rigidBody.Position;
                rbpos.Y = pos.Y + 8;
                rigidBody.Position = rbpos;
                item.Usage = item.Usage <= (1 - item.UsageDeltaPerUsage) ? item.Usage + item.UsageDeltaPerUsage : 1f;
                wallcomp.IsMoveable = false;
                return;
            }
        }

        /// <summary>
        /// Determines whether this instance is hit the specified rigidBody normal fraction.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        /// <param name="rigidBody">Rigid body.</param>
        /// <param name="normal">Normal.</param>
        /// <param name="fraction">Fraction.</param>
        public bool IsHit (RigidBody rigidBody, Vector3 normal, float fraction)
        {
            var entity = rigidBody.Tag as Entity;

            return entity != null && entity.Name.Contains ("wall") && fraction < 1 && !entity.GetComponent<WallComponent> ().IsEdge;
        }

        #endregion
    }
}
