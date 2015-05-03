using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreezingArcher.Renderer
{
    public enum ShaderType
    {
        TypeNone = 0,
        VertexShader,
        PixelShader,
        TesselationControlShader,
        TesselationEvaluationShader,
        GeometryShader
    }

    public class Shader : GraphicsResource
    {
        public string ShaderSource { get; private set; }
        public ShaderType ShaderType { get; private set; }

        internal Shader(string name, int id, string shader_source, ShaderType type) : base(name, id, GraphicsResourceType.Shader)
        {
            ShaderSource = shader_source;
            ShaderType = ShaderType;
        }
    }
}
