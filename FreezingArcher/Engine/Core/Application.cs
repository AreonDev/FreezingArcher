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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Input;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using FreezingArcher.Content;

namespace FreezingArcher.Core
{
    /// <summary>
    /// Test application.
    /// </summary>
    public class Application
    {
        /// <summary>
        /// The global application instance.
        /// </summary>
        public static Application Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.Application"/> class.
        /// </summary>
        // / <param name="game">The initial root game.</param>
        public Application (string name)
        {
            // read from settings FIXME
            Window = new Window (new Vector2i (1024, 576), new Vector2i (1920, 1080), name);
            Game = new Game (name);

            #if DEBUG
            CreateTable ();
            #endif
            
            Window.WindowResize = (GlfwWindowPtr window, int width, int height) => {
                #if DEBUG
                WriteAt (1, 5, "       ");
                WriteAt (1, 5, width.ToString ());
                WriteAt (9, 5, "        ");
                WriteAt (9, 5, height.ToString ());
                #endif

                Renderer.RendererCore.WindowResize (width, height);
            };
            
            Window.WindowMove = (GlfwWindowPtr window, int x, int y) => {
                #if DEBUG
                WriteAt (18, 5, "       ");
                WriteAt (18, 5, x.ToString ());
                WriteAt (26, 5, "       ");
                WriteAt (26, 5, y.ToString ());
                #endif
            };
            
            Window.WindowClose = (GlfwWindowPtr window) => {
                #if DEBUG
                WriteAt (17, 15, "                                                       ");
                WriteAt (17, 15, " WindowClose fired");
                #endif
            };
            
            Window.WindowFocus = (GlfwWindowPtr window, bool focus) => {
                #if DEBUG
                WriteAt (58, 9, "              ");
                WriteAt (58, 9, focus.ToString ());
                #endif
            };
            
            Window.WindowMinimize = (GlfwWindowPtr window, bool minimized) => {
                #if DEBUG
                WriteAt (58, 11, "              ");
                WriteAt (58, 11, minimized.ToString ());
                #endif
            };
            
            Window.WindowError = (GlfwError error, string desc) => {
                #if DEBUG
                WriteAt (17, 15, "                                                       ");
                WriteAt (17, 15, "WindowError: " + error + " - " + desc);
                #endif
            };
            
            Window.MouseButton = (GlfwWindowPtr window, MouseButton button, KeyAction action) => {
                #if DEBUG
                WriteAt (50, 5, "            ");
                WriteAt (50, 5, button.ToString ());
                WriteAt (63, 5, "         ");
                WriteAt (63, 5, action.ToString ());
                #endif

                InputManager.HandleMouseButton (window, button, action);
            };

            Window.MouseMove = (GlfwWindowPtr window, double x, double y) => {
                #if DEBUG
                WriteAt (34, 5, "       ");
                WriteAt (34, 5, string.Format ("{0:f}", x));
                WriteAt (42, 5, "       ");
                WriteAt (42, 5, string.Format ("{0:f}", y));
                #endif

                InputManager.HandleMouseMove (window, x, y);
            };
            
            Window.MouseOver = (GlfwWindowPtr window, bool enter) => {
                #if DEBUG
                WriteAt (58, 13, "              ");
                WriteAt (58, 13, enter.ToString ());
                #endif
            };
            
            Window.MouseScroll = (GlfwWindowPtr window, double xoffs, double yoffs) => {
                #if DEBUG
                WriteAt (24, 13, "       ");
                WriteAt (24, 13, string.Format ("{0:f}", xoffs));
                WriteAt (32, 13, "       ");
                WriteAt (32, 13, string.Format ("{0:f}", yoffs));
                #endif

                InputManager.HandleMouseScroll (window, xoffs, yoffs);
            };
            
            Window.KeyAction = (GlfwWindowPtr window, Key key, int scancode, KeyAction action, KeyModifiers mods) => {
                #if DEBUG
                WriteAt (1, 13, "             ");
                WriteAt (1, 13, key.ToString ());
                WriteAt (15, 13, "        ");
                WriteAt (15, 13, action.ToString ());
                #endif
                
                if (key == Key.F16 && action == KeyAction.Release)// F11 - dafuq?
                {
                    Window.ToggleFullscreen ();
                }

                if (key == Key.F6 && action == KeyAction.Release)
                {
                    if (Window.IsMouseCaptured ())
                        Window.ReleaseMouse ();
                    else
                        Window.CaptureMouse ();
                }

                if (key == Key.Escape && action == KeyAction.Release)
                    Window.Close ();

                InputManager.HandleKeyboardInput (window, key, scancode, action, mods);
            };
        }

