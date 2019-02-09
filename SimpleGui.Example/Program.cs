using DEngine.Render;
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
        private static Pipeline _pipeline;
        
        private static Gui gui;
        private static Text text;
        private static Control control;
        private static Button button;
        private static TextBox textBox;
        private static Image image;
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
                WindowTitle = "SimpleGui Example"
            };
            Sdl2Window window = VeldridStartup.CreateWindow(ref windowCI);

            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(window);

            gui = new Gui(window, _graphicsDevice);

            control = new Control()
            {
                Size = new Vector2(500, 500),
                Position = new Vector2(5, 5),
                ColorType = ControlColorType.Form,
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

            image = new Image("gui/Test.png")
            {
                Size = new Vector2(150, 150),
                Position = new Vector2(5, 160),
            };
            image.Initialize();
            control.AddChild(image);
            
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

                if (window.Exists)
                {
                    gui.Update(snap);

                    Draw();
                    Thread.Sleep(1);
                }
            }

            DisposeResources();
        }

        private static void CreateResources()
        {
            ResourceFactory factory = _graphicsDevice.ResourceFactory;

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
                vertexLayouts: new VertexLayoutDescription[] { Gui.ColorShader.Layout },
                shaders: new Shader[] { Gui.ColorShader.VertexShader, Gui.ColorShader.FragmentShader });
            pipelineDescription.Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            _commandList = factory.CreateCommandList();
        }

        private static void Draw()
        {
            // Begin() must be called before commands can be issued.
            _commandList.Begin();

            // We want to render directly to the output window.
            _commandList.SetFramebuffer(_graphicsDevice.SwapchainFramebuffer);
            _commandList.SetFullViewports();
            _commandList.ClearColorTarget(0, RgbaFloat.Black);
            _commandList.SetPipeline(_pipeline);
            _commandList.End();
            _graphicsDevice.SubmitCommands(_commandList);
            
            gui.Draw();

            // Once commands have been submitted, the rendered image can be presented to the application window.
            _graphicsDevice.SwapBuffers();
        }

        private static void DisposeResources()
        {
            gui.Dispose();

            _pipeline.Dispose();
            _commandList.Dispose();
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
