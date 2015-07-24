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
            var wall = rigidBody.Tag as Entity;
            var model = wall.GetComponent<ModelComponent>().Model;
            var health = wall.GetComponent<HealthComponent>();
            var wallcomp = wall.GetComponent<WallComponent>();
            health.Health = (health.Health - item.AttackStrength) < 0 ? 0 : health.Health - item.AttackStrength;
            var tmp = (health.MaximumHealth - health.Health) / health.MaximumHealth;
            var rbpos = rigidBody.Position;
            rbpos.Y = -8 * tmp;
            rigidBody.Position = rbpos;
            var pos = model.Position;
            pos.Y = -16 * tmp;
            model.Position = pos;
            item.Usage = item.Usage <= (1 - item.UsageDeltaPerUsage) ? item.Usage + item.UsageDeltaPerUsage : 1f;
            wallcomp.IsMoveable = false;
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
            if (entity != null && entity.Name.Contains("wall") && fraction < 5 && !entity.GetComponent<WallComponent>().IsEdge)
                return true;
            return false;
        }

        #endregion
    }
}
