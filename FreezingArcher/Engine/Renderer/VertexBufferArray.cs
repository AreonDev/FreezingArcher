using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace FreezingArcher.Renderer
{
    public enum RendererVertexAttribType
    {
        Byte = 5120,
        UnsignedByte = 5121,
        Short = 5122,
        UnsignedShort = 5123,
        Int = 5124,
        UnsignedInt = 5125,
        Float = 5126,
        Double = 5130,
        HalfFloat = 5131,
        Fixed = 5132,
        UnsignedInt2101010Rev = 33640,
        Int2101010Rev = 36255,
    }

    public struct VertexBufferLayoutKind
    {
        public uint AttributeID;
        public int AttributeSize;
        public RendererVertexAttribType AttributeType;
        public bool Normalized;
        public int Stride;
        public int Offset;
    }

    public class VertexBufferArray : GraphicsResource
    {
        private List<VertexBufferLayoutKind[]> _LayoutElements;
        private List<VertexBuffer> _InternalBuffers;
        private IndexBuffer _InternalOptionalIndexBuffer;

        internal VertexBufferArray(string name, int id, VertexBufferLayoutKind[] vblk, VertexBuffer vb, IndexBuffer ib = null) : base(name, id, GraphicsResourceType.VertexBufferArray)
        {
            _LayoutElements = new List<VertexBufferLayoutKind[]>();
            _InternalBuffers = new List<VertexBuffer>();

            VertexBufferLayoutKind[] layout = new VertexBufferLayoutKind[vblk.Length];
            Array.Copy(vblk, layout, layout.Length);

            _LayoutElements.Add(layout);

            _InternalBuffers.Add(vb);
            vb.SetUseCount(vb.InternalUseCount + 1);

            _InternalOptionalIndexBuffer = ib;
            if(ib != null)
                _InternalOptionalIndexBuffer.SetUseCount(_InternalOptionalIndexBuffer.InternalUseCount + 1);
        }

        public List<VertexBufferLayoutKind[]> GetLayoutElements()
        {
            List<VertexBufferLayoutKind[]> list = new List<VertexBufferLayoutKind[]>();

            foreach(VertexBufferLayoutKind[] lk in _LayoutElements)
            {
                VertexBufferLayoutKind[] temp = new VertexBufferLayoutKind[lk.Length];
                Array.Copy(lk, temp, temp.Length);

                list.Add(temp);
            }

            return list;
        }

        public void AddVertexBuffer(VertexBufferLayoutKind[] vblk, VertexBuffer vb)
        {
            if (!m_PrepareMode)
            {
                GL.BindVertexArray(ID);
            }

            if (vb.Created)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vb.ID);

                for (int i = 0; i < vblk.Length; i++)
                {
                    GL.EnableVertexAttribArray(vblk[i].AttributeID);
                    GL.VertexAttribPointer(vblk[i].AttributeID, vblk[i].AttributeSize, (VertexAttribPointerType)vblk[i].AttributeType,
                        vblk[i].Normalized, vblk[i].Stride, (IntPtr)vblk[i].Offset);
                }

                VertexBufferLayoutKind[] layout = new VertexBufferLayoutKind[vblk.Length];
                Array.Copy(vblk, layout, layout.Length);

                _LayoutElements.Add(layout);

                _InternalBuffers.Add(vb);
                vb.SetUseCount(vb.InternalUseCount + 1);
            }

            if(!m_PrepareMode)
            {
                GL.BindVertexArray(0);
            }
        }

        internal List<VertexBufferLayoutKind[]> GetLayoutElementsReferenced()
        {
            return _LayoutElements;
        }

        public override void BeginPrepare() { base.BeginPrepare(); BindVertexBufferArray(); }
        public override void EndPrepare() { base.EndPrepare(); UnbindVertexBufferArray(); }

        internal void Shutdown()
        {
            foreach (VertexBuffer vb in _InternalBuffers)
            {
                vb.SetUseCount(vb.InternalUseCount - 1);
            }

            if (_InternalOptionalIndexBuffer != null)
                _InternalOptionalIndexBuffer.SetUseCount(_InternalOptionalIndexBuffer.InternalUseCount - 1);
        }

        public List<VertexBuffer> GetBuffers()
        {
            return _InternalBuffers;
        }

        public void BindVertexBufferArray()
        {
            GL.BindVertexArray(ID);
        }

        public void UnbindVertexBufferArray()
        {
            GL.BindVertexArray(0);
        }
    }
}
