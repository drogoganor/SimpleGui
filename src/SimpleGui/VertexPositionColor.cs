using System.Numerics;
using Veldrid;

namespace SimpleGui
{
    public struct VertexPositionColor
    {
        public const uint SizeInBytes = 24;
        public Vector2 Position;
        public Vector4 Color;
        public VertexPositionColor(Vector2 position, Vector4 color)
        {
            Position = position;
            Color = color;
        }
        public VertexPositionColor(Vector2 position, HexColor color)
        {
            Position = position;
            Color = color.Vector4;
        }
    }

    public struct VertexPositionTextureColor
    {
        public const uint SizeInBytes = 32;
        public Vector2 Position;
        public Vector2 TexCoords;
        public Vector4 Color;
        public VertexPositionTextureColor(Vector2 position, Vector2 tex, Vector4 color)
        {
            Position = position;
            TexCoords = tex;
            Color = color;
        }
        public VertexPositionTextureColor(Vector2 position, Vector2 tex, HexColor color)
        {
            Position = position;
            TexCoords = tex;
            Color = color.Vector4;
        }
    }
}
