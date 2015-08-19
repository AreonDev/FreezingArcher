//
//  ScobisSmokeParticleEmitter.cs
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
using FreezingArcher.Renderer.Scene;

namespace FreezingArcher.Game
{
    public class ScobisSmokeParticleEmitter : ParticleEmitter
    {
        public ScobisSmokeParticleEmitter () : base(200)
        {
        }

        Random rnd = new Random();

        #region implemented abstract members of ParticleEmitter

        protected override void UpdateParticle (Particle par, float time)
        {
            if (par.Life >= 0.05f) 
            {
                if ((par.Position - SpawnPosition).Length > 9.05f) {
                    par.Color = new FreezingArcher.Math.Color4 (par.Color.R, par.Color.G, par.Color.B, (1 - (par.Age / par.LifeTime)));

                    par.Velocity = Math.Vector3.Zero;
                    par.Update (time);
                } else if ((par.Position - SpawnPosition).Length > 2.05f) {
                    if (par.Color.A < (1 - (par.Age / par.LifeTime)))
                        par.Color = new FreezingArcher.Math.Color4 (par.Color.R, par.Color.G, par.Color.B, par.Color.A + time * 0.3f);
                    else
                        par.Color = new FreezingArcher.Math.Color4 (par.Color.R, par.Color.G, par.Color.B, (1 - (par.Age / par.LifeTime)));

                    FreezingArcher.Math.Vector3 actual = par.Velocity;
                    par.Velocity = actual;
                    par.Update (time);
                } else 
                {
                    par.Color = new FreezingArcher.Math.Color4 (par.Color.R, par.Color.G, par.Color.B, par.Color.A - time * 0.02f);
                    FreezingArcher.Math.Vector3 actual = par.Velocity;
                    par.Velocity = actual;
                    par.Update (time);
                }

            } else
            {
                par.Reset ();

                float invert1 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert2 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;

                par.Position = SpawnPosition + (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 0.05f * invert1, 0.0f, 
                    (float)rnd.NextDouble () * 0.05f * invert2));

                par.Velocity = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 0.7f * invert1, 0.0f, 
                    (float)rnd.NextDouble () * 0.7f * invert2));

                par.Mass = 1.0f;
                par.Size = new FreezingArcher.Math.Vector2(1.3f, 0.7f);
                par.Color = new FreezingArcher.Math.Color4(0.4f, 0.4f , 0.0f, 0.01f);
                par.Life = 1.2f;
                par.LifeTime = 20.0f * (float)rnd.NextDouble();
            }
        }

        protected override void InitializeParticles (FreezingArcher.Renderer.RendererContext rc)
        {
            foreach (Particle par in Particles) 
            {
                par.Reset ();

                float invert1 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;
                float invert2 = Convert.ToBoolean (rnd.Next (0, 2)) ? -1 : 1;

                par.Position = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 0.8f * invert1, 0.0f, 
                    (float)rnd.NextDouble () * 0.8f * invert2)) + SpawnPosition;

                //par.Velocity = (new FreezingArcher.Math.Vector3 ((float)rnd.NextDouble () * 10.5f, (float)rnd.NextDouble () * 10.5f, (float)rnd.NextDouble () * 10.5f))*invert;
                par.Mass = 1.0f;
                par.Size = new FreezingArcher.Math.Vector2(1.3f, 0.7f);
                par.Color = new FreezingArcher.Math.Color4(0.4f, 0.4f , 0.0f, 0.01f);
                par.Life = 1.2f;
                par.LifeTime = 20.0f * (float)rnd.NextDouble();
            }
        }

        #endregion
    }
}

