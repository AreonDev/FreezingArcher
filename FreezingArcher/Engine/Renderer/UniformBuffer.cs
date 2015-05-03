using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;

namespace FreezingArcher.Renderer
{
    public class UniformBuffer : GraphicsResource
    {
        public int SizeInBytes { get; private set; }

        internal UniformBuffer(string name, int id, int sizeinbytes) : base(name, id, GraphicsResourceType.UniformBuffer)
        {
            SizeInBytes = sizeinbytes;
        }

        public void BindBuffer()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, ID);
        }

        public void UnbindBuffer()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void UnmapBuffer()
        {
            GL.UnmapBuffer(BufferTarget.UniformBuffer);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
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

            GL.BindBuffer(BufferTarget.UniformBuffer, ID);

            return GL.MapBufferRange(BufferTarget.UniformBuffer, (IntPtr)offset, (IntPtr)size, bam);
        }

        public void UpdateBuffer<T>(T[] data, int size) where T : struct
        {
            if (!Created)
                throw new Exception("Resource is not created!");

            if (size != SizeInBytes)
                throw new Exception("Size does not match buffer size!");

            GL.BindBuffer(BufferTarget.UniformBuffer, ID);

            GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)SizeInBytes, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferData<T>(BufferTarget.UniformBuffer, (IntPtr)SizeInBytes, data, BufferUsageHint.StreamDraw);

            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void UpdateBuffer<T>(T data, int size) where T : struct
        {
            if (!Created)
                throw new Exception("Resource is not created!");

            if (size != SizeInBytes)
                throw new Exception("Size does not match buffer size!");

            GL.BindBuffer(BufferTarget.UniformBuffer, ID);

            GL.BufferData(BufferTarget.UniformBuffer, (IntPtr)SizeInBytes, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BufferData<T>(BufferTarget.UniformBuffer, (IntPtr)SizeInBytes, ref data, BufferUsageHint.StreamDraw);

            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }

        public void SetBufferBase(int bindingpoint)
        {
            GL.BindBufferBase(BufferTarget.UniformBuffer, bindingpoint, ID);
        }
    }
}
