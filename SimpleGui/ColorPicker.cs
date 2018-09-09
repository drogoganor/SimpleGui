using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace SimpleGui
{
    public class ColorPicker : Control
    {
        public ImageColorSample SamplerImage;
        public ColoredQuad PreviewQuad;

        public Color SelectedColor;

        public bool Result;
        public Button OKButton;
        public Button CloseButton;

        public Action Closed = () => { };

        public ColorPicker()
        {
            Size = new Vector2(430, 390);
        }

        public override void Initialize()
        {
            base.Initialize();

            SamplerImage = new ImageColorSample("gui/color.png")
            {
                Size = new Vector2(320, 240),
                Position = new Vector2(5, 5),
            };
            SamplerImage.Initialize();
            AddChild(SamplerImage);

            PreviewQuad = new ColoredQuad()
            {
                Size = new Vector2(32, 32),
                Position = new Vector2(5, 245),
                Color = Color.White,
            };
            PreviewQuad.Initialize();
            AddChild(PreviewQuad);


            int x = 5;
            int y = 355;
            OKButton = new Button("OK")
            {
                Position = new Vector2(x, y),
                Size = new Vector2(90, 31),
            };
            OKButton.Initialize();
            AddChild(OKButton);
            OKButton.MouseUp = () =>
            {
                Result = true;
                Closed();
            };

            x += (int)OKButton.Size.X + 5;

            CloseButton = new Button("Cancel")
            {
                Position = new Vector2(x, y),
                Size = OKButton.Size,
            };
            CloseButton.Initialize();
            AddChild(CloseButton);
            CloseButton.MouseUp = () =>
            {
                Result = false;
                Closed();
            };

            SamplerImage.ColorSampled = () =>
            {
                SelectedColor = SamplerImage.SelectedColor;
                PreviewQuad.Color = SamplerImage.SelectedColor;
                PreviewQuad.Recreate();
            };
        }
    }
}
