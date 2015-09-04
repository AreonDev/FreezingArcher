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
using FreezingArcher.Renderer.Compositor;

namespace FreezingArcher.Game.Ghosts
{
    public class Ghost
    {
        public static int InstanceCount = 0;

        public Ghost (GameState state, AIManager aiManager, RendererContext rendererContext,
            CompositorColorCorrectionNode colorCorrectionNode)
        {
            ghostEmitter = new WhiteGhostParticleEmitter ();

            particleGhost = new ParticleSceneObject (ghostEmitter.ParticleCount);
            particleGhost.Priority = 7002;
            state.Scene.AddObject (particleGhost);
            ghostEmitter.Init (particleGhost, rendererContext);

            GhostGameState = state;

            GhostEntity = EntityFactory.Instance.CreateWith ("Ghost." + InstanceCount++, state.MessageProxy,
                new[] { typeof (ArtificialIntelligenceComponent) },
                new[] { typeof (ParticleSystem), typeof (PhysicsSystem), typeof(LightSystem) });

            GhostEntity.GetComponent<ParticleComponent> ().Emitter = ghostEmitter;
            GhostEntity.GetComponent<ParticleComponent> ().Particle = particleGhost;

            var light = GhostEntity.GetComponent<LightComponent> ().Light;
            light = new FreezingArcher.Renderer.Scene.Light (FreezingArcher.Renderer.Scene.LightType.PointLight);
            light.On = true;
            light.Color = new FreezingArcher.Math.Color4 (0.6f, 0.6f, 0.6f, 1.0f);
            light.PointLightLinearAttenuation = 0.4f;
            light.PointLightConstantAttenuation = 0.7f;
            light.PointLightExponentialAttenuation = 0.008f;

            GhostEntity.GetComponent<LightComponent> ().Light = light;

            state.Scene.AddLight (light);

            RigidBody ghostBody = new RigidBody (new SphereShape (1.2f));
            ghostBody.AffectedByGravity = false;
            ghostBody.AllowDeactivation = false;
            ghostBody.Mass = 20;
            GhostEntity.GetComponent<PhysicsComponent> ().RigidBody = ghostBody;
            GhostEntity.GetComponent<PhysicsComponent> ().World = state.PhysicsManager.World;
            GhostEntity.GetComponent<PhysicsComponent> ().PhysicsApplying = AffectedByPhysics.Position;

            state.PhysicsManager.World.AddBody (ghostBody);

            GhostEntity.GetComponent<ArtificialIntelligenceComponent>().AIManager = aiManager;
            GhostEntity.GetComponent<ArtificialIntelligenceComponent>().ArtificialIntelligence =
                new GhostAI (GhostEntity, state, colorCorrectionNode);
            aiManager.RegisterEntity (GhostEntity);
        }

        public GameState GhostGameState { get; private set;}
        public Entity GhostEntity { get; private set;}
        readonly ParticleSceneObject particleGhost;
        readonly WhiteGhostParticleEmitter ghostEmitter;
    }
}
