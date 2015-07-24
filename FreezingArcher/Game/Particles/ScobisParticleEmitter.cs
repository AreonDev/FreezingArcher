//
//  ScobisParticleEmitter.cs
//
//  Author:
//       dboeg <>
//
//  Copyright (c) 2015 dboeg
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

using FreezingArcher.Math;
using FreezingArcher.Renderer;
using FreezingArcher.Renderer.Scene.SceneObjects;

namespace FreezingArcher.Game
{
    public class ScobisParticleEmitter : ParticleEmitter
    {
        private ParticleSceneObject eye1;
        private ParticleSceneObject eye2;
        private ParticleSceneObject smoke;

        public ScobisParticleEmitter (ParticleSceneObject eye1, ParticleSceneObject eye2, ParticleSceneObject smoke) : base(100)
        {
            this.eye1 = eye1;
            this.eye2 = eye2;

            this.smoke = smoke;
        }

        public RedEyeParticleEmitter RedEye1 { get; private set;}
        public RedEyeParticleEmitter RedEye2 {get; private set;}
        public ScobisSmokeParticleEmitter Smoke {get; private set;}

        #region implemented abstract members of ParticleEmitter

        public override void EndUpdate(float time)
        {
            Vector3 looking_dir = SpawnPosition - cachedSpawnPoint;

            cachedSpawnPoint = SpawnPosition;

            if (looking_dir.Length > 0.02f) 
            {
                Vector3 up = Vector3.UnitY;
                Vector3 right = Vector3.Normalize(Vector3.Cross (up, looking_dir));

                RedEye1.SpawnPosition = SpawnPosition - right * 0.3f + Vector3.Normalize (looking_dir) * 0.5f;
                RedEye2.SpawnPosition = SpawnPosition + right * 0.3f + Vector3.Normalize (looking_dir) * 0.5f;
            }

            Smoke.SpawnPosition = SpawnPosition;

            RedEye1.Update (time);
            RedEye2.Update (time);
            Smoke.Update (time);
        }

        Random rnd = new Random ();

        Vector3 cachedSpawnPoint;

        public override void UpdateParticle (Particle par, float time)
        {
            //par.Velocity = Vector3.Zero;
            par.Position -= cachedSpawnPoint;
            par.Position += SpawnPosition;

            if (par.Life >= 0.05f) 
            {
                if ((par.Position - SpawnPosition).Length > 1.0f)
                {
                    par.Color = new FreezingArcher.Math.Color4(par.Color.R, par.Color.G, par.Color.B, par.Color.A - time * 0.40f);

                    par.Velocity = Math.Vector3.Zero;
                    par.Update (time);
                } else 
                {
                    //par.Life -= time * 4.0f;

                    if (par.Color.A < 0.6f)
                        par.Color = new FreezingArcher.Math.Color4 (par.Color.R, par.Color.G, par.Color.B, par.Color.A + time * 0.80f);

                    FreezingArcher.Math.Vector3 actual = par.Velocity;
                    par.Velocity = actual;
                    par.Update (time);
                }

            } else
            {
                par.Reset ();

                float invert1 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert2 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert3 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;

                par.Position = SpawnPosition + (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 0.2f * invert1, (float)rnd.NextDouble () * 0.2f * invert2, 
                    (float)rnd.NextDouble () * 0.2f * invert3));

                par.Velocity = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 0.4f * invert1, (float)rnd.NextDouble () * 0.4f * invert2, 
                    (float)rnd.NextDouble () * 0.4f * invert3));

                par.Life = 0.7f;
                par.LifeTime = 1.0f * (float)rnd.NextDouble();
                par.Size = new Vector2(1.2f, 1.2f);
                par.Color = new FreezingArcher.Math.Color4(0.05f, 0.01f , 0.00f, 0.3f);
            }
        }

        public override void InitializeParticles (RendererContext rc)
        {
            if (SceneObject != null) 
            {

            }

            RedEye1 = new RedEyeParticleEmitter ();
            RedEye2 = new RedEyeParticleEmitter ();
            Smoke = new ScobisSmokeParticleEmitter ();

            RedEye1.Init (eye1, rc);
            RedEye2.Init (eye2, rc);
            Smoke.Init (smoke, rc);

            foreach (Particle par in Particles) 
            {
                par.Reset ();

                float invert1 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert2 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert3 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;

                par.Position = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 0.8f * invert1, (float)rnd.NextDouble () * 0.8f * invert2, 
                    (float)rnd.NextDouble () * 0.8f * invert3)) + SpawnPosition;

                //par.Velocity = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 10.5f, (float)rnd.NextDouble () * 10.5f, (float)rnd.NextDouble () * 10.5f))*invert;
                par.Mass = 1.0f;
                par.Size = new Vector2(1.2f, 1.2f);
                par.Color = new FreezingArcher.Math.Color4(0.05f, 0.01f , 0.00f, 0.4f);
                par.Life = 1.2f;
                par.LifeTime = 1.0f * (float)rnd.NextDouble();
            }
        }

        #endregion
    }
}

