using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assimp;
using Assimp.Configs;
using Assimp.Unmanaged;

namespace FreezingArcher.Renderer
{
    public class Model
    {
        public bool IsInitialized{ get; private set;}

        public List<Mesh> Meshes { get; internal set; }
        public List<Material> Materials { get; internal set;}

        internal Model()
        {
            IsInitialized = false;
        }
    }
}
