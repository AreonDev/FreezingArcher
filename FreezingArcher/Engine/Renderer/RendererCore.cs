using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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

    public enum RendererEnableCap : int {
        PointSmooth = ((int)0x0B10)             ,
        LineSmooth = ((int)0x0B20)              ,
        LineStipple = ((int)0x0B24)             ,
        PolygonSmooth = ((int)0x0B41)           ,
        PolygonStipple = ((int)0x0B42)          ,
        CullFace = ((int)0x0B44)                ,
        Lighting = ((int)0x0B50)                ,
        ColorMaterial = ((int)0x0B57)           ,
        Fog = ((int)0x0B60)             ,
        DepthTest = ((int)0x0B71)               ,
        StencilTest = ((int)0x0B90)             ,
        Normalize = ((int)0x0BA1)               ,
        AlphaTest = ((int)0x0BC0)               ,
        Dither = ((int)0x0BD0)          ,
        Blend = ((int)0x0BE2)           ,
        IndexLogicOp = ((int)0x0BF1)            ,
        ColorLogicOp = ((int)0x0BF2)            ,
        ScissorTest = ((int)0x0C11)             ,
        TextureGenS = ((int)0x0C60)             ,
        TextureGenT = ((int)0x0C61)             ,
        TextureGenR = ((int)0x0C62)             ,
        TextureGenQ = ((int)0x0C63)             ,
        AutoNormal = ((int)0x0D80)              ,
        Map1Color4 = ((int)0x0D90)              ,
        Map1Index = ((int)0x0D91)               ,
        Map1Normal = ((int)0x0D92)              ,
        Map1TextureCoord1 = ((int)0x0D93)               ,
        Map1TextureCoord2 = ((int)0x0D94)               ,
        Map1TextureCoord3 = ((int)0x0D95)               ,
        Map1TextureCoord4 = ((int)0x0D96)               ,
        Map1Vertex3 = ((int)0x0D97)             ,
        Map1Vertex4 = ((int)0x0D98)             ,
        Map2Color4 = ((int)0x0DB0)              ,
        Map2Index = ((int)0x0DB1)               ,
        Map2Normal = ((int)0x0DB2)              ,
        Map2TextureCoord1 = ((int)0x0DB3)               ,
        Map2TextureCoord2 = ((int)0x0DB4)               ,
        Map2TextureCoord3 = ((int)0x0DB5)               ,
        Map2TextureCoord4 = ((int)0x0DB6)               ,
        Map2Vertex3 = ((int)0x0DB7)             ,
        Map2Vertex4 = ((int)0x0DB8)             ,
        Texture1D = ((int)0x0DE0)               ,
        Texture2D = ((int)0x0DE1)               ,
        PolygonOffsetPoint = ((int)0x2A01)              ,
        PolygonOffsetLine = ((int)0x2A02)               ,
        ClipPlane0 = ((int)0x3000)              ,
        ClipPlane1 = ((int)0x3001)              ,
        ClipPlane2 = ((int)0x3002)              ,
        ClipPlane3 = ((int)0x3003)              ,
        ClipPlane4 = ((int)0x3004)              ,
        ClipPlane5 = ((int)0x3005)              ,
        Light0 = ((int)0x4000)          ,
        Light1 = ((int)0x4001)          ,
        Light2 = ((int)0x4002)          ,
        Light3 = ((int)0x4003)          ,
        Light4 = ((int)0x4004)          ,
        Light5 = ((int)0x4005)          ,
        Light6 = ((int)0x4006)          ,
        Light7 = ((int)0x4007)          ,
        Convolution1D = ((int)0x8010)           ,
        Convolution1DExt = ((int)0x8010)                ,
        Convolution2D = ((int)0x8011)           ,
        Convolution2DExt = ((int)0x8011)                ,
        Separable2D = ((int)0x8012)             ,
        Separable2DExt = ((int)0x8012)          ,
        Histogram = ((int)0x8024)               ,
        HistogramExt = ((int)0x8024)            ,
        MinmaxExt = ((int)0x802E)               ,
        PolygonOffsetFill = ((int)0x8037)               ,
        RescaleNormal = ((int)0x803A)           ,
        RescaleNormalExt = ((int)0x803A)                ,
        Texture3DExt = ((int)0x806F)            ,
        VertexArray = ((int)0x8074)             ,
        NormalArray = ((int)0x8075)             ,
        ColorArray = ((int)0x8076)              ,
        IndexArray = ((int)0x8077)              ,
        TextureCoordArray = ((int)0x8078)               ,
        EdgeFlagArray = ((int)0x8079)           ,
        InterlaceSgix = ((int)0x8094)           ,
        Multisample = ((int)0x809D)             ,
        MultisampleSgis = ((int)0x809D)         ,
        SampleAlphaToCoverage = ((int)0x809E)           ,
        SampleAlphaToMaskSgis = ((int)0x809E)           ,
        SampleAlphaToOne = ((int)0x809F)                ,
        SampleAlphaToOneSgis = ((int)0x809F)            ,
        SampleCoverage = ((int)0x80A0)          ,
        SampleMaskSgis = ((int)0x80A0)          ,
        TextureColorTableSgi = ((int)0x80BC)            ,
        ColorTable = ((int)0x80D0)              ,
        ColorTableSgi = ((int)0x80D0)           ,
        PostConvolutionColorTable = ((int)0x80D1)               ,
        PostConvolutionColorTableSgi = ((int)0x80D1)            ,
        PostColorMatrixColorTable = ((int)0x80D2)               ,
        PostColorMatrixColorTableSgi = ((int)0x80D2)            ,
        Texture4DSgis = ((int)0x8134)           ,
        PixelTexGenSgix = ((int)0x8139)         ,
        SpriteSgix = ((int)0x8148)              ,
        ReferencePlaneSgix = ((int)0x817D)              ,
        IrInstrument1Sgix = ((int)0x817F)               ,
        CalligraphicFragmentSgix = ((int)0x8183)                ,
        FramezoomSgix = ((int)0x818B)           ,
        FogOffsetSgix = ((int)0x8198)           ,
        SharedTexturePaletteExt = ((int)0x81FB)         ,
        AsyncHistogramSgix = ((int)0x832C)              ,
        PixelTextureSgis = ((int)0x8353)                ,
        AsyncTexImageSgix = ((int)0x835C)               ,
        AsyncDrawPixelsSgix = ((int)0x835D)             ,
        AsyncReadPixelsSgix = ((int)0x835E)             ,
        FragmentLightingSgix = ((int)0x8400)            ,
        FragmentColorMaterialSgix = ((int)0x8401)               ,
        FragmentLight0Sgix = ((int)0x840C)              ,
        FragmentLight1Sgix = ((int)0x840D)              ,
        FragmentLight2Sgix = ((int)0x840E)              ,
        FragmentLight3Sgix = ((int)0x840F)              ,
        FragmentLight4Sgix = ((int)0x8410)              ,
        FragmentLight5Sgix = ((int)0x8411)              ,
        FragmentLight6Sgix = ((int)0x8412)              ,
        FragmentLight7Sgix = ((int)0x8413)              ,
        FogCoordArray = ((int)0x8457)           ,
        ColorSum = ((int)0x8458)                ,
        SecondaryColorArray = ((int)0x845E)             ,
        TextureRectangle = ((int)0x84F5)                ,
        TextureCubeMap = ((int)0x8513)          ,
        ProgramPointSize = ((int)0x8642)                ,
        VertexProgramPointSize = ((int)0x8642)          ,
        VertexProgramTwoSide = ((int)0x8643)            ,
        DepthClamp = ((int)0x864F)              ,
        TextureCubeMapSeamless = ((int)0x884F)          ,
        PointSprite = ((int)0x8861)             ,
        SampleShading = ((int)0x8C36)           ,
        RasterizerDiscard = ((int)0x8C89)               ,
        FramebufferSrgb = ((int)0x8DB9)         ,
        SampleMask = ((int)0x8E51)              ,
        PrimitiveRestart = ((int)0x8F9D)                ,
    }

    public enum RendererBlendingFactorDest : int {
        Zero = ((int)0)         ,
        SrcColor = ((int)0x0300)                ,
        OneMinusSrcColor = ((int)0x0301)                ,
        SrcAlpha = ((int)0x0302)                ,
        OneMinusSrcAlpha = ((int)0x0303)                ,
        DstAlpha = ((int)0x0304)                ,
        OneMinusDstAlpha = ((int)0x0305)                ,
        ConstantColor = ((int)0x8001)           ,
        ConstantColorExt = ((int)0x8001)                ,
        OneMinusConstantColor = ((int)0x8002)           ,
        OneMinusConstantColorExt = ((int)0x8002)                ,
        ConstantAlpha = ((int)0x8003)           ,
        ConstantAlphaExt = ((int)0x8003)                ,
        OneMinusConstantAlpha = ((int)0x8004)           ,
        OneMinusConstantAlphaExt = ((int)0x8004)                ,
        Src1Alpha = ((int)0x8589)               ,
        Src1Color = ((int)0x88F9)               ,
        OneMinusSrc1Color = ((int)0x88FA)               ,
        OneMinusSrc1Alpha = ((int)0x88FB)               ,
        One = ((int)1)          ,
    }
    public enum RendererBlendingFactorSrc : int {
        Zero = ((int)0)         ,
        SrcAlpha = ((int)0x0302)                ,
        OneMinusSrcAlpha = ((int)0x0303)                ,
        DstAlpha = ((int)0x0304)                ,
        OneMinusDstAlpha = ((int)0x0305)                ,
        DstColor = ((int)0x0306)                ,
        OneMinusDstColor = ((int)0x0307)                ,
        SrcAlphaSaturate = ((int)0x0308)                ,
        ConstantColor = ((int)0x8001)           ,
        ConstantColorExt = ((int)0x8001)                ,
        OneMinusConstantColor = ((int)0x8002)           ,
        OneMinusConstantColorExt = ((int)0x8002)                ,
        ConstantAlpha = ((int)0x8003)           ,
        ConstantAlphaExt = ((int)0x8003)                ,
        OneMinusConstantAlpha = ((int)0x8004)           ,
        OneMinusConstantAlphaExt = ((int)0x8004)                ,
        Src1Alpha = ((int)0x8589)               ,
        Src1Color = ((int)0x88F9)               ,
        OneMinusSrc1Color = ((int)0x88FA)               ,
        OneMinusSrc1Alpha = ((int)0x88FB)               ,
        One = ((int)1)          ,
    }

    public enum RendererAlphaFunction : int {
        Never = ((int)0x0200)           ,
        Less = ((int)0x0201)            ,
        Equal = ((int)0x0202)           ,
        Lequal = ((int)0x0203)          ,
        Greater = ((int)0x0204)         ,
        Notequal = ((int)0x0205)                ,
        Gequal = ((int)0x0206)          ,
        Always = ((int)0x0207)          ,
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

        public Effect RC2DEffect
        {
            get
            {
                return _2DEffect;
            }
        }

        public FreezingArcher.Core.Application Application{ get; private set;}

        #region Actions
        public delegate void RCActionDelegate();

        public interface RCAction { RCActionDelegate Action {get;} };
        private class RCActionNoAction : RCAction { public RCActionDelegate Action { get { return delegate() {}; } }}

        private class RCActionViewportSizeChange : RCAction
        {
            public RCActionViewportSizeChange(int width, int height, int x, int y, RendererCore core) { Width = width; Height = height; X = x; Y = y; Renderer = core;}

            public int X { get; private set; }
            public int Y { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            public RendererCore Renderer;

            public RCActionDelegate Action
            {
                get
                {
                    return delegate ()
                    {
                        Renderer.ViewportResize(X, Y, Width, Height);
                    };
                }
            }
        }

        private class RCActionDeleteGraphicsResource : RCAction
        {
            public RCActionDeleteGraphicsResource(GraphicsResource gr, RendererCore core){GraphicsResource = gr; Renderer = core;}

            public GraphicsResource GraphicsResource;

            public RendererCore Renderer;

            public RCActionDelegate Action
            { 
                get
                {
                    return delegate()
                    {
                        Renderer.DeleteGraphicsResource(GraphicsResource);
                    };
                } 
            }
        }

	public class RCActionResizeTexture : RCAction
        {
            public RCActionResizeTexture(Texture2D tex, RendererCore core, int width, int height){Texture = tex; Renderer = core; Width = width; Height = height;}

            public Texture2D Texture;
            public RendererCore Renderer;

            public int Width;
            public int Height;	    

            public RCActionDelegate Action
            {
 		get
                {
                    return delegate()
                    {
                        Texture.Resize(Width, Height);
                    };
		}
            }
        }

	private class RCActionCreateTexture2D : RCAction
        {
            private enum CreationParam
            {
                WithString = 0,
                WithBmpData = 1,
                WithPointer = 2
            }

            public RendererCore Renderer;
            public Texture2D Texture;

            public string Name;
            public bool GenerateMipMaps;
            public string Data;
            public System.Drawing.Bitmap BmpData;

            public int Width;
            public int Height;

            public IntPtr IntPtrData;

            public bool GenerateSampler;

            CreationParam Param;

            public bool Ready;

            public RCActionCreateTexture2D(RendererCore rc, string name, bool generateMipMaps, string data)
            {
                Renderer = rc;
                Name = name;
                GenerateMipMaps = generateMipMaps;
                Data = data;

                Param = CreationParam.WithString;

                Ready = false;
            }

            public RCActionCreateTexture2D(RendererCore rc, string name, bool generateMipMaps, System.Drawing.Bitmap data)
            {
                Renderer = rc;
                Name = name;
                BmpData = data;

                GenerateMipMaps = generateMipMaps;

                Param = CreationParam.WithBmpData;

                Ready = false;
            }

            public RCActionCreateTexture2D(RendererCore rc, string name, int width, int height, bool generateMipMaps, IntPtr data, bool generate_sampler)
            {
                Renderer = rc;
                Name = name;
                GenerateMipMaps = generateMipMaps;

                Width = width;
                Height = height;
                IntPtrData = data;
                GenerateSampler = generate_sampler;

                Param = CreationParam.WithPointer;

                Ready = false;
            }

            public RCActionDelegate Action
            { 
                get
                {
                    return delegate()
                    {
                        switch(Param)
                        {
                            case CreationParam.WithString:
                                Texture = Renderer.CreateTexture2D(Name, GenerateMipMaps, Data);
                                break;

                            case CreationParam.WithBmpData:
                                Texture = Renderer.CreateTexture2D(Name, GenerateMipMaps, BmpData);
                                break;

                            case CreationParam.WithPointer:
                                Texture = Renderer.CreateTexture2D(Name, Width, Height, GenerateMipMaps, IntPtrData, GenerateSampler);
                                break;
                        }

                        Ready = true;
                    };
                } 
            }
        }
        #endregion

        private ConcurrentQueue<RCAction> _RendererCoreActionsList;

        #endregion

        #region Public Members
        public BasicEffect BasicEffect { get; private set; }
        public Vector2i ViewportSize { get; private set; }
        #endregion

        #region Public Methods

        public RendererCore(Messaging.MessageProvider messageProvider)
        {
            ValidMessages = new int[] { (int)Messaging.MessageId.WindowResize };
            messageProvider += this;

            _RendererCoreActionsList = new ConcurrentQueue<RCAction>();
        }

        /// <summary>
        /// Inits RenderCore
        /// </summary>
        /// <param name="RenderContext"></param>
        /// <param name="SharedLoadingContext"></param>
        /// <returns>True or False</returns>
        public virtual bool Init(/*GlfwWindowPtr RenderContext, GlfwWindowPtr SharedLoadingContext*/FreezingArcher.Core.Application app)
        {
            Application = app;

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
            GenerateBuffer();

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

        public VertexBuffer CreateVertexBuffer(IntPtr data, int size, RendererBufferUsage rbu, string name)
        {
            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)size, data, (BufferUsageHint)rbu);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            VertexBuffer vb = new VertexBuffer(name, vbo, size, rbu);

            vb.VertexCount = size;

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
            ShaderProgram sp_Vertex = CreateShaderProgramFromFile("Internal_Basic_Effect_Vertex_Shader", ShaderType.VertexShader, "lib/Renderer/Effects/BasicEffect/VertexShader.vs");

            //Create PixelShader
            ShaderProgram sp_Pixel = CreateShaderProgramFromFile("Internal_Basic_Effect_Pixel_Sahder", ShaderType.PixelShader, "lib/Renderer/Effects/BasicEffect/PixelShader.ps");

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
            if (Application.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
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
                tex.Sampler.MagnificationFilter = MagnificationFilter.UseNearest;
                tex.Sampler.MinificationFilter = MinificationFilter.UseNearest;

                return tex;
            }
            else
            {
                RCActionCreateTexture2D ct2d = new RCActionCreateTexture2D(this, name, width, height, generateMipMaps, data, generate_sampler);
                this.AddRCActionJob(ct2d);

                while (!ct2d.Ready)
                    System.Threading.Thread.Sleep(1);

                return ct2d.Texture;
            }
                
            return null;
        }

        public Texture2D CreateTexture2D(string name, bool generateMipMaps, System.Drawing.Bitmap data)
        {
            if (Application.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
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
            }else
            {
                RCActionCreateTexture2D ct2d = new RCActionCreateTexture2D(this, name, generateMipMaps, data);
                this.AddRCActionJob(ct2d);

                while (!ct2d.Ready)
                    System.Threading.Thread.Sleep(1);

                return ct2d.Texture;
            }

            return null;
        }
            
        public Texture2D CreateTexture2D(string name, bool generateMipMaps, string data)
        {
            if (Application.ManagedThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                if (data != null)
                {
                    System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(data);
               
                    //bmp.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

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
            }
            else
            {
                RCActionCreateTexture2D ct2d = new RCActionCreateTexture2D(this, name, generateMipMaps, data);
                this.AddRCActionJob(ct2d);

                while (!ct2d.Ready)
                    System.Threading.Thread.Sleep(1);

                return ct2d.Texture;
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
            RCAction ac;

            while (_RendererCoreActionsList.TryDequeue(out ac))
            {
                ac.Action();
            }

            ErrorCode err_code = GL.GetError();

            if (err_code != ErrorCode.NoError)
                Logger.Log.AddLogEntry(LogLevel.Error, "RendererCore", FreezingArcher.Core.Status.DamnThreading, err_code.ToString()); 
        }

        public void End()
        {
            Begin();
        }

        public void EnableVertexAttribute(int location)
        {
            GL.EnableVertexAttribArray(location);
        }

        public void DisableVertexAttribute(int location)
        {
            GL.DisableVertexAttribArray(location);
        }

        public void VertexAttributePointer(VertexBufferLayoutKind vblk)
        {


            GL.VertexAttribPointer(vblk.AttributeID, vblk.AttributeSize, (VertexAttribPointerType)vblk.AttributeType, vblk.Normalized, vblk.Stride, (IntPtr)vblk.Offset);
        }

        public void VertexAttributeDivisor(int location, int divisor)
        {
            GL.VertexAttribDivisor(location, divisor);
        }

        public void CopyBuffer(GraphicsResource source, GraphicsResource dest, int size, int readoffset, int writeoffset)
        {
            GL.BindBuffer(BufferTarget.CopyReadBuffer, source.ID);
            GL.BindBuffer(BufferTarget.CopyReadBuffer, dest.ID);

            GL.CopyBufferSubData(BufferTarget.CopyReadBuffer, BufferTarget.CopyWriteBuffer, (IntPtr)readoffset,
                (IntPtr)writeoffset, (IntPtr)size);

            GL.BindBuffer(BufferTarget.CopyReadBuffer, 0);
            GL.BindBuffer(BufferTarget.CopyWriteBuffer, 0);
        }

        public void DrawArrays(int offset, int count, RendererBeginMode bm)
        {
            GL.DrawArrays((BeginMode)bm, offset, count);
        }

        public void DrawElements(int offset, int count, RendererIndexType det, RendererBeginMode bm)
        {
            GL.DrawElements((BeginMode)bm, count, (DrawElementsType)det, offset);
        }

        public void DrawElementsInstanced(int offset, int count, RendererIndexType det, RendererBeginMode bm, int primcount)
        {
            GL.DrawElementsInstanced((BeginMode)bm, count, (DrawElementsType)det, IntPtr.Zero, primcount);
        }

        public void DrawArraysInstanced(RendererBeginMode bm, int offset, int count, int primcount)
        {
            GL.DrawArraysInstanced((BeginMode)bm, offset, count, primcount);
        }

        private VertexBuffer _2DVertexBuffer;
        private VertexBuffer _2DVertexTextureOverride;
        internal VertexBufferArray _2DVertexBufferArray;

        private unsafe void GenerateBuffer(/*FreezingArcher.Math.Color4 color*/)
        {
            Vertex2D[] v2d = new Vertex2D[9];

            v2d[0] = new Vertex2D(new Vector3(0.0f, 0.0f, -5.0f), Vector4.Zero, new Vector2(0.0f, 1.0f));
            v2d[1] = new Vertex2D(new Vector3(1.0f, 0.0f, -5.0f), Vector4.Zero, new Vector2(1.0f, 1.0f));
            v2d[2] = new Vertex2D(new Vector3(0.0f, 1.0f, -5.0f), Vector4.Zero, new Vector2(0.0f, 0.0f));
            v2d[3] = new Vertex2D(new Vector3(0.0f, 1.0f, -5f), Vector4.Zero, new Vector2(0.0f, 0.0f));
            v2d[4] = new Vertex2D(new Vector3(1.0f, 0.0f, -5f), Vector4.Zero, new Vector2(1.0f, 1.0f));
            v2d[5] = new Vertex2D(new Vector3(1.0f, 1.0f, -5f), Vector4.Zero, new Vector2(1.0f, 0.0f));

            v2d[6] = new Vertex2D(new Vector3(0.0f, 0.0f, -5f), Vector4.Zero, new Vector2(0.0f, 0.0f));
            v2d[7] = new Vertex2D(new Vector3(-1.0f, -1.0f, -5f), Vector4.Zero, new Vector2(0.0f, 0.0f));
            v2d[8] = new Vertex2D(new Vector3(1.0f, -1.0f, -5f), Vector4.Zero, new Vector2(0.0f, 0.0f));

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

            Vector2[] tex_override = new Vector2[6];
            for (int i = 0; i < 6; i++)
                tex_override[i] = new Vector2(1, 1);

            _2DVertexBuffer = CreateVertexBuffer<Vertex2D>(v2d, Vertex2D.SIZE * 9, RendererBufferUsage.StaticDraw, "Internal2DVertexBuffer");
            _2DVertexTextureOverride = CreateVertexBuffer<Vector2>(tex_override, Vector2.SizeInBytes * 6, RendererBufferUsage.StaticDraw, "Internal2DVertexTextureOverride");

            _2DVertexBufferArray = CreateVertexBufferArray(_2DVertexBuffer, vblk, "Internal2DVertexBufferArray");

            VertexBufferLayoutKind[] vblk2 = new VertexBufferLayoutKind[1];
            vblk2[0].AttributeID = 11;
            vblk2[0].AttributeSize = 2;
            vblk2[0].AttributeType = RendererVertexAttribType.Float;
            vblk2[0].Normalized = false;
            vblk2[0].Offset = 0;
            vblk2[0].Stride = Vector2.SizeInBytes;

            _2DVertexBufferArray.AddVertexBuffer(vblk2, _2DVertexTextureOverride);
        }

        private void DeleteResources()
        {
            DeleteGraphicsResource(_2DVertexBufferArray);
            DeleteGraphicsResource(_2DVertexBuffer);
            DeleteGraphicsResource(_2DVertexTextureOverride);
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
                _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("DrawColor"), new Vector4(color.R, color.G, color.B, color.A));

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

            if (spr.Texture == null)
                return;

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

            if(!spr.CustomEffect)
                _2DEffect.BindPipeline();

            DrawArrays(0, 6, RendererBeginMode.Triangles);

            if(!spr.CustomEffect)
                _2DVertexBuffer.UnbindBuffer();

            spr.Texture.Unbind();

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }

        private void DrawFilledRectangle(ref Vector2 position, ref Vector2 size, ref Vector2 screen_size, ref FreezingArcher.Math.Color4 color, int count = 1, bool relative = false)
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
            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("DrawColor"), new Vector4(color.R, color.G, color.B, color.A));

            if (count > 1)
            {
                _2DEffect.VertexProgram.SetUniform(_2DEffect.VertexProgram.GetUniformLocation("InstancedDrawing"), 1);
                _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("InstancedDrawing"), 1);

                _2DEffect.BindPipeline();

                DrawArraysInstanced(RendererBeginMode.Triangles, 0, 6, count);
            }
            else
            {
                _2DEffect.VertexProgram.SetUniform(_2DEffect.VertexProgram.GetUniformLocation("InstancedDrawing"), 0);
                _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("InstancedDrawing"), 0);

                _2DEffect.BindPipeline();

                DrawArrays(0, 6, RendererBeginMode.Triangles);
            }

            _2DVertexBufferArray.UnbindVertexBufferArray();

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
            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("DrawColor"), new Vector4(color.R, color.G, color.B, color.A));

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
            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("DrawColor"), new Vector4(color.R, color.G, color.B, color.A));

            _2DEffect.BindPipeline();

            DrawArrays(0, 1, RendererBeginMode.Points);

            _2DUniformBuffer.UnbindBuffer();

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }

        private void DrawTexturedRectangle(ref Vector2 position, ref Vector2 size, 
            Vector2[] tex_override, ref Vector2 screen_size, int count = 1, bool relative = false, bool no_blend = false)
        {
            GL.Enable(EnableCap.Blend);
            if(!no_blend)
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Disable(EnableCap.DepthTest);

            MatricesBlock2D md = new MatricesBlock2D();
            md.World = Matrix.CreateScale(size.X, size.Y, 1.0f) * Matrix.CreateTranslation(new Vector3(position.X, position.Y, 0.0f));

            md.Projection = Matrix.CreateOrthographicOffCenter(0.0f, screen_size.X, screen_size.Y, 0.0f, 1.0f, 100.0f);

            _2DVertexTextureOverride.UpdateBuffer<Vector2>(tex_override, tex_override.Length * Vector2.SizeInBytes);
            _2DUniformBuffer.UpdateBuffer<MatricesBlock2D>(md, sizeof(float) * 4 * 4 * 2);

            _2DVertexBufferArray.BindVertexBufferArray();

            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("UseTexture"), 1.0f);
            _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("Texture1"), 0);

            _2DEffect.VertexProgram.SetUniform(_2DEffect.VertexProgram.GetUniformLocation("OverrideTextureCoords"), 1);

            if (count > 1)
            {
                _2DEffect.VertexProgram.SetUniform(_2DEffect.VertexProgram.GetUniformLocation("InstancedDrawing"), 1);
                _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("InstancedDrawing"), 1);

                _2DEffect.BindPipeline();

                DrawArraysInstanced(RendererBeginMode.Triangles, 0, 6, count);
            }
            else
            {
                _2DEffect.VertexProgram.SetUniform(_2DEffect.VertexProgram.GetUniformLocation("InstancedDrawing"), 0);
                _2DEffect.PixelProgram.SetUniform(_2DEffect.PixelProgram.GetUniformLocation("InstancedDrawing"), 0);

                _2DEffect.BindPipeline();

                DrawArrays(0, 6, RendererBeginMode.Triangles);
            }

            _2DVertexBufferArray.UnbindVertexBufferArray();

            _2DUniformBuffer.UnbindBuffer();

            _2DEffect.VertexProgram.SetUniform(_2DEffect.VertexProgram.GetUniformLocation("OverrideTextureCoords"), 0);

            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Blend);
        }


        public void DrawRectangleAbsolute(ref Vector2 position, ref Vector2 size, float line_width, ref FreezingArcher.Math.Color4 color, int count = 1)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(ViewportSize.X, ViewportSize.Y);

            DrawRectangle(ref position, ref size, ref vs, line_width, ref color);
        }

        public void DrawRectangleRelative(ref Vector2 position, ref Vector2 size, ref FreezingArcher.Math.Color4 color, float line_width, int count = 1)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(1.0f, 1.0f);

            DrawRectangle(ref position, ref size, ref vs, line_width, ref color, true);
        }

        public void DrawFilledRectangleAbsolute(ref Vector2 position, ref Vector2 size, ref FreezingArcher.Math.Color4 color, int count = 1)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(ViewportSize.X, ViewportSize.Y);

            DrawFilledRectangle(ref position, ref size, ref vs, ref color, count);
        }

        public void DrawTexturedRectangleAbsolute(Texture2D tex, ref Vector2 position, ref Vector2 size, Vector2[] texcoords, int count = 1, bool no_blend = false)
        {
            Vector2 vs = new Vector2(ViewportSize.X, ViewportSize.Y);

            tex.Bind(0);

            DrawTexturedRectangle(ref position, ref size, texcoords, ref vs, count, no_blend:no_blend);

            tex.Unbind();
        }

        public void DrawTexturedRectangleRelative(Texture2D tex, ref Vector2 position, ref Vector2 size, Vector2[] texcoords, int count = 1, bool no_blend = false)
        {
            Vector2 vs = new Vector2(1.0f, 1.0f);

            tex.Bind(0);

            DrawTexturedRectangle(ref position, ref size, texcoords, ref vs, count, no_blend);

            tex.Unbind();
        }

        public void DrawSpriteAbsolute(Sprite spr)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(ViewportSize.X, ViewportSize.Y);

            DrawSprite(ref vs, spr);
        }

        public void DrawFilledRectangleRelative(ref Vector2 position, ref Vector2 size, ref FreezingArcher.Math.Color4 color, int count)
        {
            Vector2 vs = new FreezingArcher.Math.Vector2(1.0f, 1.0f);

            DrawFilledRectangle(ref position, ref size, ref vs, ref color, count, true);
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

        public void DeleteGraphicsResourceAsync(GraphicsResource gr)
        {
            _RendererCoreActionsList.Enqueue(new RCActionDeleteGraphicsResource(gr, this));
        }

        public void AddRCActionJob(RCAction act)
        {
            _RendererCoreActionsList.Enqueue(act);
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



        public int GetBlendSrc()
        {
            int param = 0;

            GL.GetInteger(GetPName.BlendSrc, out param);

            return param; 
        }

        public int GetBlendDst()
        {
            int param = 0;

            GL.GetInteger(GetPName.BlendDst, out param);

            return param;
        }

        public bool IsEnabled(RendererEnableCap cap)
        {
            return GL.IsEnabled((EnableCap)cap);
        }

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

        public void SetBlendFunc(RendererBlendingFactorSrc src, RendererBlendingFactorDest dst)
        {
            GL.BlendFunc((BlendingFactorSrc)src, (BlendingFactorDest)dst);
        }
            
        public void Enable(RendererEnableCap cap)
        {
            GL.Enable((EnableCap)cap);
        }

        public void Disable(RendererEnableCap cap)
        {
            GL.Disable((EnableCap)cap);
        }

        public void Clear(FreezingArcher.Math.Color4 color)
        {
            GL.ClearColor(new Pencil.Gaming.Graphics.Color4(color.R, color.G, color.B, color.A));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
        }

        public void Clear(FreezingArcher.Math.Color4 color, int attachment)
        {
            GL.ColorMask((uint)attachment, true, true, true, true);

            GL.ClearColor(new Pencil.Gaming.Graphics.Color4(color.R, color.G, color.B, color.A));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.ColorMask(true, true, true, true);
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
                _RendererCoreActionsList.Enqueue(new RCActionViewportSizeChange(wrm.Width, wrm.Height, 0, 0, this));
            }
        }

        public int[] ValidMessages
        {
            get;
            private set;
        }
    }
}
