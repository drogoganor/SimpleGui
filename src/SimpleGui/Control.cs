using System;
using System.Numerics;
using Veldrid;
using SimpleGui.Scene;
using System.Collections.Generic;
using System.Linq;

namespace SimpleGui
{
    public class Control : ControlAbstract
    {
        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;

        public ControlSettings Settings { get; set; }

        public int BorderWidth { get; set; } = 1;

        private uint RenderCount = 8;

        public Control()
        {
            Settings = Gui.Settings.DefaultControlSettings.Copy();
        }

        public virtual void Initialize()
        {
            var topLeft = new Vector2(0, 0);
            var topRight = new Vector2(Size.X, 0);
            var bottomLeft = new Vector2(0, Size.Y);
            var bottomRight = new Vector2(Size.X, Size.Y);
            var borderSize = new Vector2(BorderWidth);
            var colorConfig = Settings.Colors.Colors[ColorType];
            var vertList = new List<VertexPositionColor>();

            RenderCount = 8;
            if (BorderWidth == 0)
            {
                RenderCount = 4;
            }

            var addVertsAction = new Action<HexColor, HexColor>((border, fill) =>
            {
                if (BorderWidth == 0)
                {
                    VertexPositionColor[] fillVertices =
                    {
                        new VertexPositionColor(topLeft, colorConfig.Fill.Normal),
                        new VertexPositionColor(topRight, colorConfig.Fill.Normal),
                        new VertexPositionColor(bottomLeft, colorConfig.Fill.Normal),
                        new VertexPositionColor(bottomRight, colorConfig.Fill.Normal),
                    };
                    vertList.AddRange(fillVertices);
                }
                else
                {
                    VertexPositionColor[] fillVertices =
                    {
                        new VertexPositionColor(topLeft, border),
                        new VertexPositionColor(topRight, border),
                        new VertexPositionColor(bottomLeft, border),
                        new VertexPositionColor(bottomRight, border),
                        new VertexPositionColor(topLeft + borderSize, fill),
                        new VertexPositionColor(topRight + new Vector2(-borderSize.X, borderSize.Y), fill),
                        new VertexPositionColor(bottomLeft + new Vector2(borderSize.X, -borderSize.Y), fill),
                        new VertexPositionColor(bottomRight - borderSize, fill),
                    };
                    vertList.AddRange(fillVertices);
                }
            });
            
            addVertsAction(colorConfig.Border.Normal, colorConfig.Fill.Normal);
            addVertsAction(colorConfig.Border.Hover, colorConfig.Fill.Hover);
            addVertsAction(colorConfig.Border.Clicked, colorConfig.Fill.Clicked);
            addVertsAction(colorConfig.Border.Disabled, colorConfig.Fill.Disabled);
            
            var vbDescription = new BufferDescription(
                RenderCount * 4 * VertexPositionColor.SizeInBytes,
                BufferUsage.VertexBuffer);
            _vertexBuffer = AddDisposable(Gui.Factory.CreateBuffer(vbDescription));
            Gui.Device.UpdateBuffer(_vertexBuffer, 0, vertList.ToArray());

            ushort[] quadIndices = Enumerable.Range(0, (int)(RenderCount * 4) - 1).Select(i => (ushort)i).ToArray();
            BufferDescription ibDescription = new BufferDescription(
                (uint)RenderCount * 4 * sizeof(ushort),
                BufferUsage.IndexBuffer);
            _indexBuffer = AddDisposable(Gui.Factory.CreateBuffer(ibDescription));
            Gui.Device.UpdateBuffer(_indexBuffer, 0, quadIndices);
        }


        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            Gui.BeginDraw();
            DrawBatched();
            Gui.EndDraw();

            base.Draw();
        }

        public void DrawBatched()
        {
            Gui.CommandList.UpdateBuffer(Gui.ColorShader.WorldBuffer, 0, new Vector4(AbsolutePosition, 0, 0));
            Gui.CommandList.SetVertexBuffer(0, _vertexBuffer);
            Gui.CommandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            //Gui.CommandList.SetGraphicsResourceSet(0, Gui.ColorShader.ResourceSet);
            uint offset = (uint)State * 8;

            // Issue a Draw command for a single instance with 4 indices.
            Gui.CommandList.DrawIndexed(
                indexCount: RenderCount,
                instanceCount: 1,
                indexStart: offset,
                vertexOffset: 0,
                instanceStart: 0);

        }

        public void SetCenterScreen()
        {
            Position = Gui.GetCenterScreenPosFor(Size);
        }
    }
}
