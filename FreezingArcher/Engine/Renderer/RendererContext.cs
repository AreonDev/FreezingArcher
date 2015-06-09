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

using System.Xml;

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

        public SimpleMaterial SimpleMaterial { get; private set;}

        public RendererContext(MessageManager mssgmngr) : base(mssgmngr)
        {
            m_AssimpContext = new AssimpContext();
        }

        ~RendererContext()
        {
            m_AssimpContext.Dispose();
        }

        public override bool Init()
        {
            base.Init();

            SimpleMaterial = new SimpleMaterial();
            SimpleMaterial.Init(this);

            return true;
        }

        public Model LoadModel(string path)
        {
            bool use_xml = false;

            Model mdl = new Model();

            string folder_path = System.IO.Path.GetDirectoryName(path);

            if (System.IO.Path.GetExtension(path) == ".xml")
                use_xml = true;

            string file_path = path;

            Material bla_mat = new Material();

            #region XML
            if (use_xml)
            {
                using (XmlReader reader = XmlReader.Create(path))
                {
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "model"))
                        {
                            while((reader.NodeType != XmlNodeType.Element) || (reader.Name != "object")) reader.Read();

                            reader.Read();

                            if (!reader.HasValue)
                                return null;

                            file_path = reader.Value;
                        }

                        if((reader.NodeType == XmlNodeType.Element) && (reader.Name == "material"))
                        {
                            reader.ReadToFollowing("graphics");

                            if (reader.HasAttributes)
                            {
                                bla_mat.TextureDiffuse = 
                                    CreateTexture2D(file_path + "_Material_XML_TextureDiffuse_" + DateTime.Now.Ticks, true, folder_path + "/" + reader.GetAttribute("diffuse"));
                                bla_mat.TextureNormal = 
                                    CreateTexture2D(file_path + "_Material_XML_TextureNormal_" + DateTime.Now.Ticks, true, folder_path + "/" + reader.GetAttribute("normal"));

                                //More material information
                                bla_mat.ColorDiffuse = FreezingArcher.Math.Color4.White;
                                bla_mat.ColorSpecular = FreezingArcher.Math.Color4.White;
                            }
                        }
                    }
                }
            }
            #endregion


            /*
            Assimp.Scene scn = m_AssimpContext.ImportFile(folder_path + "/" + file_path);
            NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(66.0f);
            m_AssimpContext.SetConfig(config);
            */

            //Tue unfug!
            //Load OBJ Mesh


            //Copy mesh data to model
            mdl.Meshes = new List<Mesh>();

            //foreach (Assimp.Mesh actual_mesh in scn.Meshes)
            //{
                //Export faces, hopefully, every face is Triangle
            int[] indices;//= new int[actual_mesh.FaceCount * 3];
            Pencil.Gaming.MathUtils.Vector4[] positions;
            Pencil.Gaming.MathUtils.Vector3[] normals;
            Pencil.Gaming.MathUtils.Vector2[] texcoords;

            Pencil.Gaming.Graphics.GL.Utils.LoadModel(folder_path + "/" + file_path, out positions, out normals, out texcoords, out indices);

            //Convert zeugs
            Vector3[] npositions = new Vector3[positions.Length];
            for (int i = 0; i < positions.Length; i++)
                npositions[i] = new Vector3(positions[i].X, positions[i].Y, positions[i].Z);

            Vector3[] nnormals = new Vector3[normals.Length];
            for (int i = 0; i < normals.Length; i++)
                nnormals[i] = new Vector3(normals[i].X, normals[i].Y, normals[i].Z);

            //Convert texcoords to 3d texcoords
            Vector3[] texcoords3d = new Vector3[texcoords.Length];
            List<Vector3> tex_coords_3d_list = new List<Vector3>();
            List<Vector3>[] tex_coord_list_array = new List<Vector3>[] { tex_coords_3d_list };

            for (int i = 0; i < texcoords.Length; i++)
                tex_coords_3d_list.Add(new Vector3(texcoords[i].X, texcoords[i].Y, 1));



                //for(int i = 0; i < actual_mesh.FaceCount; i++)
                //{
                //    for(int j = 0; j < actual_mesh.Faces[i].IndexCount; j++)
                //        indices[(i*3)+j] = actual_mesh.Faces[i].Indices[j];
                //}

                int mat_index = 0;

                if (use_xml)
                    mat_index = 0;

            mdl.Meshes.Add(new Mesh(this, folder_path + "/" + file_path, mat_index, indices, npositions, nnormals, null, null, 
                    tex_coord_list_array, null, Mesh.PrimitiveType.Triangles));


                //mdl.Meshes.Add(new Mesh(this, path, mat_index, indices, 
                //    actual_mesh.Vertices.ToArray(), actual_mesh.Normals.ToArray(), actual_mesh.Tangents.ToArray(), actual_mesh.BiTangents.ToArray(),
                //    actual_mesh.TextureCoordinateChannels, actual_mesh.VertexColorChannels, (Mesh.PrimitiveType)actual_mesh.PrimitiveType));
            //}
             
            mdl.Materials = new List<Material>();

            if (!use_xml)
            {
                /*
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
                    material.TextureAmbient = mat.HasTextureAmbient ? this.CreateTexture2D(file_path + "_Material_" + mdl.Materials.Count + "_TextureAmbient_" + ticks, true,
                        folder_path + "/" + mat.TextureAmbient.FilePath) : null;

                    material.TextureDiffuse = mat.HasTextureDiffuse ? this.CreateTexture2D(file_path + "_Material_" + mdl.Materials.Count + "_TextureDiffuse_" + ticks, true,
                        folder_path + "/" + mat.TextureDiffuse.FilePath) : null;

                    material.TextureEmissive = mat.HasTextureEmissive ? this.CreateTexture2D(file_path + "_Material_" + mdl.Materials.Count + "_TextureEmissive_" + ticks, true,
                        folder_path + "/" + mat.TextureEmissive.FilePath) : null;

                    material.TextureLightMap = mat.HasTextureLightMap ? this.CreateTexture2D(file_path + "_Material_" + mdl.Materials.Count + "_TextureLightMap_" + ticks, true,
                        folder_path + "/" + mat.TextureLightMap.FilePath) : null;

                    material.TextureNormal = mat.HasTextureNormal ? this.CreateTexture2D(file_path + "_Material_" + mdl.Materials.Count + "_TextureNormal_" + ticks, true,
                        folder_path + "/" + mat.TextureNormal.FilePath) : null;

                    material.TextureOpacity = mat.HasTextureOpacity ? this.CreateTexture2D(file_path + "_Material_" + mdl.Materials.Count + "_TextureOpacity_" + ticks, true,
                        folder_path + "/" + mat.TextureOpacity.FilePath) : null;

                    material.TextureReflection = mat.HasTextureReflection ? this.CreateTexture2D(file_path + "_Material_" + mdl.Materials.Count + "_TextureReflection_" + ticks, true,
                        folder_path + "/" + mat.TextureReflection.FilePath) : null;

                    material.TextureDisplacement = mat.HasTextureDisplacement ? this.CreateTexture2D(file_path + "_Material_" + mdl.Materials.Count + "_TextureDisplacement_" + ticks, true,
                        folder_path + "/" + mat.TextureDisplacement.FilePath) : null;

                    material.TextureReflective = null;


                    material.TextureSpecular = mat.HasTextureSpecular ? this.CreateTexture2D(path + "_Material_" + mdl.Materials.Count + "_TextureSpecular_" + ticks, true,
                        folder_path + "/" + mat.TextureSpecular.FilePath) : null;

                    mdl.Materials.Add(material);
                }*/
            }
            else
                mdl.Materials.Add(bla_mat);

            //Hopefully, everything went right....
            return mdl;
        }

        public void DrawModel(Model mdl, Matrix world, int count = 1, CoreScene scene = null)
        {
            if (mdl != null)
            {
                EnableDepthTest(true);

                foreach (Mesh msh in mdl.Meshes)
                {
                    Material mat = null;

                    if (msh.MaterialIndex >= 0)
                    {
                        mat = mdl.Materials[msh.MaterialIndex];
                    }
                    else
                    {
                        mat = new Material();
                        mat.ColorDiffuse = FreezingArcher.Math.Color4.White;
                        mat.ColorSpecular = FreezingArcher.Math.Color4.White;

                        mdl.Materials.Add(mat);

                        msh.MaterialIndex = 0;
                    }

                    if(!mat.HasOptionalEffect)
                    {
                        SimpleMaterial.SpecularColor = mat.ColorSpecular;
                        SimpleMaterial.DiffuseColor = mat.ColorDiffuse;

                        SimpleMaterial.NormalTexture = mat.TextureNormal;
                        SimpleMaterial.ColorTexture = mat.TextureDiffuse;

                        SimpleMaterial.Tile = 1.0f;

                        if (scene == null)
                            SimpleMaterial.Plane = new Vector2(0.1f, 100.0f);
                        else
                        {
                            BaseCam cam = scene.CamManager.GetActiveCam();

                            if(cam != null)
                                SimpleMaterial.Plane = new Vector2(cam.zNear, cam.zFar);
                        }

                        mat = SimpleMaterial;
                    }

                    //Set Scene Camera settings
                    mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("WorldMatrix"), world);

                    if(scene != null)
                    {
                        BaseCam cam = scene.CamManager.GetActiveCam();

                        if (cam != null)
                        {
                            mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("ViewMatrix"),
                                scene.CamManager.GetActiveCam().ViewMatrix);
                            mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("ProjectionMatrix"),
                                scene.CamManager.GetActiveCam().ProjectionMatrix);
                        }
                    }

                    if (count > 1)
                        mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("InstancedDrawing"), 1);
                    else
                        mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("InstancedDrawing"), 0);

                    mat.OptionalEffect.BindPipeline();

                    //Draw all mesh
                    DrawMesh(msh, count);

                    mat.OptionalEffect.UnbindPipeline();
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
                if (Scene.FrameBufferDepthStencilTexture.Width != ViewportSize.X ||
                   Scene.FrameBufferDepthStencilTexture.Height != ViewportSize.Y)
                    Scene.ResizeTextures(ViewportSize.X, ViewportSize.Y);

                Scene.FrameBuffer.UseAttachments(new FrameBuffer.AttachmentUsage[]
                    {FrameBuffer.AttachmentUsage.Color0,
                        FrameBuffer.AttachmentUsage.Color1, FrameBuffer.AttachmentUsage.Color2,
                        FrameBuffer.AttachmentUsage.Color3
                    });

                //Scene.FrameBuffer.Bind(FrameBuffer.FrameBufferTarget.Draw);

                this.Clear(Scene.BackgroundColor, 1);

                foreach (SceneObject obj in Scene.GetObjects())
                {
                    obj.Update();

                    obj.Draw(this);
                }

                //Scene.FrameBuffer.Unbind();
            }

            //Sprite spr = new Sprite();
            //spr.Init(Scene.FrameBufferColorTexture);
            //spr.AbsolutePosition = new Vector2(0.0f, 0.0f);
            //spr.Scaling = new Vector2(1, 1);

            //DrawSpriteAbsolute(spr);
        }
    }
}

