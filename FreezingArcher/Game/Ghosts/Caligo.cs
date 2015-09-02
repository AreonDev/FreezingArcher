//
//  Caligo.cs
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
using FreezingArcher.Renderer.Compositor;

namespace FreezingArcher.Game.Ghosts
{
    public class Caligo
    {
        public static int InstanceCount = 0;

        public Caligo (GameState state, AIManager aiManager, RendererContext rendererContext,
            CompositorWarpingNode warpingNode)
        {
            caligoEmitter = new CaligoParticleEmitter ();
            particleCaligo = new ParticleSceneObject (caligoEmitter.ParticleCount);
            particleCaligo.Priority = 7000;
            state.Scene.AddObject (particleCaligo);
            caligoEmitter.Init (particleCaligo, rendererContext);

            caligoEntity = EntityFactory.Instance.CreateWith ("Caligo." + InstanceCount++, state.MessageProxy,
                new[] { typeof (ArtificialIntelligenceComponent) },
                new[] { typeof (ParticleSystem), typeof (PhysicsSystem) });

            caligoEntity.GetComponent<ParticleComponent> ().Emitter = caligoEmitter;
            caligoEntity.GetComponent<ParticleComponent> ().Particle = particleCaligo;

            RigidBody caligoBody = new RigidBody (new SphereShape (1.2f));
            caligoBody.AffectedByGravity = false;
            caligoBody.AllowDeactivation = false;
            caligoBody.Mass = 20;
            caligoEntity.GetComponent<PhysicsComponent> ().RigidBody = caligoBody;
            caligoEntity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
            caligoEntity.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (caligoBody);

            var AIcomp = caligoEntity.GetComponent<ArtificialIntelligenceComponent>();
            AIcomp.AIManager = aiManager;
            AIcomp.ArtificialIntelligence = new CaligoAI (caligoEntity, state, warpingNode);
            aiManager.RegisterEntity (caligoEntity);
        }

        readonly ParticleSceneObject particleCaligo;
        readonly CaligoParticleEmitter caligoEmitter;
        readonly Entity caligoEntity;
    }
}
