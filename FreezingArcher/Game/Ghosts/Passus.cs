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
using FreezingArcher.Renderer.Compositor;

namespace FreezingArcher.Game.Ghosts
{
    public class Passus
    {
        public static int InstanceCount = 0;

        public Passus (CompositorColorCorrectionNode colorCorrectionNode,
            GameState state, AIManager aiManager, RendererContext rendererContext)
        {
            passusEmitter = new PassusGhostParticleEmitter ();

            particlePassus = new ParticleSceneObject (passusEmitter.ParticleCount);
            particlePassus.Priority = 7001;
            state.Scene.AddObject (particlePassus);
            passusEmitter.Init (particlePassus, rendererContext);

            passusEntity = EntityFactory.Instance.CreateWith ("Passus." + InstanceCount++, state.MessageProxy,
                new[] { typeof (ArtificialIntelligenceComponent) },
                new[] { typeof (ParticleSystem), typeof (PhysicsSystem) });

            passusEntity.GetComponent<ParticleComponent> ().Emitter = passusEmitter;
            passusEntity.GetComponent<ParticleComponent> ().Particle = particlePassus;

            RigidBody passusBody = new RigidBody (new SphereShape (1.2f));
            passusBody.AffectedByGravity = false;
            passusBody.AllowDeactivation = false;
            passusBody.Mass = 20;
            passusEntity.GetComponent<PhysicsComponent> ().RigidBody = passusBody;
            passusEntity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
            passusEntity.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (passusBody);

            var AIcomp = passusEntity.GetComponent<ArtificialIntelligenceComponent>();
            AIcomp.AIManager = aiManager;
            AIcomp.ArtificialIntelligence = new PassusAI (passusEntity, state, colorCorrectionNode);
            aiManager.RegisterEntity (passusEntity);
        }

        readonly Entity passusEntity;
        readonly ParticleSceneObject particlePassus;
        readonly PassusGhostParticleEmitter passusEmitter;
    }
}
