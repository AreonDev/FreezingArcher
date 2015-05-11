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
    public class ShaderProgram : GraphicsResource
    {
        private List<Shader> _Shaders;


        public Shader[] Shaders { get { return _Shaders.ToArray(); } }

        internal ShaderProgram(string name, int id) : base(name, id, GraphicsResourceType.ShaderProgram)
        {
            _Shaders = new List<Shader>();
        }

        public void AddShader(Shader sh)
        {
            foreach (Shader r in _Shaders)
            {
                if ((r.ID == sh.ID) || (r.Name == sh.Name))
                    throw new Exception("Shader with name or ID is already attached!");
            }

            GL.AttachShader(ID, sh.ID);

            sh.SetUseCount(sh.InternalUseCount + 1);
            _Shaders.Add(sh);
        }

        public void Link()
        {
            GL.LinkProgram(ID);
        }

        public int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(ID, name);
        }

        public int GetUniformBlockIndex(string name)
        {
           int index = GL.GetUniformBlockIndex(ID, name);
           return index;
        }

        #region Set Uniform
        public void SetUniform(int location, float value)
        {
            GL.ProgramUniform1(ID, location, value);
        }

        public void SetUniform(int location, int value)
        {
            GL.ProgramUniform1(ID, location, value);
        }

        public void SetUniform(int location, Pencil.Gaming.MathUtils.Vector2 value)
        {
            GL.ProgramUniform2(ID, location, value.X, value.Y);
        }

        public void SetUniform(int location, Pencil.Gaming.MathUtils.Vector2i value)
        {
            GL.ProgramUniform2(ID, location, value.X, value.Y);
        }

        public void SetUniform(int location, Pencil.Gaming.MathUtils.Vector3 value)
        {
            GL.ProgramUniform3(ID, location, value.X, value.Y, value.Z);
        }

        public void SetUniform(int location, Pencil.Gaming.MathUtils.Vector3i value)
        {
            GL.ProgramUniform3(ID, location, value.X, value.Y, value.Z);
        }

        public void SetUniform(int location, Pencil.Gaming.MathUtils.Vector4 value)
        {
            GL.ProgramUniform4(ID, location, value.X, value.Y, value.Z, value.W);
        }

        public void SetUniform(int location, Pencil.Gaming.MathUtils.Vector4i value)
        {
            GL.ProgramUniform4(ID, location, value.X, value.Y, value.Z, value.W);
        }

        #endregion

        public void SetUniformBlockBinding(int blockindex, int blockbinding)
        {
            GL.UniformBlockBinding(ID, blockindex, blockbinding);
        }

        public void SetUniformBlockBinding(string name, int blockbinding)
        {
            GL.UniformBlockBinding(ID, GetUniformBlockIndex(name), blockbinding);
        }

        public void DeleteShader(Shader sh)
        {
            GL.DetachShader(ID, sh.ID);

            _Shaders.Remove(sh);
            sh.SetUseCount(sh.InternalUseCount - 1);
        }

        void UseProgram()
        {
            if (Effect.ActualBoundEffect != null)
                Effect.ActualBoundEffect.UnbindPipeline();

            GL.UseProgram(ID);
        }
    }
}
