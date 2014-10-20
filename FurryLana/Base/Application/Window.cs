//
//  Window.cs
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
using System.Threading;
using FurryLana.Engine.Graphics.Interfaces;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using FurryLana.Base.Application.Interfaces;

namespace FurryLana.Base.Application
{
    /// <summary>
    /// Window.
    /// </summary>
    public class Window : IWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Base.Application.Window"/> class.
        /// </summary>
        /// <param name="windowedSize">Windowed size.</param>
        /// <param name="fullscreenSize">Fullscreen size.</param>
        /// <param name="title">Title.</param>
        /// <param name="resource">Resource.</param>
        public Window (Vector2i windowedSize, Vector2i fullscreenSize, string title, IGraphicsResource resource)
        {
	    MWindowSize = windowedSize;
	    MFullscreenSize = fullscreenSize;
	    MTitle = title;
	    Resource = resource;
        }

        #region IResource implementation

        /// <summary>
        /// Init this resource. Initialzes the resource within a valid gl context.
        /// 
        /// Why not use the constructor?:
        /// The constructor may not have a valid gl context to initialize gl components.
        /// </summary>
        public void Init ()
        {
            try
            {
                Glfw.Init ();
            }
            catch (Exception e)
            {
                Glfw.Terminate ();
                throw new OperationCanceledException ("Glfw initialization failed!", e);
            }
        }

