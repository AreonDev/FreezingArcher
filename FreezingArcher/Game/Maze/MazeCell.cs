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
        /// Initializes a new instance of the MapNode class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="position">Position.</param>
        /// <param name="weight">Weight.</param>
        /// <param name="preview">If set to <c>true</c> preview.</param>
        public MazeCell(string name, Vector2i position, int weight, //ModelSceneObject[,] rectangles,
            /*MazeColorTheme theme,*/ bool preview = false)
        {
            Weight = weight;
            this.preview = preview;
            Name = name;
            //models = rectangles;
            Position = position;
            //this.theme = theme;
            final = false;
        }

        MazeCellType mazeType;
        //MazeColorTheme theme;
        bool preview;
        bool final;
        bool isPortal;
        bool isSpawn;
        bool isExit;
        bool isPath;

        //readonly ModelSceneObject[,] models;

        /// <summary>
        /// The type of the labyrinth item.
        /// </summary>
        public MazeCellType MazeCellType
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
        public bool IsPreview
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
        public bool IsFinal
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
        /// Gets or sets a value indicating whether this instance is a dead end.
        /// </summary>
        /// <value><c>true</c> if this instance is a dead end; otherwise, <c>false</c>.</value>
        public bool IsDeadEnd { get; set; }

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
        /// Gets or sets a value indicating whether this instance is part of the path going out of the maze.
        /// </summary>
        /// <value><c>true</c> if this instance is path; otherwise, <c>false</c>.</value>
        public bool IsPath
        {
            get
            {
                return isPath;
            }
            set
            {
                isPath = value;
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
            /*if (MazeCellType == MazeCellType.Wall && !IsPreview)
                models[Position.X, Position.Y].Color = theme.WallColor;
            else if (MazeCellType == MazeCellType.Wall && IsPreview)
                models[Position.X, Position.Y].Color = theme.PreviewWallColor;
            else if (MazeCellType == MazeCellType.Ground && IsPortal)
                models[Position.X, Position.Y].Color = theme.PortalColor;
            else if (MazeCellType == MazeCellType.Ground && IsSpawn)
                models[Position.X, Position.Y].Color = theme.SpawnColor;
            else if (MazeCellType == MazeCellType.Ground && IsExit)
                models[Position.X, Position.Y].Color = theme.ExitColor;
            else if (MazeCellType == MazeCellType.Ground && IsPath)
                models[Position.X, Position.Y].Color = theme.PathColor;
            else if (MazeCellType == MazeCellType.Ground && !IsFinal)
                models[Position.X, Position.Y].Color = theme.GroundColor;
            else if (MazeCellType == MazeCellType.Ground && IsFinal)
                models[Position.X, Position.Y].Color = theme.FinalGroundColor;
            else if (MazeCellType == MazeCellType.Undefined)
                models[Position.X, Position.Y].Color = theme.UndefinedColor;
            else
                models[Position.X, Position.Y].Color = theme.ErrorColor;*/
            // TODO
        }
    }
}
