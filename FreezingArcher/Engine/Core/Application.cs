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
using System.Linq;
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
    public sealed class Application : IMessageCreator, IMessageConsumer
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

        public int ManagedThreadId { get; private set;}

        /// <summary>
        /// Indicating whether command line interface is active or not.
        /// </summary>
        public bool IsCommandLineInterface { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreezingArcher.Core.Application"/> class.
        /// </summary>
        // / <param name="game">The initial root game.</param>
        public Application (string name, string[] args)
        {
            ManagedThreadId = Thread.CurrentThread.ManagedThreadId;

            Name = name;
            Frametime = 16;
            Logger.Initialize (name);
            ObjectManager = new ObjectManager();
            ValidMessages = new[] { (int) MessageId.Input };
            MessageManager = new MessageManager ();
            MessageManager += this;
            ConfigManager.Initialize (MessageManager);
            EntityFactory.Instance = new EntityFactory(ObjectManager);

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
                IsCommandLineInterface = true;
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
            MessageManager += Window;
            Game = new Game (name, ObjectManager, MessageManager, null, RendererContext);
            LoadAgain = false;
            InitAgain = false;

            Window.WindowResize = (GlfwWindowPtr window, int width, int height) => {
                Window.MSize = new Vector2i(width, height);

                if (MessageCreated != null)
                    MessageCreated (new WindowResizeMessage (Window, width, height));
            };
            
            Window.WindowMove = (GlfwWindowPtr window, int x, int y) => {
                if (MessageCreated != null)
                    MessageCreated (new WindowMoveMessage (Window, x, y));
            };
            
            Window.WindowClose = (GlfwWindowPtr window) => {
                if (MessageCreated != null)
                    MessageCreated (new WindowCloseMessage (Window));
            };
            
            Window.WindowFocus = (GlfwWindowPtr window, bool focus) => {
                Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Window '{0}' changed focus state to '{1}'",
                    Window.Title, focus);
                if (MessageCreated != null)
                    MessageCreated (new WindowFocusMessage (Window, focus));
            };
            
            Window.WindowMinimize = (GlfwWindowPtr window, bool minimized) => {
                Logger.Log.AddLogEntry (LogLevel.Debug, ClassName,
                    "Window '{0}' changed minimized state to '{1}'", Window.Title, minimized);
                if (MessageCreated != null)
                    MessageCreated (new WindowMinimizeMessage (Window, minimized));
            };
            
            Window.WindowError = (GlfwError error, string desc) => {
                Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Window '{0}' threw an error: [{1}] {2}",
                    Window.Title, error.ToString (), desc);
                if (MessageCreated != null)
                    MessageCreated (new WindowErrorMessage (Window, error.ToString (), desc));
            };
            
            Window.MouseButton = (GlfwWindowPtr wnd, MouseButton btn, KeyAction action) =>
                InputManager.HandleMouseButton(wnd, btn, action);

            Window.MouseMove = (GlfwWindowPtr wnd, double x, double y) => InputManager.HandleMouseMove(wnd, x, y);
            
            Window.MouseOver = (GlfwWindowPtr window, bool enter) => {
                if (enter)
                    Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Mouse entered window '{0}'",
                        Window.Title);
                else
                    Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Mouse leaved window '{0}'",
                        Window.Title);

                if (MessageCreated != null)
                    MessageCreated (new WindowMouseOverMessage (Window, enter));
            };
            
            Window.MouseScroll = (GlfwWindowPtr wnd, double xoffset, double yoffset) =>
                InputManager.HandleMouseScroll(wnd, xoffset, yoffset);
            
            Window.KeyAction = (GlfwWindowPtr wnd, Key key, int scanCode, KeyAction action, KeyModifiers mods) =>
                InputManager.HandleKeyboardInput(wnd, key, scanCode, action, mods);
        }

        /// <summary>
        /// The periodic update task.
        /// </summary>
        private PeriodicTask PeriodicUpdateTask;

        /// <summary>
        /// The periodic input task.
        /// </summary>
        private PeriodicTask PeriodicInputTask;

        private Stopwatch updateStopwatch;

        private Stopwatch frameStopwatch;

        private PeriodicTask PeriodicFramecounterTask;

        /// <summary>
        /// Gets or sets the frametime in ms.
        /// </summary>
        /// <value>The frametime.</value>
        public int Frametime { get; set; }

        /// <summary>
        /// Run this instance.
        /// </summary>
        public void Run ()
        {
            if (IsCommandLineInterface)
                return;

            Logger.Log.AddLogEntry (LogLevel.Fine, ClassName, "Running application '{0}' ...", Name);
            MessageManager.StartProcessing ();

            if (MessageCreated != null)
                MessageCreated(new RunningMessage(this));

            updateStopwatch.Start ();
            PeriodicUpdateTask.Start ();
            PeriodicInputTask.Start ();
            PeriodicFramecounterTask.Start();

            while (!Window.ShouldClose ())
            {
                frameStopwatch.Restart();
                // reexec loader if ressources need to be loaded again
                // TODO: check if there is enough time left to do a reloading to avoid lag spikes
                if (LoadAgain)
                {
                    Loader.ExecJobsSequential ();
                    LoadAgain = false;
                }

                //Draw
                RendererContext.Begin();
                if (RendererContext.Compositor == null)
                    RendererContext.DrawScene();
                else
                    RendererContext.Compose();
                RendererContext.End();

                Window.SwapBuffers ();
                Window.PollEvents ();

                frames++;

                int sleep = Frametime - (int) frameStopwatch.ElapsedMilliseconds;
                if (sleep > 0)
                    Thread.Sleep (sleep);
            }
        }

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <value>The window.</value>
        public Window Window { get; private set; }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public Content.Game Game { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

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
        public AudioManager AudioManager { get; private set; }

        /// <summary>
        /// Gets the current frames per second.
        /// </summary>
        /// <value>The FPS value.</value>
        public uint FPSCounter { get; private set; }

        private uint frames = 0;

        ///<summary>
        /// Gets the RendererContext
        /// </summary>
        /// <value>>The renderer</value>
        public RendererContext RendererContext{ get; private set;}

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
        /// </summary>
        public void Init ()
        {
            if (IsCommandLineInterface)
                return;

            Loaded = false;

            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Initializing application '{0}' ...", Name);
            InputManager = new InputManager (MessageManager, this);
            updateStopwatch = new Stopwatch();
            frameStopwatch = new Stopwatch();
            PeriodicUpdateTask = new PeriodicTask (16, () => {
                MessageCreated(new UpdateMessage(updateStopwatch.Elapsed));
                updateStopwatch.Restart();
            });
            PeriodicInputTask = new PeriodicTask (8, InputManager.GenerateInputMessage);
            PeriodicFramecounterTask = new PeriodicTask(1000, () => {
                FPSCounter = frames;
                frames = 0;
            });

            Initer = new JobExecuter ();
            Initer.InsertJobs (GetInitJobs (new List<Action>()));
            Initer.DoReexec += () => { InitAgain = true; };
            Loader = new JobExecuter ();
            Loader.InsertJobs (GetLoadJobs (new List<Action>(), Loader.NeedsReexecHandler));
            Loader.DoReexec += () => { LoadAgain = true; };
            Initer.ExecJobsParallel (System.Environment.ProcessorCount);
        }

        /// <summary>
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            Window.GetInitJobs (list);
            return list;
        }

        /// <summary>
        /// Load this resource. This method will be called from the main thread within a valid gl context.
        /// </summary>
        public void Load ()
        {
            if (IsCommandLineInterface)
                return;

            Loaded = false;
            Logger.Log.AddLogEntry (LogLevel.Debug, ClassName, "Loading application '{0}' ...", Name);
            Loader.ExecJobsSequential ();
            Loaded = true;

            //Later some more stuff
            RendererContext.Init(this);
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
            Logger.Log.AddLogEntry(LogLevel.Fine, ClassName, "Destroying application '{0}' ...", Name);
            Loaded = false;
            MessageManager.StopProcessing ();

            if (!IsCommandLineInterface)
            {
                Game.Destroy();
                PeriodicInputTask.Stop ();
                PeriodicUpdateTask.Stop ();
                PeriodicFramecounterTask.Stop (); 
                updateStopwatch.Stop ();
                frameStopwatch.Stop();
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
        public bool Loaded { get; private set; }
        
        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        public event Handler NeedsLoad;

        #endregion
        
        /// <summary>
        /// The loader.
        /// </summary>
        private JobExecuter Loader;

        /// <summary>
        /// Indicating whether to load again.
        /// </summary>
        private bool LoadAgain;
        
        /// <summary>
        /// The initer.
        /// </summary>
        private JobExecuter Initer;

        /// <summary>
        /// Indicating whether to initialize again.
        /// </summary>
        private bool InitAgain;

        /// <summary>
        /// The input manager.
        /// </summary>
        internal InputManager InputManager;

        #region IMessageCreator implementation

        /// <summary>
        /// Occurs when a new message is created an is ready for processing
        /// </summary>
        public event MessageEvent MessageCreated;

        #endregion

        #region IMessageConsumer implementation

        public void ConsumeMessage(IMessage msg)
        {
            InputMessage im = msg as InputMessage;

            if (im != null)
            {
                if (im.IsActionPressedAndRepeated("fullscreen"))
                {
                    Window.ToggleFullscreen();
                }
                if (im.IsActionPressedAndRepeated("capture_mouse"))
                {
                    if (Window.IsMouseCaptured ())
                        Window.ReleaseMouse ();
                    else
                        Window.CaptureMouse ();
                }
                if (im.Keys.Any(k => k.Action == KeyAction.Release && k.Key == Key.F4 && k.Modifier == KeyModifiers.Alt))
                {
                    Window.Close();
                }
                if (im.IsActionPressedAndRepeated("save"))
                {
                    ConfigManager.Instance.SaveAll();
                }
            }
        }

        public int[] ValidMessages { get; private set; }

        #endregion
    }
}
