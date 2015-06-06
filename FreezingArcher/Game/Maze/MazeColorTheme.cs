//
//  MazeColorTheme.cs
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
using FreezingArcher.Math;

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Maze color theme.
    /// </summary>
    public class MazeColorTheme
    {
        /// <summary>
        /// The overworld color theme.
        /// </summary>
        public static MazeColorTheme Overworld = new MazeColorTheme (Color4.DarkSalmon, Color4.DarkKhaki,
            Color4.NavajoWhite, Color4.WhiteSmoke, Color4.Fuchsia, Color4.IndianRed, Color4.LightBlue,
            Color4.DarkOrchid, Color4.DeepPink, Color4.LimeGreen);

        /// <summary>
        /// The underworld color theme.
        /// </summary>
        public static MazeColorTheme Underworld = new MazeColorTheme (Color4.BurlyWood, Color4.DarkGoldenrod,
            Color4.GhostWhite, Color4.FloralWhite, Color4.Purple, Color4.IndianRed, Color4.LightBlue,
            Color4.DarkOrchid, Color4.DeepPink, Color4.LimeGreen);

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Game.Maze.MazeColorTheme"/> class.
        /// </summary>
        /// <param name="wall">Wall.</param>
        /// <param name="previewWall">Preview wall.</param>
        /// <param name="ground">Ground.</param>
        /// <param name="finalGround">Final ground.</param>
        /// <param name="undefined">Undefined.</param>
        /// <param name="error">Error.</param>
        /// <param name="path">Path.</param>
        /// <param name="portal">Portal.</param>
        /// <param name="spawn">Spawn.</param>
        /// <param name="exit">Exit.</param>
        public MazeColorTheme (Color4 wall, Color4 previewWall, Color4 ground, Color4 finalGround, Color4 undefined,
            Color4 error, Color4 path, Color4 portal, Color4 spawn, Color4 exit)
        {
            WallColor = wall;
            PreviewWallColor = previewWall;
            GroundColor = ground;
            FinalGroundColor = finalGround;
            UndefinedColor = undefined;
            ErrorColor = error;
            PathColor = path;
            PortalColor = portal;
            SpawnColor = spawn;
            ExitColor = exit;
        }

        /// <summary>
        /// The color of the wall.
        /// </summary>
        public readonly Color4 WallColor;
        /// <summary>
        /// The color of the preview wall.
        /// </summary>
        public readonly Color4 PreviewWallColor;
        /// <summary>
        /// The color of the ground.
        /// </summary>
        public readonly Color4 GroundColor;
        /// <summary>
        /// The final color of the ground.
        /// </summary>
        public readonly Color4 FinalGroundColor;
        /// <summary>
        /// The color of an undefined cell.
        /// </summary>
        public readonly Color4 UndefinedColor;
        /// <summary>
        /// The color of an error cell.
        /// </summary>
        public readonly Color4 ErrorColor;
        /// <summary>
        /// The color of a dead end cell.
        /// </summary>
        public readonly Color4 PathColor;
        /// <summary>
        /// The color of the portal.
        /// </summary>
        public readonly Color4 PortalColor;
        /// <summary>
        /// The color of the spawn.
        /// </summary>
        public readonly Color4 SpawnColor;
        /// <summary>
        /// The color of the exit.
        /// </summary>
        public readonly Color4 ExitColor;
    }
}
