using SimpleGui;
using Veldrid;

namespace DEngine.Render
{
    public class TextShader : ShaderAbstract
    {
        public DeviceBuffer ProjectionBuffer;
        public DeviceBuffer WorldBuffer;
        public ResourceLayout ProjViewLayout;
        public ResourceLayout TextureLayout;

        public ResourceSet ProjViewSet;

        public TextShader(ResourceFactory factory) : base(factory, "Text")
        {
            Layout = new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                        new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

            ProjectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            WorldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            ProjViewLayout = AddDisposable(factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex))
                    ));

            TextureLayout = AddDisposable(factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SourceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SourceSampler", ResourceKind.Sampler, ShaderStages.Fragment))));

            ProjViewSet = AddDisposable(factory.CreateResourceSet(new ResourceSetDescription(
                ProjViewLayout,
                ProjectionBuffer,
                WorldBuffer)));
        }

        public void UpdateBuffers()
        {

        }
    }
}
