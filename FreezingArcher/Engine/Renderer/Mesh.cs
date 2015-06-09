using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FreezingArcher.Math;
using FreezingArcher.Output;

namespace FreezingArcher.Renderer
{
    public class Mesh
    {
        public enum PrimitiveType
        {
            Points = 1,
            Lines = 2,
            Triangles = 4
        }

        internal VertexBufferArray m_VertexBufferArray;

        internal IndexBuffer m_Indices;

        internal VertexBuffer m_VertexPosition;
        internal VertexBuffer m_VertexNormal;
        internal VertexBuffer m_VertexTangent;
        internal VertexBuffer m_VertexBiTangent;

        internal VertexBuffer[] m_VertexTexCoords;

        internal VertexBuffer[] m_VertexColors;

        internal int   m_MaterialIndex;
        internal PrimitiveType m_PrimitiveType;

        #region Properties
        public int MaterialIndex
        {
            get
            {
                return m_MaterialIndex;
            }

            internal set
            {
                m_MaterialIndex = value;
            }
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

        internal Mesh(RendererCore rc, string name, int matidx, int[] indices, Vector3[] positions, Vector3[] normals,
            Vector3[] tangents, Vector3[] bitangents, List<Vector3>[] texcoords, List<Color4>[] colors, PrimitiveType type)
        {
            if (rc == null || indices == null || positions == null || name == null)
            {
                Logger.Log.AddLogEntry(LogLevel.Fatal, "Renderer.Mesh", FreezingArcher.Core.Status.BadArgument);
                throw new Exception();
            }
                
            m_MaterialIndex = matidx;
            m_PrimitiveType = type;

            //Init buffers

            long ticks = DateTime.Now.Ticks;

            m_VertexTexCoords = null;
            m_VertexColors = null;

            m_Indices = rc.CreateIndexBuffer(indices, indices.Length * 4, RendererBufferUsage.StaticDraw, name + "_Indices_"+ticks);
            m_VertexPosition = rc.CreateVertexBuffer(positions, positions.Length * 3 * 4, RendererBufferUsage.StaticDraw, name + "_Positions_"+ticks);

            if (normals == null)
            {
                m_VertexNormal = new VertexBuffer();
  
                //TODO: Generate simple normals!
            }
            else
                m_VertexNormal = rc.CreateVertexBuffer(normals, normals.Length * 3 * 4, RendererBufferUsage.StreamDraw, name + "_Normals_"+ticks);

            if (tangents == null || tangents.Length == 0)
            {
                //TODO: generate tangents
                Vector3[] gen_tangents = new Vector3[positions.Length];

                for (int i = 0; i < gen_tangents.Length; i++)
                {
                    Vector3 c1 = Vector3.Cross(normals[i], Vector3.UnitZ);
                    Vector3 c2 = Vector3.Cross(normals[i], Vector3.UnitY);

                    if (c1.Length > c2.Length)
                        gen_tangents[i] = c1;
                    else
                        gen_tangents[i] = c2;

                    gen_tangents[i] = Vector3.Normalize(gen_tangents[i]);
                }

                tangents = gen_tangents;
            }

            m_VertexTangent = rc.CreateVertexBuffer(tangents, tangents.Length * 3 * 4, RendererBufferUsage.StaticDraw, name + "_Tangents_"+ticks);

            if (bitangents == null || bitangents.Length == 0)
            {
                //TODO: generate bitangents
                Vector3[] gen_bitangents = new Vector3[positions.Length];

                for (int i = 0; i < gen_bitangents.Length; i++)
                    gen_bitangents[i] = Vector3.Normalize(Vector3.Cross(normals[i], tangents[i]));

                bitangents = gen_bitangents;
            }

            m_VertexBiTangent = rc.CreateVertexBuffer(bitangents, bitangents.Length * 3 * 4, RendererBufferUsage.StaticDraw, name + "_BiNormals_"+ticks);

            if (texcoords != null && texcoords.Length > 0)
            {
                int count = 0;
                //check, which arrays are empty
                foreach (List<Vector3> texcs in texcoords)
                    if (texcs.Count > 0)
                        count++;

                m_VertexTexCoords = new VertexBuffer[count];

                for (int i = 0; i < count; i++)
                {
                    if (texcoords[i].Count > 0)
                    {
                        m_VertexTexCoords[i] = 
                        rc.CreateVertexBuffer(texcoords[i].ToArray(), texcoords[i].ToArray().Length * 3 * 4, RendererBufferUsage.StaticDraw, name + "_TexCoord" + i + "_"+ticks);
                    }
                }

            }

            if (colors != null && colors.Length > 0)
            {
                m_VertexColors = new VertexBuffer[colors.Length];

                for (int i = 0; i < colors.Length; i++)
                    m_VertexColors[i] = 
                        rc.CreateVertexBuffer(colors[i].ToArray(), colors[i].ToArray().Length * 4 * 4, RendererBufferUsage.StaticDraw, name + "_Colors" + i + "_"+ticks);
            }



            //Now, put everything into the VBA :'(
            VertexBufferLayoutKind[] vblk = new VertexBufferLayoutKind[1];
            vblk[0].AttributeID = 0;
            vblk[0].AttributeSize = 3;
            vblk[0].AttributeType = RendererVertexAttribType.Float;
            vblk[0].Normalized = false;
            vblk[0].Offset = 0;
            vblk[0].Stride = 3 * 4;

            m_VertexBufferArray = rc.CreateVertexBufferArray(m_VertexPosition, vblk, name + "_VertexBufferArray_"+ticks, m_Indices);

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
                    if (foo_bla_counter < 3)
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
            }

            /*
            //Colors
            foo_bla_counter = 0;

            if (m_VertexColors != null)
            {
                foreach (VertexBuffer vb in m_VertexColors)
                {
                    if (foo_bla_counter < 3)
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
            }*/

            m_VertexBufferArray.EndPrepare();
        }
    }
}