        /// <summary>
        /// Load this resource. This method *should* be called from an extra loading thread with a shared gl context.
        /// </summary>
        public void Load ()
        {
            try
            {
                CreateWindow ();
                
                Glfw.SetTime (0.0);
            }
            catch (Exception e)
            {
                Destroy ();
                throw new OperationCanceledException ("Caught exception while loading the window!", e);
            }
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
            Glfw.DestroyWindow (Win);
            Glfw.Terminate ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Base.Application.Window"/> is loaded.
        /// </summary>
        /// <value><c>true</c> if loaded; otherwise, <c>false</c>.</value>
        public bool Loaded { get; protected set; }
        
        /// <summary>
        /// Fire this event when you need the Load function to be called.
        /// For example after init or when new resources needs to be loaded.
        /// </summary>
        /// <value>NeedsLoad handlers.</value>
        public EventHandler NeedsLoad { get; set; }

        #endregion

        #region IWindow implementation

        /// <summary>
        /// Toggles the fullscreen.
        /// </summary>
        public void ToggleFullscreen ()
        {
            Fullscreen = !Fullscreen;
        }

        /// <summary>
        /// Show this window.
        /// </summary>
        public void Show ()
        {
            Glfw.ShowWindow (Win);
        }

        /// <summary>
        /// Hide this window.
        /// </summary>
        public void Hide ()
        {
            Glfw.HideWindow (Win);
        }

        /// <summary>
        /// Minimize this window.
        /// </summary>
        public void Minimize ()
        {
            Glfw.IconifyWindow (Win);
        }

        /// <summary>
        /// Restore this window (unminimize).
        /// </summary>
        public void Restore ()
        {
            Glfw.RestoreWindow (Win);
        }

        /// <summary>
        /// Run this instance.
        /// </summary>
        public void Run ()
        {
            while (!Glfw.WindowShouldClose (Win))
            {
                float deltaTime = (float) Glfw.GetTime ();
                Glfw.SetTime (0.0);

                GL.ClearColor (Color4.DodgerBlue);
                GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                Resource.FrameSyncedUpdate (deltaTime);
                Resource.Draw ();

                Glfw.SwapBuffers (Win);
                Glfw.PollEvents ();

                if (RecreateWindow)
                {
                    Glfw.DestroyWindow (Win);
                    CreateWindow ();
                    RecreateWindow = false;
                }

                Thread.Sleep (17);
            }
        }

        /// <summary>
        /// Gets or sets the size of the window in windowed mode.
        /// </summary>
        /// <value>The size.</value>
        public Vector2i WindowedSize
	{
            get
	    {
                return MWindowSize;
            }
            set
	    {
                MWindowSize = value;
		if (!Fullscreen)
		    Glfw.SetWindowSize (Win, value.X, value.Y);
            }
        }

        /// <summary>
        /// Gets or sets resolution in fullscreen mode.
        /// </summary>
        /// <value>The resolution.</value>
        public Vector2i FullscreenSize
	{
            get
	    {
                return MFullscreenSize;
            }
            set
	    {
                MFullscreenSize = value;
		if (Fullscreen)
		    Glfw.SetWindowSize (Win, value.X, value.Y);
            }
        }

        /// <summary>
        /// Gets or sets the window title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
	{
            get
	    {
                return MTitle;
            }
            set
	    {
                MTitle = value;
		Glfw.SetWindowTitle (Win, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Base.Application.Window"/> is fullscreen.
        /// </summary>
        /// <value><c>true</c> if fullscreen; otherwise, <c>false</c>.</value>
        public bool Fullscreen
	{
            get
	    {
                return MFullscreen;
            }
            set
	    {
                if (MFullscreen != value)
		    RecreateWindow = true;
		MFullscreen = value;
            }
        }

        /// <summary>
        /// Gets or sets the binded resource.
        /// </summary>
        /// <value>The resource.</value>
        public IGraphicsResource    Resource       { get; set; }

        /// <summary>
        /// Gets or sets the mouse move handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwCursorPosFun     MouseMove      { get; set; }

        /// <summary>
        /// Gets or sets the mouse over handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwCursorEnterFun   MouseOver      { get; set; }

        /// <summary>
        /// Gets or sets the mouse button handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwMouseButtonFun   MouseButton    { get; set; }

        /// <summary>
        /// Gets or sets the mouse scroll handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwScrollFun        MouseScroll    { get; set; }

        /// <summary>
        /// Gets or sets the key action handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwKeyFun           KeyAction      { get; set; }

        /// <summary>
        /// Gets or sets the window close handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwWindowCloseFun   WindowClose    { get; set; }

        /// <summary>
        /// Gets or sets the window focus handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwWindowFocusFun   WindowFocus    { get; set; }

        /// <summary>
        /// Gets or sets the window minimize handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwWindowIconifyFun WindowMinimize { get; set; }

        /// <summary>
        /// Gets or sets the window move handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwWindowPosFun     WindowMove     { get; set; }

        /// <summary>
        /// Gets or sets the window resize handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwWindowSizeFun    WindowResize   { get; set; }

        /// <summary>
        /// Gets or sets the window error handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwErrorFun         WindowError    { get; set; }

        #endregion

        /// <summary>
        /// The fullscreen member variable.
        /// </summary>
	protected bool          MFullscreen;

        /// <summary>
        /// The title member variable.
        /// </summary>
	protected string        MTitle;

        /// <summary>
        /// The fullscreen resolution member variable.
        /// </summary>
	protected Vector2i      MFullscreenSize;

        /// <summary>
        /// The window size member variable.
        /// </summary>
	protected Vector2i      MWindowSize;

        /// <summary>
        /// The recreate window toggle.
        /// </summary>
	protected bool          RecreateWindow;

        /// <summary>
        /// The window.
        /// </summary>
        protected GlfwWindowPtr Win;

        /// <summary>
        /// Creates the window.
        /// </summary>
	protected void CreateWindow ()
        {
            Win = Fullscreen ? CreateFullscreen () : CreateWindowed ();

            Glfw.MakeContextCurrent (Win);

            Glfw.SetCursorPosCallback     (Win, MouseMove);
            Glfw.SetCursorEnterCallback   (Win, MouseOver);
            Glfw.SetMouseButtonCallback   (Win, MouseButton);
            Glfw.SetScrollCallback        (Win, MouseScroll);
            Glfw.SetKeyCallback           (Win, KeyAction);
            Glfw.SetWindowCloseCallback   (Win, WindowClose);
            Glfw.SetWindowFocusCallback   (Win, WindowFocus);
            Glfw.SetWindowIconifyCallback (Win, WindowMinimize);
            Glfw.SetWindowPosCallback     (Win, WindowMove);
            Glfw.SetWindowSizeCallback    (Win, WindowResize);
            Glfw.SetErrorCallback         (WindowError);
        }

        /// <summary>
        /// Create a windowed window.
        /// </summary>
        /// <returns>The window.</returns>
        protected GlfwWindowPtr CreateWindowed ()
        {
            return Glfw.CreateWindow (WindowedSize.X, WindowedSize.Y, Title,
                                      GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
        }

        /// <summary>
        /// Create a fullscreen window.
        /// </summary>
        /// <returns>The window.</returns>
        protected GlfwWindowPtr CreateFullscreen ()
        {
            return Glfw.CreateWindow (FullscreenSize.X, FullscreenSize.Y, Title,
                                      Glfw.GetPrimaryMonitor (), GlfwWindowPtr.Null);
        }
    }
}
