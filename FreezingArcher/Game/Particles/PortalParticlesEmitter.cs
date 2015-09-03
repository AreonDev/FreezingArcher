//
//  PortalParticles.cs
//
//  Author:
//       david <${AuthorEmail}>
//
//  Copyright (c) 2015 david
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
using FreezingArcher.Renderer.Scene;

namespace FreezingArcher.Game.Particles
{
    public class PortalParticles : ParticleEmitter
    {
        Random rnd = new Random();

        public PortalParticles () : base(40)
        {
        }

        #region implemented abstract members of ParticleEmitter

        protected override void UpdateParticle (Particle par, float time)
        {
            if (par.Life >= 0.05f)
            {
                if (par.Position.Y > 10.0f)
                {
                    par.Color = new Color4 (par.Color.R, par.Color.G, par.Color.B, (1.0f - ((float)par.Age / (float)par.LifeTime) * 0.1f));
                    
                    par.Velocity = Math.Vector3.Zero;
                    par.Update (time);
                }
                else
                {
                    if (par.Color.A < 0.7f)
                        par.Color = new Color4 (par.Color.R, par.Color.G, par.Color.B, par.Color.A + time * 0.8f);

                    FreezingArcher.Math.Vector3 actual = par.Velocity;
                    par.Velocity = actual;

                    par.Update (time);
                }
            }
            else
            {
                par.Reset ();

                float invert1 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert2 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;

                par.Position = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 3f * invert1, 0.0f, 
                    (float)rnd.NextDouble () * 3f * invert2)) + SpawnPosition;

                par.Velocity = new FreezingArcher.Math.Vector3 (0.0f, (float)rnd.NextDouble () * 5f, 0);
                par.Mass = 1.0f;
                par.Size = new Vector2(0.5f, 0.5f);
                par.Color = new FreezingArcher.Math.Color4(0.4f, 0.4f , 0.4f, 1f);
                par.Life = 1.2f;
                par.LifeTime = 3.0f * (float)rnd.NextDouble();
            }
        }

        protected override void InitializeParticles (FreezingArcher.Renderer.RendererContext rc)
        {
            if (SceneObject != null) 
            {
                SceneObject.BillboardTexture = rc.CreateTexture2D ("BillboardTexture_Sphere_Particles_" + DateTime.Now.Ticks, true, "Content/Particles/particle_01.png");
            }
                
            foreach (Particle par in Particles) 
            {
                par.Reset ();

                float invert1 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert2 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;

                par.Position = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 3f * invert1, (float)rnd.NextDouble() * 2f, 
                    (float)rnd.NextDouble () * 3f * invert2)) + SpawnPosition;

                par.Velocity = new FreezingArcher.Math.Vector3 (0.0f, (float)rnd.NextDouble () * 5f, 0);
                par.Mass = 1.0f;
                par.Size = new Vector2(0.9f, 0.9f);
                par.Color = new FreezingArcher.Math.Color4(0.1f, 0.1f , 0.1f, 1f);
                par.Life = 1.2f;
                par.LifeTime = 3.0f * (float)rnd.NextDouble();
            }


        }

        #endregion
    }
}

