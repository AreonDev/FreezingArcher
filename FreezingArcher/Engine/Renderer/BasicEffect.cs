using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Pencil.Gaming.MathUtils;
using Pencil.Gaming.Graphics;
using FreezingArcher.Math;

using FreezingArcher.Output;

namespace FreezingArcher.Renderer
{
    public class BasicEffect : Effect
    {
        private struct MatricesBlock
        {
            public static int SIZE = sizeof(float) * 4 * 4 * 3;

            public FreezingArcher.Math.Matrix World;
            public FreezingArcher.Math.Matrix View;
            public FreezingArcher.Math.Matrix Projection;
        }

        //[StructLayout(LayoutKind.Sequential)]
        private struct ConfigurationBlock
        {
            public static int SIZE = 8;  

            public int unfug;
            public int unfug2;
        }

        #region Internal

        internal BasicEffect(string name, int id) : base(name, id)
        {
            m_MatricesBlock = new MatricesBlock();

            m_MatricesBlock.World = FreezingArcher.Math.Matrix.Identity;
            m_MatricesBlock.View = FreezingArcher.Math.Matrix.Identity;
            m_MatricesBlock.Projection = FreezingArcher.Math.Matrix.Identity;

            m_MatricesBlockUniformBuffer = null;

            m_ConfigurationBlock = new ConfigurationBlock();

            m_ConfigurationUniformBuffer = null;
        }

        internal bool Init(RendererCore rc)
        {
            m_MatricesBlockUniformBuffer = rc.CreateUniformBuffer<MatricesBlock>(m_MatricesBlock, MatricesBlock.SIZE, "Internal_Basic_Effect_Vertex_Shader_MatricesBlock");

            this.VertexProgram.SetUniformBlockBinding("MatricesBlock", 10);
            m_MatricesBlockUniformBuffer.SetBufferBase(10);

            m_ConfigurationUniformBuffer = rc.CreateUniformBuffer<ConfigurationBlock>(m_ConfigurationBlock, ConfigurationBlock.SIZE, "Internal_Basic_Effect_Vertex_Pixel_Shader_ConfigurationBlock");

            //this.VertexProgram.SetUniformBlockBinding("ConfigurationBlock", 5);
            //this.PixelProgram.SetUniformBlockBinding("ConfigurationBlock", 5);

            //m_ConfigurationUniformBuffer.SetBufferBase(5);

            return true;
        }
        #endregion

        #region Private Blocks
        private MatricesBlock m_MatricesBlock;
        private UniformBuffer m_MatricesBlockUniformBuffer;

        private ConfigurationBlock m_ConfigurationBlock;
        private UniformBuffer m_ConfigurationUniformBuffer;

        private Texture m_Texture1;
        private Texture m_Texture2;
        private Texture m_Texture3;
        private Texture m_Texture4;
        private Texture m_Texture5;

        #endregion


        #region Public Members
        public FreezingArcher.Math.Matrix World
        {
            get
            {
                return m_MatricesBlock.World;
            }
            set
            {
                m_MatricesBlock.World = value;
            }
        }
        public FreezingArcher.Math.Matrix View
        {
            get
            {
                return m_MatricesBlock.View;
            }
            set
            {
                m_MatricesBlock.View = value;
            }
        }
        public FreezingArcher.Math.Matrix Projection
        {
            get
            {
                return m_MatricesBlock.Projection;
            }
            set
            {
                m_MatricesBlock.Projection = value;
            }
        }

        #region Locations
        public uint LocationInPosition
        {
            get
            {
                return 0;
            }
        }

        public uint LocationInNormal
        {
            get
            {
                return 1;
            }
        }

        public uint LocationInTexCoord1
        {
            get
            {
                return 2;
            }
        }

        public uint LocationInTexCoord2
        {
            get
            {
                return 3;
            }
        }

        public uint LocationInTexCoord3
        {
            get
            {
                return 4;
            }
        }

        public uint LocationInTexCoord4
        {
            get
            {
                return 5;
            }
        }

