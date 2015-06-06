//
//  MapNode.cs
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
using FreezingArcher.Renderer.Scene.SceneObjects;

namespace FreezingArcher.Game.Maze
{
    /// <summary>
    /// Map node.
    /// </summary>
    public sealed class MazeCell
    {
        /// <summary>
        /// The color of the wall.
        /// </summary>
        public static Color4 WallColor = Color4.DarkSalmon;
        /// <summary>
        /// The color of the preview wall.
        /// </summary>
        public static Color4 PreviewWallColor = Color4.Chocolate;
        /// <summary>
        /// The color of the ground.
        /// </summary>
        public static Color4 GroundColor = Color4.NavajoWhite;
        /// <summary>
        /// The final color of the ground.
        /// </summary>
        public static Color4 FinalGroundColor = Color4.WhiteSmoke;
        /// <summary>
        /// The color of an undefined cell.
        /// </summary>
        public static Color4 UndefinedColor = Color4.Fuchsia;
        /// <summary>
        /// The color of an error cell.
        /// </summary>
        public static Color4 ErrorColor = Color4.IndianRed;
        /// <summary>
        /// The color of a dead end cell.
        /// </summary>
        public static Color4 PathColor = Color4.DeepSkyBlue;
        /// <summary>
        /// The color of the portal.
        /// </summary>
        public static Color4 PortalColor = Color4.DarkOrchid;
        /// <summary>
        /// The color of the spawn.
        /// </summary>
        public static Color4 SpawnColor = Color4.DeepPink;
        /// <summary>
        /// The color of the exit.
        /// </summary>
        public static Color4 ExitColor = Color4.LimeGreen;

        /// <summary>
        /// Initializes a new instance of the MapNode class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="position">Position.</param>
        /// <param name="weight">Weight.</param>
        /// <param name="rectangles">Rectangles.</param>
        /// <param name="preview">If set to <c>true</c> preview.</param>
        public MazeCell(string name, Vector2i position, int weight, RectangleSceneObject[,] rectangles, bool preview = false)
        {
            Weight = weight;
            this.preview = preview;
            Name = name;
            recs = rectangles;
            Position = position;
            final = false;
        }

        MazeCellType mazeType;
        bool preview;
        bool final;
        bool isPortal;
        bool isSpawn;
        bool isExit;

        readonly RectangleSceneObject[,] recs;

        /// <summary>
        /// The type of the labyrinth item.
        /// </summary>
        public MazeCellType MazeType
        {
            get
            {
                return mazeType;
            }
            set
            {
                mazeType = value;
                updateColor();
            }
        }

        /// <summary>
        /// The weight.
        /// </summary>
        public int Weight;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FreezingArcher.Game.Maze.MazeCell"/> is a preview
        /// node.
        /// </summary>
        /// <value><c>true</c> if preview; otherwise, <c>false</c>.</value>
        public bool Preview
        {
            get
            {
                return preview;
            }
            set
            {
                preview = value;
                updateColor();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FreezingArcher.Game.Maze.MazeCell"/> is a final node.
        /// </summary>
        /// <value><c>true</c> if final; otherwise, <c>false</c>.</value>
        public bool Final
        {
            get
            {
                return final;
            }
            set
            {
                final = value;
                updateColor();
            }
        }

        /// <summary>
        /// The name.
        /// </summary>
        public string Name;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a portal.
        /// </summary>
        /// <value><c>true</c> if this instance is a portal; otherwise, <c>false</c>.</value>
        public bool IsPortal
        {
            get
            {
                return isPortal;
            }
            set
            {
                isPortal = value;
                updateColor();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is the player spawn.
        /// </summary>
        /// <value><c>true</c> if this instance is spawn; otherwise, <c>false</c>.</value>
        public bool IsSpawn
        {
            get
            {
                return isSpawn;
            }
            set
            {
                isSpawn = value;
                updateColor();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this node is an exit node.
        /// </summary>
        /// <value><c>true</c> if this instance is exit; otherwise, <c>false</c>.</value>
        public bool IsExit
        {
            get
            {
                return isExit;
            }
            set
            {
                isExit = value;
                updateColor();
            }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector2i Position { get; private set; }

        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init()
        {
            updateColor();
        }

        void updateColor()
        {
            if (MazeType == MazeCellType.Wall && !Preview)
                recs[Position.X, Position.Y].Color = WallColor;
            else if (MazeType == MazeCellType.Wall && Preview)
                recs[Position.X, Position.Y].Color = PreviewWallColor;
            else if (MazeType == MazeCellType.Ground && IsPortal)
                recs[Position.X, Position.Y].Color = PortalColor;
            else if (MazeType == MazeCellType.Ground && IsSpawn)
                recs[Position.X, Position.Y].Color = SpawnColor;
            else if (MazeType == MazeCellType.Ground && IsExit)
                recs[Position.X, Position.Y].Color = ExitColor;
            else if (MazeType == MazeCellType.Ground && !Final)
                recs[Position.X, Position.Y].Color = GroundColor;
            else if (MazeType == MazeCellType.Ground && Final)
                recs[Position.X, Position.Y].Color = FinalGroundColor;
            else if (MazeType == MazeCellType.Undefined)
                recs[Position.X, Position.Y].Color = UndefinedColor;
            else
                recs[Position.X, Position.Y].Color = ErrorColor;
        }
    }
}
