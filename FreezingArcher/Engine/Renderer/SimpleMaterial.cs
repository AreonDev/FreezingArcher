//
//  SimpleMaterial.cs
//
//  Author:
//       dboeg <${AuthorEmail}>
//
//  Copyright (c) 2015 dboeg
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;
using FreezingArcher.Math;

namespace FreezingArcher.Renderer
{
    public class SimpleMaterial : Material
    {
        public SimpleMaterial()
        {
            OptionalEffect = null;
            HasOptionalEffect = false;

            specularColor = Color4.White;
            diffuseColor = Color4.White;

            tile = 1.0f;

            plane = new Vector2(0.1f, 100.0f);
        }

        private Color4 specularColor;
        private Color4 diffuseColor;

        private Vector2 plane;
        private float tile;

        private Texture2D colorTexture;
        private Texture2D normalTexture;

        public Texture2D ColorTexture
        {
            get
            {
                return colorTexture;
            }

            set
            {
                if (colorTexture != null)
                    colorTexture.SetUseCount(colorTexture.InternalUseCount - 1);

                colorTexture = value;

                if (value != null)
                {
                    colorTexture.SetUseCount(colorTexture.InternalUseCount + 1);
                    colorTexture.Bind(1);
                    OptionalEffect.PixelProgram.SetUniform(OptionalEffect.PixelProgram.GetUniformLocation("ColorTexture"), 1);
                }
            }
        }

        public Texture2D NormalTexture
        {
            get
            {
                return normalTexture;
            }

            set
            {
                if (normalTexture != null)
                    normalTexture.SetUseCount(normalTexture.InternalUseCount - 1);

                normalTexture = value;

                if (value != null)
                {
                    normalTexture.SetUseCount(normalTexture.InternalUseCount + 1);
                    normalTexture.Bind(0);
                    OptionalEffect.PixelProgram.SetUniform(OptionalEffect.PixelProgram.GetUniformLocation("NormalTexture"), 0);
                }
            }
        }

        public Vector2 Plane
        {
            get
            {
                return plane;
            }

            set
            {
                plane = value;
                if (OptionalEffect != null)
                    OptionalEffect.PixelProgram.SetUniform(OptionalEffect.PixelProgram.GetUniformLocation("PlaneInformation"), plane);
            }
        }

        public float Tile
        {
            get
            {
                return tile;
            }

            set
            {
                tile = value;
                if (OptionalEffect != null)
                    OptionalEffect.PixelProgram.SetUniform(OptionalEffect.PixelProgram.GetUniformLocation("Tile"), tile);
            }
        }

        public Color4 SpecularColor
        {
            get
            {
                return specularColor;
            }

            set
            {
                specularColor = value;
                if (OptionalEffect != null)
                    OptionalEffect.PixelProgram.SetUniform(OptionalEffect.PixelProgram.GetUniformLocation("SpecularColor"), new Vector4(specularColor.R,
                        specularColor.G, specularColor.B, specularColor.A));
            }
        }

        public Color4 DiffuseColor
        {
            get
            {
                return diffuseColor;
            }

            set
            {
                diffuseColor = value;
                if (OptionalEffect != null)
                    OptionalEffect.PixelProgram.SetUniform(OptionalEffect.PixelProgram.GetUniformLocation("DiffuseColor"), new Vector4(diffuseColor.R, 
                        diffuseColor.G, diffuseColor.B, diffuseColor.A));
            }
        }



        public bool Init(RendererContext rc)
        {
            long ticks = DateTime.Now.Ticks;

            OptionalEffect = rc.CreateEffect("SimpleMaterial_" + ticks);
            if (OptionalEffect == null)
                return false;

            OptionalEffect.BindPipeline();

            OptionalEffect.VertexProgram = rc.CreateShaderProgramFromFile("SimpleMaterial_VS_" + ticks, ShaderType.VertexShader,
                "lib/Renderer/Effects/SimpleMaterial/vertex_shader.vs");

            OptionalEffect.PixelProgram = rc.CreateShaderProgramFromFile("SimpleMaterial_PS_" + ticks, ShaderType.PixelShader,
                "lib/Renderer/Effects/SimpleMaterial/pixel_shader.ps");

            DiffuseColor = diffuseColor;
            SpecularColor = specularColor;

            OptionalEffect.UnbindPipeline();
                
            return true;
        }
    }
}

