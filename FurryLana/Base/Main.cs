/*using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using System;

namespace FurryLana
{
    class MainClass
    {
        static int origRow;

        static void CreateTable ()
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

        static void WriteAt (int x, int y, string s)
        {
            Console.SetCursorPosition (x, origRow + y);
            Console.Write (s);
        }

        public static void Main (string[] args)
        {
            Window win = new Window (new Size (1024, 576), new Size (1920, 1080),
                                     "FurryLana", new DummyDrawable ());

            CreateTable ();

            win.WindowResize = (GlfwWindowPtr window, int width, int height) => {
                WriteAt (1, 5, "       ");
                WriteAt (1, 5, width.ToString ());
                WriteAt (9, 5, "        ");
                WriteAt (9, 5, height.ToString ());
                GL.Viewport (0, 0, width, height);
            };

            win.WindowMove = (GlfwWindowPtr window, int x, int y) => {
                WriteAt (18, 5, "       ");
                WriteAt (18, 5, x.ToString ());
                WriteAt (26, 5, "       ");
                WriteAt (26, 5, y.ToString ());
            };

            win.WindowClose = (GlfwWindowPtr window) => {
                WriteAt (17, 15, "                                                       ");
                WriteAt (17, 15, " WindowClose fired");
            };

            win.WindowFocus = (GlfwWindowPtr window, bool focus) => {
                WriteAt (58, 9, "              ");
                WriteAt (58, 9, focus.ToString ());
            };

            win.WindowMinimize = (GlfwWindowPtr window, bool minimized) => {
                WriteAt (58, 11, "              ");
                WriteAt (58, 11, minimized.ToString ());
            };

            win.WindowError = (GlfwError error, string desc) => {
                WriteAt (17, 15, "                                                       ");
                WriteAt (17, 15, "WindowError: " + error + " - " + desc);
            };

            win.MouseButton = (GlfwWindowPtr window, MouseButton button, KeyAction action) => {
                WriteAt (50, 5, "            ");
                WriteAt (50, 5, button.ToString ());
                WriteAt (63, 5, "         ");
                WriteAt (63, 5, action.ToString ());
            };

            win.MouseMove = (GlfwWindowPtr window, double x, double y) => {
                WriteAt (34, 5, "       ");
                WriteAt (34, 5, string.Format ("{0:f}", x));
                WriteAt (42, 5, "       ");
                WriteAt (42, 5, string.Format ("{0:f}", y));
            };

            win.MouseOver = (GlfwWindowPtr window, bool enter) => {
                WriteAt (58, 13, "              ");
                WriteAt (58, 13, enter.ToString ());
            };

            win.MouseScroll = (GlfwWindowPtr window, double xoffs, double yoffs) => {
                WriteAt (24, 13, "       ");
                WriteAt (24, 13, string.Format ("{0:f}", xoffs));
                WriteAt (32, 13, "       ");
                WriteAt (32, 13, string.Format ("{0:f}", yoffs));
            };

            win.KeyAction = (GlfwWindowPtr window, Key key, int scancode, KeyAction action, KeyModifier mods) => {
                WriteAt (1, 13, "             ");
                WriteAt (1, 13, key.ToString ());
                WriteAt (15, 13, "        ");
                WriteAt (15, 13, action.ToString ());

                if (key == Key.F16 && action == KeyAction.Release)// F11 - dafuq?
                {
                    win.ToggleFullscreen ();
                }
            };

            win.Run ();

            Console.SetCursorPosition (0, origRow + 17);
        }
    }
}
*/