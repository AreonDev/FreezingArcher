using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FreezingArcher.Math;

using FreezingArcher.Renderer;
using System.Runtime.InteropServices;

namespace FreezingArcher.Renderer.HelperClasses
{
    public class SimpleCube
    {
        VertexBufferLayoutKind[] m_VertexBufferLayoutKind;
        CubeVertex[] m_Vertices;

        VertexBuffer m_VertexBuffer;
        VertexBufferArray m_VertexBufferArray;

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct CubeVertex
        {
            public static int SIZE = sizeof(Vector3) + sizeof(Vector3) + sizeof(Vector3) + sizeof(Vector4);

            public CubeVertex(Vector3 position, Vector3 normal, Vector3 tex_coord, Color4 color)
            {
                _PositionX = position.X;
                _PositionY = position.Y;
                _PositionZ = position.Z;

                _NormalX = normal.X;
                _NormalY = normal.Y;
                _NormalZ = normal.Z;

                _TexCoordU = tex_coord.X;
                _TexCoordV = tex_coord.Y;
                _TexCoordIntensity = tex_coord.Z;

                _ColorR = color.R;
                _ColorG = color.G;
                _ColorB = color.B;
                _ColorA = color.A;
            }

            private float _PositionX;
            private float _PositionY;
            private float _PositionZ;

            private float _NormalX;
            private float _NormalY;
            private float _NormalZ;

            private float _TexCoordU;
            private float _TexCoordV;
            private float _TexCoordIntensity;

            private float _ColorR;
            private float _ColorG;
            private float _ColorB;
            private float _ColorA;

            public Vector3 Position
            {
                get
                {
                    return new Vector3(_PositionX, _PositionY, _PositionZ);
                }

                set
                {
                    _PositionX = value.X;
                    _PositionY = value.Y;
                    _PositionZ = value.Z;
                }
            }
            public Vector3 Normal
            {
                get
                {
                    return new Vector3(_NormalX, _NormalY, _NormalZ);
                }

                set
                {
                    _NormalX = value.X;
                    _NormalY = value.Y;
                    _NormalZ = value.Z;
                }
            }
            public Vector2 TexCoord
            {
                get
                {
                    return new Vector2(_TexCoordU, _TexCoordV);
                }

                set
                {
                    _TexCoordU = value.X;
                    _TexCoordV = value.Y;
                }
            }
            public float TexCoordIntensity
            {
                get
                {
                    return _TexCoordIntensity;
                }

                set
                {
                    _TexCoordIntensity = value;
                }
            }

            public Color4 Color
            {
                get
                {
                    return new Color4(_ColorR, _ColorG, _ColorB, _ColorA);
                }

                set
                {
                    _ColorR = value.R;
                    _ColorG = value.G;
                    _ColorB = value.B;
                    _ColorA = value.A;
                }
            }
        }