        /// <summary>
        /// A TaskFactory to provide an update threading manager.
        /// </summary>
        protected TaskFactory TaskFactory = new TaskFactory ();

        /// <summary>
        /// Run this instance.
        /// </summary>
        public void Run ()
        {
            //int counter = 0;FIXME
            while (!Window.ShouldClose ())
            {
                double deltaTime = Window.GetDeltaTime ();
                
                GL.ClearColor (Color4.DodgerBlue);
                GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                /*if (counter > 4)
                {
                    TaskFactory.StartNew (
                        () => Resource.Update (Application.Instance.ResourceManager.
                                           InputManager.GenerateUpdateDescription ((float) deltaTime)));
                    counter = 0;
                }
                counter++; FUCKING FIX THIS - THIS IS BROKEN BY DESIGN!!!!!1111elf WHAT THE FUUUUUCK! FIXME
                */

                Game.FrameSyncedUpdate (deltaTime);
                Renderer.RendererCore.Draw ();

                Window.SwapBuffers ();
                Window.PollEvents ();
                
                Thread.Sleep (16);
            }
        }

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <value>The window.</value>
        public Window Window { get; protected set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public Game Game { get; protected set; }

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {
            Loaded = false;

            InputManager = new InputManager ();
            Initer = new JobExecuter ();
            Initer.InsertJobs (GetInitJobs (new List<Action>()));
            Loader = new JobExecuter ();
            Loader.InsertJobs (GetLoadJobs (new List<Action>(), Loader.NeedsReexecHandler));
            Initer.ExecJobsParallel (Environment.ProcessorCount);
        }

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            list = Window.GetInitJobs (list);
            list = Game.GetInitJobs (list);
            return list;
        }

        /// <summary>
        /// Load this resource. This method will be called from the main thread within a valid gl context.
        /// </summary>
        public void Load ()
        {
            Loaded = false;
            Loader.ExecJobsSequential ();
            Loaded = true;
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">The NeedLoad event handler.</param>
        public List<Action> GetLoadJobs (List<Action> list, Handler reloader)
        {
            list = Window.GetLoadJobs (list, reloader);
            list = Game.GetLoadJobs (list, reloader);
            NeedsLoad = reloader;
            return list;
        }

        /// <summary>
        /// Destroy this resource.
        /// 
        /// Why not IDisposable:
        /// IDisposable is called from within the grabage collector context so we do not have a valid gl context there.
        /// Therefore I added the Destroy function as this would be called by the parent instance within a valid gl
        /// context.
        /// </summary>
        public void Destroy ()
        {
            Loaded = false;
            Window.Destroy ();

            #if DEBUG
            Console.SetCursorPosition (0, origRow + 17);
            #endif
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="FreezingArcher.Core.Application"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }
        
        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        public event Handler NeedsLoad;

        #endregion

        #if DEBUG
        /// <summary>
        /// The original command line row.
        /// </summary>
        protected int origRow;
        #endif
        
        /// <summary>
        /// The loader.
        /// </summary>
        protected JobExecuter Loader;
        
        /// <summary>
        /// The initer.
        /// </summary>
        protected JobExecuter Initer;

        /// <summary>
        /// The input manager.
        /// </summary>
        protected InputManager InputManager;

        #if DEBUG
        /// <summary>
        /// Creates the event table.
        /// </summary>
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

        /// <summary>
        /// Writes s at x and y.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="s">The string to write.</param>
        protected void WriteAt (int x, int y, string s)
        {
            Console.SetCursorPosition (x, origRow + y);
            Console.Write (s);
        }
        #endif
    }
}
