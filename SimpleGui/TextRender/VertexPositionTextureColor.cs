using System.Numerics;
using Veldrid;

namespace SimpleGui.TextRender
{
    internal struct VertexPositionTextureColor
    {
        public const uint SizeInBytes = 32;
        public Vector2 Position;
        public Vector2 TexCoords;
        public Vector4 Color;
        public VertexPositionTextureColor(Vector2 position, Vector2 tex, RgbaFloat color)
        {
            Position = position;
            TexCoords = tex;
            Color = color.ToVector4();
        }
    }
}