        public SimpleCube(Color4 color)
        {
            m_VertexBufferLayoutKind = new VertexBufferLayoutKind[4];
            
            m_Vertices = new CubeVertex[36];

            //Fläche, die nach vorne!
            m_Vertices[0] = new CubeVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), color);
            m_Vertices[1] = new CubeVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[2] = new CubeVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 1.0f), color);

            m_Vertices[3] = new CubeVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 1.0f), color);
            m_Vertices[4] = new CubeVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[5] = new CubeVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), color);

            //Fläche, die nach rechts zeigt!
            m_Vertices[6] = new CubeVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), color);
            m_Vertices[7] = new CubeVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[8] = new CubeVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f), color);

            m_Vertices[9] = new CubeVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f), color);
            m_Vertices[10] = new CubeVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[11] = new CubeVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), color);

            //Fläche, die nach hinten zeigt!
            m_Vertices[12] = new CubeVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 0.0f, 1.0f), color);
            m_Vertices[13] = new CubeVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[14] = new CubeVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 1.0f, 1.0f), color);

            m_Vertices[15] = new CubeVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 1.0f, 1.0f), color);
            m_Vertices[16] = new CubeVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[17] = new CubeVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 1.0f, 1.0f), color);

            //Fläche, die nach links zeigt!
            m_Vertices[18] = new CubeVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), color);
            m_Vertices[19] = new CubeVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[20] = new CubeVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f), color);

            m_Vertices[21] = new CubeVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f), color);
            m_Vertices[22] = new CubeVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[23] = new CubeVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), color);

            //Fläche, die nach unten zeigt
            m_Vertices[24] = new CubeVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), color);
            m_Vertices[25] = new CubeVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[26] = new CubeVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f), color);

            m_Vertices[27] = new CubeVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f), color);
            m_Vertices[28] = new CubeVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[29] = new CubeVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), color);

            //Fläche die nach oben zeigt!
            m_Vertices[30] = new CubeVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), color);
            m_Vertices[31] = new CubeVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[32] = new CubeVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f), color);

            m_Vertices[33] = new CubeVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f), color);
            m_Vertices[34] = new CubeVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f), color);
            m_Vertices[35] = new CubeVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f), color);

        }

        public unsafe bool Init(RendererCore rc)
        {
            m_VertexBufferLayoutKind[0].AttributeID = rc.BasicEffect.LocationInPosition;
            m_VertexBufferLayoutKind[0].AttributeSize = 3;
            m_VertexBufferLayoutKind[0].AttributeType = RendererVertexAttribType.Float;
            m_VertexBufferLayoutKind[0].Normalized = false;
            m_VertexBufferLayoutKind[0].Offset = 0;
            m_VertexBufferLayoutKind[0].Stride = CubeVertex.SIZE;

            m_VertexBufferLayoutKind[1].AttributeID = rc.BasicEffect.LocationInNormal;
            m_VertexBufferLayoutKind[1].AttributeSize = 3;
            m_VertexBufferLayoutKind[1].AttributeType = RendererVertexAttribType.Float;
            m_VertexBufferLayoutKind[1].Normalized = false;
            m_VertexBufferLayoutKind[1].Offset = sizeof(Vector3);
            m_VertexBufferLayoutKind[1].Stride = CubeVertex.SIZE;

            m_VertexBufferLayoutKind[2].AttributeID = rc.BasicEffect.LocationInTexCoord1;
            m_VertexBufferLayoutKind[2].AttributeSize = 3;
            m_VertexBufferLayoutKind[2].AttributeType = RendererVertexAttribType.Float;
            m_VertexBufferLayoutKind[2].Normalized = false;
            m_VertexBufferLayoutKind[2].Offset = sizeof(Vector3) + sizeof(Vector3);
            m_VertexBufferLayoutKind[2].Stride = CubeVertex.SIZE;

            m_VertexBufferLayoutKind[3].AttributeID = rc.BasicEffect.LocationInColor;
            m_VertexBufferLayoutKind[3].AttributeSize = 4;
            m_VertexBufferLayoutKind[3].AttributeType = RendererVertexAttribType.Float;
            m_VertexBufferLayoutKind[3].Normalized = false;
            m_VertexBufferLayoutKind[3].Offset = sizeof(Vector3) + sizeof(Vector3) + sizeof(Vector3);
            m_VertexBufferLayoutKind[3].Stride = CubeVertex.SIZE;

            Int64 next = System.DateTime.Now.Ticks;

            m_VertexBuffer = rc.CreateVertexBuffer<CubeVertex>(m_Vertices, m_Vertices.Length * CubeVertex.SIZE, RendererBufferUsage.StaticDraw, "CubeVertexVertexBuffer_" + next);
            m_VertexBufferArray = rc.CreateVertexBufferArray(m_VertexBuffer, m_VertexBufferLayoutKind, "CubeVertexVertexBufferArray_" + next);

            return true;
        }
        public void Destroy(RendererCore rc)
        {
            rc.DeleteGraphicsResource(m_VertexBufferArray);
            rc.DeleteGraphicsResource(m_VertexBuffer);
        }

        public void Draw(RendererCore rc)
        {
            Pencil.Gaming.Graphics.GL.Enable(Pencil.Gaming.Graphics.EnableCap.DepthTest);

            m_VertexBufferArray.BindVertexBufferArray();

            rc.DrawArrays(0, m_Vertices.Length, RendererBeginMode.Triangles);

            m_VertexBufferArray.UnbindVertexBufferArray();
        }
    }
}
