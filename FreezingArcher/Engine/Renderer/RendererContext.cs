﻿//
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

using Gwen;
using Gwen.Control;
using Gwen.Renderer;

using Pencil.Gaming.Graphics;
using FreezingArcher.Renderer.Compositor;
using FreezingArcher.Renderer.Scene;
using FreezingArcher.Renderer.Scene.SceneObjects;
using FreezingArcher.Math;
using FreezingArcher.Output;

namespace FreezingArcher.Renderer
{
    public class RendererContext : RendererCore
    {
        public Gwen.Renderer.Base GwenRenderer { get; private set;}
        public Gwen.Skin.TexturedBase Skin { get; private set;}

        public Gwen.Control.Canvas _PrivateCanvas;
        public Gwen.Control.Canvas Canvas{ get; private set;}

        private class RCActionInitSceneObject : RCAction
        {
            public SceneObject Object;
            public RendererContext Context;

            public RCActionInitSceneObject(SceneObject toinit, RendererContext ctx) 
            {
                Object = toinit;
                Context = ctx;
            }

            public RCActionDelegate Action
            {
                get
                {
                    return delegate()
                    {
                        if(!Object.IsInitialized)
                            Object.Init(Context);
                    };
                }
            }
        }

        private AssimpContext m_AssimpContext;

        private BasicCompositor PrivateCompositor;

        public BasicCompositor Compositor
        {
            get
            {
                return PrivateCompositor;
            }

            set
            {
                PrivateCompositor = value;
            }
        }

        public SimpleMaterial SimpleMaterial { get; private set;}

        public RendererContext(MessageProvider messageProvider) : base(messageProvider)
        {
            m_AssimpContext = new AssimpContext();

            PrivateCompositor = null;
        }

        ~RendererContext()
        {
            m_AssimpContext.Dispose();
        }

        public override bool Init(FreezingArcher.Core.Application app)
        {
            base.Init(app);

            SimpleMaterial = new SimpleMaterial();
            SimpleMaterial.Init(this);

            GwenRenderer = new Gwen.Renderer.FreezingArcherGwenRenderer(this);
            Skin = new Gwen.Skin.TexturedBase(GwenRenderer, "lib/UI/Skins/NoWayOutSkin.png");
            Canvas = new Gwen.Control.Canvas(Skin);
            _PrivateCanvas = Canvas;

            return true;
        }

        public Canvas CreateCanvas()
        {
            return new Canvas (Skin);
        }

        public void DestroyCanvas(Canvas canvas)
        {
            if (Canvas == canvas)
                SetCanvas (null);

            canvas.Dispose ();
        }

