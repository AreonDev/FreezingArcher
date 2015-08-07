//
//  MazeBitmapExporter.cs
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
using FreezingArcher.Math;
using System.Collections.Generic;
using FreezingArcher.DataStructures.Graphs;
using System.Drawing;
using System.Drawing.Imaging;

namespace FreezingArcher.Game.Maze
{
    public static class MazeImageExporter
    {
        static class MazeColors
        {
            public static Pen Ground = new Pen(Color.LightGray, 1);

            public static Pen PathToExit = new Pen(Color.White, 1);

            public static Pen Wall = new Pen(Color.Black, 1);

            public static Pen Spawn = new Pen(Color.Green, 1);

            public static Pen Portal = new Pen(Color.Blue, 1);

            public static Pen Exit = new Pen(Color.Red, 1);
        }

        public static void ExportAsImage(this Maze maze, string filepath)
        {
            int x = 0;
            int y = 0;

            var startNode = maze.graph.Nodes.FirstOrDefault (n => n.Data.IsSpawn);

            Vector2i playerPosition = Vector2i.Zero;

            if (startNode != null)
            {
                playerPosition = startNode.Data.Position;
            }

            Bitmap bmp = new Bitmap(maze.Size.X, maze.Size.Y);

            using (var g = Graphics.FromImage(bmp))
            {
                foreach (var node in (IEnumerable<WeightedNode<MazeCell, MazeCellEdgeWeight>>) maze.graph)
                {
                    if (node.Data.MazeCellType == MazeCellType.Ground)
                    {
                        if (node.Data.IsSpawn)
                        {
                            g.DrawRectangle(MazeColors.Spawn, x, y, 1, 1);
                        }
                        else if (node.Data.IsExit)
                        {
                            g.DrawRectangle(MazeColors.Exit, x, y, 1, 1);
                        }
                        else if (node.Data.IsPortal)
                        {
                            g.DrawRectangle(MazeColors.Portal, x, y, 1, 1);
                        }
                        else if (node.Data.IsPath)
                        {
                            g.DrawRectangle(MazeColors.PathToExit, x, y, 1, 1);
                        }
                        else
                        {
                            g.DrawRectangle(MazeColors.Ground, x, y, 1, 1);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(MazeColors.Wall, x, y, 1, 1);
                    }

                    if (++x >= maze.Size.X)
                    {
                        x = 0;
                        y++;
                    }
                }
            }

            bmp.Save(filepath, ImageFormat.Png);
        }
    }
}
