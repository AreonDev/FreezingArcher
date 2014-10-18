/*using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using System.Threading;
using FurryLana.Interfaces;

namespace FurryLana
{
    public class Window
    {
        public Window (Size windowedSize, Size fullscreenSize, string title, IDrawable drawable)
        {
            MWindowedSize = windowedSize;
            MFullscreenSize = fullscreenSize;
            MTitle = title;
            Drawable = drawable;
        }

        protected Size MWindowedSize;
        public Size WindowedSize
        {
            get
            {
                return MWindowedSize;
            }
            set
            {
                MWindowedSize = value;
                if (!Fullscreen)
                    Glfw.SetWindowSize (Win, value.Width, value.Height);
            }
        }

        protected Size MFullscreenSize;
        public Size FullscreenSize
        {
            get
            {
                return MFullscreenSize;
            }
            set
            {
                MFullscreenSize = value;
                if (Fullscreen)
                    Glfw.SetWindowSize (Win, value.Width, value.Height);
            }
        }

        protected string MTitle;
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

        protected GlfwWindowPtr Win;

        public IDrawable Drawable { get; set; }

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

        protected bool MFullscreen;
        protected bool RecreateWindow;
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
            try
            {
                Glfw.Init ();
                try
                {
                    CreateWindow ();

                    Glfw.SetTime (0.0);
                    while (!Glfw.WindowShouldClose (Win))
                    {
                        float deltaTime = (float) Glfw.GetTime ();
                        Glfw.SetTime (0.0);

                        GL.ClearColor (Color4.DodgerBlue);
                        GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                        Drawable.Draw ();

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
                finally
                {
                    Glfw.DestroyWindow (Win);
                }
            }
            finally
            {
                Glfw.Terminate ();
            }
        }

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
            return Glfw.CreateWindow (WindowedSize.Width, WindowedSize.Height, Title,
                                      GlfwMonitorPtr.Null, GlfwWindowPtr.Null);
        }

        protected GlfwWindowPtr CreateFullscreen ()
        {
            return Glfw.CreateWindow (FullscreenSize.Width, FullscreenSize.Height, Title,
                                      Glfw.GetPrimaryMonitor (), GlfwWindowPtr.Null);
        }
    }
}
*/