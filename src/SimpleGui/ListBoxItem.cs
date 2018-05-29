using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SimpleGui
{
    public class ListBoxItem : Control
    {
        public Text Label { get; protected set; }
        public string Text { get; set; } = string.Empty;
        public ListBox ListBox { get; set; }

        protected int XPadding = 6;

        public ListBoxItem(ListBox parent)
        {
            ListBox = parent;

            //ColorType = ControlColorType.InputItem;
            IsHoverable = true;
            IsClickable = true;
        }

        public override void Initialize()
        {
            base.Initialize();

            Label = new Text(Text)
            {
                Position = new Vector2(XPadding, 0),
                Size = Size - new Vector2(XPadding, 0),
                TextAlignment = TextRender.TextAlignment.Leading,
            };
            Label.Initialize();
            AddChild(Label);
        }


        protected override void OnToggle()
        {
            base.OnToggle();

            if (IsToggled)
            {
                ListBox.SelectItem(this);
            }
        }
    }
}
