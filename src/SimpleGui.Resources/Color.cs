using ShaderGen;
using System.Numerics;
using static ShaderGen.ShaderBuiltins;

[assembly: ShaderSet("Color", "SimpleGui.Resources.Color.VS", "SimpleGui.Resources.Color.FS")]

namespace SimpleGui.Resources
{
    public class Color
    {
        [ResourceSet(0)]
        public Matrix4x4 Projection;
        [ResourceSet(0)]
        public Vector4 World;

        [VertexShader]
        public FragmentInput VS(VertexInput input)
        {
            FragmentInput output;
            var worldPosition = Mul(Projection, new Vector4(input.Position, 0, 1) + World);
            output.SystemPosition = worldPosition;
            output.Color = input.Color;

            return output;
        }

        [FragmentShader]
        public Vector4 FS(FragmentInput input)
        {
            return input.Color;
        }

        public struct VertexInput
        {
            [PositionSemantic] public Vector2 Position;
            [ColorSemantic] public Vector4 Color;
        }

        public struct FragmentInput
        {
            [SystemPositionSemantic] public Vector4 SystemPosition;
            [ColorSemantic] public Vector4 Color;
        }
    }
}
