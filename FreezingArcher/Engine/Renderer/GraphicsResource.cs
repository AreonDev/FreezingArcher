using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreezingArcher.Renderer
{
    public enum GraphicsResourceType
    {
        Unknown = 0,
        VertexBuffer,
        IndexBuffer,
        UniformBuffer,
        FrameBuffer,
        VertexBufferArray,
        Shader,
        ShaderProgram,
        ProgramPipeline,
        Texture1D,
        Texture2D,
        Texture3D,
        Sampler,
    }

    public class GraphicsResource
    {
        #region Public Properties
        public bool Created { get; private set; }
        public String Name { get; private set; }
        public int ID { get; private set; }
        public GraphicsResourceType Type { get; private set; }
        public uint InternalUseCount { get; private set; }
        #endregion

        #region Public Methods

        protected bool m_PrepareMode;
        public virtual void BeginPrepare() { m_PrepareMode = true; }
        public virtual void EndPrepare() { m_PrepareMode = false; }

        #endregion

        #region Internal Things
        internal GraphicsResource(string name, int id, GraphicsResourceType type)
        {
            Name = name;
            ID = id;

            Created = true;

            InternalUseCount = 0;

            Type = type;
        }

        internal void SetUseCount(uint usecount)
        {
            InternalUseCount = usecount;
        }

        internal void SetCreated(bool created)
        {
            Created = created;
        }

        #endregion
    }
}
