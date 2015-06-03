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
//#define DEBUG_EVENTS
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using FreezingArcher.Audio;
using FreezingArcher.Configuration;
using FreezingArcher.Content;
using FreezingArcher.Core.Interfaces;
using FreezingArcher.Input;
using FreezingArcher.Localization;
using FreezingArcher.Math;
using FreezingArcher.Messaging;
using FreezingArcher.Messaging.Interfaces;
using FreezingArcher.Output;
using FreezingArcher.Renderer;
using Pencil.Gaming;
using Section = System.Collections.Generic.Dictionary<string, FreezingArcher.Configuration.Value>;

namespace FreezingArcher.Core
{
    /// <summary>
    /// Test application.
    /// </summary>
    public class Application : IMessageCreator
    {
        /// <summary>
        /// The name of the class.
        /// </summary>
        public static readonly string ClassName = "Application";

        /// <summary>
        /// The global application instance.
        /// </summary>
        public static Application Instance;

        /// <summary>
        /// Create an application with the specified name. THIS IS GLOBAL - DO NOT USE MORE THAN ONCE!
        /// This should be the last call of your main method. Only run in main thread!
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="args">Command line arguments.</param>
        public static void Create (string name, string[] args)
        {
            Instance = new Application (name, args);
            Instance.Init ();
            Instance.Load ();
            Instance.Run ();
            Instance.Destroy ();
        }

        /// <summary>
        /// Create an application with the specified name.
        /// This should be the last call of your main method. Only run in main thread!
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="args">Command line arguments.</param>
        /// <param name="application">The created application.</param>
        public static void Create (string name, string[] args, out Application application)
        {
            application = CreateApp (name, args);
        }

        /// <summary>
        /// Create an application with the specified name.
        /// This should be the last call of your main method. Only run in main thread!
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="args">Command line arguments.</param>
        public static Application CreateApp (string name, string[] args)
        {
            Application app = new Application (name, args);
            app.Init ();
            app.Load ();
            app.Run ();
            app.Destroy ();
            return app;
        }