        public uint LocationInTexCoord5
        {
            get
            {
                return 6;
            }
        }

        public uint LocationInColor
        {
            get
            {
                return 7;
            }
        }

        public uint LocationInTexCoordIntensity1
        {
            get
            {
                return 8;
            }
        }

        public uint LocationInTexCoordIntensity2
        {
            get
            {
                return 9;
            }
        }

        public uint LocationInTexCoordIntensity3
        {
            get
            {
                return 10;
            }
        }

        public uint LocationInTexCoordIntensity4
        {
            get
            {
                return 11;
            }
        }

        public uint LocationInTexCoordIntensity5
        {
            get
            {
                return 12;
            }
        }

        #endregion

        public Texture Texture1
        {
            get
            {
                return m_Texture1;
            }
            set
            {
                if (m_Texture1 != null)
                    m_Texture1.SetUseCount(m_Texture1.InternalUseCount - 1);

                m_Texture1 = value;

                if (value != null)
                {
                    m_Texture1.SetUseCount(m_Texture1.InternalUseCount + 1);
                    m_Texture1.Bind(0);
                    PixelProgram.SetUniform(PixelProgram.GetUniformLocation("Texture1"), 0);
                }
            }
        }

        public Texture Texture2
        {
            get
            {
                return m_Texture2;
            }
            set
            {
                if (m_Texture2 != null)
                    m_Texture2.SetUseCount(m_Texture2.InternalUseCount - 1);

                m_Texture2 = value;

                if (value != null)
                {
                    m_Texture2.SetUseCount(m_Texture2.InternalUseCount + 1);
                    m_Texture2.Bind(1);
                    PixelProgram.SetUniform(PixelProgram.GetUniformLocation("Texture2"), 1);
                }
            }
        }

        public Texture Texture3
        {
            get
            {
                return m_Texture3;
            }
            set
            {
                if (m_Texture3 != null)
                    m_Texture3.SetUseCount(m_Texture3.InternalUseCount - 1);

                m_Texture3 = value;

                if (value != null)
                {
                    m_Texture3.SetUseCount(m_Texture3.InternalUseCount + 1);
                    m_Texture3.Bind(2);
                    PixelProgram.SetUniform(PixelProgram.GetUniformLocation("Texture3"), 2);
                }
            }
        }

        public Texture Texture4
        {
            get
            {
                return m_Texture4;
            }
            set
            {
                if (m_Texture4 != null)
                    m_Texture4.SetUseCount(m_Texture4.InternalUseCount - 1);

                m_Texture4 = value;

                if (value != null)
                {
                    m_Texture4.SetUseCount(m_Texture4.InternalUseCount + 1);
                    m_Texture4.Bind(3);
                    PixelProgram.SetUniform(PixelProgram.GetUniformLocation("Texture4"), 3);
                }
            }
        }

        public Texture Texture5
        {
            get
            {
                return m_Texture5;
            }
            set
            {
                if (m_Texture5 != null)
                    m_Texture5.SetUseCount(m_Texture5.InternalUseCount - 1);

                m_Texture5 = value;

                if (value != null)
                {
                    m_Texture5.SetUseCount(m_Texture5.InternalUseCount + 1);
                    m_Texture5.Bind(4);
                    PixelProgram.SetUniform(PixelProgram.GetUniformLocation("Texture5"), 4);
                }
            }
        }

        #endregion



        public void Update()
        {
            m_MatricesBlockUniformBuffer.UpdateBuffer<MatricesBlock>(m_MatricesBlock, MatricesBlock.SIZE);

            m_ConfigurationUniformBuffer.UpdateBuffer<ConfigurationBlock>(m_ConfigurationBlock, ConfigurationBlock.SIZE);
        }

        public void ClearTextures()
        {
            Texture1 = null;
            Texture2 = null;
            Texture3 = null;
            Texture4 = null;
            Texture5 = null;
        }

        public void Use()
        {
            this.BindPipeline();
        }
    }
}
