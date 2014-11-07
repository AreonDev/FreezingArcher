using FurryLana.Engine.Interfaces.Material;
using FurryLana.Engine.Texture.Interfaces;
using FurryLana.Engine.Renderer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurryLana.Engine.Material
{
    public class Material : IMaterial
    {
        public Material(ITexture diffuse, ITexture normalHeight, ITexture specular, ITexture ambient, IShaderProgram shaderProgramm) 
        {
            Diffuse = diffuse;
            NormalHeight = normalHeight;
            Specular = specular;
            Ambient = ambient;
            ShaderProgram = shaderProgramm;
        }

        public ITexture Diffuse { get; set; }

        public ITexture NormalHeight { get; set; }

        public ITexture Specular { get; set; }

        public ITexture Ambient { get; set; }

        public IShaderProgram ShaderProgram { get; set; }
    }
}
