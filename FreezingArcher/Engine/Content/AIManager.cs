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
        public AIManager (WeightedGraph<IMapNodeData, IMapEdgeWeight> map)
        {
            Map = map;
        }

        readonly List<Entity> entities = new List<Entity>();

        public WeightedGraph<IMapNodeData, IMapEdgeWeight> Map { get; private set; }

        public List<Entity> CollectEntitiesNearby (TransformComponent transform, float maximumEntityDistance)
        {
            List<Entity> nearby = new List<Entity>();

            float d;
            lock (entities)
            {
                Vector3 pos1, pos2;
                pos2 = transform.Position;
                foreach (var e in entities)
                {
                    pos1 = e.GetComponent<TransformComponent> ().Position;
                    Vector3.Distance (ref pos1, ref pos2, out d);
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
    }
}
