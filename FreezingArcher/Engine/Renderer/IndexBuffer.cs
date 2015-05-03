using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace FreezingArcher.Renderer
{
    public class IndexBuffer : GraphicsResource
    {
        public int SizeInBytes { get; private set; }
        public RendererBufferUsage BufferUsage { get; private set; }

        internal IndexBuffer(string name, int id, int sizeinbytes, RendererBufferUsage rbu) : base(name, id, GraphicsResourceType.IndexBuffer)
        {
            SizeInBytes = sizeinbytes;
            BufferUsage = rbu;
        }


        public void BindBuffer()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
        }

        public void UnbindBuffer()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public void UnmapBuffer()
        {
            GL.UnmapBuffer(BufferTarget.ElementArrayBuffer);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        public IntPtr MapBuffer(int offset, int size, RendererBufferAccess rba)
        {
            if (!Created)
                throw new Exception("Resource is not created!");

            BufferAccessMask bam = BufferAccessMask.MapUnsynchronizedBit;

            if (rba == RendererBufferAccess.ReadWrite)
            {
                bam |= BufferAccessMask.MapReadBit | BufferAccessMask.MapWriteBit;
                bam &= ~BufferAccessMask.MapUnsynchronizedBit;
            }
            else if (rba == RendererBufferAccess.WriteOnly)
                bam |= BufferAccessMask.MapWriteBit;
            else if (rba == RendererBufferAccess.ReadOnly)
            {
                bam |= BufferAccessMask.MapReadBit;
                bam &= ~BufferAccessMask.MapUnsynchronizedBit;
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

            return GL.MapBufferRange(BufferTarget.ElementArrayBuffer, (IntPtr)offset, (IntPtr)size, bam);
        }

        public void UpdateBuffer<T>(T[] data, int size) where T : struct
        {
            if (!Created)
                throw new Exception("Resource is not created!");

            if (size != SizeInBytes)
                throw new Exception("Size does not match buffer size!");

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)SizeInBytes, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferData<T>(BufferTarget.ElementArrayBuffer, (IntPtr)SizeInBytes, data, BufferUsageHint.StreamDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}
