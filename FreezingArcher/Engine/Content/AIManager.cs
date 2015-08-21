//
//  AIManager.cs
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
using System.Collections.Generic;
using FreezingArcher.DataStructures.Graphs;
using FreezingArcher.Math;
using FreezingArcher.Output;

namespace FreezingArcher.Content
{
    public class AIManager
    {
        public AIManager (object map, Random rand)
        {
            Map = map;
            this.rand = rand;
        }

        readonly List<Entity> entities = new List<Entity>();

        public object Map { get; private set; }

        readonly Random rand;

        public List<Entity> CollectEntitiesNearby (Vector3 position, float maximumEntityDistance)
        {
            List<Entity> nearby = new List<Entity>();

            float d;
            lock (entities)
            {
                Vector3 pos;
                foreach (var e in entities)
                {
                    pos = e.GetComponent<TransformComponent> ().Position;
                    Vector3.Distance (ref pos, ref position, out d);
                    if (d <= maximumEntityDistance)
                        nearby.Add (e);
                }
            }

            return nearby;
        }

        public void RegisterEntity (Entity entity)
        {
            if (!entity.HasComponent<TransformComponent>())
            {
                Logger.Log.AddLogEntry (LogLevel.Error, "AIManager",
                    "An error occured while registering new entity in AIManager: " +
                    "'{0}' has no TransformComponent!}");
            }

            lock (entities)
            {
                entities.Add (entity);
            }
        }

        public void UnregisterEntity (Entity entity)
        {
            lock (entity)
            {
                entities.Remove (entity);
            }
        }

        public void CalculateSpawnPositions ()
        {
            lock (entities)
            {
                foreach (var e in entities)
                {
                    if (e.HasComponent<ArtificialIntelligenceComponent>())
                    {
                        var ai_component = e.GetComponent<ArtificialIntelligenceComponent>();
                        ai_component.ArtificialIntelligence.SetSpawnPosition (e.GetComponent<PhysicsComponent>(), Map,
                            rand);
                    }
                }
            }
        }
    }
}
