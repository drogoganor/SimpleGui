using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SimpleGui.Scene
{
    public class SceneGraph
    {
        public SceneNode Root { get; set; } = new SceneNode();

        public SceneGraph()
        {

        }

        public void Update()
        {
            Root.Update();
        }

        public void Draw()
        {
            Root.Draw();
        }

        public void DisposeAll()
        {
            Root.Dispose();
            Root = null;
        }
    }
}
