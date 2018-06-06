using ShaderGen;
using System.Numerics;
using static ShaderGen.ShaderBuiltins;

[assembly: ShaderSet("Tex", "SimpleGui.Resources.Tex.VS", "SimpleGui.Resources.Tex.FS")]

namespace SimpleGui.Resources
{
    public class Tex
    {
        [ResourceSet(0)]
        public Matrix4x4 Projection;
        [ResourceSet(0)]
        public Matrix4x4 World;
        [ResourceSet(1)]
        public Texture2DResource SurfaceTexture;
        [ResourceSet(1)]
        public SamplerResource SurfaceSampler;

        [VertexShader]
        public FragmentInput VS(VertexInput input)
        {
            FragmentInput output;
            var outPos = Mul(World, new Vector4(input.Position, 0, 1));
            outPos = Mul(Projection, outPos);
            output.SystemPosition = outPos;
            output.Color = input.Color;
            output.TexCoords = input.TexCoords;

            return output;
        }

        [FragmentShader]
        public Vector4 FS(FragmentInput input)
        {
            return Sample(SurfaceTexture, SurfaceSampler, input.TexCoords);
        }

        public struct VertexInput
        {
            [PositionSemantic] public Vector2 Position;
            [TextureCoordinateSemantic] public Vector2 TexCoords;
            [ColorSemantic] public Vector4 Color;
        }

        public struct FragmentInput
        {
            [SystemPositionSemantic] public Vector4 SystemPosition;
            [TextureCoordinateSemantic] public Vector2 TexCoords;
            [ColorSemantic] public Vector4 Color;
        }
    }
}
