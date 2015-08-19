//
//  SphereParticleEmitter.cs
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
using FreezingArcher.Renderer.Scene;

namespace FreezingArcher.Game
{
    public class RedEyeParticleEmitter : ParticleEmitter
    {
        Random rnd = new Random();

        Vector3 cachedSpawnPoint;

        public RedEyeParticleEmitter () : base (10)
        {
            
        }

        #region implemented abstract members of ParticleEmitter

        protected override void UpdateParticle (Particle par, float time)
        {
            par.Velocity = Vector3.Zero;
            par.Position -= cachedSpawnPoint;
            par.Position += SpawnPosition;

            if (par.Life >= 0.05f) 
            {
                if ((par.Position - SpawnPosition).Length > 0.05f)
                {
                    par.Velocity = Math.Vector3.Zero;
                    par.Update (time);
                } else 
                {
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

                par.Position = SpawnPosition + (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 0.05f * invert1, (float)rnd.NextDouble () * 0.05f * invert2, 
                    (float)rnd.NextDouble () * 0.05f * invert3));

                par.Velocity = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 0.05f * invert1, (float)rnd.NextDouble () * 0.05f * invert2, 
                    (float)rnd.NextDouble () * 0.05f * invert3));

                par.Color = new FreezingArcher.Math.Color4(0.0f, 0.5f , 0.0f, 1f);
                par.Life = 1.2f;
                par.Mass = 1.0f;
                par.Size = new Vector2(0.3f, 0.3f);
            }
        }

        public override void EndUpdate (float time)
        {
            cachedSpawnPoint = SpawnPosition;
        }

        protected override void InitializeParticles (RendererContext rc)
        {
            if (SceneObject != null) 
            {
                SceneObject.BillboardTexture = rc.CreateTexture2D ("BillboardTexture_Sphere_Particles_" + DateTime.Now.Ticks, true, "Content/Particles/particle_05.png");
            }

            foreach (Particle par in Particles) 
            {
                par.Reset ();

                float invert1 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert2 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert3 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;

                par.Position = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 0.05f * invert1, (float)rnd.NextDouble () * 0.05f * invert2, 
                    (float)rnd.NextDouble () * 0.05f * invert3)) + SpawnPosition;

                //par.Velocity = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 10.5f, (float)rnd.NextDouble () * 10.5f, (float)rnd.NextDouble () * 10.5f))*invert;
                par.Mass = 1.0f;
                par.Size = new Vector2(0.3f, 0.3f);
                par.Color = new FreezingArcher.Math.Color4(0.0f, 0.5f , 0.0f, 1f);
                par.Life = 1.2f;
                par.LifeTime = 1.0f * (float)rnd.NextDouble();
            }

            cachedSpawnPoint = SpawnPosition;
        }

        #endregion
    }
}

