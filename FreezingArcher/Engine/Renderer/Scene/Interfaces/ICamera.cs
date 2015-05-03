using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FreezingArcher.Math;

namespace FreezingArcher.Renderer.Scene
{
    public interface ICamera
    {
        Matrix ProjectionMatrix { get; }
        Matrix ViewMatrix { get; }

        int Width { get; set; }
        int Height { get; set; }
    }
}
