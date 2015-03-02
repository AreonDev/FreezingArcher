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
        public static void Draw()
        {
            //TODO
            //throw new NotImplementedException();
        }

        public static void Clear(Color4 color)
        {
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        public static void WindowResize(int width, int height)
        {
            GL.Viewport(0, 0, width, height);
        }
    }
}
