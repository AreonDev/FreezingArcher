using FurryLana.Engine.Interfaces.Material;
using FurryLana.Engine.Renderer.Interfaces;
using FurryLana.Engine.Texture.Interfaces;

namespace FurryLana.Engine.Material
{
    /// <summary>
    /// Material.
    /// </summary>
    public class Material : IMaterial
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FurryLana.Engine.Material.Material"/> class.
        /// </summary>
        /// <param name="diffuse">Diffuse texture.</param>
        /// <param name="normalHeight">Normal/Height texture.</param>
        /// <param name="specular">Specular texture.</param>
        /// <param name="ambient">Ambient color texture.</param>
        /// <param name="shaderProgram">Shader program.</param>
        public Material (ITexture diffuse,
                         ITexture normalHeight,
                         ITexture specular,
                         ITexture ambient,
                         IShaderProgram shaderProgram) 
        {
            Diffuse = diffuse;
            NormalHeight = normalHeight;
            Specular = specular;
            Ambient = ambient;
            ShaderProgram = shaderProgram;
        }

        /// <summary>
        /// Gets or sets the diffuse color map.
        /// RGBA channel used.
        /// </summary>
        /// <value>The diffuse color map.</value>
        public ITexture Diffuse { get; set; }

        /// <summary>
        /// Gets or sets the height and normal map.
        /// Normalmap channels: RGB
        /// Heightmap channels: A
        /// </summary>
        /// <value>The height and normal map.</value>
        public ITexture NormalHeight { get; set; }

        /// <summary>
        /// Gets or sets the specular map.
        /// Specularcolor: RGB
        /// Specularintensity: A
        /// </summary>
        /// <value>The specular map.</value>
        public ITexture Specular { get; set; }

        /// <summary>
        /// Gets or sets the ambient map.
        /// </summary>
        /// <value>The ambient map.</value>
        public ITexture Ambient { get; set; }

        /// <summary>
        /// Gets or sets the shader program.
        /// </summary>
        /// <value>The shader program.</value>
        public IShaderProgram ShaderProgram { get; set; }
    }
}
