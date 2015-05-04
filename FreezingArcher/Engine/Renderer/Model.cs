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

        public static Model LoadModel(RendererCore rc, string path)
        {
            Model mdl = new Model();

            Assimp.Scene scn = SomeResources.AssimpCnt.ImportFile(path);



            return mdl;
        }
    }
}