        /// <summary>
        /// Indicating whether command line interface is active or not.
        /// </summary>
        protected bool Cli = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.Application"/> class.
        /// </summary>
        // / <param name="game">The initial root game.</param>
        public Application (string name, string[] args)
        {
            Name = name;
            Logger.Initialize (name);
            ObjectManager = new ObjectManager();
            ComponentRegistry.Instance = new ComponentRegistry(ObjectManager);
            MessageManager = new MessageManager ();
            MessageManager += this;
            ConfigManager.Initialize (MessageManager);

            CommandLineInterface.Instance.SetHelp ("Freezing Archer 3D game engine/framework", "Alpha 0.0.1",
                "AreonDev", 2015, 'h', "help", true, true, null,
                new string[] {"Authors: David Bögelsack, Fin Christensen, Willy Failla und Martin Koppehel\n"});

            // set fullscreen if overriden by command line.
            CommandLineInterface.Instance.AddOption<bool> (b => {
                if (b)
                    ConfigManager.Instance["freezing_archer"].AddOverride ("general", "fullscreen", new Value (true));
            }, 'f', "fullscreen", "Set window to fullscreen. By default the value from the config file is used.");

            CommandLineInterface.Instance.AddOption<int> (
                i => ConfigManager.Instance ["freezing_archer"].AddOverride ("general", "fullscreen_monitor",
                    new Value (i)), 'm', "fullscreen-monitor",
                "Set the physical monitor the application should use for a fullscreen window.", "INT");

            CommandLineInterface.Instance.AddOption<string> (
                r => ConfigManager.Instance ["freezing_archer"].AddOverride ("general", "resolution", new Value (r)),
                'r', "resolution", "Set the resolution a fullscreen window should have.", "RES", false,
                ConfigManager.Instance["freezing_archer"].GetString ("general", "resolution"));

            CommandLineInterface.Instance.AddOption<string> (
                s => ConfigManager.Instance ["freezing_archer"].AddOverride ("general", "size", new Value (s)),
                's', "size", "Set window size.", "SIZE", false,
                ConfigManager.Instance ["freezing_archer"].GetString ("general", "size"));

            CommandLineInterface.Instance.AddOption<int> (i => {
                if (i > 0)
                {
                    if (i > 8)
                    {
                        Logger.Log.AddLogEntry (LogLevel.Error, "CommandLineInterface", Status.BadArgument,
                            "The given loglevel '{0}' is not valid. Using configuration value...", i);
                        return;
                    }
                    ConfigManager.Instance["freezing_archer"].AddOverride ("general", "loglevel", new Value (i));
                }
            }, 'l', "loglevel", "Set loglevel. If 0 or nothing is given the value from the config file is used.",
                "INT");

            if (!CommandLineInterface.Instance.ParseArguments (args))
            {
                Cli = true;
                return;
            }

            Logger.Log.SetLogLevel ((LogLevel) ConfigManager.Instance["freezing_archer"]
                .GetInteger ("general", "loglevel"));

            Logger.Log.AddLogEntry (LogLevel.Info, ClassName, "Creating new application '{0}'", name);
            AudioManager = new AudioManager ();
            RendererContext = new RendererContext(MessageManager);
            Localizer.Initialize (MessageManager);

            Logger.Log.AddLogEntry(LogLevel.Warning, ClassName, Status.ZombieApocalypse);

            Window = new Window (
                ParserUtils.ParseVector (
                    ConfigManager.Instance["freezing_archer"].GetString ("general", "size")),
                ParserUtils.ParseVector (
                    ConfigManager.Instance["freezing_archer"].GetString ("general", "resolution")),
                name);
            Game = new Game (name);
            LoadAgain = false;
            InitAgain = false;

            #if DEBUG_EVENTS
            CreateTable ();
            #endif
            
            Window.WindowResize = (GlfwWindowPtr window, int width, int height) => {
                #if DEBUG_EVENTS
                WriteAt (1, 5, "       ");
                WriteAt (1, 5, width.ToString ());
                WriteAt (9, 5, "        ");
                WriteAt (9, 5, height.ToString ());
                #endif

                Window.MSize = new Vector2i(width, height);

                if (MessageCreated != null)
                    MessageCreated (new WindowResizeMessage (Window, width, height));
            };
            
            Window.WindowMove = (GlfwWindowPtr window, int x, int y) => {
                #if DEBUG_EVENTS
                WriteAt (18, 5, "       ");
                WriteAt (18, 5, x.ToString ());
                WriteAt (26, 5, "       ");
                WriteAt (26, 5, y.ToString ());
                #endif
                if (MessageCreated != null)
                    MessageCreated (new WindowMoveMessage (Window, x, y));
            };
            
            Window.WindowClose = (GlfwWindowPtr window) => {
                #if DEBUG_EVENTS
                WriteAt (17, 15, "                                                       ");
                WriteAt (17, 15, " WindowClose fired");
                #endif
                if (MessageCreated != null)
                    MessageCreated (new WindowCloseMessage (Window));
            };
            
            Window.WindowFocus = (GlfwWindowPtr window, bool focus) => {
                #if DEBUG_EVENTS
                WriteAt (58, 9, "              ");
                WriteAt (58, 9, focus.ToString ());
                #endif
                Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Window '{0}' changed focus state to '{1}'",
                    Window.Title, focus);
                if (MessageCreated != null)
                    MessageCreated (new WindowFocusMessage (Window, focus));
            };
            
            Window.WindowMinimize = (GlfwWindowPtr window, bool minimized) => {
                #if DEBUG_EVENTS
                WriteAt (58, 11, "              ");
                WriteAt (58, 11, minimized.ToString ());
                #endif
                Logger.Log.AddLogEntry (LogLevel.Debug, ClassName,
                    "Window '{0}' changed minimized state to '{1}'", Window.Title, minimized);
                if (MessageCreated != null)
                    MessageCreated (new WindowMinimizeMessage (Window, minimized));
            };
            
