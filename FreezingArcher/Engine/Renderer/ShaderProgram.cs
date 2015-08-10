using System;
using System.Collections.Generic;
using FreezingArcher.Math;
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

        public void SetUniform(int location, Vector2 value)
        {
            GL.ProgramUniform2(ID, location, value.X, value.Y);
        }

        public void SetUniform(int location, Vector2i value)
        {
            GL.ProgramUniform2(ID, location, value.X, value.Y);
        }

        public void SetUniform(int location, Vector3 value)
        {
            GL.ProgramUniform3(ID, location, value.X, value.Y, value.Z);
        }

        public void SetUniform(int location, Vector3i value)
        {
            GL.ProgramUniform3(ID, location, value.X, value.Y, value.Z);
        }

        public void SetUniform(int location, Vector4 value)
        {
            GL.ProgramUniform4(ID, location, value.X, value.Y, value.Z, value.W);
        }

        public void SetUniform(int location, FreezingArcher.Math.Color4 value)
        {
            GL.ProgramUniform4(ID, location, value.R, value.G, value.B, value.A);
        }

        public void SetUniform(int location, Vector4i value)
        {
            GL.ProgramUniform4(ID, location, value.X, value.Y, value.Z, value.W);
        }

        public void SetUniform(int location, FreezingArcher.Math.Matrix value)
        {
            float[] data = { value.M11, value.M12, value.M13, value.M14, 
                value.M21, value.M22, value.M23, value.M24, 
                value.M31, value.M32, value.M33, value.M34,
                value.M41, value.M42, value.M43, value.M44};

            GL.ProgramUniformMatrix4(ID, location, 1, false, data); 
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
