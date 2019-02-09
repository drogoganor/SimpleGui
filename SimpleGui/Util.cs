using Newtonsoft.Json;
using System.IO;
using System.Numerics;

namespace SimpleGui
{
    internal static class Util
    {
        public static Vector4 ToVector4(this System.Drawing.Color color)
        {
            return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }
    }
}
