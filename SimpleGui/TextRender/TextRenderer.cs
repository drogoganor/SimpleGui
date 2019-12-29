using System.Numerics;
using DEngine.Render;
using OpenSage;
using Veldrid;

namespace SimpleGui.TextRender
{
    public class TextRenderer : DisposableBase
    {
        internal GraphicsDevice Device { get; private set; }
        internal TextShader Shader { get; private set; }
        internal CommandList CommandList { get; private set; }

        private Pipeline _pipeline;
        
        public TextRenderer(GraphicsDevice device)
        {
            Device = device;
            var factory = device.ResourceFactory;

            Shader = AddDisposable(new TextShader(factory));

            // Create pipeline
            var pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleAlphaBlend;
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
            pipelineDescription.ResourceLayouts = new[] { Shader.ProjViewLayout, Shader.TextureLayout };
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { Shader.Layout },
                shaders: new Shader[] { Shader.VertexShader, Shader.FragmentShader });
            pipelineDescription.Outputs = Device.SwapchainFramebuffer.OutputDescription;

            _pipeline = AddDisposable(factory.CreateGraphicsPipeline(pipelineDescription));
            CommandList = AddDisposable(factory.CreateCommandList());
        }
        
        public void BeginDraw()
        {
            CommandList.Begin();
            CommandList.SetFramebuffer(Device.SwapchainFramebuffer);
            CommandList.SetFullViewports();
            CommandList.SetPipeline(_pipeline);
            CommandList.UpdateBuffer(Shader.ProjectionBuffer, 0, Matrix4x4.CreateOrthographicOffCenter(0, Device.SwapchainFramebuffer.Width, Device.SwapchainFramebuffer.Height, 0, 0, 1));
        }
        
        public void EndDraw()
        {
            CommandList.End();
            Device.SubmitCommands(CommandList);
        }
    }
}
