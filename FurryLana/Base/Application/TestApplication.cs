//
//  TestApplication.cs
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
using System;
using FurryLana.Engine.Game.Interfaces;
using FurryLana.Engine.Graphics;
using FurryLana.Engine.Input.Interfaces;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using FurryLana.Base.Application.Interfaces;

namespace FurryLana.Base.Application
{
    public class TestApplication : IApplication
    {
        #region IApplication implementation

        public void Run ()
        {
            CreateTable ();
            
            Window.WindowResize = (GlfwWindowPtr window, int width, int height) => {
                WriteAt (1, 5, "       ");
                WriteAt (1, 5, width.ToString ());
                WriteAt (9, 5, "        ");
                WriteAt (9, 5, height.ToString ());
                GL.Viewport (0, 0, width, height);
            };
            
            Window.WindowMove = (GlfwWindowPtr window, int x, int y) => {
                WriteAt (18, 5, "       ");
                WriteAt (18, 5, x.ToString ());
                WriteAt (26, 5, "       ");
                WriteAt (26, 5, y.ToString ());
            };
            
            Window.WindowClose = (GlfwWindowPtr window) => {
                WriteAt (17, 15, "                                                       ");
                WriteAt (17, 15, " WindowClose fired");
            };
            
            Window.WindowFocus = (GlfwWindowPtr window, bool focus) => {
                WriteAt (58, 9, "              ");
                WriteAt (58, 9, focus.ToString ());
            };
            
            Window.WindowMinimize = (GlfwWindowPtr window, bool minimized) => {
                WriteAt (58, 11, "              ");
                WriteAt (58, 11, minimized.ToString ());
            };
            
            Window.WindowError = (GlfwError error, string desc) => {
                WriteAt (17, 15, "                                                       ");
                WriteAt (17, 15, "WindowError: " + error + " - " + desc);
            };
            
            Window.MouseButton = (GlfwWindowPtr window, MouseButton button, KeyAction action) => {
                WriteAt (50, 5, "            ");
                WriteAt (50, 5, button.ToString ());
                WriteAt (63, 5, "         ");
                WriteAt (63, 5, action.ToString ());
            };
            
            Window.MouseMove = (GlfwWindowPtr window, double x, double y) => {
                WriteAt (34, 5, "       ");
                WriteAt (34, 5, string.Format ("{0:f}", x));
                WriteAt (42, 5, "       ");
                WriteAt (42, 5, string.Format ("{0:f}", y));
            };
            
            Window.MouseOver = (GlfwWindowPtr window, bool enter) => {
                WriteAt (58, 13, "              ");
                WriteAt (58, 13, enter.ToString ());
            };
            
            Window.MouseScroll = (GlfwWindowPtr window, double xoffs, double yoffs) => {
                WriteAt (24, 13, "       ");
                WriteAt (24, 13, string.Format ("{0:f}", xoffs));
                WriteAt (32, 13, "       ");
                WriteAt (32, 13, string.Format ("{0:f}", yoffs));
            };
            
            Window.KeyAction = (GlfwWindowPtr window, Key key, int scancode, KeyAction action, KeyModifier mods) => {
                WriteAt (1, 13, "             ");
                WriteAt (1, 13, key.ToString ());
                WriteAt (15, 13, "        ");
                WriteAt (15, 13, action.ToString ());
                
                if (key == Key.F16 && action == KeyAction.Release)// F11 - dafuq?
                {
                    Window.ToggleFullscreen ();
                }
            };
            
            Window.Run ();
            
            Console.SetCursorPosition (0, origRow + 17);
        }

        public IGameManager  GameManager  { get; protected set; }
        public IWindow       Window       { get; protected set; }
        public IInputManager InputManager { get; protected set; }

        #endregion

        #region IResource implementation

        public void Init ()
        {
            Loaded = false;
            //InputManager = new InputManager ();TODO
            //GameManager = new GameManager ();TODO
            Window = new Window (new Vector2i (1024, 576), new Vector2i (1920, 1080),
                                 "FurryLana", new DummyGraphicsResource ()/*GameManager.RootGame*/);
            Window.Init ();
            //InputManager.Init ();
            //GameManager.Init ();
        }

        public void Load ()
        {
            Loaded = false;
            Window.Load ();
            //InputManager.Load ();
            //GameManager.Load ();
            Loaded = true;
        }

        public void Destroy ()
        {
            Loaded = false;
            //GameManager.Destroy ();
            //InputManager.Destroy ();
            Window.Destroy ();
        }

        public bool Loaded { get; protected set; }

        #endregion

        protected int origRow;
        
        protected void CreateTable ()
        {
            Console.Clear ();
            origRow = Console.CursorTop;
            string s =
                "+----------------+---------------+---------------+----------------------+\n" +
                    "|  WindowResize  |  WindowMove   |   MouseMove   |     MouseButton      |\n" +
                    "+-------+--------+-------+-------+-------+-------+------------+---------+\n" +
                    "| Width | Height |   X   |   Y   |   X   |   Y   |   Button   | Action  |\n" +
                    "+-------+--------+-------+-------+-------+-------+------------+---------+\n" +
                    "|       |        |       |       |       |       |            |         |\n" +
                    "+-------+--------+-------+-------+-------+-------+------------+---------+\n" +
                    "                                                                         \n" +
                    "+----------------------+---------------+-----------------+--------------+\n" +
                    "|       KeyAction      |  MouseScroll  | WindowFocus:    |              |\n" +
                    "+-------------+--------+-------+-------+-----------------+--------------+\n" +
                    "|     Key     | Action |   X   |   Y   | WindowMinimize: |              |\n" +
                    "+-------------+--------+-------+-------+-----------------+--------------+\n" +
                    "|             |        |       |       | MouseOver:      |              |\n" +
                    "+-------------+-+------+-------+-------+-----------------+--------------+\n" +
                    "| Other Events: |                                                       |\n" +
                    "+---------------+-------------------------------------------------------+\n";
            Console.Write (s);
        }
        
        protected void WriteAt (int x, int y, string s)
        {
            Console.SetCursorPosition (x, origRow + y);
            Console.Write (s);
        }
    }
}
