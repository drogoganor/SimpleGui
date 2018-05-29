using System.Numerics;

namespace SimpleGui
{
    public class Checkbox : ControlAbstract
    {
        protected Control CheckControl { get; set; }
        protected Text Label { get; set; }
        public string Text { get; set; } = string.Empty;

        public Checkbox()
        {
            IsHoverable = true;
            IsClickable = true;
            IsToggleable = true;
            ClickUntoggles = true;
        }

        public virtual void Initialize()
        {
            CheckControl = new Control()
            {
                Size = new Vector2(Size.Y),
                IsHoverable = true,
                //IsClickable = true,
                IsToggleable = true,
                //ClickUntoggles = true,
                //ColorType = ControlColorType.Button,
            };
            CheckControl.Initialize();
            AddChild(CheckControl);

            Label = new Text(Text)
            {
                Position = new Vector2(Size.Y + 4, 0),
                Size = Size - new Vector2(Size.Y + 4, 0),
                TextAlignment = TextRender.TextAlignment.Leading,
            };
            Label.Initialize();
            AddChild(Label);
        }

        public override void Update()
        {
            base.Update();

            if (IsToggled)
                CheckControl.State = ControlState.Clicked;
            CheckControl.IsToggled = IsToggled;
        }
    }
}
