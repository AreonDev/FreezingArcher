using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FreezingArcher.Math;
using FreezingArcher.Output;

namespace FreezingArcher.Renderer
{
    class Mesh
    {
        private VertexBufferArray m_VertexBufferArray;

        private IndexBuffer m_Indices;

        private VertexBuffer m_VertexPosition;
        private VertexBuffer m_VertexNormal;
        private VertexBuffer m_VertexTangent;
        private VertexBuffer m_VertexBiTangent;

        private VertexBuffer[] m_VertexTexCoords;

        private VertexBuffer[] m_VertexColors;

        private int   m_MaterialIndex;

        #region Properties
        public int MaterialIndex
        {
            get
            {
                return m_MaterialIndex;
            }

            //set
            //{
                //Vllt doch nicht?
            //}
        }

        public int VertexCount
        {
            get
            {
                return m_VertexPosition.VertexCount;
            }
        }

        public bool HasVertices
        {
            get
            {
                return m_VertexPosition.VertexCount > 0;
            }
        }

        public bool HasNormals
        {
            get
            {
                return m_VertexNormal.VertexCount > 0;
            }
        }

        public bool HasTangets
        {
            get
            {
                return m_VertexTangent.VertexCount > 0;
            }
        }

        public bool HasBiTangents
        {
            get
            {
                return m_VertexBiTangent.VertexCount > 0;
            }
        }

        public bool HasTangentBasis
        {
            get
            {
                return m_VertexBiTangent.VertexCount > 0 && m_VertexTangent.VertexCount > 0;
            }
        }

        public bool HasTexCoords
        {
            get
            {
                return m_VertexTexCoords != null && m_VertexTexCoords.Length > 0 && m_VertexTexCoords[0].VertexCount > 0;
            }
        }

        public bool HasColors
        {
            get
            {
                return m_VertexColors != null && m_VertexColors.Length > 0 && m_VertexColors[0].VertexCount > 0;
            }
        }

        #endregion

        public Mesh(RendererCore rc, string name, int matidx, int[] indices, Vector3[] positions, Vector3[] normals,
            Vector3[] tangents, Vector3[] bitangents, List<Vector3>[] texcoords, List<Color4>[] colors)
        {
            if (rc == null || indices == null || positions == null || name == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Fatal, "Renderer.Mesh", FreezingArcher.Core.Status.BadArgument);
                throw new Exception();
            }

            m_MaterialIndex = matidx;

            //Init buffers

            m_VertexTexCoords = null;
            m_VertexColors = null;

            m_Indices = rc.CreateIndexBuffer(indices, indices.Length * 4, RendererBufferUsage.StaticDraw, name + "_Indices");
            m_VertexPosition = rc.CreateVertexBuffer(positions, positions.Length * 3 * 4, RendererBufferUsage.StaticDraw, name + "_Positions");

            if (normals == null)
                m_VertexNormal = new VertexBuffer();
            else
                m_VertexNormal = rc.CreateVertexBuffer(normals, normals.Length * 3 * 4, RendererBufferUsage.StreamDraw, name + "_Normals");

            if (tangents == null)
                m_VertexTangent = new VertexBuffer();
            else
                m_VertexTangent = rc.CreateVertexBuffer(tangents, tangents.Length * 3 * 4, RendererBufferUsage.StaticDraw, name + "_Tangents");

            if (bitangents == null)
                m_VertexBiTangent = new VertexBuffer();
            else
                m_VertexBiTangent = rc.CreateVertexBuffer(bitangents, bitangents.Length * 3 * 4, RendererBufferUsage.StaticDraw, name + "_BiNormals");

            if (texcoords != null && texcoords.Length > 0)
            {
                m_VertexTexCoords = new VertexBuffer[texcoords.Length];

                for (int i = 0; i < texcoords.Length; i++)
                    m_VertexTexCoords[i] = 
                        rc.CreateVertexBuffer(texcoords[i].ToArray(), texcoords[i].ToArray().Length * 2 * 4, RendererBufferUsage.StaticDraw, name + "_TexCoord" + i);

            }

