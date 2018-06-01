using DEngine.Render;
using SimpleGui.Scene;
using System;
using System.IO;
using System.Numerics;
using TextRender;
using Veldrid;
using Veldrid.Sdl2;

namespace SimpleGui
{
    public class Gui : IDisposable
    {
        public SceneGraph SceneGraph { get; set; } = new SceneGraph();

        public static Sdl2Window Window { get; protected set; }
        public static GraphicsDevice Device { get; protected set; }
        public static ResourceFactory Factory { get; protected set; }
        public static CommandList CommandList;
        public static Pipeline Pipeline;
        public static Pipeline TexturePipeline;

        internal static ColorShader ColorShader;
        internal static TextureShader TextureShader;

        public static GuiSettings Settings { get; set; }

        public static TextRenderer TextRenderer { get; protected set; }
        
        public Gui(Sdl2Window window, GraphicsDevice device)
        {
            LoadSettings();

            Window = window;
            Device = device;
            Factory = device.ResourceFactory;

            TextRenderer = new TextRenderer(Device);
            
            ColorShader = new ColorShader(Factory);
            TextureShader = new TextureShader(Factory);
            
            // Create pipeline
            var pipelineDescription = new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                new DepthStencilStateDescription(
                    depthTestEnabled: true,
                    depthWriteEnabled: true,
                    comparisonKind: ComparisonKind.LessEqual),
                new RasterizerStateDescription(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false),
                PrimitiveTopology.TriangleStrip,
                new ShaderSetDescription(
                    vertexLayouts: new VertexLayoutDescription[] { ColorShader.Layout },
                    shaders: new Shader[] { ColorShader.VertexShader, ColorShader.FragmentShader }),
                new[] { ColorShader.ResourceLayout },
                Device.SwapchainFramebuffer.OutputDescription
                );

            Pipeline = Factory.CreateGraphicsPipeline(pipelineDescription);


            var texturePipelineDesc = new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                new DepthStencilStateDescription(
                    depthTestEnabled: true,
                    depthWriteEnabled: true,
                    comparisonKind: ComparisonKind.LessEqual),
                new RasterizerStateDescription(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false),
                PrimitiveTopology.TriangleStrip,
                new ShaderSetDescription(
                    vertexLayouts: new VertexLayoutDescription[] { TextureShader.Layout },
                    shaders: new Shader[] { TextureShader.VertexShader, TextureShader.FragmentShader }),
                new[] { TextureShader.ProjViewLayout, TextureShader.TextureLayout },
                Device.SwapchainFramebuffer.OutputDescription
                );
            
            TexturePipeline = Factory.CreateGraphicsPipeline(texturePipelineDesc);
            CommandList = Factory.CreateCommandList();
        }
        
        public void Dispose()
        {
            if (SceneGraph != null)
            {
                SceneGraph.DisposeAll();
                SceneGraph = null;
            }

            if (TextRenderer != null)
            {
                TextRenderer.Dispose();
                TextRenderer = null;
            }

            if (CommandList != null)
            {
                CommandList.Dispose();
                CommandList = null;
            }

            if (TexturePipeline != null)
            {
                TexturePipeline.Dispose();
                TexturePipeline = null;
            }

            if (Pipeline != null)
            {
                Pipeline.Dispose();
                Pipeline = null;
            }

            if (TextureShader != null)
            {
                TextureShader.Dispose();
                TextureShader = null;
            }

            if (ColorShader != null)
            {
                ColorShader.Dispose();
                ColorShader = null;
            }

        }

        private Shader LoadShader(ShaderStages stage)
        {
            string extension = null;
            switch (Device.BackendType)
            {
                case GraphicsBackend.Direct3D11:
                    extension = "hlsl.bytes";
                    break;
                case GraphicsBackend.Vulkan:
                    extension = "spv";
                    break;
                case GraphicsBackend.OpenGL:
                    extension = "glsl";
                    break;
                case GraphicsBackend.Metal:
                    extension = "metallib";
                    break;
                default: throw new System.InvalidOperationException();
            }

            string entryPoint = stage == ShaderStages.Vertex ? "VS" : "FS";
            string path = Path.Combine(System.AppContext.BaseDirectory, "Shaders", $"Vertex-{stage.ToString().ToLowerInvariant()}.{extension}");
            byte[] shaderBytes = File.ReadAllBytes(path);
            return Factory.CreateShader(new ShaderDescription(stage, shaderBytes, entryPoint));
        }


        protected void LoadSettings()
        {
            var filename = "gui.json";
            if (File.Exists(filename))
            {
                try
                {
                    Settings = Util.LoadFromJsonFile<GuiSettings>(filename);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (Settings == null)
            {
                Settings = new GuiSettings();
            }

            // Debug save
            Settings.SaveToJsonFile(filename);


            // Copy default theme to default settings
            var defaultTheme = Settings.Themes[Settings.Theme];
            Settings.DefaultControlSettings.Colors = defaultTheme.Copy();
        }


        public void Update()
        {
            SceneGraph.Update();
        }

        public static void BeginDraw()
        {
            // Begin() must be called before commands can be issued.
            CommandList.Begin();

            // We want to render directly to the output window.
            CommandList.SetFramebuffer(Device.SwapchainFramebuffer);
            CommandList.SetFullViewports();
            CommandList.UpdateBuffer(ColorShader.ProjectionBuffer, 0, Matrix4x4.CreateOrthographicOffCenter(0, Device.SwapchainFramebuffer.Width, Device.SwapchainFramebuffer.Height, 0, 0, 1));
            CommandList.UpdateBuffer(ColorShader.WorldBuffer, 0, Matrix4x4.CreateTranslation(Vector3.Zero));
            CommandList.SetPipeline(Pipeline);
            CommandList.SetGraphicsResourceSet(0, ColorShader.ResourceSet);
        }

        public static void EndDraw()
        {
            // End() must be called before commands can be submitted for execution.
            CommandList.End();
            Device.SubmitCommands(CommandList);
        }

        public void Draw()
        {
            SceneGraph.Draw();
        }


        public static Vector2 GetCenterScreenPosFor(Vector2 size)
        {
            var width = Device.SwapchainFramebuffer.Width;
            var height = Device.SwapchainFramebuffer.Height;

            var screenSize = new Vector2(width, height);
            return (screenSize - size) / 2f;
        }
    }
}
