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
        public Scobis (GameState state, AIManager aiManager, RendererContext rendererContext)
        {
            paremitter = new ScobisParticleEmitter ();
            particle_eye1 = new ParticleSceneObject (paremitter.RedEye1.ParticleCount);
            particle_eye2 = new ParticleSceneObject (paremitter.RedEye2.ParticleCount);
            particle_smoke = new ParticleSceneObject (paremitter.Smoke.ParticleCount);
            particle_eye1.Priority = 5999;
            particle_eye2.Priority = 5999;
            particle_smoke.Priority = 6000;

            state.Scene.AddObject (particle_eye1);
            state.Scene.AddObject (particle_eye2);
            state.Scene.AddObject (particle_smoke);

            particle = new ParticleSceneObject (paremitter.ParticleCount);
            particle.Priority = 5998;
            state.Scene.AddObject (particle);

            paremitter.Init (particle, particle_eye1, particle_eye2, particle_smoke, rendererContext);

            ScobisEntity = EntityFactory.Instance.CreateWith ("Scobis", state.MessageProxy, systems:
                new[] { typeof(ParticleSystem), typeof (ArtificialIntelligenceSystem), typeof (PhysicsSystem) });

            ScobisEntity.GetComponent<ParticleComponent> ().Emitter = paremitter;
            ScobisEntity.GetComponent<ParticleComponent> ().Particle = particle;
            RigidBody scobisBody = new RigidBody (new SphereShape (0.3f));
            scobisBody.AffectedByGravity = false;
            scobisBody.AllowDeactivation = false;
            ScobisEntity.GetComponent<PhysicsComponent> ().RigidBody = scobisBody;
            ScobisEntity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
            ScobisEntity.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (scobisBody);

            ScobisEntity.GetComponent<ArtificialIntelligenceComponent>().AIManager = aiManager;
            ScobisEntity.GetComponent<ArtificialIntelligenceComponent>().ArtificialIntelligence = new ScobisAI ();
            aiManager.RegisterEntity (ScobisEntity);
        }

        readonly ParticleSceneObject particle;
        readonly ParticleSceneObject particle_eye1;
        readonly ParticleSceneObject particle_eye2;
        readonly ParticleSceneObject particle_smoke;
        readonly ScobisParticleEmitter paremitter;
        readonly Entity ScobisEntity;
    }
}
