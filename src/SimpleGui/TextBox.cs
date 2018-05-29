using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace SimpleGui
{
    public class TextBox : Control
    {
        public Text Label { get; set; }
        public string Text { get; set; }

        public ColoredQuad CursorControl { get; set; }

        protected int XPadding = 6;
        protected int CursorIndex { get; set; } = 0;

        public TextBox()
        {
            IsClickable = false;
            IsHoverable = true;

            ColorType = ControlColorType.Input;
        }

        public override void Initialize()
        {
            base.Initialize();

            Label = new Text(Text)
            {
                Size = Size - new Vector2(XPadding, 0),
                Position = new Vector2(XPadding, 0),
                TextAlignment = TextRender.TextAlignment.Leading,
            };
            Label.Initialize();
            AddChild(Label);

            CursorControl = new ColoredQuad()
            {
                Size = new Vector2(2, Label.Size.Y - 6),
                Position = new Vector2(2, 3),
                Color = Settings.Colors.CursorColor.Color
            };
            CursorControl.Initialize();
            AddChild(CursorControl);
        }

        public override void Update()
        {
            base.Update();

            if (IsMouseHoveringOver)
            {
                if (Text.Length > 0)
                {
                    if (InputTracker.GetKeyDown(Veldrid.Key.BackSpace))
                    {
                        if (CursorIndex > 0)
                        {
                            if (CursorIndex == Text.Length)
                            {
                                Text = Text.Substring(0, Text.Length - 1);
                            }
                            else
                            {
                                var restOfLine = Text.Substring(CursorIndex, (Text.Length - CursorIndex));
                                Text = Text.Substring(0, CursorIndex - 1) + restOfLine;
                            }
                            Label.Content = Text;
                            Label.Recreate();
                            CursorIndex--;
                            CursorControl.Position = GetTextIndexPosition(CursorIndex);
                        }
                    }
                    else if (InputTracker.GetKeyDown(Veldrid.Key.Delete))
                    {
                        if (CursorIndex < Text.Length)
                        {
                            var startLine = Text.Substring(0, CursorIndex);
                            Text = startLine + Text.Substring(CursorIndex + 1, (Text.Length - CursorIndex) - 1);
                            Label.Content = Text;
                            Label.Recreate();
                        }
                    }
                }

                foreach (var k in InputTracker._newKeysThisFrame)
                {
                    if (KeyHelper.IsInputKey(k))
                    {
                        InsertTextAtCursor(KeyHelper.GetKey(k).ToString());
                    }
                }
            }
        }


        protected void InsertTextAtCursor(string s)
        {
            if (Text.Length > 0)
            {
                if (CursorIndex == 0)
                {
                    Text = s + Text;
                }
                else if (CursorIndex == Text.Length)
                {
                    Text = Text + s;
                }
                else
                {
                    var restOfLine = Text.Substring(CursorIndex, (Text.Length - CursorIndex));
                    Text = Text.Substring(0, CursorIndex) + s + restOfLine;
                }
            }
            else
            {
                Text = s;
            }
            
            Label.Content = Text;
            Label.Recreate();
            CursorIndex++;
            CursorControl.Position = GetTextIndexPosition(CursorIndex);
        }


        protected override void OnMouseDown()
        {
            base.OnMouseDown();

            var cursorPos = GetCursorPosition();
            if (cursorPos != Vector2.Zero)
            {
                CursorControl.Position = cursorPos;
            }
        }

        public Vector2 GetTextIndexPosition(int index)
        {
            var substr = Text.Substring(0, index);
            var size = TextMeasurer.Measure(substr, new RendererOptions(Label.DrawableText.Font));
            return new Vector2(size.Width + Label.Position.X - 1, 3);
        }

        public Vector2 GetCursorPosition()
        {
            var mousePos = InputTracker.MousePosition;
            if (IsMouseHoveringOver)
            {
                // Iterate all combinations of letters and get closest

                var xPosList = new List<float>() { 0 };
                for (int i = 1; i <= Text.Length; i++)
                {
                    var substr = Text.Substring(0, i);
                    var size = TextMeasurer.Measure(substr, new RendererOptions(Label.DrawableText.Font));

                    xPosList.Add(size.Width);
                }

                var relativePos = mousePos - (Label.AbsolutePosition + new Vector2(CursorControl.Size.X, 0));
                var minDistance = xPosList.Min(n => Math.Abs(relativePos.X - n));
                var closest = xPosList.First(n => Math.Abs(relativePos.X - n) == minDistance);
                CursorIndex = xPosList.IndexOf(closest);
                
                return new Vector2(closest + Label.Position.X - 1, 3);
            }
            return Vector2.Zero;
        }
    }
}
