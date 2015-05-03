using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pencil.Gaming;
using Pencil.Gaming.Graphics;

using FreezingArcher.Output;

namespace FreezingArcher.Renderer
{
    public class FrameBuffer : GraphicsResource
    {
        private bool m_IsBound;
        private FrameBufferTarget m_LastBoundTarget;
        private List<TextureAttachment> m_Textures;

        private class TextureAttachment
        {
            public Texture tex;
            public AttachmentUsage au;

            public TextureAttachment(Texture t, AttachmentUsage a)
            {
                tex = t;
                au = a;
            }
        }

        public enum FrameBufferTarget : int
        {
            Read = 
                FramebufferTarget.ReadFramebuffer,
            Draw = FramebufferTarget.DrawFramebuffer,
            Both = FramebufferTarget.Framebuffer,
        }

        public enum AttachmentUsage : int
        {
            Nothing = 0,
            Color0 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment0,
            Color1 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment1,
            Color2 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment2,
            Color3 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment3,
            Color4 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment4,
            Color5 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment5,
            Color6 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment6,
            Color7 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment7,
            Color8 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment8,
            Color9 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment9,
            Color10 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment10,
            Color11 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment11,
            Color12 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment12,
            Color13 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment13,
            Color14 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment14,
            Color15 = Pencil.Gaming.Graphics.FramebufferAttachment.ColorAttachment15,

            DepthStencil = Pencil.Gaming.Graphics.FramebufferAttachment.DepthStencilAttachment
        }

        internal FrameBuffer(string name, int id) : base(name, id, GraphicsResourceType.FrameBuffer)
        {
            m_IsBound = false;
            m_LastBoundTarget = FrameBufferTarget.Both;

            m_Textures = new List<TextureAttachment>();
        }

        public void Bind(FrameBufferTarget fbt)
        {
            Unbind();

            m_IsBound = true;
            m_LastBoundTarget = fbt;

            foreach (TextureAttachment ta in m_Textures)
                ta.tex.Unbind();

            GL.BindFramebuffer((FramebufferTarget)fbt, ID);
        }

        public void Unbind()
        {
            if(m_IsBound)
            {
                GL.BindFramebuffer((FramebufferTarget)m_LastBoundTarget, 0);
                m_IsBound = false;
                m_LastBoundTarget = FrameBufferTarget.Both;
            }
        }

        public void AddTexture(Texture2D tex, AttachmentUsage au)
        {
            if(!m_PrepareMode)
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

            DeleteTexture(au, true);

            if((tex != null) && (tex.Created))
            {
                tex.SetUseCount(tex.InternalUseCount + 1);
                m_Textures.Add(new TextureAttachment(tex, au));

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, (FramebufferAttachment)au, TextureTarget.Texture2D, tex.ID, 0);
            }

            if(!m_PrepareMode)
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void DeleteTexture(AttachmentUsage au, bool bound = false)
        {
            if(!(bound || m_PrepareMode))
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

            TextureAttachment tata = null;

            foreach(TextureAttachment ta in m_Textures)
            {
                if(ta.au == au)
                {
                    tata = ta;

                    ta.tex.SetUseCount(ta.tex.InternalUseCount - 1);
                    GL.FramebufferTexture(FramebufferTarget.Framebuffer, (FramebufferAttachment)au, 0, 0);
                }
            }

            if (tata != null)
                m_Textures.Remove(tata);

            if (!(bound || m_PrepareMode))
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void UseAttachments(AttachmentUsage[] attachments)
        {
            if (!m_PrepareMode)
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);

            DrawBuffersEnum[] dbe = new DrawBuffersEnum[attachments.Length];
            for (int i = 0; i < dbe.Length; i++)
            {
                if (attachments[i] == AttachmentUsage.DepthStencil)
                    throw new ArgumentException();

                dbe[i] = (DrawBuffersEnum)attachments[i];
            }

            GL.DrawBuffers(attachments.Length, dbe);

            if(!m_PrepareMode)
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public override void BeginPrepare()
        {
            base.BeginPrepare();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ID);
        }

        public override void EndPrepare()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);

            base.EndPrepare();
        }
    }
}
