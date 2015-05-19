//
//  PositionComponen.cs
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
using System.Threading;

namespace FreezingArcher.Content
{
    // must be sealed
    public class TransformComponent : EntityComponent
    {
        // has to pass cause of FAObject
        public override void Init(Entity entity)
        {

        }

        // must fail cause its private
        private int foo;

        // has to pass
        public Vector3 Position { get; set; }

        // has to fail
        public Quaternion Rotation { get; private set; }

        // has to pass
        internal Vector3 Scale { get; set; }

        // must fail
        protected int bar;

        // fail
        int asdf () {return 0;}

        // fail
        protected internal void qwert() {}
    }
}