            Window.WindowError = (GlfwError error, string desc) => {
                #if DEBUG_EVENTS
                WriteAt (17, 15, "                                                       ");
                WriteAt (17, 15, "WindowError: " + error + " - " + desc);
                #endif
                Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Window '{0}' threw an error: [{1}] {2}",
                    Window.Title, error.ToString (), desc);
                if (MessageCreated != null)
                    MessageCreated (new WindowErrorMessage (Window, error.ToString (), desc));
            };
            
            Window.MouseButton = (GlfwWindowPtr window, MouseButton button, KeyAction action) => {
                #if DEBUG_EVENTS
                WriteAt (50, 5, "            ");
                WriteAt (50, 5, button.ToString ());
                WriteAt (63, 5, "         ");
                WriteAt (63, 5, action.ToString ());
                #endif

                InputManager.HandleMouseButton (window, button, action);
            };

            Window.MouseMove = (GlfwWindowPtr window, double x, double y) => {
                #if DEBUG_EVENTS
                WriteAt (34, 5, "       ");
                WriteAt (34, 5, string.Format ("{0:f}", x));
                WriteAt (42, 5, "       ");
                WriteAt (42, 5, string.Format ("{0:f}", y));
                #endif

                InputManager.HandleMouseMove (window, x, y);
            };
            
