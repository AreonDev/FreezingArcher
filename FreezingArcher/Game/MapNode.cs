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

namespace FreezingArcher.Game
{
    /// <summary>
    /// Map node.
    /// </summary>
    public sealed class MapNode
    {
        /// <summary>
        /// The color of the wall.
        /// </summary>
        public static Color4 WallColor = Color4.Chocolate;
        /// <summary>
        /// The color of the preview wall.
        /// </summary>
        public static Color4 PreviewWallColor = Color4.DarkGoldenrod;
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
        /// Initializes a new instance of the MapNode class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="position">Position.</param>
        /// <param name="weight">Weight.</param>
        /// <param name="rectangles">Rectangles.</param>
        /// <param name="preview">If set to <c>true</c> preview.</param>
        public MapNode(string name, Vector2i position, int weight, RectangleSceneObject[,] rectangles, bool preview = false)
        {
            Weight = weight;
            this.preview = preview;
            Name = name;
            recs = rectangles;
            Position = position;
            final = false;
        }

        LabyrinthItemType labtype;
        bool preview;
        bool final;

        RectangleSceneObject[,] recs;

        /// <summary>
        /// The type of the labyrinth item.
        /// </summary>
        public LabyrinthItemType LabyrinthType
        {
            get
            {
                return labtype;
            }
            set
            {
                labtype = value;
                updateColor();
            }
        }

        /// <summary>
        /// The weight.
        /// </summary>
        public int Weight;

        /// <summary>
        /// The preview flag.
        /// </summary>
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
        /// The final flag.
        /// </summary>
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
        /// The position.
        /// </summary>
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
            if (LabyrinthType == LabyrinthItemType.Wall && !Preview)
                recs[Position.X, Position.Y].Color = WallColor;
            else if (LabyrinthType == LabyrinthItemType.Wall && Preview)
                recs[Position.X, Position.Y].Color = PreviewWallColor;
            else if (LabyrinthType == LabyrinthItemType.Ground && !Final)
                recs[Position.X, Position.Y].Color = GroundColor;
            else if (LabyrinthType == LabyrinthItemType.Ground && Final)
                recs[Position.X, Position.Y].Color = FinalGroundColor;
            else if (LabyrinthType == LabyrinthItemType.Undefined)
                recs[Position.X, Position.Y].Color = UndefinedColor;
            else
                recs[Position.X, Position.Y].Color = ErrorColor;
        }
    }
}

