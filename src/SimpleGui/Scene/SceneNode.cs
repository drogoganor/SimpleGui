using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SimpleGui.Scene
{
    public class SceneNode : DisposableBase
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        public Vector2 AbsolutePosition { get; protected set; }
        protected SceneNode Parent { get; set; }
        protected List<SceneNode> Children { get; set; } = new List<SceneNode>();

        public SceneNode()
        {

        }

        public virtual void AddChild(SceneNode item)
        {
            item.Parent = this;
            item.AbsolutePosition = AbsolutePosition + item.Position;
            Children.Add(item);
        }

        public virtual void RemoveChild(SceneNode item)
        {
            Children.Remove(item);
            item.Parent = null;
        }

        public virtual void Update()
        {
            AbsolutePosition = Position;
            if (Parent != null)
            {
                AbsolutePosition += Parent.AbsolutePosition;
            }

            for (int i = 0; i < Children.Count; i++)
            //foreach (var item in Children)
            {
                //item.Update();
                Children[i].Update();
            }
        }

        public virtual void Draw()
        {
            foreach (var item in Children)
            {
                item.Draw();
            }
        }
    }
}
