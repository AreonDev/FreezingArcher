//
//  RandomParticleSystem.cs
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

using FreezingArcher.Renderer;

namespace FreezingArcher.Game
{
    public class NiceGhostParticleEmitter : ParticleEmitter
    {
        public NiceGhostParticleEmitter () : base(100)
        {
            //SpawnPosition = Math.Vector3.Zero;
        }

        #region implemented abstract members of ParticleEmitter

        Random rnd = new Random ();

        public override void UpdateParticle (Particle par, float time)
        {
            if (par.Life >= 0.05f) 
            {
                if ((par.Position - SpawnPosition).Length > 0.7f)
                {
                    par.Color = new FreezingArcher.Math.Color4(par.Color.R, par.Color.G, par.Color.B, par.Color.A - time * 0.40f);

                    par.Velocity = Math.Vector3.Zero;
                    par.Update (time);
                } else 
                {
                    //par.Life -= time * 4.0f;

                    if (par.Color.A < 0.7f)
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
                par.Size = new FreezingArcher.Math.Vector2(1.2f, 1.2f);
                par.Color = new FreezingArcher.Math.Color4(1.0f, 1.0f , 1.0f, 0.3f);
            }
        }

        public override void InitializeParticles (RendererContext rc)
        {
            if (SceneObject != null) 
            {

            }

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
                par.Size = new FreezingArcher.Math.Vector2(1.2f, 1.2f);
                par.Color = new FreezingArcher.Math.Color4(1.0f, 1.0f , 1.0f, 0.5f);
                par.Life = 1.2f;
                par.LifeTime = 1.0f * (float)rnd.NextDouble();
            }
        }

        #endregion
    }
}

