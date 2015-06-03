using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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
        private struct LightBlock1
        {
            public static int SIZE = sizeof(float) * 4 + sizeof(float)*3;

            public float LightColorR;
            public float LightColorG;
            public float LightColorB;
            public float LightColorA;

            public float LightPositionX;
            public float LightPositionY;
            public float LightPositionZ;
        }

        private struct MaterialBlock
        {
            public static int SIZE = sizeof(float) * 4 * 3 + sizeof(float);

            public Color4 Ambient;
            public Color4 Diffuse;
            public Color4 Specular;
            public float Shininess;
        }

        #region Internal

        internal BasicEffect(string name, int id) : base(name, id)
        {
            m_MatricesBlock = new MatricesBlock();

            m_MatricesBlock.World = FreezingArcher.Math.Matrix.Identity;
            m_MatricesBlock.View = FreezingArcher.Math.Matrix.Identity;
            m_MatricesBlock.Projection = FreezingArcher.Math.Matrix.Identity;

            m_MatricesBlockUniformBuffer = null;

            m_LightBlock = new LightBlock1();
            m_LightBlock.LightColorA = 1.0f;
            m_LightBlock.LightColorB = 1.0f;
            m_LightBlock.LightColorG = 1.0f;
            m_LightBlock.LightColorR = 1.0f;

            m_MaterialBlock = new MaterialBlock();

            m_LightBlockUniformBuffer = null;

            m_MaterialBlockUniformBuffer = null;
        }

        internal bool Init(RendererCore rc)
        {
            m_MatricesBlockUniformBuffer = rc.CreateUniformBuffer<MatricesBlock>(m_MatricesBlock, MatricesBlock.SIZE, "Internal_Basic_Effect_Vertex_Shader_MatricesBlock");

            this.VertexProgram.SetUniformBlockBinding("MatricesBlock", 10);
            m_MatricesBlockUniformBuffer.SetBufferBase(10);

            m_LightBlockUniformBuffer = rc.CreateUniformBuffer<LightBlock1>(m_LightBlock, LightBlock1.SIZE, "Internal_Basic_Effect_Vertex_Pixel_Shader_LightBlock");

            this.VertexProgram.SetUniformBlockBinding("LightBlock", 4);

            m_LightBlockUniformBuffer.SetBufferBase(4);

            m_MaterialBlockUniformBuffer = rc.CreateUniformBuffer<MaterialBlock>(m_MaterialBlock, MaterialBlock.SIZE, "Internal_Basic_Effect_MaterialBlock");

            this.PixelProgram.SetUniformBlockBinding("MaterialBlock", 6);
            m_MaterialBlockUniformBuffer.SetBufferBase(6);

            return true;
        }
        #endregion

        #region Private Blocks
        private MatricesBlock m_MatricesBlock;
        private UniformBuffer m_MatricesBlockUniformBuffer;

        private LightBlock1 m_LightBlock;
        private UniformBuffer m_LightBlockUniformBuffer;

        private MaterialBlock m_MaterialBlock;
        private UniformBuffer m_MaterialBlockUniformBuffer;
       

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

        public uint LocationInTangent
        {
            get
            {
                return 2;
            }
        }

        public uint LocationInBiTangent
        {
            get
            {
                return 3;
            }
        }

        public uint LocationInTexCoord1
        {
            get
            {
                return 4;
            }
        }

        public uint LocationInTexCoord2
        {
            get
            {
                return 5;
            }
        }

        public uint LocationInTexCoord3
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

        private bool m_UseColor;
        public bool UseColor
        {
            get
            {
                return m_UseColor;
            }

            set
            {
                m_UseColor = value;
                PixelProgram.SetUniform(PixelProgram.GetUniformLocation("UseColor"), m_UseColor ? 1.0f : 0.0f);
            }
        }

        public Color4 LightColor
        {
            get
            {
                return new Color4(m_LightBlock.LightColorR, m_LightBlock.LightColorG,
                    m_LightBlock.LightColorB, m_LightBlock.LightColorA);
            }
            set
            {
                m_LightBlock.LightColorR = value.R;
                m_LightBlock.LightColorG = value.G;
                m_LightBlock.LightColorB = value.B;
                m_LightBlock.LightColorA = value.A;
            }
        }

        public Vector3 LightPosition
        {
            get
            {
                return new Vector3(m_LightBlock.LightPositionX, 
                    m_LightBlock.LightPositionY, m_LightBlock.LightPositionZ);
            }
                
            set
            {
                m_LightBlock.LightPositionX = value.X;
                m_LightBlock.LightPositionY = value.Y;
                m_LightBlock.LightPositionZ = value.Z;
            }
        }

        public Color4 Ambient
        {
            get
            {
                return m_MaterialBlock.Ambient;
            }

            set
            {
                m_MaterialBlock.Ambient = value;
            }
        }

        public Color4 Diffuse
        {
            get
            {
                return m_MaterialBlock.Diffuse;
            }

            set
            {
                m_MaterialBlock.Diffuse = value;
            }
        }

        public Color4 Specular
        {
            get
            {
                return m_MaterialBlock.Specular;
            }

            set
            {
                m_MaterialBlock.Specular = value;
            }
        }

        public float Shininess
        {
            get
            {
                return m_MaterialBlock.Shininess;
            }

            set
            {
                m_MaterialBlock.Shininess = value;
            }
        }

        #endregion



        public void Update()
        {
            m_MatricesBlockUniformBuffer.UpdateBuffer<MatricesBlock>(m_MatricesBlock, MatricesBlock.SIZE);

            m_LightBlockUniformBuffer.UpdateBuffer<LightBlock1>(m_LightBlock, LightBlock1.SIZE);

            m_MaterialBlockUniformBuffer.UpdateBuffer<MaterialBlock>(m_MaterialBlock, MaterialBlock.SIZE);
        }

        public void ClearTextures()
        {
            Texture1 = null;
            Texture2 = null;
            Texture3 = null;
        }

        public void Use()
        {
            this.BindPipeline();
        }
    }
}
