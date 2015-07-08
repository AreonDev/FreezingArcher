using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FreezingArcher.Math;

namespace FreezingArcher.Renderer
{
    public class Sprite
    {
        Texture2D MyTexture;

        public Sprite()
        {
            MyTexture = null;
        }

        public Texture2D Texture { get { return MyTexture; } }
        public Vector2 Scaling { get; set; }
        public Vector2 AbsolutePosition { get; set; }
        public Vector2 RelativePosition { get; set; }
        public float Rotation { get; set; }
        public Vector2 RotationPoint { get; set; }
        public bool CustomEffect { get; set;}

        public bool Init(Texture2D tex)
        {
            if (tex == null || !tex.Created)
                return false;

            Scaling = new Vector2(1.0f, 1.0f);
            AbsolutePosition = new Vector2(0.0f, 0.0f);
            RelativePosition = new Vector2(0.0f, 0.0f);
            Rotation = 0;
            RotationPoint = new Vector2(0.0f, 0.0f);

            MyTexture = tex;
            MyTexture.SetUseCount(MyTexture.InternalUseCount + 1);

            return true;
        }

        public void Destroy()
        {
            MyTexture.SetUseCount(MyTexture.InternalUseCount - 1);
            MyTexture = null;
        }
    }
}
