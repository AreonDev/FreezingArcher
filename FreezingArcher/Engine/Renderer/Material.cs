//  Material.cs
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
using System.Collections.Generic;

using FreezingArcher.Math;

namespace FreezingArcher.Renderer
{
    public class Material
    {
        public string Name { get; internal set;}

        #region Colors
        public Color4 ColorAmbient { get; internal set;}
        public Color4 ColorDiffuse { get; internal set;}
        public Color4 ColorEmmissive { get; internal set;}
        public Color4 ColorSpecular{ get; internal set;}
        public Color4 ColorReflective{ get; internal set;}
        #endregion

        #region Other
        public bool TwoSided{ get; internal set;}
        public bool WireFramed{ get; internal set;}
        #endregion

        #region Shininess
        public float Shininess { get; internal set;}
        public float ShininessStrength{ get; internal set;}
        #endregion

        #region Textures 
        public Texture2D TextureAmbient { get; internal set;}
        public Texture2D TextureDiffuse{ get; internal set;}
        public Texture2D TextureEmissive{ get; internal set;}
        public Texture2D TextureSpecular{ get; internal set;}
        public Texture2D TextureReflective { get; internal set;}
        public Texture2D TextureReflection { get; internal set;}
        public Texture2D TextureNormal { get; internal set;}
        public Texture2D TextureDisplacement{ get; internal set;}
        public Texture2D TextureLightMap { get; internal set;}
        public Texture2D TextureOpacity{ get; internal set;}

        public bool HasTextureAmbient
        {
            get
            {
                return TextureAmbient != null;
            }
        }

        public bool HasTextureDiffuse
        {
            get
            {
                return TextureDiffuse != null;
            }
        }

        public bool HasTextureEmissive
        {
            get
            {
                return TextureEmissive != null;
            }
        }

        public bool HasTextureSpecular
        {
            get
            {
                return TextureSpecular != null;
            }
        }

        public bool HasTextureReflective
        {
            get
            {
                return TextureReflective != null;
            }
        }

        public bool HasTextureReflection
        {
            get
            {
                return TextureReflection != null;
            }
        }

        public bool HasTextureNormal
        {
            get
            {
                return TextureNormal != null;
            }
        }

        public bool HasTextureDisplacement
        {
            get
            {
                return TextureDisplacement != null;
            }
        }

        public bool HasTextureOpacity
        {
            get
            {
                return TextureOpacity != null;
            }
        }

        public bool HasTextureLightMap
        {
            get
            {
                return TextureLightMap != null;
            }
        }
        #endregion

        public Effect OptionalEffect { get; protected set;}
        public bool HasOptionalEffect{ get; protected set;}

        public Material()
        {
            
        }
    }
}

