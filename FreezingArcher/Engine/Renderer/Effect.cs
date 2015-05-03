using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace FreezingArcher.Renderer
{
    public class Effect : GraphicsResource
    {
        private ShaderProgram m_VertexProgram;
        private ShaderProgram m_PixelProgram;
        private ShaderProgram m_TesselationControlProgram;
        private ShaderProgram m_TesselationEvaluationProgram;
        private ShaderProgram m_GeometryProgram;

        public bool HasVertexProgram { get; set; }
        public bool HasPixelProgram { get; set; }
        public bool HasTesselationControlProgram { get; set; }
        public bool HasTesselationEvaluationProgram { get; set; }
        public bool HasGeometryProgram { get; set; }

        public ShaderProgram VertexProgram
        {
            get
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                return m_VertexProgram;
            }
            set
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                if (m_VertexProgram != null)
                    m_VertexProgram.SetUseCount(m_VertexProgram.InternalUseCount - 1);

                m_VertexProgram = null;

                if (value == null)
                {
                    HasVertexProgram = false;
                    GL.UseProgramStages(ID, ProgramStageMask.VertexShaderBit, 0);
                }
                else
                {
                    GL.UseProgramStages(ID, ProgramStageMask.VertexShaderBit, value.ID);

                    m_VertexProgram = value;
                    m_VertexProgram.SetUseCount(m_VertexProgram.InternalUseCount + 1);

                    HasVertexProgram = true;
                }
            }
        }
        public ShaderProgram PixelProgram 
        {
            get
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                return m_PixelProgram;
            }
            set
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                if (m_PixelProgram != null)
                    m_PixelProgram.SetUseCount(m_PixelProgram.InternalUseCount - 1);

                m_PixelProgram = null;

                if (value == null)
                {
                    HasPixelProgram = false;
                    GL.UseProgramStages(ID, ProgramStageMask.FragmentShaderBit, 0);

                }
                else
                {
                    GL.UseProgramStages(ID, ProgramStageMask.FragmentShaderBit, value.ID);

                    m_PixelProgram = value;
                    m_PixelProgram.SetUseCount(m_PixelProgram.InternalUseCount + 1);

                    HasPixelProgram = true;
                }
            }
        }
        public ShaderProgram TesselationControlProgram
        {
            get
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                return m_TesselationControlProgram;
            }
            set
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                if (m_TesselationControlProgram != null)
                    m_TesselationControlProgram.SetUseCount(m_TesselationControlProgram.InternalUseCount - 1);

                m_TesselationControlProgram = null;

                if (value == null)
                {
                    HasTesselationControlProgram = false;
                    GL.UseProgramStages(ID, ProgramStageMask.TessControlShaderBit, 0);
                }
                else
                {
                    GL.UseProgramStages(ID, ProgramStageMask.TessControlShaderBit, value.ID);

                    m_TesselationControlProgram = value;
                    m_TesselationControlProgram.SetUseCount(m_TesselationControlProgram.InternalUseCount + 1);

                    HasTesselationControlProgram = true;
                }
            }
        }
        public ShaderProgram TesselationEvaluationProgram 
        {
            get
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                return m_TesselationEvaluationProgram;
            }
            set
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                if (m_TesselationEvaluationProgram != null)
                    m_TesselationEvaluationProgram.SetUseCount(m_TesselationEvaluationProgram.InternalUseCount - 1);

                m_TesselationEvaluationProgram = null;

                if (value == null)
                {
                    HasTesselationEvaluationProgram = false;
                    GL.UseProgramStages(ID, ProgramStageMask.TessEvaluationShaderBit, 0);
                }
                else
                {
                    GL.UseProgramStages(ID, ProgramStageMask.TessEvaluationShaderBit, value.ID);

                    m_TesselationEvaluationProgram = value;
                    m_TesselationEvaluationProgram.SetUseCount(m_TesselationEvaluationProgram.InternalUseCount + 1);

                    HasTesselationEvaluationProgram = true;
                }
            }
        }
        public ShaderProgram GeometryProgram 
        {
            get
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                return m_GeometryProgram;
            }
            set
            {
                if (!Created)
                    throw new Exception("Resource is not created!");

                if (m_GeometryProgram != null)
                    m_GeometryProgram.SetUseCount(m_GeometryProgram.InternalUseCount - 1);

                m_GeometryProgram = null;

                if (value == null)
                {
                    HasGeometryProgram = false;
                    GL.UseProgramStages(ID, ProgramStageMask.GeometryShaderBit, 0);
                }
                else
                {
                    GL.UseProgramStages(ID, ProgramStageMask.GeometryShaderBit, value.ID);

                    m_GeometryProgram = value;
                    m_GeometryProgram.SetUseCount(m_GeometryProgram.InternalUseCount + 1);

                    HasGeometryProgram = true;
                }
            }
        }

        internal Effect(string name, int ID) : base(name, ID, GraphicsResourceType.ProgramPipeline)
        {
            m_VertexProgram = null;
            m_PixelProgram = null;
            m_TesselationControlProgram = null;
            m_TesselationEvaluationProgram = null;
            m_GeometryProgram = null;

            HasVertexProgram = false;
            HasPixelProgram = false;
            HasTesselationControlProgram = false;
            HasTesselationEvaluationProgram = false;
            HasGeometryProgram = false;
        }

        public void BindPipeline()
        {
            GL.BindProgramPipeline(ID);
        }

        public void SetActiveProgram(ShaderType st)
        {
            switch(st)
            {
                case ShaderType.VertexShader:
                    GL.ActiveShaderProgram(ID, m_VertexProgram.ID);
                    break;

                case ShaderType.PixelShader:
                    GL.ActiveShaderProgram(ID, m_PixelProgram.ID);
                    break;

                case ShaderType.TesselationControlShader:
                    GL.ActiveShaderProgram(ID, m_TesselationControlProgram.ID);
                    break;

                case ShaderType.TesselationEvaluationShader:
                    GL.ActiveShaderProgram(ID, m_TesselationEvaluationProgram.ID);
                    break;

                case ShaderType.GeometryShader:
                    GL.ActiveShaderProgram(ID, m_GeometryProgram.ID);
                    break;
            }
        }
    }
}
