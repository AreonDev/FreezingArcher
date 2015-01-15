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
#define DEBUG
#define LINUX_INTEL_COMPATIBLE
using System;
using Pencil.Gaming;
using Pencil.Gaming.MathUtils;
using FurryLana.Engine.Application.Interfaces;
using System.Collections.Generic;

namespace FurryLana.Engine.Application
{
    /// <summary>
    /// Window.
    /// </summary>
    public class Window : IWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Application.Window"/> class.
        /// </summary>
        /// <param name="size">Windowed size.</param>
        /// <param name="resolution">Resolution of the framebuffer.</param>
        /// <param name="title">Title.</param>
        public Window (Vector2i size, Vector2i resolution, string title)
        {
	    MSize = size;
	    MResolution = resolution;
	    MTitle = title;
        }

        #region IResource implementation

        /// <summary>
        /// Init this resource. This method may not be called from the main thread as the initialization process is
        /// multi threaded.
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
        /// Gets the init jobs.
        /// </summary>
        /// <returns>The init jobs.</returns>
        /// <param name="list">List.</param>
        public List<Action> GetInitJobs (List<Action> list)
        {
            return list;
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
        /// Gets the load jobs.
        /// </summary>
        /// <returns>The load jobs.</returns>
        /// <param name="list">List.</param>
        /// <param name="reloader">The NeedLoad event handler.</param>
        public List<Action> GetLoadJobs (List<Action> list, EventHandler reloader)
        {
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
            Glfw.DestroyWindow (Win);
            Glfw.Terminate ();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Application.Window"/> is loaded.
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
        /// Toggles the fullscreen mode.
        /// </summary>
        public void ToggleFullscreen ()
        {
            Fullscreen = !Fullscreen;
        }

        /// <summary>
        /// Show this window in its specified states.
        /// </summary>
        public void Show ()
        {
            Glfw.ShowWindow (Win);
            Glfw.MakeContextCurrent (Win);
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
        /// Indicating whether the window should close or not.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        public bool ShouldClose ()
        {
            return Glfw.WindowShouldClose (Win);
        }

        /// <summary>
        /// Gets the time passed by since the last call of this function.
        /// </summary>
        /// <returns>The delta time.</returns>
        public double GetDeltaTime ()
        {
            double delta = Glfw.GetTime ();
            Glfw.SetTime (0.0);
            return delta;
        }

        /// <summary>
        /// Swaps the front and back buffers.
        /// </summary>
        public void SwapBuffers ()
        {
            Glfw.SwapBuffers (Win);
        }

        /// <summary>
        /// Polls the events from the window manager.
        /// </summary>
        public void PollEvents ()
        {
            Glfw.PollEvents ();
        }

        /// <summary>
        /// Captures the mouse pointer.
        /// </summary>
        public void CaptureMouse ()
        {
            CursorMode = CursorMode.CursorCaptured | CursorMode.CursorHidden;
            Glfw.SetInputMode (Win, InputMode.CursorMode, CursorMode);
        }

        /// <summary>
        /// Releases the mouse pointer.
        /// </summary>
        public void ReleaseMouse ()
        {
            CursorMode = CursorMode.CursorNormal;
            Glfw.SetInputMode (Win, InputMode.CursorMode, CursorMode);
        }

        /// <summary>
        /// Sets the mouse position.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void SetMousePosition (double x, double y)
        {
            Glfw.SetCursorPos (Win, x, y);
        }

        /// <summary>
        /// Determines whether the mouse is captured.
        /// </summary>
        /// <returns>true</returns>
        /// <c>false</c>
        public bool IsMouseCaptured ()
        {
            return CursorMode == CursorMode.CursorCaptured;
        }

        /// <summary>
        /// Gets or sets the size of the window in windowed mode.
        /// </summary>
        /// <value>The size.</value>
        public Vector2i Size
	{
            get
	    {
                return MSize;
            }
            set
	    {
                MSize = value;
                Glfw.SetWindowSize (Win, value.X, value.Y);
            }
        }

        /// <summary>
        /// Gets or sets the resolution of the framebuffer.
        /// </summary>
        /// <value>The framebuffer resolution.</value>
        public Vector2i Resolution
        {
            get
            {
                return MResolution;
            }
            set
            {
                MResolution = value;
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
        /// Gets or sets a value indicating whether this <see cref="FurryLana.Engine.Application.Window"/> is fullscreen.
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
                //TODO toggle fullscreen
		MFullscreen = value;
            }
        }

        /// <summary>
        /// Gets or sets the mouse move handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwCursorPosFun MouseMove { get; set; }

        /// <summary>
        /// Gets or sets the mouse over handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwCursorEnterFun MouseOver { get; set; }

        /// <summary>
        /// Gets or sets the mouse button handlers.
        /// </summary>
        /// <value>The handlers.</value>
        public GlfwMouseButtonFun MouseButton { get; set; }

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
	protected Vector2i      MResolution;

        /// <summary>
        /// The window size member variable.
        /// </summary>
	protected Vector2i      MSize;

        /// <summary>
        /// The window.
        /// </summary>
        protected GlfwWindowPtr Win;

        /// <summary>
        /// The cursor mode.
        /// </summary>
        protected CursorMode CursorMode;

        /// <summary>
        /// Creates the window.
        /// </summary>
	protected void CreateWindow ()
        {
#if LINUX_INTEL_COMPATIBLE
            Glfw.WindowHint (WindowHint.ContextVersionMajor, 3);
            Glfw.WindowHint (WindowHint.ContextVersionMinor, 3);
#else
            Glfw.WindowHint (WindowHint.ContextVersionMajor, 4);
            Glfw.WindowHint (WindowHint.ContextVersionMinor, 0);
#endif
#if DEBUG
            Glfw.WindowHint (WindowHint.OpenGLDebugContext, 1);
#endif

            Win = Glfw.CreateWindow (Size.X, Size.Y, Title,
                                     GlfwMonitorPtr.Null, GlfwWindowPtr.Null);

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

            CursorMode = 0;
            CaptureMouse ();
        }
    }
}
