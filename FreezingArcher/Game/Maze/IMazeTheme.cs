﻿//
//  IMazeTheme.cs
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
using FreezingArcher.Messaging;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Core;

namespace FreezingArcher.Game.Maze
{
    public interface IMazeTheme
    {
        void Init (GameState state);

        Entity ProcessAndAddCell (MazeCell cell, Vector3 worldPosition, Vector2i gridPosition, uint sizex, uint sizey);

        void Finish ();
    }
}
