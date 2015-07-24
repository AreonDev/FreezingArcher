//
//  ParticleSystem.cs
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
using System.Threading;
using FreezingArcher.Math;
using FreezingArcher.Renderer;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Output;

namespace FreezingArcher.Game
{
    public abstract class ParticleEmitter
    {
        public int ParticleCount { get; private set;}
        public Particle[] Particles { get; private set;}

        public ParticleSceneObject SceneObject { get; set;}

        public Vector3 SpawnPosition { get; set;}

        public ParticleEmitter (int particlecount)
        {
            ParticleCount = particlecount;

            Particles = new Particle[ParticleCount];

            for (int i = 0; i < ParticleCount; i++)
                Particles [i] = new Particle ();

            updatethread = new Thread (InnerUpdate);
            updatethread.Start ();

            time = 0.0f;
        }

        public virtual bool Init(ParticleSceneObject obj, RendererContext rc)
        {
            SceneObject = obj;

            InitializeParticles (rc);

            return true;
        }

        public virtual void EndUpdate(float time) {}

        public abstract void UpdateParticle(Particle par, float time);
        public abstract void InitializeParticles(RendererContext rc);

        readonly AutoResetEvent updateEvent = new AutoResetEvent(false);
        bool running = false;
        bool particleUpdateRunning = false;
        float time;

        private void InnerUpdate()
        {
            running = true;
            while (running)
            {
                updateEvent.WaitOne();

                particleUpdateRunning = true;
                for (int i = 0; i < ParticleCount; i++) 
                {
                    UpdateParticle (Particles [i], (float)time);
                    if (SceneObject != null) 
                    {
                        SceneObject.Particles [i].Position = Particles [i].Position;
                        SceneObject.Particles [i].Color = Particles [i].Color;
                        SceneObject.Particles [i].Life = Particles [i].Life;
                        SceneObject.Particles [i].Size = Particles [i].Size;
                    }
                }

                EndUpdate (time);

                particleUpdateRunning = false;
            }
        }

        Thread updatethread;

        public void Update(float time)
        {
            if (particleUpdateRunning)
            {
                Logger.Log.AddLogEntry(LogLevel.Warning, "ParticleEmitter",
                    "A particle update is already running - ignoring update request...\n" +
                    "A possible reason could be too many objects in the scene, the update cycle is too fast or your " +
                    "computer is too slow."
                );   
            }
            else
            {
                this.time = time;
                if (running)
                    updateEvent.Set();
            }
        }

        /// <summary>
        /// Destroy this instance.
        /// </summary>
        public virtual void Destroy ()
        {
            running = false;
            updateEvent.Close();
            updateEvent.Dispose();
            updatethread.Abort();
        }
    }
}