            if (colors != null && colors.Length > 0)
            {
                m_VertexColors = new VertexBuffer[colors.Length];

                for (int i = 0; i < colors.Length; i++)
                    m_VertexColors[i] = 
                        rc.CreateVertexBuffer(colors[i].ToArray(), colors[i].ToArray().Length * 4 * 4, RendererBufferUsage.StaticDraw, name + "_Colors" + i);
            }



            //Now, put everything into the VBA :'(
            VertexBufferLayoutKind[] vblk = new VertexBufferLayoutKind[1];
            vblk[0].AttributeID = 0;
            vblk[0].AttributeSize = 3;
            vblk[0].AttributeType = RendererVertexAttribType.Float;
            vblk[0].Normalized = false;
            vblk[0].Offset = 0;
            vblk[0].Stride = 3 * 4;

            m_VertexBufferArray = rc.CreateVertexBufferArray(m_VertexPosition, vblk, name + "_VertexBufferArray", m_Indices);

            m_VertexBufferArray.BeginPrepare();

            //Normals
            vblk[0].AttributeID = 1;
            vblk[0].AttributeSize = 3;
            vblk[0].AttributeType = RendererVertexAttribType.Float;
            vblk[0].Normalized = false;
            vblk[0].Offset = 0;
            vblk[0].Stride = 3 * 4;

            m_VertexBufferArray.AddVertexBuffer(vblk, m_VertexNormal);

            //Tangents
            vblk[0].AttributeID = 2;
            vblk[0].AttributeSize = 3;
            vblk[0].AttributeType = RendererVertexAttribType.Float;
            vblk[0].Normalized = false;
            vblk[0].Offset = 0;
            vblk[0].Stride = 3 * 4;

            m_VertexBufferArray.AddVertexBuffer(vblk, m_VertexTangent);

            //BiTangents
            vblk[0].AttributeID = 3;
            vblk[0].AttributeSize = 3;
            vblk[0].AttributeType = RendererVertexAttribType.Float;
            vblk[0].Normalized = false;
            vblk[0].Offset = 0;
            vblk[0].Stride = 3 * 4;

            m_VertexBufferArray.AddVertexBuffer(vblk, m_VertexTangent);


            //Textures
            uint foo_bla_counter = 0;

            if (m_VertexTexCoords != null)
            {
                foreach (VertexBuffer vb in m_VertexTexCoords)
                {
                    vblk[0].AttributeID = (uint)4 + foo_bla_counter;
                    vblk[0].AttributeSize = 3;
                    vblk[0].AttributeType = RendererVertexAttribType.Float;
                    vblk[0].Normalized = false;
                    vblk[0].Offset = 0;
                    vblk[0].Stride = 3 * 4;

                    m_VertexBufferArray.AddVertexBuffer(vblk, vb);

                    foo_bla_counter++;
                }
            }

            //Colors
            foo_bla_counter = 0;

            if (m_VertexColors != null)
            {
                foreach (VertexBuffer vb in m_VertexColors)
                {
                    vblk[0].AttributeID = (uint)7 + foo_bla_counter;
                    vblk[0].AttributeSize = 4;
                    vblk[0].AttributeType = RendererVertexAttribType.Float;
                    vblk[0].Normalized = false;
                    vblk[0].Offset = 0;
                    vblk[0].Stride = 4 * 4;

                    m_VertexBufferArray.AddVertexBuffer(vblk, vb);

                    foo_bla_counter++;
                }
            }

            m_VertexBufferArray.EndPrepare();
        }

        public void Draw(RendererCore rc)
        {
            //Materials and Matrices should be correctly set

            m_VertexBufferArray.BindVertexBufferArray();

            rc.DrawElements(0, m_Indices.IndexCount, RendererIndexType.UnsignedInt, RendererBeginMode.Triangles);

            m_VertexBufferArray.UnbindVertexBufferArray();
        }
    }
}
