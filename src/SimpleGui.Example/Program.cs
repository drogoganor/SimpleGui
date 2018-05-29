using SixLabors.Fonts;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace SimpleGui.Example
{
    class Program
    {
        private static GraphicsDevice _graphicsDevice;
        private static CommandList _commandList;
        private static DeviceBuffer _vertexBuffer;
        private static DeviceBuffer _indexBuffer;
        private static Shader _vertexShader;
        private static Shader _fragmentShader;
        private static Pipeline _pipeline;
        
        private static Gui gui;
        private static Text text;
        private static Control control;
        private static Button button;
        private static TextBox textBox;
        private static ImageColorSample image;
        private static ColoredQuad colorQuad;
        private static ListBox listBox;
        private static Checkbox checkbox;

        private static ColorPicker colorPicker;

        static void Main(string[] args)
        {
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "SimpleGui Tutorial"
            };
            Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window);

            gui = new Gui(window, _graphicsDevice);

            control = new Control()
            {
                Size = new Vector2(500, 500),
                Position = new Vector2(5, 5),
                IsHoverable = false,
                IsClickable = false
            };
            control.Initialize();
            control.SetCenterScreen();
            gui.SceneGraph.Root.AddChild(control);

            text = new Text("Text")
            {
                Position = new Vector2(5, 5),
                Size = new Vector2(100, 34),
            };
            text.Initialize();
            control.AddChild(text);

            button = new Button("Button")
            {
                Size = new Vector2(100, 34),
                Position = new Vector2(5, 40),
            };
            button.Initialize();
            control.AddChild(button);
            button.MouseUp = () =>
            {
                text.Content = "Hello";
                text.Recreate();

                colorPicker = new ColorPicker()
                {

                };
                colorPicker.Initialize();
                colorPicker.SetCenterScreen();
                gui.SceneGraph.Root.AddChild(colorPicker);

                colorPicker.Closed = () =>
                {
                    colorQuad.Color = colorPicker.SelectedColor;
                    colorQuad.Recreate();

                    gui.SceneGraph.Root.RemoveChild(colorPicker);
                    colorPicker.Dispose();
                };
            };

            textBox = new TextBox()
            {
                Size = new Vector2(160, 34),
                Position = new Vector2(5, 80),
                Text = "TextBox"
            };
            textBox.Initialize();
            control.AddChild(textBox);

            checkbox = new Checkbox()
            {
                Size = new Vector2(120, 24),
                Position = new Vector2(5, 120),
                Text = "Checkbox"
            };
            checkbox.Initialize();
            control.AddChild(checkbox);

            image = new ImageColorSample("gui/color.png")
            {
                Size = new Vector2(320, 240),
                Position = new Vector2(5, 240),
            };
            image.Initialize();
            control.AddChild(image);
            image.ColorSampled = () =>
            {
                colorQuad.Color = image.SelectedColor;
                colorQuad.Recreate();
            };

            colorQuad = new ColoredQuad()
            {
                Size = new Vector2(20, 20),
                Position = new Vector2(330, 240),
                Color = Color.Yellow,
            };
            colorQuad.Initialize();
            control.AddChild(colorQuad);

            listBox = new ListBox()
            {
                Size = new Vector2(120, 120),
                Position = new Vector2(180, 5),
            };
            listBox.Initialize();
            listBox.AddItem("ListBoxItem 1");
            listBox.AddItem("ListBoxItem 2");
            listBox.AddItem("ListBoxItem 3");
            control.AddChild(listBox);

            CreateResources();

            while (window.Exists)
            {
                var snap = window.PumpEvents();
                InputTracker.UpdateFrameInput(snap);

                if (window.Exists)
                {
                    gui.Update();

                    Draw();
                    Thread.Sleep(1);
                }
            }

            DisposeResources();
        }

        private static void CreateResources()
        {
            ResourceFactory factory = _graphicsDevice.ResourceFactory;

            VertexPositionColor[] quadVertices =
            {
                new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Yellow),
                new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Green)
            };
            BufferDescription vbDescription = new BufferDescription(
                4 * VertexPositionColor.SizeInBytes,
                BufferUsage.VertexBuffer);
            _vertexBuffer = factory.CreateBuffer(vbDescription);
            _graphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);

            ushort[] quadIndices = { 0, 1, 2, 3 };
            BufferDescription ibDescription = new BufferDescription(
                4 * sizeof(ushort),
                BufferUsage.IndexBuffer);
            _indexBuffer = factory.CreateBuffer(ibDescription);
            _graphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4));

            _vertexShader = LoadShader(ShaderStages.Vertex);
            _fragmentShader = LoadShader(ShaderStages.Fragment);

            // Create pipeline
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(
                depthTestEnabled: true,
                depthWriteEnabled: true,
                comparisonKind: ComparisonKind.LessEqual);
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: new Shader[] { _vertexShader, _fragmentShader });
            pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            _commandList = factory.CreateCommandList();
        }

        private static Shader LoadShader(ShaderStages stage)
        {
            string extension = null;
            switch (_graphicsDevice.BackendType)
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
            string path = Path.Combine(System.AppContext.BaseDirectory, "Shaders", $"{stage.ToString()}.{extension}");
            byte[] shaderBytes = File.ReadAllBytes(path);
            return _graphicsDevice.ResourceFactory.CreateShader(new ShaderDescription(stage, shaderBytes, entryPoint));
        }

        private static void Draw()
        {
            // Begin() must be called before commands can be issued.
            _commandList.Begin();

            // We want to render directly to the output window.
            _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
            _commandList.SetFullViewports();
            _commandList.ClearColorTarget(0, RgbaFloat.Black);

            // Set all relevant state to draw our quad.
            _commandList.SetVertexBuffer(0, _vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _commandList.SetPipeline(_pipeline);
            // Issue a Draw command for a single instance with 4 indices.
            _commandList.DrawIndexed(
                indexCount: 4,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

            // End() must be called before commands can be submitted for execution.
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);
            
            gui.Draw();

            // Once commands have been submitted, the rendered image can be presented to the application window.
            _graphicsDevice.SwapBuffers();
        }

        private static void DisposeResources()
        {
            _pipeline.Dispose();
            _vertexShader.Dispose();
            _fragmentShader.Dispose();
            _commandList.Dispose();
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
            _graphicsDevice.Dispose();
        }
    }

    struct VertexPositionColor
    {
        public const uint SizeInBytes = 24;
        public Vector2 Position;
        public RgbaFloat Color;
        public VertexPositionColor(Vector2 position, RgbaFloat color)
        {
            Position = position;
            Color = color;
        }
    }
}
