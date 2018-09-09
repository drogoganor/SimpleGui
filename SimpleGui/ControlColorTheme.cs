using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace SimpleGui
{
    public enum ControlColorType
    {
        Default,
        Form,
        Button,
        Input,
        InputItem
    }

    [Serializable]
    public class ControlColorTheme
    {
        public Dictionary<ControlColorType, ControlColorSet> Colors { get; set; } = new Dictionary<ControlColorType, ControlColorSet>();

        public HexColor CursorColor { get; set; } = (HexColor)RgbaFloat.Yellow;

        public ControlColorTheme()
        {
            Colors.Add(ControlColorType.Default, new ControlColorSet());
            Colors.Add(ControlColorType.Form, new ControlColorSet());
            Colors.Add(ControlColorType.Button, new ControlColorSet());
            Colors.Add(ControlColorType.Input, new ControlColorSet());
            Colors.Add(ControlColorType.InputItem, new ControlColorSet());
        }
    }

    [Serializable]
    public class ControlColorSet
    {
        public ColorSet Fill { get; set; } = new ColorSet();
        public ColorSet Border { get; set; } = new ColorSet();
        public ColorSet Font { get; set; } = new ColorSet();
    }

    [Serializable]
    public class ColorSet
    {
        public HexColor Normal { get; set; } = (HexColor)RgbaFloat.Grey;
        public HexColor Hover { get; set; } = (HexColor)RgbaFloat.LightGrey;
        public HexColor Clicked { get; set; } = (HexColor)RgbaFloat.White;
        public HexColor Disabled { get; set; } = (HexColor)RgbaFloat.DarkRed;

        public ColorSet()
        {
        }
    }
}
