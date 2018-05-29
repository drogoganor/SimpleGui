using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SimpleGui
{
    public class ImageColorSample : Image
    {
        protected Bitmap Bitmap;
        public Color SelectedColor;

        public Action ColorSampled = () => { };

        public ImageColorSample(string filename) : base(filename)
        {
            Bitmap = (Bitmap)System.Drawing.Image.FromFile(filename);
        }

        protected override void OnMouseDown()
        {
            base.OnMouseDown();

            var ms = InputTracker.MousePosition - AbsolutePosition;
            var pixel = Bitmap.GetPixel((int)ms.X, (int)ms.Y);
            SelectedColor = pixel;

            ColorSampled();
        }
    }
}
