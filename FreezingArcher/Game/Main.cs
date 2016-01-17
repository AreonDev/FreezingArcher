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
using FreezingArcher.Core;
using FreezingArcher.Renderer.Compositor;
using FreezingArcher.Localization;
using Gwen.Control.Property;

namespace FreezingArcher.Game
{
    /// <summary>
    /// FurryLana static main class.
    /// </summary>
    public class FurryLana
    {
        public static Audio.Source MainMenuMusic;

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main (string[] args)
        {
            Application.Instance = new Application ("No Way Out", args);
            Application.Instance.Init ();
            Application.Instance.Load ();

            Application.Instance.AudioManager.LoadSound ("main_menu_Sound", "Content/Audio/main_menu.ogg");
            MainMenuMusic = Application.Instance.AudioManager.CreateSource ("main_menu_SoundSource", "main_menu_Sound");
            MainMenuMusic.Loop = true;
            MainMenuMusic.Gain = 0.6f;

            MainMenuMusic.Play ();

            MazeTest maze = null;
            if (!Application.Instance.IsCommandLineInterface)
            {
                Content.Game game = Application.Instance.Game;
                var rc = Application.Instance.RendererContext;
                var messageManager = Application.Instance.MessageManager;
                var objmnr = Application.Instance.ObjectManager;

                var compositor = new BasicCompositor (objmnr, rc);

                var sceneNode = new CompositorNodeScene (rc, messageManager);
                var outputNode = new CompositorNodeOutput (rc, messageManager);
                var colorCorrectionNode = new CompositorColorCorrectionNode (rc, messageManager);
                var healthOverlayNode = new CompositorImageOverlayNode (rc, messageManager);
                var warpingNode = new CompositorWarpingNode (rc, messageManager);

                compositor.AddNode (sceneNode);
                compositor.AddNode (outputNode);
                compositor.AddNode (healthOverlayNode);
                compositor.AddNode (colorCorrectionNode);
                compositor.AddNode (warpingNode);

                compositor.AddConnection (sceneNode, healthOverlayNode, 0, 0);
                compositor.AddConnection (healthOverlayNode, colorCorrectionNode, 0, 0);
                compositor.AddConnection (colorCorrectionNode, warpingNode, 0, 0);
                compositor.AddConnection (warpingNode, outputNode, 0, 0);

                rc.Compositor = compositor;

                new MainMenu(Application.Instance, () => {
                    outputNode.EnableUI = false;
                    maze = new MazeTest(messageManager, objmnr, rc, game, Application.Instance, sceneNode,
                        healthOverlayNode, colorCorrectionNode, outputNode, warpingNode);
                    outputNode.EnableUI = true;
                    maze.Generate();
                }, colorCorrectionNode);
            }

            Application.Instance.Run ();
            if (maze != null)
                maze.Destroy ();
            Application.Instance.Destroy ();
        }
    }
}
