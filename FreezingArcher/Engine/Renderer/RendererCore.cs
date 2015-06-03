using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using FreezingArcher.Output;
using FreezingArcher.Math;

using System.Runtime.InteropServices;

namespace FreezingArcher.Renderer
{
    public enum RendererBeginMode
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6,
        Quads = 7,
        QuadStrip = 8,
        Polygon = 9,
        LinesAdjacency = 10,
        LineStripAdjacency = 11,
        TrianglesAdjacency = 12,
        TriangleStripAdjacency = 13,
        Patches = 14,
    }

    public enum RendererIndexType
    {
        UnsignedByte = ((int)0x1401)            ,
        UnsignedShort = ((int)0x1403)           ,
        UnsignedInt = ((int)0x1405)             
    }

    public enum RendererBufferAccess
    {
        ReadOnly = 35000,
        WriteOnly = 35001,
        ReadWrite = 35002,
    }

    public enum RendererBufferUsage
    {
        StreamDraw = 35040,
        StreamRead = 35041,
        StreamCopy = 35042,
        StaticDraw = 35044,
        StaticRead = 35045,
        StaticCopy = 35046,
        DynamicDraw = 35048,
        DynamicRead = 35049,
        DynamicCopy = 35050,
    }

    public enum RendererCullMode
    {
        Front = ((int)0x0404)           ,
        Back = ((int)0x0405)            ,
        FrontAndBack = ((int)0x0408)            ,
    }

    internal class SomeResources
    {
        //public static Assimp.AssimpContext AssimpCnt;

        static SomeResources()
        {
            //AssimpCnt = new Assimp.AssimpContext();
        }
    }

    public class RendererCore : Messaging.Interfaces.IMessageConsumer
    {
        private struct MatricesBlock2D
        {
            public Matrix World;
            public Matrix Projection;
        }

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct Vertex2D
        {
            public static int SIZE = sizeof(Vector3) + sizeof(Vector4) + sizeof(Vector2);

            public Vertex2D(Vector3 position, Vector4 color, Vector2 texCoord)
            {
                PositionX = position.X;
                PositionY = position.Y;
                PositionZ = position.Z;
                ColorR = color.X;
                ColorG = color.Y;
                ColorB = color.Z;
                ColorA = color.W;
                TexCoordU = texCoord.X;
                TexCoordV = texCoord.Y;
            }

            public Vertex2D(Vector3 position, Vector4 color)
            {
                PositionX = position.X;
                PositionY = position.Y;
                PositionZ = position.Z;
                ColorR = color.X;
                ColorG = color.Y;
                ColorB = color.Z;
                ColorA = color.W;
                TexCoordU = 0;
                TexCoordV = 0;
            }

            public Vertex2D(Vector3 position, Vector2 texCoord)
            {
                PositionX = position.X;
                PositionY = position.Y;
                PositionZ = position.Z;
                ColorR = 0;
                ColorG = 0;
                ColorB = 0;
                ColorA = 0;
                TexCoordU = texCoord.X;
                TexCoordV = texCoord.Y;
            }

            private float PositionX;
            private float PositionY;
            private float PositionZ;
            private float ColorR;
            private float ColorG;
            private float ColorB;
            private float ColorA;
            private float TexCoordU;
            private float TexCoordV;

            public Vector3 Position
            {
                get
                {
                    return new Vector3(PositionX, PositionY, PositionZ);
                }

                set
                {
                    PositionX = value.X;
                    PositionY = value.Y;
                    PositionZ = value.Z;
                }
            }

            public Vector4 Color
            {
                get
                {
                    return new Vector4(ColorR, ColorG, ColorB, ColorA);
                }

                set
                {
                    ColorR = value.X;
                    ColorG = value.Y;
                    ColorB = value.Z;
                    ColorA = value.W;
                }
            }

            public Vector2 TexCoord
            {
                get
                {
                    return new Vector2(TexCoordU, TexCoordV);
                }

                set
                {
                    TexCoordU = value.X;
                    TexCoordV = value.Y;
                }
            }
        }

        #region Private Members
        private GlfwWindowPtr _RendererContext;
        private GlfwWindowPtr _SharedLoadingContext;

        private GraphicsResourceManager _GraphicsResourceManager;

        private Effect _2DEffect;
        private UniformBuffer _2DUniformBuffer;

        #region Actions
        private interface RCAction { };
        private class RCActionNoAction { };

        private class RCActionViewportSizeChange : RCAction
        {
            public RCActionViewportSizeChange(int width, int height, int x, int y) { Width = width; Height = height; X = x; Y = y; }

            public int X { get; private set; }
            public int Y { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }
        }

        #endregion

        private object _RendererCoreActionsListLock;
        private List<RCAction> _RendererCoreActionsList;

        #endregion

        #region Public Members
        public BasicEffect BasicEffect { get; private set; }
        public Vector2i ViewportSize { get; private set; }
        #endregion

        #region Public Methods

        public RendererCore(Messaging.MessageManager mssgmngr)
        {
            ValidMessages = new int[] { (int)Messaging.MessageId.WindowResizeMessage };
            mssgmngr += this;

            _RendererCoreActionsListLock = new object();
            _RendererCoreActionsList = new List<RCAction>();
        }

        /// <summary>
        /// Inits RenderCore
        /// </summary>
        /// <param name="RenderContext"></param>
        /// <param name="SharedLoadingContext"></param>
        /// <returns>True or False</returns>
        public bool Init(/*GlfwWindowPtr RenderContext, GlfwWindowPtr SharedLoadingContext*/)
        {
            /*
            try
            {
                Glfw.MakeContextCurrent(RenderContext);
                //Glfw.MakeContextCurrent(SharedLoadingContext);

                _RendererContext = RenderContext;
                _SharedLoadingContext = SharedLoadingContext;
            }catch{return false;}
             */

            //Do more init
            _GraphicsResourceManager = new GraphicsResourceManager();

            //Init BasicEffect
            CreateBasicEffect("Internal_Basic_Effect");
            BasicEffect.Init(this);

            //Init 2D Rendering Effect
            Create2DEffect("Internal_2D_Effect");
            GenerateBuffer(FreezingArcher.Math.Color4.Black);

            return true;
        }

        public void MakeLoadingContextCurrent()
        {
            Glfw.MakeContextCurrent(_SharedLoadingContext);
        }

        public void MakeDrawContextCurrent()
        {
            Glfw.MakeContextCurrent(_RendererContext);
        }

        /// <summary>
        /// Closes RenderCore
        /// </summary>
        public void Shutdown()
        {
            DeleteResources();

            //Do shutdown
            //Delete all resources!!
            GraphicsResource[] all_resources = _GraphicsResourceManager.GetAllResources();

            foreach (GraphicsResource r in all_resources)
            {
                _GraphicsResourceManager.DeleteResource(r);

                //And shutdown all resources!!
                DeleteGraphicsResource(r);
            }

            _RendererContext = GlfwWindowPtr.Null;
            _SharedLoadingContext = GlfwWindowPtr.Null;
        }









        public VertexBuffer CreateVertexBuffer<T>(T[] data, int size, RendererBufferUsage rbu, string name) where T : struct
        {
            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData<T>(BufferTarget.ArrayBuffer, (IntPtr)size, data, (BufferUsageHint)rbu);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            VertexBuffer vb = new VertexBuffer(name, vbo, size, rbu);

            vb.VertexCount = data.Length;

            _GraphicsResourceManager.AddResource(vb);

            return vb;
        }

        public IndexBuffer CreateIndexBuffer<T>(T[] data, int size, RendererBufferUsage rbu, string name) where T : struct
        {
            int ibo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData<T>(BufferTarget.ElementArrayBuffer, (IntPtr)size, data, (BufferUsageHint)rbu);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            IndexBuffer ib = new IndexBuffer(name, ibo, size, rbu);

            ib.IndexCount = data.Length;

            _GraphicsResourceManager.AddResource(ib);

            return ib;
        }

        public unsafe UniformBuffer CreateUniformBuffer<T>(T data, int size, string name) where T : struct
        {
            int ubo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, ubo);

            GL.BufferData<T>(BufferTarget.UniformBuffer, (IntPtr)size, ref data, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            UniformBuffer ub = new UniformBuffer(name, ubo, size);
            _GraphicsResourceManager.AddResource(ub);

            return ub;
        }

        public VertexBufferArray CreateVertexBufferArray(VertexBuffer vb, VertexBufferLayoutKind[] vblk, string name, IndexBuffer ib = null)
        {
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vb.ID);

            for (int i = 0; i < vblk.Length; i++)
            {
                GL.EnableVertexAttribArray(vblk[i].AttributeID);
                GL.VertexAttribPointer(vblk[i].AttributeID, vblk[i].AttributeSize, (VertexAttribPointerType)vblk[i].AttributeType,
                    vblk[i].Normalized, vblk[i].Stride, (IntPtr)vblk[i].Offset);
            }

            if (ib != null)
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib.ID);

            VertexBufferArray vba = new VertexBufferArray(name, vao, vblk, vb, ib);
            _GraphicsResourceManager.AddResource(vba);

            GL.BindVertexArray(0);

            return vba;
        }

        public Shader CreateShader(ShaderType st, string shadersource, string name)
        {
            uint shader_id = 0;

            switch (st)
            {
                case ShaderType.VertexShader:
                    shader_id = GL.CreateShader(Pencil.Gaming.Graphics.ShaderType.VertexShader);
                    break;

                case ShaderType.PixelShader:
                    shader_id = GL.CreateShader(Pencil.Gaming.Graphics.ShaderType.FragmentShader);
                    break;
            }

            GL.ShaderSource(shader_id, shadersource);
            GL.CompileShader(shader_id);
            //Hier noch Fehlermeldung loggen!!

            string log = GL.GetShaderInfoLog((int)shader_id);
            Console.WriteLine(log);

            Shader sh = new Shader(name, (int)shader_id, shadersource, st);
            _GraphicsResourceManager.AddResource(sh);

            return sh;
        }

        public ShaderProgram CreateShaderProgram(string name)
        {
            uint shader_program = GL.CreateProgram();

            ShaderProgram sp = new ShaderProgram(name, (int)shader_program);
            _GraphicsResourceManager.AddResource(sp);

            return sp;
        }

        public ShaderProgram CreateShaderProgram(string name, ShaderType st, string source)
        {
            uint shader_program = 0;

            string[] codes = { source };

            switch (st)
            {
                case ShaderType.VertexShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.VertexShader, 1, codes);
                    break;

                case ShaderType.PixelShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.FragmentShader, 1, codes);
                    break;

                case ShaderType.TesselationControlShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.TessControlShader, 1, codes);
                    break;

                case ShaderType.TesselationEvaluationShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.TessEvaluationShader, 1, codes);
                    break;

                case ShaderType.GeometryShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.GeometryShader, 1, codes);
                    break;
            }

            ShaderProgram sp = new ShaderProgram(name, (int)shader_program);
            _GraphicsResourceManager.AddResource(sp);

            return sp;
        }

        public ShaderProgram CreateShaderProgramFromFile(string name, ShaderType st, string file)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(file);
            string source = sr.ReadToEnd();
            sr.Close();

            uint shader_program = 0;

            string[] codes = { source };

            switch (st)
            {
                case ShaderType.VertexShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.VertexShader, 1, codes);
                    break;

                case ShaderType.PixelShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.FragmentShader, 1, codes);
                    break;

                case ShaderType.TesselationControlShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.TessControlShader, 1, codes);
                    break;

                case ShaderType.TesselationEvaluationShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.TessEvaluationShader, 1, codes);
                    break;

                case ShaderType.GeometryShader:
                    shader_program = GL.CreateShaderProgram(Pencil.Gaming.Graphics.ShaderType.GeometryShader, 1, codes);
                    break;
            }

            string error = GL.GetProgramInfoLog((int)shader_program);

            if (error.Length > 0)
                Logger.Log.AddLogEntry(LogLevel.Fatal, "RendererCore", GL.GetProgramInfoLog((int)shader_program) + " in " + file);

            ShaderProgram sp = new ShaderProgram(name, (int)shader_program);
            _GraphicsResourceManager.AddResource(sp);

            return sp;
        }

        public Effect CreateEffect(string name)
        {
            int id = GL.GenProgramPipeline();

            Effect eff = new Effect(name, id);
            _GraphicsResourceManager.AddResource(eff);

            GL.BindProgramPipeline(id);
            GL.UseProgramStages(id, ProgramStageMask.AllShaderBits, 0);

            GL.BindProgramPipeline(0);

            return eff;
        }

        private void CreateBasicEffect(string name)
        {
            int id = GL.GenProgramPipeline();

            BasicEffect = new BasicEffect(name, id);
            _GraphicsResourceManager.AddResource(BasicEffect);

            GL.BindProgramPipeline(id);
            GL.UseProgramStages(id, ProgramStageMask.AllShaderBits, 0);

            //Create VertexShader
            ShaderProgram sp_Vertex = CreateShaderProgramFromFile("Internal_Basic_Effect_Vertex_Shader", ShaderType.VertexShader, "lib/Renderer/Effects/SimpleMaterial/vertex_shader.vs");

            //Create PixelShader
            ShaderProgram sp_Pixel = CreateShaderProgramFromFile("Internal_Basic_Effect_Pixel_Sahder", ShaderType.PixelShader, "lib/Renderer/Effects/SimpleMaterial/pixel_shader.ps");

            BasicEffect.VertexProgram = sp_Vertex;
            BasicEffect.PixelProgram = sp_Pixel;

            GL.BindProgramPipeline(0);
        }

        private void Create2DEffect(string name)
        {
            int id = GL.GenProgramPipeline();

            _2DEffect = new Effect(name, id);
            _GraphicsResourceManager.AddResource(_2DEffect);

            GL.BindProgramPipeline(id);
            GL.UseProgramStages(id, ProgramStageMask.AllShaderBits, 0);

            //Create VertexShader
            ShaderProgram sp_Vertex = CreateShaderProgramFromFile("Internal_2D_Effect_Vertex_Shader", ShaderType.VertexShader, "lib/Renderer/Effects/2D/VertexShader.vs");

            //Create PixelShader
            ShaderProgram sp_Pixel = CreateShaderProgramFromFile("Internal_2D_Effect_Pixel_Shader", ShaderType.PixelShader, "lib/Renderer/Effects/2D/PixelShader.ps");

            _2DEffect.VertexProgram = sp_Vertex;
            _2DEffect.PixelProgram = sp_Pixel;

            GL.BindProgramPipeline(0);

            MatricesBlock2D mb = new MatricesBlock2D();
            mb.World = Matrix.Identity;
            mb.Projection = Matrix.Identity;

            _2DUniformBuffer = CreateUniformBuffer<MatricesBlock2D>(mb, sizeof(float) * 4 * 4 * 2, "Internal_2D_UniformBuffer");
            _2DEffect.VertexProgram.SetUniformBlockBinding("MatricesBlock", 5);
            _2DUniformBuffer.SetBufferBase(5);
        }

        public Sampler CreateSampler(string name)
        {
            int id = GL.GenSampler();

            Sampler s2d = new Sampler(name, id);
            _GraphicsResourceManager.AddResource(s2d);

            return s2d;
        }

        public Texture1D CreateTexture1D(string name, int width, bool generateMipMaps, IntPtr data)
        {
            int id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture1D, id);
            GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rgba, width, 0, PixelFormat.Bgra, PixelType.Float, data);

            if (generateMipMaps)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture1D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture1D, 0);

            Texture1D tex = new Texture1D(width, generateMipMaps, name, id);
            _GraphicsResourceManager.AddResource(tex);

            Sampler s2d = CreateSampler(name + "_Sampler");
            tex.Sampler = s2d;
            tex.SamplerAllowed = true;

            return tex;
        }

        public Texture2D CreateTexture2D(string name, int width, int height, bool generateMipMaps, IntPtr data, bool generate_sampler = true)
        {
            int id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data);

            if (generateMipMaps)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            Texture2D tex = new Texture2D(width, height, generateMipMaps, name, id);
            _GraphicsResourceManager.AddResource(tex);

            Sampler s2d = CreateSampler(name + "_Sampler");
            tex.Sampler = s2d;
            tex.SamplerAllowed = generate_sampler;

            return tex;
        }

        public Texture2D CreateTexture2D(string name, bool generateMipMaps, System.Drawing.Bitmap data)
        {
            if (data != null)
            {
                System.Drawing.Imaging.BitmapData bmp_data = data.LockBits(new System.Drawing.Rectangle(0, 0, data.Width, data.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int id = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, id);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                data.UnlockBits(bmp_data);

                if (generateMipMaps)
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.BindTexture(TextureTarget.Texture2D, 0);

                Texture2D tex = new Texture2D(data.Width, data.Height, generateMipMaps, name, id);
                _GraphicsResourceManager.AddResource(tex);

                Sampler s2d = CreateSampler(name + "_Sampler");
                tex.Sampler = s2d;
                tex.SamplerAllowed = true;

                return tex;
            }

            return null;
        }
            
        public Texture2D CreateTexture2D(string name, bool generateMipMaps, string data)
        {
            if (data != null)
            {
                System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(data);
               
                bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

                System.Drawing.Imaging.BitmapData bmp_data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int id = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2D, id);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

                bmp.UnlockBits(bmp_data);

                if (generateMipMaps)
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                GL.BindTexture(TextureTarget.Texture2D, 0);

                Texture2D tex = new Texture2D(bmp.Width, bmp.Height, generateMipMaps, name, id);

                bmp.Dispose();

                _GraphicsResourceManager.AddResource(tex);

                Sampler s2d = CreateSampler(name + "_Sampler");
                tex.Sampler = s2d;
                tex.SamplerAllowed = true;

                return tex;
            }

            return null;
        }

        public Texture3D CreateTexture3D(string name, int width, int height, int depth, bool generateMipMaps, IntPtr data)
        {
            int id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture3D, id);
            GL.TexImage3D(TextureTarget.Texture3D, 0, PixelInternalFormat.Rgba, width, height, depth, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data);

            if (generateMipMaps)
                GL.GenerateMipmap(GenerateMipmapTarget.Texture3D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture3D, 0);

            Texture3D tex = new Texture3D(width, height, depth, generateMipMaps, name, id);
            _GraphicsResourceManager.AddResource(tex);

            Sampler s2d = CreateSampler(name + "_Sampler");
            tex.Sampler = s2d;
            tex.SamplerAllowed = true;

            return tex;
        }

        public TextureDepthStencil CreateTextureDepthStencil(string name, int width, int height, IntPtr data, bool generate_sampler = true)
        {
            int id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, width, height, 0, PixelFormat.DepthStencil, PixelType.UnsignedInt248, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            TextureDepthStencil tds = new TextureDepthStencil(width, height, name, id);
            _GraphicsResourceManager.AddResource(tds);

            Sampler s2d = CreateSampler(name + "_Sampler");
            tds.Sampler = s2d;
            tds.SamplerAllowed = generate_sampler;

            return tds;
        }

        public FrameBuffer CreateFrameBuffer(string name)
        {
            int id = GL.GenFramebuffer();

            FrameBuffer fb = new FrameBuffer(name, id);
            _GraphicsResourceManager.AddResource(fb);

            return fb;
        }

        public void Begin()
        {
            lock (_RendererCoreActionsListLock)
            {
                foreach (RCAction rca in _RendererCoreActionsList)
                {
                    RCActionViewportSizeChange rcavsc = rca as RCActionViewportSizeChange;
                    if (rcavsc != null)
                        ViewportResize(rcavsc.X, rcavsc.Y, rcavsc.Width, rcavsc.Height);
                }

                _RendererCoreActionsList.Clear();
            }
        }

        public void End()
        {
            Begin();
        }

        public void DrawArrays(int offset, int count, RendererBeginMode bm)
        {
            GL.DrawArrays((BeginMode)bm, offset, count);
        }

        public void DrawElements(int offset, int count, RendererIndexType det, RendererBeginMode bm)
        {
            GL.DrawElements((BeginMode)bm, count, (DrawElementsType)det, offset);
        }


        private VertexBuffer _2DVertexBuffer;
        private VertexBufferArray _2DVertexBufferArray;

        private unsafe void GenerateBuffer(FreezingArcher.Math.Color4 color)
        {
            Vertex2D[] v2d = new Vertex2D[9];

            v2d[0] = new Vertex2D(new Vector3(0.0f, 0.0f, -5.0f), new Vector4(color.R, color.G, color.B, color.A), new Vector2(0.0f, 0.0f));
            v2d[1] = new Vertex2D(new Vector3(1.0f, 0.0f, -5.0f), new Vector4(color.R, color.G, color.B, color.A), new Vector2(1.0f, 0.0f));
            v2d[2] = new Vertex2D(new Vector3(0.0f, 1.0f, -5.0f), new Vector4(color.R, color.G, color.B, color.A), new Vector2(0.0f, 1.0f));
            v2d[3] = new Vertex2D(new Vector3(0.0f, 1.0f, -5f), new Vector4(color.R, color.G, color.B, color.A), new Vector2(0.0f, 1.0f));
            v2d[4] = new Vertex2D(new Vector3(1.0f, 0.0f, -5f), new Vector4(color.R, color.G, color.B, color.A), new Vector2(1.0f, 0.0f));
            v2d[5] = new Vertex2D(new Vector3(1.0f, 1.0f, -5f), new Vector4(color.R, color.G, color.B, color.A), new Vector2(1.0f, 1.0f));

            v2d[6] = new Vertex2D(new Vector3(0.0f, 0.0f, -5f), new Vector4(color.R, color.G, color.B, color.A), new Vector2(0.0f, 0.0f));
            v2d[7] = new Vertex2D(new Vector3(-1.0f, -1.0f, -5f), new Vector4(color.R, color.G, color.B, color.A), new Vector2(0.0f, 0.0f));
            v2d[8] = new Vertex2D(new Vector3(1.0f, -1.0f, -5f), new Vector4(color.R, color.G, color.B, color.A), new Vector2(0.0f, 0.0f));

            VertexBufferLayoutKind[] vblk = new VertexBufferLayoutKind[3];
            vblk[0].AttributeID = 0;
            vblk[0].AttributeSize = 3;
            vblk[0].AttributeType = RendererVertexAttribType.Float;
            vblk[0].Normalized = false;
            vblk[0].Offset = 0;
            vblk[0].Stride = Vertex2D.SIZE;

            vblk[1].AttributeID = 1;
            vblk[1].AttributeSize = 4;
            vblk[1].AttributeType = RendererVertexAttribType.Float;
            vblk[1].Normalized = false;
            vblk[1].Offset = sizeof(Vector3);
            vblk[1].Stride = Vertex2D.SIZE;

            vblk[2].AttributeID = 2;
            vblk[2].AttributeSize = 2;
            vblk[2].AttributeType = RendererVertexAttribType.Float;
            vblk[2].Normalized = false;
            vblk[2].Offset = sizeof(Vector3) + sizeof(Vector4);
            vblk[2].Stride = Vertex2D.SIZE;

            _2DVertexBuffer = CreateVertexBuffer<Vertex2D>(v2d, Vertex2D.SIZE * 9, RendererBufferUsage.StaticDraw, "Internal2DVertexBuffer");
            _2DVertexBufferArray = CreateVertexBufferArray(_2DVertexBuffer, vblk, "Internal2DVertexBufferArray");
        }

        private void DeleteResources()
        {
            DeleteGraphicsResource(_2DVertexBufferArray);
            DeleteGraphicsResource(_2DVertexBuffer);
        }

        private void DrawFilledCircle(ref Vector2 position, ref Vector2 screen_size, float radius, float max_angle, ref FreezingArcher.Math.Color4 color, bool relative = false)
        {
            float log = (float) System.Math.Log(radius) / MathHelper.Pi;
            uint segments = (uint)(radius / (log * log));

            if(radius < 0.0f)
            {
                Logger.Log.AddLogEntry(LogLevel.Severe, "RendererCore", Core.Status.ConfoundedByPonies);

                return;
            }

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.CullFace(CullFaceMode.FrontAndBack);
            GL.Disable(EnableCap.DepthTest);

            MatricesBlock2D md = new MatricesBlock2D();

            md.Projection = Matrix.CreateOrthographicOffCenter(0.0f, screen_size.X, screen_size.Y, 0.0f, 1.0f, 100.0f);

            float angle = 0;

            float _scale_factor = 1.0f * (float)System.Math.Tan(MathHelper.ToRadians(180.0f / (double)segments));

            for (int i = 0; i < segments; i++)
            {
                md.World = Matrix.CreateScale(_scale_factor * radius, radius, 1.0f) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(new Vector3(position));
               
                angle += MathHelper.ToRadians(360.0f / (float)segments);
                if (MathHelper.ToDegrees(angle) >= max_angle + 1.0f)
                    break;

                _2DUniformBuffer.UpdateBuffer<MatricesBlock2D>(md, sizeof(float) * 4 * 4 * 2);
                _2DVertexBufferArray.BindVertexBufferArray();

                _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("UseTexture"), 0.0f);
                _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("DrawColor"), new Pencil.Gaming.MathUtils.Vector4(color.R, color.G, color.B, color.A));

                _2DEffect.BindPipeline();

                DrawArrays(6, 3, RendererBeginMode.Triangles);
            }

            _2DVertexBuffer.UnbindBuffer();

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }

        private void DrawSprite(ref Vector2 screen_size, Sprite spr, bool relative = false)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.CullFace(CullFaceMode.FrontAndBack);
            GL.Disable(EnableCap.DepthTest);

            MatricesBlock2D md = new MatricesBlock2D();

            if (!relative)
                md.World = Matrix.CreateTranslation(new Vector3(-spr.RotationPoint)) * Matrix.CreateRotationZ(spr.Rotation) * Matrix.CreateScale(spr.Texture.Width * spr.Scaling.X, spr.Texture.Height * spr.Scaling.Y, 1.0f) * Matrix.CreateTranslation(new Vector3(spr.AbsolutePosition.X, spr.AbsolutePosition.Y, 0.0f));
            else
                md.World = Matrix.CreateTranslation(new Vector3(-spr.RotationPoint)) * Matrix.CreateRotationZ(spr.Rotation) * Matrix.CreateScale((spr.Texture.Width / (float)screen_size.X) * spr.Scaling.X, ((float)spr.Texture.Height / (float)screen_size.Y) * spr.Scaling.Y, 1.0f) * Matrix.CreateTranslation(new Vector3(spr.RelativePosition.X, spr.RelativePosition.Y, 0.0f));

            md.Projection = Matrix.CreateOrthographicOffCenter(0.0f, screen_size.X, screen_size.Y, 0.0f, 1.0f, 100.0f);

            _2DUniformBuffer.UpdateBuffer<MatricesBlock2D>(md, sizeof(float) * 4 * 4 * 2);
            _2DVertexBufferArray.BindVertexBufferArray();

            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("UseTexture"), 1.0f);
            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("Texture1"), 0);

            spr.Texture.Bind(0);

            _2DEffect.BindPipeline();

            DrawArrays(0, 6, RendererBeginMode.Triangles);

            _2DVertexBuffer.UnbindBuffer();
            spr.Texture.Unbind();

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }

        private void DrawFilledRectangle(ref Vector2 position, ref Vector2 size, ref Vector2 screen_size, ref FreezingArcher.Math.Color4 color, bool relative = false)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Disable(EnableCap.DepthTest);

            MatricesBlock2D md = new MatricesBlock2D();
            md.World = Matrix.CreateScale(size.X, size.Y, 1.0f) * Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));

            md.Projection = Matrix.CreateOrthographicOffCenter(0.0f, screen_size.X, screen_size.Y, 0.0f, 1.0f, 100.0f);

            _2DUniformBuffer.UpdateBuffer<MatricesBlock2D>(md, sizeof(float) * 4 * 4 * 2);

            _2DVertexBufferArray.BindVertexBufferArray();

            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("UseTexture"), 0.0f);
            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("DrawColor"), new Pencil.Gaming.MathUtils.Vector4(color.R, color.G, color.B, color.A));

            _2DEffect.BindPipeline();

            DrawArrays(0, 6, RendererBeginMode.Triangles);

            _2DUniformBuffer.UnbindBuffer();

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }

        private void DrawCircle(ref Vector2 position, ref Vector2 size, ref Vector2 screen_size, ref FreezingArcher.Math.Color4 color, bool relative = false)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Disable(EnableCap.DepthTest);

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }

        private void DrawRectangle(ref Vector2 position, ref Vector2 size, ref Vector2 screen_size, float line_width, ref FreezingArcher.Math.Color4 color, bool relative = false)
        {
            Vector2 position1 = position + new Vector2(line_width / 2, 0.0f);
            Vector2 position2 = position + new Vector2(size.X, 0.0f);
            Vector2 position3 = position + new Vector2(line_width / 2, size.Y);
            Vector2 position4 = position + size + new Vector2(line_width / 2, -line_width / 2);
            Vector2 position5 = position3 - new Vector2(0.0f, line_width / 2);
            Vector2 position6 = position + new Vector2(size.X, -line_width / 2);
            Vector2 position7 = position + size;

            DrawLine(ref position, ref position2, ref screen_size, line_width, ref color, relative);
            DrawLine(ref position1, ref position3, ref screen_size, line_width, ref color, relative);
            DrawLine(ref position5, ref position4, ref screen_size, line_width, ref color, relative);
            // DrawLine(ref position6, ref position7, ref screen_size, line_width, ref color, relative);
            DrawLine(ref position7, ref position6, ref screen_size, line_width, ref color, relative);
        }

        private void DrawLine(ref Vector2 position1, ref Vector2 position2, ref Vector2 screen_size, float thickness, ref FreezingArcher.Math.Color4 color, bool relative = false)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.CullFace(CullFaceMode.FrontAndBack);
            GL.Disable(EnableCap.DepthTest);

            GL.LineWidth(thickness);

            MatricesBlock2D md = new MatricesBlock2D();

            Vector2 dir = position2 - position1;

            float winkel = (float)System.Math.Acos(Vector2.Dot(Vector2.UnitX, Vector2.Normalize(dir)) / (Vector2.Normalize(dir).Length));
            if (dir.Y < 0 || dir.X < 0)
                winkel *= -1;


            md.World = Matrix.CreateScale(dir.Length, 1.0f, 1.0f) * Matrix.CreateRotationZ(winkel) * Matrix.CreateTranslation(new FreezingArcher.Math.Vector3(position1));

            md.Projection = Matrix.CreateOrthographicOffCenter(0.0f, screen_size.X, screen_size.Y, 0.0f, 1.0f, 100.0f);

            _2DUniformBuffer.UpdateBuffer<MatricesBlock2D>(md, sizeof(float) * 4 * 4 * 2);
            _2DVertexBufferArray.BindVertexBufferArray();

            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("UseTexture"), 0.0f);
            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("DrawColor"), new Pencil.Gaming.MathUtils.Vector4(color.R, color.G, color.B, color.A));

            _2DEffect.BindPipeline();

            DrawArrays(0, 2, RendererBeginMode.Lines);

            _2DUniformBuffer.UnbindBuffer();

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }

        private void DrawPoint(ref Vector2 position, ref Vector2 screen_size, float PointWidth, ref FreezingArcher.Math.Color4 color, bool relative = false)
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.CullFace(CullFaceMode.FrontAndBack);
            GL.Disable(EnableCap.DepthTest);

            GL.PointSize(PointWidth);

            MatricesBlock2D md = new MatricesBlock2D();
            md.World = Matrix.CreateTranslation(new Vector3(position));
            md.Projection = Matrix.CreateOrthographicOffCenter(0.0f, screen_size.X, screen_size.Y, 0.0f, 1.0f, 100.0f);

            _2DUniformBuffer.UpdateBuffer<MatricesBlock2D>(md, sizeof(float) * 4 * 4 * 2);
            _2DVertexBufferArray.BindVertexBufferArray();

            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("UseTexture"), 0.0f);
            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("DrawColor"), new Pencil.Gaming.MathUtils.Vector4(color.R, color.G, color.B, color.A));

            _2DEffect.BindPipeline();

            DrawArrays(0, 1, RendererBeginMode.Points);

            _2DUniformBuffer.UnbindBuffer();

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }

        public void DrawRectangleAbsolute(ref Vector2 position, ref Vector2 size, float line_width, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(ViewportSize.X, ViewportSize.Y);

            DrawRectangle(ref position, ref size, ref vs, line_width, ref color);
        }

        public void DrawRectangleRelative(ref Vector2 position, ref Vector2 size, float line_width, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(1.0f, 1.0f);

            DrawRectangle(ref position, ref size, ref vs, line_width, ref color, true);
        }

        public void DrawFilledRectangleAbsolute(ref Vector2 position, ref Vector2 size, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(ViewportSize.X, ViewportSize.Y);

            DrawFilledRectangle(ref position, ref size, ref vs, ref color);
        }

        public void DrawSpriteAbsolute(Sprite spr)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(ViewportSize.X, ViewportSize.Y);

            DrawSprite(ref vs, spr);
        }

        public void DrawFilledRectangleRelaive(ref Vector2 position, ref Vector2 size, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(1.0f, 1.0f);

            DrawFilledRectangle(ref position, ref size, ref vs, ref color, true);
        }

        public void DrawSpriteRelative(Sprite spr)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(1.0f, 1.0f);

            DrawSprite(ref vs, spr, true);
        }

        public void DrawLineAbsolute(ref Vector2 Position1, ref Vector2 Position2, float LineWidth, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new Vector2(ViewportSize.X, ViewportSize.Y);

            DrawLine(ref Position1, ref Position2, ref vs, LineWidth, ref color);
        }

        public void DrawLineRelative(ref Vector2 Position1, ref Vector2 Position2, float LineWidth, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new Vector2(1.0f, 1.0f);

            DrawLine(ref Position1, ref Position2, ref vs, LineWidth, ref color);
        }

        public void DrawPointAbsolute(ref Vector2 Position, float PointWidth, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new Vector2(ViewportSize.X, ViewportSize.Y);

            DrawPoint(ref Position, ref vs, PointWidth, ref color);
        }

        public void DrawPointRelative(ref Vector2 Position, float PointWidth, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new Vector2(ViewportSize.X, ViewportSize.Y);

            DrawPoint(ref Position, ref vs, PointWidth, ref color);
        }

        public void DrawFilledCircleAbsolute(ref Vector2 Position, float radius, float max_angle, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new Vector2(ViewportSize.X, ViewportSize.Y);

            DrawFilledCircle(ref Position, ref vs, radius, max_angle, ref color);
        }

        public void DrawFilledCircleRelative(ref Vector2 Position, float radius, float max_angle, ref FreezingArcher.Math.Color4 color)
        {
            Vector2 vs = new Vector2(1.0f, 1.0f);

            DrawFilledCircle(ref Position, ref vs, radius, max_angle, ref color);
        }

        public void DrawPointsAbsolute(Vector2[] position, float[] PointWidth, FreezingArcher.Math.Color4[] color)
        {
            throw new NotImplementedException();
        }

        public void DrawPointsAbsolute(Vector2[] position, float PointWidth, FreezingArcher.Math.Color4 color)
        {
            throw new NotImplementedException();
        }

        public void DeleteGraphicsResource(GraphicsResource gr)
        {
            if (gr.Created)
            {
                _GraphicsResourceManager.DeleteResource(gr);

                switch (gr.Type)
                {
                    case GraphicsResourceType.Shader:
                        GL.DeleteShader(gr.ID);
                        break;

                    case GraphicsResourceType.ShaderProgram:
                        Shader[] shaders = ((ShaderProgram)gr).Shaders;
                        foreach (Shader sh in shaders)
                            ((ShaderProgram)gr).DeleteShader(sh);

                        GL.DeleteProgram(gr.ID);
                        break;

                    case GraphicsResourceType.ProgramPipeline:
                        Effect eff = (Effect)gr;

                        eff.VertexProgram = null;
                        eff.PixelProgram = null;
                        eff.TesselationControlProgram = null;
                        eff.TesselationEvaluationProgram = null;
                        eff.GeometryProgram = null;

                        GL.DeleteProgramPipeline(gr.ID);
                        break;

                    case GraphicsResourceType.Sampler:
                        GL.DeleteSampler(gr.ID);
                        break;

                    case GraphicsResourceType.Texture2D:
                    case GraphicsResourceType.Texture3D:
                    case GraphicsResourceType.Texture1D:
                        ((Texture2D)gr).Sampler = null;
                        GL.DeleteTexture(gr.ID);
                        break;

                    case GraphicsResourceType.VertexBuffer:
                        GL.DeleteBuffer(gr.ID);
                        break;

                    case GraphicsResourceType.IndexBuffer:
                        GL.DeleteBuffer(gr.ID);
                        break;

                    case GraphicsResourceType.UniformBuffer:
                        GL.DeleteBuffer(gr.ID);
                        break;

                    case GraphicsResourceType.VertexBufferArray:
                        ((VertexBufferArray)gr).Shutdown();
                        GL.DeleteVertexArray(gr.ID);
                        break;
                }

                gr.SetCreated(false);
            }
        }

        #endregion

        #region Waste

        public void SetCullMode(RendererCullMode rcm)
        {
            GL.CullFace((CullFaceMode)rcm);
        }

        public void EnableDepthTest(bool enable)
        {
            if (enable)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);
        }

        public void Clear(FreezingArcher.Math.Color4 color)
        {
            GL.ClearColor(new Pencil.Gaming.Graphics.Color4(color.R, color.G, color.B, color.A));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        public void ViewportResize(int x, int y, int width, int height)
        {
            GL.Viewport(x, y, width, height);
            ViewportSize = new Vector2i(width, height);
        }
        #endregion

        public void ConsumeMessage(Messaging.Interfaces.IMessage msg)
        {
            Messaging.WindowResizeMessage wrm = msg as Messaging.WindowResizeMessage;
            if (wrm != null)
            {
                lock (_RendererCoreActionsListLock)
                {
                    _RendererCoreActionsList.Add(new RCActionViewportSizeChange(wrm.Width, wrm.Height, 0, 0));
                }
            }
        }

        public int[] ValidMessages
        {
            get;
            private set;
        }
    }
}
