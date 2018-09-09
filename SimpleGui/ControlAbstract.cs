using SimpleGui.Scene;
using SimpleGui;
using System;
using Veldrid;
using Veldrid.Sdl2;
using System.Numerics;

namespace SimpleGui
{
    public enum ControlState
    {
        Normal,
        Hover,
        Clicked,
        Disabled
    }

    public class ControlAbstract : SceneNode
    {
        public ControlColorType ColorType { get; set; } = ControlColorType.Default;
        public ControlState State { get; set; }
        public bool IsMouseHoveringOver { get; protected set; }
        public bool IsClickable { get; set; }
        public bool IsHoverable { get; set; }
        public bool IsToggleable { get; set; }
        public bool ClickUntoggles { get; set; }

        public Action MouseDown { get; set; } = () => { };
        public Action MouseUp { get; set; } = () => { };
        public Action Toggled { get; set; } = () => { };

        public bool IsMouseDown { get; protected set; }
        public bool IsToggled { get; set; }

        public ControlAbstract()
        {
            Size = new Vector2(100, 100);
        }
        
        public override void Update()
        {
            base.Update();

            var mousePos = InputTracker.MousePosition;

            if (mousePos.X >= AbsolutePosition.X && mousePos.X < AbsolutePosition.X + Size.X
                && mousePos.Y >= AbsolutePosition.Y && mousePos.Y < AbsolutePosition.Y + Size.Y)
            {
                IsMouseHoveringOver = true;
            }
            else
            {
                IsMouseHoveringOver = false;
            }

            
            if (IsMouseHoveringOver)
            {
                if (InputTracker.GetMouseButtonDown(global::Veldrid.MouseButton.Left))
                {
                    OnMouseDown();
                }
                if (InputTracker.GetMouseButton(Veldrid.MouseButton.Left))
                {
                    IsMouseDown = true;
                    if (IsClickable)
                        State = ControlState.Clicked;
                }
                else
                {
                    if (IsMouseDown)
                    {
                        OnMouseUp();
                        IsMouseDown = false;
                    }

                    if (!IsToggled)
                    {
                        if (IsHoverable)
                            State = ControlState.Hover;
                        else
                            State = ControlState.Normal;
                    }
                }
            }
            else
            {
                if (!IsToggled)
                {
                    State = ControlState.Normal;
                }
                IsMouseDown = false;
            }
        }

        protected virtual void OnMouseDown()
        {
            if (IsToggleable)
            {
                if (IsToggled && ClickUntoggles)
                    IsToggled = false;
                else
                    IsToggled = true;
                OnToggle();
            }

            MouseDown();
        }
        
        protected virtual void OnMouseUp()
        {
            MouseUp();
        }

        protected virtual void OnToggle()
        {
            Toggled();
        }
    }
}
