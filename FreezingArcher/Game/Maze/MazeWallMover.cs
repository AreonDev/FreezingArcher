//
//  MazeWallMover.cs
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using FreezingArcher.DataStructures.Graphs;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Messaging;
using System.Threading;

namespace FreezingArcher.Game.Maze
{
    public sealed class MazeWallMover : IMessageConsumer
    {
        public MazeWallMover (Maze maze, MessageProvider messageProvider)
        {
            Maze = maze;
            rand = new Random(maze.Seed);
            ValidMessages = new[] { (int) MessageId.Update };
            messageProvider += this;
        }

        public Maze Maze { get; private set; }

        readonly Random rand;

        public void Step ()
        {
            foreach (var deadEnd in GetDeadEndGrounds ())
            {
                if (rand.Next() % 100 != 0)
                    continue;

                WeightedNode<MazeCell, MazeCellEdgeWeight> connection = null;
                var new_ground_edge = deadEnd.Edges.FirstOrDefault(e => {
                    var n = e.FirstNode != deadEnd ? e.FirstNode : e.SecondNode;
                    if (n.Data.MazeCellType == MazeCellType.Wall && e.Weight.Direction != Direction.Diagonal)
                    {
                        foreach (var e2 in n.Edges)
                        {
                            var n2 = e2.FirstNode != n ? e2.FirstNode : e2.SecondNode;
                            if (e2.Weight.Direction != Direction.Diagonal &&
                                n2.Data.MazeCellType == MazeCellType.Ground && !n2.Data.IsDeadEnd && !n2.Data.IsPortal)
                            {
                                connection = n2;
                                break;
                            }
                        }

                        return connection != null;
                    }
                    return false;
                });

                if (connection == null)
                    continue;
                
                var grounds = connection.Edges.Where (e => {
                    var n = e.FirstNode != connection ? e.FirstNode : e.SecondNode;
                    return n.Data.MazeCellType == MazeCellType.Ground;
                });

                if (grounds.Any (g => g.Weight.Direction != grounds.ElementAt (0).Weight.Direction) ||
                    grounds.Count () > 2)
                    continue;

                if (new_ground_edge != null)
                {
                    var tmp2 = connection.Edges.FirstOrDefault(e => e.Weight.IsNextGenerationStep);
                    var tmp = connection.Edges.FirstOrDefault(e => e.Weight.Direction == tmp2.Weight.Direction && e != tmp2);
                    if (tmp != null)
                    {
                        var next_gen_ground = tmp.FirstNode != connection ? tmp.FirstNode : tmp.SecondNode;
                        next_gen_ground.Data.MazeCellType = MazeCellType.Wall; // here
                        var new_ground = new_ground_edge.FirstNode != deadEnd ?
                            new_ground_edge.FirstNode : new_ground_edge.SecondNode;
                        new_ground.Data.MazeCellType = MazeCellType.Ground; // here
                        deadEnd.Data.IsDeadEnd = false;
                        new_ground_edge.Weight.IsNextGenerationStep = true;

                        foreach (var e in new_ground.Edges)
                        {
                            var n = e.FirstNode != new_ground ? e.FirstNode : e.SecondNode;
                            if (n.Data.MazeCellType == MazeCellType.Ground)
                            {
                                n.Data.IsDeadEnd = true;
                                e.Weight.IsNextGenerationStep = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        IEnumerable<WeightedNode<MazeCell, MazeCellEdgeWeight>> GetDeadEndGrounds ()
        {
            return Maze.graph.Nodes.Where(n => n.Data.IsDeadEnd && !n.Data.IsPortal);
        }

        #region IMessageConsumer implementation
        public void ConsumeMessage (IMessage msg)
        {
            if (msg.MessageId == (int) MessageId.Update)
            {
                var um = msg as UpdateMessage;

            }
        }

        public int[] ValidMessages { get; private set; }
        #endregion
    }
}
