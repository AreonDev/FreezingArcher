using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace FreezingArcher.Renderer
{
    /// <summary>
    /// Represents all magnification filter modes applied to textures
    /// </summary>
    public enum MagnificationFilter
    {
        /// <summary>
        /// Use nearest texel
        /// </summary>
        UseNearest = 0x2600,
        /// <summary>
        /// Interpolate linear between 2^Dimension texels
        /// </summary>
        InterpolateLinear = 0x2601,
    }

    /// <summary>
    /// Represents all minification filter modes applied to textures
    /// </summary>
    public enum MinificationFilter
    {
        /// <summary>
        /// Use nearest texel
        /// </summary>
        UseNearest = ((int)0x2600),
        /// <summary>
        /// Interpolate linear between 2^Dimension texels
        /// </summary>
        InterpolateLinear = ((int)0x2601),
        /// <summary>
        /// Use nearest texel  and use mipmap mode nearest
        /// </summary>
        UseNearestAndMipmapNearest = ((int)0x2700),
        /// <summary>
        /// Interpolate texels and use mipmap mode nearest
        /// </summary>
        InterpolateLinearAndMipmapNearest = ((int)0x2701),
        /// <summary>
        /// Use nearest texel and interpolate linear between mipmap levels
        /// </summary>
        UseNearestAndInterpolateMipmapLinear = ((int)0x2702),
        /// <summary>
        /// Interpolate linear and interpolate linear between mipmap levels
        /// </summary>
        InterpolateLinearAndInterpolateMipmapLinear = ((int)0x2703),
    }

    /// <summary>
    /// Describes how texture units should behave with texture coordinates out of the valid range
    /// </summary>
    public enum EdgeBehaviour
    {
        /// <summary>
        /// Texture coordinates are clamped between 0 and 1
        /// </summary>
        Clamp = ((int)0x2900),
        /// <summary>
        /// Texture is repeated
        /// </summary>
        Repeat = ((int)0x2901),
        /// <summary>
        /// If out of range, the texture unit will return the border color
        /// </summary>
        ClampToBorder = ((int)0x812D),
        /// <summary>
        /// Texture coordinates are clamped between 0 and 1
        /// </summary>
        ClampToEdge = ((int)0x812F),
        /// <summary>
        /// Texture is repeated but mirrored in x and y direction if texture coordinate is odd
        /// </summary>
        MirroredRepeat = ((int)0x8370),
    }

    public class Sampler : GraphicsResource
    {
        internal Sampler(string name, int id) : base(name, id, GraphicsResourceType.Sampler)
        {

        }


        private int last_bound_index = 0;

        public void Bind(int index)
        {
            last_bound_index = index;
            GL.BindSampler(index, ID);
        }

        public void Unbind()
        {
            GL.BindSampler(last_bound_index, 0);
            last_bound_index = 0;
        }


        /// <summary>
        /// Gets or sets the magnification filter.
        /// </summary>
        /// <value>The minification filter.</value>
        public MagnificationFilter MagnificationFilter
        {
            get
            {
                int param;
                GL.GetSamplerParameter(ID, SamplerParameter.TextureMagFilter, out param);

                return (MagnificationFilter)param;
            }
            set
            {
                GL.SamplerParameter(ID, SamplerParameter.TextureMagFilter, (int)value);
            }
        }

        /// <summary>
        /// Gets or sets the minification filter.
        /// </summary>
        /// <value>The minification filter.</value>
        public MinificationFilter MinificationFilter
        { 
            get
            {
                int param = 0;

                GL.GetSamplerParameter(ID, SamplerParameter.TextureMinFilter, out param);

                return (MinificationFilter)param;
            }
            set
            {
                GL.SamplerParameter(ID, SamplerParameter.TextureMinFilter, (int)value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>The color of the border.</value>
        public Color4 BorderColor
        {
            get
            {
                float[] param = new float[4];
                GL.GetSamplerParameter(ID, SamplerParameter.TextureBorderColor, param);

                return new Color4(param[0], param[1], param[2], param[3]);
            }

            set
            {
                float[] param = new float[4];
                param[0] = value.R;
                param[1] = value.G;
                param[2] = value.B;
                param[3] = value.A;

                GL.SamplerParameter(ID, SamplerParameter.TextureBorderColor, param);
            }
        }

        public EdgeBehaviour EdgeBehaviorX
        {
            get
            {
                int param = 0;
                GL.GetSamplerParameter(ID, SamplerParameter.TextureWrapS, out param);

                return (EdgeBehaviour)param;
            }

            set
            {
                GL.SamplerParameter(ID, SamplerParameter.TextureWrapS, (int)value);
            }
        }

        public EdgeBehaviour EdgeBehaviorY
        {
            get
            {
                int param = 0;
                GL.GetSamplerParameter(ID, SamplerParameter.TextureWrapT, out param);

                return (EdgeBehaviour)param;
            }

            set
            {
                GL.SamplerParameter(ID, SamplerParameter.TextureWrapT, (int)value);
            }
        }

        public EdgeBehaviour EdgeBehaviorZ
        {
            get
            {
                int param = 0;
                GL.GetSamplerParameter(ID, SamplerParameter.TextureWrapR, out param);

                return (EdgeBehaviour)param;
            }

            set
            {
                GL.SamplerParameter(ID, SamplerParameter.TextureWrapR, (int)value);
            }
        }
    }
}
