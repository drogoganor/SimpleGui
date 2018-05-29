using SimpleGui;
using Veldrid;

namespace DEngine.Render
{
    internal class ColorShader : ShaderAbstract
    {
        public DeviceBuffer ProjectionBuffer;
        public DeviceBuffer WorldBuffer;
        public ResourceSet ResourceSet;
        public ResourceLayout ResourceLayout;

        public ColorShader(ResourceFactory factory) : base(factory, "Color")
        {
            Layout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.Position, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.Color, VertexElementFormat.Float4));

            ProjectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            WorldBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));

            ResourceLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("World", ResourceKind.UniformBuffer, ShaderStages.Vertex))
                    );

            ResourceSet = factory.CreateResourceSet(new ResourceSetDescription(
                ResourceLayout,
                ProjectionBuffer,
                WorldBuffer));
        }
    }
}
