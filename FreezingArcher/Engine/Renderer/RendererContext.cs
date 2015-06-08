//
//  RendererContext.cs
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
using FreezingArcher.Messaging;
using Assimp;
using Assimp.Configs;
using System.Collections.Generic;

using Pencil.Gaming.Graphics;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Math;

namespace FreezingArcher.Renderer
{
    public class RendererContext : RendererCore
    {
        private AssimpContext m_AssimpContext;


        private CoreScene PrivateScene;
        /// <summary>
        /// Gets or sets the scene.
        /// </summary>
        /// <value>The scene.</value>
        public CoreScene Scene 
        {
            get
            {
                return PrivateScene;
            }
            set
            {
                PrivateScene = value;
                PrivateScene.Init(this);
            }
        }

        public RendererContext(MessageManager mssgmngr) : base(mssgmngr)
        {
            m_AssimpContext = new AssimpContext();
        }

        ~RendererContext()
        {
            m_AssimpContext.Dispose();
        }

        public Model LoadModel(string path)
        {
            Model mdl = new Model();

            string folder_path = System.IO.Path.GetDirectoryName(path);

            Assimp.Scene scn = m_AssimpContext.ImportFile(path);
            NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(66.0f);
            m_AssimpContext.SetConfig(config);

            //Copy mesh data to model
            mdl.Meshes = new List<Mesh>();

            foreach (Assimp.Mesh actual_mesh in scn.Meshes)
            {
                //Export faces, hopefully, every face is Triangle
                int[] indices = new int[actual_mesh.FaceCount * 3];
                for(int i = 0; i < actual_mesh.FaceCount; i++)
                {
                    for(int j = 0; j < actual_mesh.Faces[i].IndexCount; j++)
                        indices[(i*3)+j] = actual_mesh.Faces[i].Indices[j];
                }

                mdl.Meshes.Add(new Mesh(this, path, actual_mesh.MaterialIndex, indices, 
                    actual_mesh.Vertices.ToArray(), actual_mesh.Normals.ToArray(), actual_mesh.Tangents.ToArray(), actual_mesh.BiTangents.ToArray(),
                    actual_mesh.TextureCoordinateChannels, actual_mesh.VertexColorChannels, (Mesh.PrimitiveType)actual_mesh.PrimitiveType));
            }

            //Materials??? Ulalalala xD
            // FIXME: Please, HERE!
            mdl.Materials = new List<Material>();
            foreach (Assimp.Material mat in scn.Materials)
            {
                long ticks = DateTime.Now.Ticks;

                if (mat.Name == "DefaultMaterial")
                    continue;

                Material material = new Material();

                material.Name = mat.Name;

                material.Shininess = mat.Shininess;
                material.ShininessStrength = mat.ShininessStrength;

                material.TwoSided = mat.IsTwoSided;
                material.WireFramed = mat.IsWireFrameEnabled;

                //Copy all colors
                material.ColorAmbient = mat.ColorAmbient;
                material.ColorDiffuse = mat.ColorDiffuse;
                material.ColorEmmissive = mat.ColorEmissive;
                material.ColorReflective = mat.ColorReflective;
                material.ColorSpecular = mat.ColorSpecular;

                //Load all textures
                material.TextureAmbient = mat.HasTextureAmbient ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureAmbient_"+ticks, true,
                    folder_path + "/" + mat.TextureAmbient.FilePath) : null;

                material.TextureDiffuse = mat.HasTextureDiffuse ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureDiffuse_"+ticks, true,
                    folder_path + "/" + mat.TextureDiffuse.FilePath) : null;

                material.TextureEmissive = mat.HasTextureEmissive ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureEmissive_"+ticks, true,
                    folder_path + "/" + mat.TextureEmissive.FilePath) : null;

                material.TextureLightMap = mat.HasTextureLightMap ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureLightMap_"+ticks, true,
                    folder_path + "/" + mat.TextureLightMap.FilePath) : null;

                material.TextureNormal = mat.HasTextureNormal ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureNormal_"+ticks, true,
                    folder_path + "/" + mat.TextureNormal.FilePath) : null;

                material.TextureOpacity = mat.HasTextureOpacity ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureOpacity_"+ticks, true,
                    folder_path + "/" + mat.TextureOpacity.FilePath) : null;

                material.TextureReflection = mat.HasTextureReflection ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureReflection_"+ticks, true,
                    folder_path + "/" + mat.TextureReflection.FilePath) : null;

                material.TextureDisplacement = mat.HasTextureDisplacement ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureDisplacement_"+ticks, true,
                    folder_path + "/" + mat.TextureDisplacement.FilePath) : null;

                material.TextureReflective = null;


                material.TextureSpecular = mat.HasTextureSpecular ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureSpecular_"+ticks, true,
                    folder_path + "/" + mat.TextureSpecular.FilePath) : null;

                mdl.Materials.Add(material);
            }

            //Hopefully, everything went right....
            return mdl;
        }

        public void DrawModel(Model mdl, Matrix world, int count = 1)
        {
            if (mdl != null)
            {
                foreach (Mesh msh in mdl.Meshes)
                {
                    Material mat = mdl.Materials[msh.MaterialIndex];
                    if(!mat.HasOptionalEffect)
                    {
                        SimpleMaterial mat2 = new SimpleMaterial();
                        mat2.Init(this);

                        mat2.SpecularColor = mat.ColorSpecular;
                        mat2.DiffuseColor = mat.ColorDiffuse;

                        mat2.NormalTexture = mat.TextureNormal;
                        mat2.ColorTexture = mat.TextureDiffuse;

                        mat = mat2;
                    }

                    //Set Scene Camera settings
                    mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("WorldMatrix"), world);


                    //Draw all mesh
                    DrawMesh(msh, count);
                }
            }
        }

        public void DrawMesh(Mesh msh, int count = 1)
        {
            //Materials and Matrices should be correctly set
            msh.m_VertexBufferArray.BindVertexBufferArray();

            if (count == 1)
                DrawElements(0, msh.m_Indices.IndexCount, RendererIndexType.UnsignedInt, 
                    (msh.m_PrimitiveType == Mesh.PrimitiveType.Triangles) ? RendererBeginMode.Triangles : RendererBeginMode.Points);
            else
                DrawElementsInstanced(0, msh.m_Indices.IndexCount, RendererIndexType.UnsignedInt,
                    (msh.m_PrimitiveType == Mesh.PrimitiveType.Triangles) ? RendererBeginMode.Triangles : RendererBeginMode.Points, count);

            msh.m_VertexBufferArray.UnbindVertexBufferArray();
        }



        /// <summary>
        /// FOR THE FUCKING FIN
        /// </summary>
        public void DrawScene()
        {
            if (Scene != null)
            {
                Scene.FrameBuffer.Bind(FrameBuffer.FrameBufferTarget.Draw);

                this.Clear(Scene.BackgroundColor, 1);

                foreach (SceneObject obj in Scene.GetObjects())
                {
                    obj.Update();

                    obj.Draw(this);
                }

                Scene.FrameBuffer.Unbind();
            }
        }
    }
}

