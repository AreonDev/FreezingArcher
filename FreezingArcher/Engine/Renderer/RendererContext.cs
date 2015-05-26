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

namespace FreezingArcher.Renderer
{
    public class RendererContext : RendererCore
    {
        private AssimpContext m_AssimpContext;

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

            //Hopefully, everything went right....
            return mdl;
        }

        public void DrawModel(Model mdl)
        {
            foreach (Mesh msh in mdl.Meshes)
            {
                #region Test

                #endregion

                #region TODO
                //TODO: Set Materials, and textures
                //Each material has its Effect Class

                //Sort each effect class

                //Configure for material

                //Store all matrices correctly
                #endregion

                //Draw all mesh
                DrawMesh(msh);
            }
        }

        public void DrawMesh(Mesh msh)
        {
            //Materials and Matrices should be correctly set
            msh.m_VertexBufferArray.BindVertexBufferArray();

            DrawElements(0, msh.m_Indices.IndexCount, RendererIndexType.UnsignedInt, 
                (msh.m_PrimitiveType == Mesh.PrimitiveType.Triangles) ? RendererBeginMode.Triangles : RendererBeginMode.Points);

            msh.m_VertexBufferArray.UnbindVertexBufferArray();
        }
    }
}

