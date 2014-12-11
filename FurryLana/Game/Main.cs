//
//  Main.cs
//
//  Author:
//       Fin Christensen <christensen.fin@gmail.com>
//
//  Copyright (c) 2014 Fin Christensen
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
using FurryLana.Engine.Application;
using FurryLana.Engine.Entity;
using FurryLana.Engine.Entity.Interfaces;
using FurryLana.Engine.Game;
using FurryLana.Engine.Game.Interfaces;
using FurryLana.Engine.Map;
using FurryLana.Engine.Map.Interfaces;
using Pencil.Gaming.MathUtils;

namespace FurryLana.Game
{
    /// <summary>
    /// FurryLana static main class.
    /// </summary>
    public class FurryLana
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main (string[] args)
        {
            IGame game = new global::FurryLana.Engine.Game.Game ("FurryLana");
            IMap map = new TiledMap (5f, new Vector2i (10, 10));
            IEntity entity = new SnakeBody ();
            game.LevelManager.Add (new Level ("Introduction", map, new ProjectionDescription ()));
            game.LevelManager.GetByName ("Introduction").Entities.Add (entity);
            Application.Instance = new Application (game);
            Application.Instance.Init ();
            Application.Instance.Load ();
            Application.Instance.Run ();
            Application.Instance.Destroy ();
        }
    }
}
