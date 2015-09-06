//
//  Roaches.cs
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
using FreezingArcher.Renderer;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using FreezingArcher.Game.AI;

namespace FreezingArcher.Game.Ghosts
{
    public class Roaches
    {
        public static int InstanceCount = 0;

        public Roaches (GameState state, AIManager aiManager, RendererContext rendererContext)
        {
            RoachGameState = state;

            roachGroup = new RoachGroup (state.Scene);

            RoachEntity = EntityFactory.Instance.CreateWith("Roach." + InstanceCount++, state.MessageProxy,
                new[] { typeof (ArtificialIntelligenceComponent) },
                new[] { typeof (PhysicsSystem), typeof (RoachGroupSystem) });

            RoachEntity.GetComponent<RoachGroupComponent>().RoachGroup = roachGroup;

            RigidBody roachBody = new RigidBody (new CylinderShape (0.05f, 1));
            roachBody.AffectedByGravity = false;
            roachBody.AllowDeactivation = false;
            roachBody.Mass = 20;
            roachBody.Tag = roachGroup;
            RoachEntity.GetComponent<PhysicsComponent> ().RigidBody = roachBody;
            RoachEntity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
            RoachEntity.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (roachBody);

            RoachEntity.GetComponent<ArtificialIntelligenceComponent>().AIManager = aiManager;
            RoachEntity.GetComponent<ArtificialIntelligenceComponent>().ArtificialIntelligence =
                new RoachesAI (roachGroup, RoachEntity, state);
            aiManager.RegisterEntity (RoachEntity);
        }

        public GameState RoachGameState { get; private set; }
        public Entity RoachEntity { get; private set; }
        readonly RoachGroup roachGroup;
    }
}

