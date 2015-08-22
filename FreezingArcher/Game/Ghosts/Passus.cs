//
//  Passus.cs
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
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Content;
using Jitter.Dynamics;
using Jitter.Collision.Shapes;
using FreezingArcher.Renderer;
using FreezingArcher.Game.AI;

namespace FreezingArcher.Game.Ghosts
{
    public class Passus
    {
        public static int InstanceCount = 0;

        public Passus (GameState state, AIManager aiManager, RendererContext rendererContext)
        {
            ghostEmitter = new PassusGhostParticleEmitter ();

            particleGhost = new ParticleSceneObject (ghostEmitter.ParticleCount);
            particleGhost.Priority = 7001;
            state.Scene.AddObject (particleGhost);
            ghostEmitter.Init (particleGhost, rendererContext);

            ghostEntity = EntityFactory.Instance.CreateWith ("Passus." + InstanceCount++, state.MessageProxy,
                new[] { typeof (ArtificialIntelligenceComponent) },
                new[] { typeof (ParticleSystem), typeof (PhysicsSystem) });

            ghostEntity.GetComponent<ParticleComponent> ().Emitter = ghostEmitter;
            ghostEntity.GetComponent<ParticleComponent> ().Particle = particleGhost;

            RigidBody passusBody = new RigidBody (new SphereShape (1.2f));
            passusBody.AffectedByGravity = false;
            passusBody.AllowDeactivation = false;
            passusBody.Mass = 20;
            ghostEntity.GetComponent<PhysicsComponent> ().RigidBody = passusBody;
            ghostEntity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
            ghostEntity.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (passusBody);

            ghostEntity.GetComponent<ArtificialIntelligenceComponent>().AIManager = aiManager;
            ghostEntity.GetComponent<ArtificialIntelligenceComponent>().ArtificialIntelligence = new PassusAI ();
            aiManager.RegisterEntity (ghostEntity);
        }

        readonly Entity ghostEntity;
        readonly ParticleSceneObject particleGhost;
        readonly PassusGhostParticleEmitter ghostEmitter;
    }
}
