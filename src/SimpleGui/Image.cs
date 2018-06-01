using System;
using System.Numerics;
using Veldrid;
using AssetPrimitives;
using System.IO;
using AssetProcessor;

namespace SimpleGui
{
    public class Image : ControlAbstract
    {
        private readonly ProcessedTexture ProcessedTexture;

        private DeviceBuffer _vertexBuffer;
        private DeviceBuffer _indexBuffer;

        private ResourceSet textureSet;
        private Texture texture;
        private TextureView textureView;

        public ControlSettings Settings { get; set; }
        
        public Image(string filename)
        {
            Settings = Gui.Settings.DefaultControlSettings.Copy();

            ProcessedTexture = LoadFileAsset<ProcessedTexture>(filename);
        }

        public virtual void Initialize()
        {
            var device = Gui.Device;

            var topLeft = new Vector2(0, 0);
            var topRight = new Vector2(Size.X, 0);
            var bottomLeft = new Vector2(0, Size.Y);
            var bottomRight = new Vector2(Size.X, Size.Y);
            
            VertexPositionTextureColor[] fillVertices =
            {
                new VertexPositionTextureColor(topLeft, new Vector2(0, 0), RgbaFloat.Yellow.ToVector4()),
                new VertexPositionTextureColor(topRight, new Vector2(1, 0), RgbaFloat.Yellow.ToVector4()),
                new VertexPositionTextureColor(bottomLeft, new Vector2(0, 1), RgbaFloat.Yellow.ToVector4()),
                new VertexPositionTextureColor(bottomRight, new Vector2(1, 1), RgbaFloat.Yellow.ToVector4()),
            };
            
            var vbDescription = new BufferDescription(
                4 * VertexPositionTextureColor.SizeInBytes,
                BufferUsage.VertexBuffer);
            _vertexBuffer = AddDisposable(Gui.Factory.CreateBuffer(vbDescription));
            device.UpdateBuffer(_vertexBuffer, 0, fillVertices);

            ushort[] quadIndices = new ushort[] { 0, 1, 2, 3 };
            BufferDescription ibDescription = new BufferDescription(
                4 * sizeof(ushort),
                BufferUsage.IndexBuffer);
            _indexBuffer = AddDisposable(Gui.Factory.CreateBuffer(ibDescription));
            device.UpdateBuffer(_indexBuffer, 0, quadIndices);
            
            texture = AddDisposable(ProcessedTexture.CreateDeviceTexture(Gui.Device, Gui.Factory, TextureUsage.Sampled));
            textureView = AddDisposable(Gui.Factory.CreateTextureView(texture));

            textureSet = AddDisposable(device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                Gui.TextureShader.TextureLayout,
                textureView,
                device.Aniso4xSampler)));
        }
        
        public T LoadFileAsset<T>(string name)
        {
            var isProcessor = new ImageSharpProcessor();

            object processedAsset;
            using (FileStream fs = File.OpenRead(name))
            {
                processedAsset = isProcessor.Process(fs, ".png");
            }

            return (T)processedAsset;
        }

        public override void Update()
        {
            base.Update();
        }

        protected override void OnMouseDown()
        {
            base.OnMouseDown();
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
            Gui.CommandList.UpdateBuffer(Gui.TextureShader.ProjectionBuffer, 0, Matrix4x4.CreateOrthographicOffCenter(0, Gui.Device.SwapchainFramebuffer.Width, Gui.Device.SwapchainFramebuffer.Height, 0, 0, 1));
            Gui.CommandList.UpdateBuffer(Gui.TextureShader.WorldBuffer, 0, new Vector4(AbsolutePosition, 0, 0));
            Gui.CommandList.SetPipeline(Gui.TexturePipeline);
            Gui.CommandList.SetVertexBuffer(0, _vertexBuffer);
            Gui.CommandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);

            Gui.CommandList.SetGraphicsResourceSet(0, Gui.TextureShader.ResourceSet);
            Gui.CommandList.SetGraphicsResourceSet(1, textureSet);

            uint offset = (uint)State * 8;

            Gui.CommandList.DrawIndexed(
                indexCount: 4,
                instanceCount: 1,
                indexStart: offset,
                vertexOffset: 0,
                instanceStart: 0);
        }
    }
}
