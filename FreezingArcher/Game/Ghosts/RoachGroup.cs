//
//  Roach.cs
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
using FreezingArcher.Content;
using System.Collections.Generic;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Math;
using FreezingArcher.Core;
using FreezingArcher.Renderer.Scene;

namespace FreezingArcher.Game.Ghosts
{
    public class RoachGroup
    {
        public RoachGroup (CoreScene scene)
        {
            Roaches = new ModelSceneObject[5];
            offset = new Vector3[Roaches.Length];

            for (int i = 0; i < Roaches.Length; i++)
            {
                Roaches[i] = new ModelSceneObject ("lib/Renderer/TestGraphics/Roach/roach.xml");
                scene.AddObject(Roaches[i]);
            }

            offset[0] = new Vector3(1 , 0, 1    ) / 10;
            offset[1] = new Vector3(-2, 0, 1    ) / 10;
            offset[2] = new Vector3(2 , 0, 0    ) / 10;
            offset[3] = new Vector3(0 , 0, 1    ) / 10;
            offset[4] = new Vector3(-2, 0, -1.5f) / 10;
        }

        FastRandom random = new FastRandom();

        const float maximumDistance = 2;

        public ModelSceneObject[] Roaches;
        Vector3[] offset;

        Vector3 position;

        void updateRoachPosition()
        {
            float distOverTwo = maximumDistance / 2;
            for (int i = 0; i < Roaches.Length; i++)
            {
                //float xOffs = offset[i].X + (float) (random.NextDouble() - 0.5) / 1000;
                //float zOffs = offset[i].Z + (float) (random.NextDouble() - 0.5) / 1000;

                //offset[i] = Vector3.Transform(Vector3.UnitX, Quaternion.FromAxisAngle(Vector3.UnitY, random.NextDouble() * MathHelper.PiOver3));

                /*if (xOffs > distOverTwo)
                {
                    xOffs = distOverTwo;
                }
                else if (xOffs < -distOverTwo)
                {
                    xOffs = -distOverTwo;
                }

                if (zOffs > distOverTwo)
                {
                    zOffs = distOverTwo;
                }
                else if (zOffs < -distOverTwo)
                {
                    zOffs = distOverTwo;
                }

                offset[i].X = xOffs;
                offset[i].Z = zOffs;*/

                var newPosition = new Vector3(position.X + offset[i].X, position.Y, position.Z + offset[i].Z);

                var direction = newPosition - Roaches[i].Position;
                float rotation = Vector3.CalculateAngle(Vector3.UnitX, new Vector3 (direction.X, 0, direction.Z));
                Roaches[i].Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, rotation);
                Roaches[i].Position = newPosition;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
                updateRoachPosition();
            }
        }
    }
}
