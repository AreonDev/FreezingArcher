//
//  Ghost.cs
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
    public class Ghost
    {
        public static int InstanceCount = 0;

        public Ghost (GameState state, AIManager aiManager, RendererContext rendererContext)
        {
            ghostEmitter = new NiceGhostParticleEmitter ();

            particleGhost = new ParticleSceneObject (ghostEmitter.ParticleCount);
            particleGhost.Priority = 7002;
            state.Scene.AddObject (particleGhost);
            ghostEmitter.Init (particleGhost, rendererContext);

            ghostEntity = EntityFactory.Instance.CreateWith ("Ghost." + InstanceCount++, state.MessageProxy,
                new[] { typeof (ArtificialIntelligenceComponent) },
                new[] { typeof (ParticleSystem), typeof (PhysicsSystem), typeof(LightSystem) });

            ghostEntity.GetComponent<ParticleComponent> ().Emitter = ghostEmitter;
            ghostEntity.GetComponent<ParticleComponent> ().Particle = particleGhost;

            var light = ghostEntity.GetComponent<LightComponent> ().Light;
            light = new FreezingArcher.Renderer.Scene.Light (FreezingArcher.Renderer.Scene.LightType.PointLight);
            light.On = true;
            light.Color = new FreezingArcher.Math.Color4 (0.6f, 1.0f, 0.6f, 1.0f);
            light.PointLightLinearAttenuation = 0.4f;
            light.PointLightConstantAttenuation = 0.7f;
            light.PointLightExponentialAttenuation = 0.008f;

            ghostEntity.GetComponent<LightComponent> ().Light = light;

            state.Scene.AddLight (light);

            RigidBody ghostBody = new RigidBody (new SphereShape (1.2f));
            ghostBody.AffectedByGravity = false;
            ghostBody.AllowDeactivation = false;
            ghostBody.Mass = 20;
            ghostEntity.GetComponent<PhysicsComponent> ().RigidBody = ghostBody;
            ghostEntity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
            ghostEntity.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (ghostBody);

            ghostEntity.GetComponent<ArtificialIntelligenceComponent>().AIManager = aiManager;
            ghostEntity.GetComponent<ArtificialIntelligenceComponent>().ArtificialIntelligence = new GhostAI ();
            aiManager.RegisterEntity (ghostEntity);
        }

        readonly Entity ghostEntity;
        readonly ParticleSceneObject particleGhost;
        readonly NiceGhostParticleEmitter ghostEmitter;
    }
}