            Window.MouseOver = (GlfwWindowPtr window, bool enter) => {
                #if DEBUG_EVENTS
                WriteAt (58, 13, "              ");
                WriteAt (58, 13, enter.ToString ());
                #endif
                if (enter)
                    Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Mouse entered window '{0}'",
                        Window.Title);
                else
                    Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Mouse leaved window '{0}'",
                        Window.Title);

                if (MessageCreated != null)
                    MessageCreated (new WindowMouseOverMessage (Window, enter));
            };
            
            Window.MouseScroll = (GlfwWindowPtr window, double xoffs, double yoffs) => {
                #if DEBUG_EVENTS
                WriteAt (24, 13, "       ");
                WriteAt (24, 13, string.Format ("{0:f}", xoffs));
                WriteAt (32, 13, "       ");
                WriteAt (32, 13, string.Format ("{0:f}", yoffs));
                #endif

                InputManager.HandleMouseScroll (window, xoffs, yoffs);
            };
            
            Window.KeyAction = (GlfwWindowPtr window, Key key, int scancode, KeyAction action, KeyModifiers mods) => {
                #if DEBUG_EVENTS
                WriteAt (1, 13, "             ");
                WriteAt (1, 13, key.ToString ());
                WriteAt (15, 13, "        ");
                WriteAt (15, 13, action.ToString ());
                #endif
                
                if (key == Key.F11 && action == KeyAction.Release)// F11 - dafuq?
                {
                    Window.ToggleFullscreen ();
                }

                if (key == Key.F1 && action == KeyAction.Release)
                {
                    if (Window.IsMouseCaptured ())
                        Window.ReleaseMouse ();
                    else
                        Window.CaptureMouse ();
                }

                if (key == Key.F2 && action == KeyAction.Release)
                    ConfigManager.Instance.SaveAll ();

                if (key == Key.Escape && action == KeyAction.Release)
                    Window.Close ();

                InputManager.HandleKeyboardInput (window, key, scancode, action, mods);
            };
        }

        /// <summary>
        /// The periodic update task.
        /// </summary>
        protected PeriodicTask PeriodicUpdateTask;

        /// <summary>
        /// The periodic input task.
        /// </summary>
        protected PeriodicTask PeriodicInputTask;

        private Stopwatch updateStopwatch;

        /// <summary>
        /// Run this instance.
        /// </summary>
        public void Run ()
        {
            if (Cli)
                return;

            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Running application '{0}' ...", Name);
            MessageManager.StartProcessing ();

            updateStopwatch.Start ();
            PeriodicUpdateTask.Start ();
            PeriodicInputTask.Start ();

            while (!Window.ShouldClose ())
            {
                // reexec loader if ressources need to be loaded again
                // TODO: check if there is enough time left to do a reloading to avoid lag spikes
                if (LoadAgain)
                {
                    Loader.ExecJobsSequential ();
                    LoadAgain = false;
                }

                //Draw

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

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the message manager.
        /// </summary>
        /// <value>The message manager.</value>
        public MessageManager MessageManager { get; set; }

        /// <summary>
        /// Global object manager instance.
        /// </summary>
        public readonly ObjectManager ObjectManager;

        /// <summary>
        /// Gets or sets the audio manager.
        /// </summary>
        /// <value>The audio manager.</value>
        public AudioManager AudioManager { get; protected set; }


        ///<summary>
        /// Gets the RendererContext
        /// </summary>
        /// <value>>The renderer</value>
        public RendererContext RendererContext{ get; protected set;}

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {
            if (Cli)
                return;

            Loaded = false;

            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Initializing application '{0}' ...", Name);
            InputManager = new InputManager (MessageManager);
            updateStopwatch = new Stopwatch();
            PeriodicUpdateTask = new PeriodicTask (32, () => {
                MessageCreated(new UpdateMessage(updateStopwatch.Elapsed));
                updateStopwatch.Restart();
            });
            PeriodicInputTask = new PeriodicTask (16, InputManager.GenerateInputMessage);


            Initer = new JobExecuter ();
            Initer.InsertJobs (GetInitJobs (new List<Action>()));
            Initer.DoReexec += () => { InitAgain = true; };
            Loader = new JobExecuter ();
            Loader.InsertJobs (GetLoadJobs (new List<Action>(), Loader.NeedsReexecHandler));
            Loader.DoReexec += () => { LoadAgain = true; };
            Initer.ExecJobsParallel (Environment.ProcessorCount);
        }

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            Window.GetInitJobs (list);
            Game.GetInitJobs (list);
            return list;
        }

        /// <summary>
        /// Load this resource. This method will be called from the main thread within a valid gl context.
        /// </summary>
        public void Load ()
        {
            if (Cli)
                return;

            Loaded = false;
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Loading application '{0}' ...", Name);
            Loader.ExecJobsSequential ();
            Loaded = true;

            //Give a chance to load everything... and than create RendererStuff
            Thread.Sleep(1000);

            //Later some more stuf
            RendererContext.Init();
            RendererContext.ViewportResize(0, 0, Window.Size.X, Window.Size.Y);
        }

        /// <summary>
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">The NeedLoad event handler.</param>
        public List<Action> GetLoadJobs (List<Action> list, Handler reloader)
        {
            Window.GetLoadJobs (list, reloader);
            Game.GetLoadJobs (list, reloader);
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
            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Destroying application '{0}' ...", Name);
            Loaded = false;
            MessageManager.StopProcessing ();

            if (!Cli)
            {
                PeriodicInputTask.Stop ();
                PeriodicUpdateTask.Stop ();
                updateStopwatch.Stop ();
                AudioManager.Dispose ();
                Window.Destroy ();
            }

            #if DEBUG_EVENTS
            Console.SetCursorPosition (0, OrigRow + 17);
            #endif

            Logger.Log.Dispose ();
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

        #if DEBUG_EVENTS
        /// <summary>
        /// The original command line row.
        /// </summary>
        protected int OrigRow;
        #endif
        
        /// <summary>
        /// The loader.
        /// </summary>
        protected JobExecuter Loader;

        /// <summary>
        /// Indicating whether to load again.
        /// </summary>
        protected bool LoadAgain;
        
        /// <summary>
        /// The initer.
        /// </summary>
        protected JobExecuter Initer;

        /// <summary>
        /// Indicating whether to initialize again.
        /// </summary>
        protected bool InitAgain;

        /// <summary>
        /// The input manager.
        /// </summary>
        internal InputManager InputManager;

        #if DEBUG_EVENTS
        /// <summary>
        /// Creates the event table.
        /// </summary>
        protected void CreateTable ()
        {
            Console.Clear ();
            OrigRow = Console.CursorTop;
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
            Console.SetCursorPosition (x, OrigRow + y);
            Console.Write (s);
        }
        #endif

        #region IMessageCreator implementation

        /// <summary>
        /// Occurs when a new message is created an is ready for processing
        /// </summary>
        public event MessageEvent MessageCreated;

        #endregion
    }
}
