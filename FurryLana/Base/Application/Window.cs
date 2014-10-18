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
    public class Window : IWindow
    {
        public Window (Vector2i windowedSize, Vector2i fullscreenSize, string title, IGraphicsResource resource)
        {
	    MWindowSize = windowedSize;
	    MFullscreenSize = fullscreenSize;
	    MTitle = title;
	    Resource = resource;
        }

        #region IResource implementation

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

        public void Destroy ()
        {
            Glfw.DestroyWindow (Win);
            Glfw.Terminate ();
        }

        public bool Loaded { get; protected set; }

        #endregion

        #region IWindow implementation

        public void ToggleFullscreen ()
        {
            Fullscreen = !Fullscreen;
        }

        public void Show ()
        {
            Glfw.ShowWindow (Win);
        }

        public void Hide ()
        {
            Glfw.HideWindow (Win);
        }

        public void Minimize ()
        {
            Glfw.IconifyWindow (Win);
        }

        public void Restore ()
        {
            Glfw.RestoreWindow (Win);
        }

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

        public IGraphicsResource    Resource       { get; set; }
        public GlfwCursorPosFun     MouseMove      { get; set; }
        public GlfwCursorEnterFun   MouseOver      { get; set; }
        public GlfwMouseButtonFun   MouseButton    { get; set; }
        public GlfwScrollFun        MouseScroll    { get; set; }
        public GlfwKeyFun           KeyAction      { get; set; }
        public GlfwWindowCloseFun   WindowClose    { get; set; }
        public GlfwWindowFocusFun   WindowFocus    { get; set; }
        public GlfwWindowIconifyFun WindowMinimize { get; set; }
        public GlfwWindowPosFun     WindowMove     { get; set; }
        public GlfwWindowSizeFun    WindowResize   { get; set; }
        public GlfwErrorFun         WindowError    { get; set; }

        #endregion

	protected bool          MFullscreen;
	protected string        MTitle;
	protected Vector2i      MFullscreenSize;
	protected Vector2i      MWindowSize;
	protected bool          RecreateWindow;
        protected GlfwWindowPtr Win;

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

        protected GlfwWindowPtr CreateWindowed ()
        {
            return Glfw.CreateWindow (WindowedSize.X, WindowedSize.Y, Title,
                                      GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
        }

        protected GlfwWindowPtr CreateFullscreen ()
        {
            return Glfw.CreateWindow (FullscreenSize.X, FullscreenSize.Y, Title,
                                      Glfw.GetPrimaryMonitor (), GlfwWindowPtr.Null);
        }
    }
}
