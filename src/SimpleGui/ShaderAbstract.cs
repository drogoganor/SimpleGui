using OpenSage;
using System.IO;
using System.Runtime.InteropServices;
using Veldrid;

namespace SimpleGui
{
    internal class ShaderAbstract : DisposableBase
    {
        public string Name { get; protected set; }
        public Shader VertexShader { get; protected set; }
        public Shader FragmentShader { get; protected set; }
        public VertexLayoutDescription Layout { get; protected set; }

        public ShaderAbstract(ResourceFactory factory, string name)
        {
            Name = name;

            VertexShader = AddDisposable(LoadShader(factory, ShaderStages.Vertex, "VS"));
            FragmentShader = AddDisposable(LoadShader(factory, ShaderStages.Fragment, "FS"));
        }
        
        protected Stream OpenEmbeddedAssetStream(string name) => GetType().Assembly.GetManifestResourceStream(name);

        protected Shader LoadShader(ResourceFactory factory, ShaderStages stage, string entryPoint)
        {
            string name = $"SimpleGui.Resources.{Name}-{stage.ToString().ToLower()}.{GetExtension(factory.BackendType)}";
            return factory.CreateShader(new ShaderDescription(stage, ReadEmbeddedAssetBytes(name), entryPoint));
        }

        protected byte[] ReadEmbeddedAssetBytes(string name)
        {
            using (Stream stream = OpenEmbeddedAssetStream(name))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }

        private static string GetExtension(GraphicsBackend backendType)
        {
            bool isMacOS = RuntimeInformation.OSDescription.Contains("Darwin");

            return (backendType == GraphicsBackend.Direct3D11)
                ? "hlsl.bytes"
                : (backendType == GraphicsBackend.Vulkan)
                    ? "450.glsl.spv"
                    : (backendType == GraphicsBackend.Metal)
                        ? isMacOS ? "metallib" : "ios.metallib"
                        : (backendType == GraphicsBackend.OpenGL)
                            ? "330.glsl"
                            : "300.glsles";
        }
    }
}
