using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming.Graphics;

namespace FreezingArcher.Renderer
{
    class RendererCore
    {
        public static void WindowResize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }
    }
}
