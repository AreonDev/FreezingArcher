//
//  ItemTemplate.cs
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
using FreezingArcher.Math;
using FreezingArcher.Content;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;

namespace FreezingArcher.Game.Maze
{
    public class ItemTemplate
    {
        public string Name { get; set; }

        public string ModelPath { get; set; }

        public string ImageLocation { get; set; }

        public Vector3 PositionOffset { get; set; }

        public Quaternion Rotation { get; set; }

        public string Description { get; set; }

        public AttackClass AttackClasses { get; set; }

        public ItemUsage ItemUsages { get; set; }

        public Protection Protection { get; set; }

        public Vector2i Size { get; set; }

        public float HealthDelta { get; set; }

        public float AttackStrength { get; set; }

        public float ThrowPower { get; set; }

        public float Mass { get; set; }

        public Material PhysicsMaterial { get; set; }

        public float UsageDeltaPerUsage { get; set; }

        public float Usage { get; set; }

        public Shape Shape { get; set; }
    }
}
