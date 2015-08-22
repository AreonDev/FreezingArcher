//
//  Scobis.cs
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
using FreezingArcher.Game.AI;
using FreezingArcher.Renderer;

namespace FreezingArcher.Game.Ghosts
{
    public class Scobis
    {
        public static int InstanceCount = 0;

        public Scobis (GameState state, AIManager aiManager, RendererContext rendererContext)
        {
            paremitter = new ScobisParticleEmitter ();
            particleEye1 = new ParticleSceneObject (paremitter.RedEye1.ParticleCount);
            particleEye2 = new ParticleSceneObject (paremitter.RedEye2.ParticleCount);
            particleSmoke = new ParticleSceneObject (paremitter.Smoke.ParticleCount);
            particleEye1.Priority = 5999;
            particleEye2.Priority = 5999;
            particleSmoke.Priority = 6000;

            state.Scene.AddObject (particleEye1);
            state.Scene.AddObject (particleEye2);
            state.Scene.AddObject (particleSmoke);

            particle = new ParticleSceneObject (paremitter.ParticleCount);
            particle.Priority = 5998;
            state.Scene.AddObject (particle);

            paremitter.Init (particle, particleEye1, particleEye2, particleSmoke, rendererContext);

            scobisEntity = EntityFactory.Instance.CreateWith ("Scobis." + InstanceCount++, state.MessageProxy,
                new[] { typeof (ArtificialIntelligenceComponent) },
                new[] { typeof (ParticleSystem), typeof (PhysicsSystem) });

            scobisEntity.GetComponent<ParticleComponent> ().Emitter = paremitter;
            scobisEntity.GetComponent<ParticleComponent> ().Particle = particle;
            RigidBody scobisBody = new RigidBody (new SphereShape (0.3f));
            scobisBody.AffectedByGravity = false;
            scobisBody.AllowDeactivation = false;
            scobisBody.Mass = 20;
            scobisEntity.GetComponent<PhysicsComponent> ().RigidBody = scobisBody;
            scobisEntity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
            scobisEntity.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (scobisBody);

            scobisEntity.GetComponent<ArtificialIntelligenceComponent>().AIManager = aiManager;
            scobisEntity.GetComponent<ArtificialIntelligenceComponent>().ArtificialIntelligence = new ScobisAI ();
            aiManager.RegisterEntity (scobisEntity);
        }

        readonly ParticleSceneObject particle;
        readonly ParticleSceneObject particleEye1;
        readonly ParticleSceneObject particleEye2;
        readonly ParticleSceneObject particleSmoke;
        readonly ScobisParticleEmitter paremitter;
        readonly Entity scobisEntity;
    }
}