        public void SetCanvas(Canvas canvas)
        {
            if (canvas == null)
                Canvas = _PrivateCanvas;
            else
                Canvas = canvas;
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
                                try
                                {
                                    bla_mat.TextureDiffuse = 
                                        CreateTexture2D(file_path + "_Material_XML_TextureDiffuse_" + DateTime.Now.Ticks, true, folder_path + "/" + reader.GetAttribute("diffuse"));
                                        
                                    bla_mat.TextureDiffuse.Sampler.EdgeBehaviorX = EdgeBehaviour.ClampToEdge;
                                    bla_mat.TextureDiffuse.Sampler.EdgeBehaviorY = EdgeBehaviour.ClampToEdge;
                                    bla_mat.TextureDiffuse.Sampler.EdgeBehaviorZ = EdgeBehaviour.ClampToEdge;

                                    bla_mat.TextureNormal = 
                                        CreateTexture2D(file_path + "_Material_XML_TextureNormal_" + DateTime.Now.Ticks, true, folder_path + "/" + reader.GetAttribute("normal"));
                                }catch(System.IO.FileNotFoundException e)
                                {

                                }

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
                tex_coords_3d_list.Add(new Vector3(texcoords[i].X, 1-texcoords[i].Y, 1));



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

        public void DeleteModel(Model mdl)
        {
            if (mdl != null)
            {
                foreach (Mesh msh in mdl.Meshes)
                {
                    DeleteGraphicsResourceAsync(msh.m_VertexBufferArray);

                    DeleteGraphicsResourceAsync(msh.m_Indices);
                    DeleteGraphicsResourceAsync(msh.m_VertexBiTangent);

                    if (msh.m_VertexColors != null)
                        foreach (VertexBuffer vb in msh.m_VertexColors)
                            DeleteGraphicsResourceAsync(vb);

                    DeleteGraphicsResourceAsync(msh.m_VertexNormal);
                    DeleteGraphicsResourceAsync(msh.m_VertexPosition);
                    DeleteGraphicsResourceAsync(msh.m_VertexTangent);

                    foreach (VertexBuffer vb in msh.m_VertexTexCoords)
                        DeleteGraphicsResourceAsync(vb);
                }

                foreach (Material mat in mdl.Materials)
                {
                    DeleteGraphicsResourceAsync(mat.TextureSpecular);
                    DeleteGraphicsResourceAsync(mat.TextureReflective);
                    DeleteGraphicsResourceAsync(mat.TextureReflection);
                    DeleteGraphicsResourceAsync(mat.TextureOpacity);
                    DeleteGraphicsResourceAsync(mat.TextureNormal);
                    DeleteGraphicsResourceAsync(mat.TextureLightMap);
                    DeleteGraphicsResourceAsync(mat.TextureEmissive);
                    DeleteGraphicsResourceAsync(mat.TextureDisplacement);
                    DeleteGraphicsResourceAsync(mat.TextureDiffuse);
                    DeleteGraphicsResourceAsync(mat.TextureAmbient);
                }
            }
        }

        public void DrawModel(Model mdl, Matrix world, int count = 1, CoreScene scene = null)
        {
            if (mdl != null)
            {
                EnableDepthTest(mdl.EnableDepthTest);
                EnableDepthMaskWriting(mdl.EnableDepthTest);

                SetCullMode(RendererCullMode.Front);

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

                        mat = SimpleMaterial;
                    }

                    if (count > 1)
                        mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("InstancedDrawing"), 1);
                    else
                        mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("InstancedDrawing"), 0);


                    //Set Scene Camera settings
                    mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("WorldMatrix"), world);

                    if(scene != null)
                    {
                        BaseCamera cam = scene.CameraManager.ActiveCamera;

                        if (cam != null)
                        {
                            mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("ViewMatrix"),
                                scene.CameraManager.ActiveCamera.ViewMatrix);
                            mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("ProjectionMatrix"),
                                scene.CameraManager.ActiveCamera.ProjectionMatrix);

                            mat.OptionalEffect.VertexProgram.SetUniform(mat.OptionalEffect.VertexProgram.GetUniformLocation("EyePosition"), 
                                scene.CameraManager.ActiveCamera.Position);
                        }
                    }

                    mat.OptionalEffect.PixelProgram.SetUniform(mat.OptionalEffect.PixelProgram.GetUniformLocation("EnableLighting"), mdl.EnableLighting ? 1 : 0);
                        
                    mat.OptionalEffect.BindPipeline();

                    //Draw all mesh
                    DrawMesh(msh, count);

                    mat.OptionalEffect.UnbindPipeline();
                }

                EnableDepthTest(true);
                EnableDepthMaskWriting(true);
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

        public void Compose()
        {
            if (PrivateCompositor != null)
            {
                PrivateCompositor.StartCompositing();
            }
        }

        public override void ConsumeMessage(Messaging.Interfaces.IMessage msg)
        {
            base.ConsumeMessage(msg);

            Messaging.WindowResizeMessage wrm = msg as Messaging.WindowResizeMessage;
            if (wrm != null)
            {
                Canvas.SetBounds(new System.Drawing.Rectangle(0, 0, wrm.Width, wrm.Height));
            }
        }
    }
}

