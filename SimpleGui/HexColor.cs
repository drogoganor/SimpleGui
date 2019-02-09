using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Numerics;
using System.Xml.Serialization;
using Veldrid;

namespace SimpleGui
{
    [Serializable]
    public class HexColor
    {
        public string Value { get; set; } = "ffffffff";
        
        [JsonIgnore]
        [YamlIgnore]
        public Color Color
        {
            get
            {
                return ColorTranslator.FromHtml("#" + Value);
            }
            set
            {
                Value = ColorTranslator.ToHtml(value).Substring(1);
            }
        }

        [JsonIgnore]
        [YamlIgnore]
        public Vector4 Vector4
        {
            get
            {
                var col = Color;
                return new Vector4(
                    col.R / 255f,
                    col.G / 255f,
                    col.B / 255f,
                    col.A / 255f
                    );
            }
            set
            {
                var color = Color.FromArgb(
                    (int)(value.W * 255f),
                    (int)(value.X * 255f),
                    (int)(value.Y * 255f),
                    (int)(value.Z * 255f)
                    );
                Value = color.Name;
            }
        }

        public HexColor()
        {

        }

        public HexColor(string value)
        {
            Value = value;
        }

        public HexColor(Color value)
        {
            Color = value;
        }

        public HexColor(Vector4 value)
        {
            Vector4 = value;
        }

        public HexColor(RgbaFloat value)
        {
            Vector4 = value.ToVector4();
        }


        public static explicit operator HexColor(Color c)
        {
            return new HexColor(c);
        }

        public static explicit operator HexColor(Vector4 c)
        {
            return new HexColor(c);
        }

        public static explicit operator HexColor(RgbaFloat c)
        {
            return new HexColor(c);
        }
    }
}
