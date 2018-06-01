using System.Numerics;

namespace SimpleGui
{
    public class Text : ControlAbstract
    {
        public string Content { get; set; }
        public string FontName { get; set; } = "Arial";
        public int FontSize { get; set; } = 14;
        public TextRender.TextAlignment TextAlignment { get; set; } = TextRender.TextAlignment.Center;

        public TextRender.Text DrawableText;
        
        public Text(string text)
        {
            Content = text;
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            base.Dispose(disposeManagedResources);

            if (DrawableText != null)
            {
                RemoveAndDispose(ref DrawableText);
                DrawableText = null;
            }
        }

        public void Initialize()
        {
            DrawableText = AddDisposable(new TextRender.Text(Gui.TextRenderer, Content)
            {
                Position = Position,
                Size = Size,
                TextAlignment = TextAlignment,
                FontName = FontName,
                FontSize = FontSize
            });
            DrawableText.Initialize();
        }

        public void Recreate()
        {
            if (DrawableText != null)
            {
                RemoveAndDispose(ref DrawableText);
            }
            Initialize();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();

            DrawableText.Position = AbsolutePosition;
            DrawableText.Draw();
        }
    }
}
