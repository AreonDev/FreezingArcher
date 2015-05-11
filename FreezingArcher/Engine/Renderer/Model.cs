using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assimp;
using Assimp.Configs;

namespace FreezingArcher.Renderer
{
    class Model
    {
        public List<Mesh> Meshes { get; private set; }
        public List<Material> Materials { get; private set;}

        public static Model LoadModel(RendererCore rc, string path)
        {
            Model mdl = new Model();

            Assimp.Scene scn = SomeResources.AssimpCnt.ImportFile(path);

            //Copy mesh data to model
            mdl.Meshes = new List<Mesh>();

            foreach (Assimp.Mesh actual_mesh in scn.Meshes)
            {
                //Export faces, hopefully, every face is Triangle
                int[] indices = new int[actual_mesh.FaceCount * 3];
                for(int i = 0; i < indices.Length; i+=3)
                {
                    for(int j = 0; j < actual_mesh.Faces[i].IndexCount; j++)
                        indices[i+j] = actual_mesh.Faces[i].Indices[j];
                }
                    
                mdl.Meshes.Add(new Mesh(rc, path, actual_mesh.MaterialIndex, indices, 
                    actual_mesh.Vertices.ToArray(), actual_mesh.Normals.ToArray(), actual_mesh.Tangents.ToArray(), actual_mesh.BiTangents.ToArray(),
                    actual_mesh.TextureCoordinateChannels, actual_mesh.VertexColorChannels));

            }
                
            //Materials??? Ulalalala xD
            // FIXME: Please, HERE!
            mdl.Materials = new List<Material>();

            //Hopefully, everything went right....
            return mdl;
        }
            
        public void Draw(RendererCore rc)
        {
            foreach (Mesh msh in Meshes)
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
                msh.Draw(rc);
            }
        }
    }
}
