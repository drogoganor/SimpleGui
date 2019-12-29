using SixLabors.Fonts;
using System.Numerics;
using Veldrid;

namespace SimpleGui.TextRender
{
    public class Text : DisposableBase
    {
        private TextRenderer _renderer;

        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = new Vector2(200, 100);
        public string Content { get; set; } = string.Empty;
        public int FontSize { get; set; } = 12;
        public string FontName { get; set; } = "Arial";
        public FontStyle FontStyle { get; set; } = FontStyle.Regular;
        public TextAlignment TextAlignment { get; set; }
        public RgbaFloat Color { get; set; } = RgbaFloat.White;

        public Font Font { get; protected set; }

        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private ResourceSet textureSet;
        private Texture texture;
        private TextureView textureView;
        private TextCache textCache;

        public Text(TextRenderer renderer, string text)
        {
            Content = text;
            _renderer = renderer;
        }

        public void Initialize()
        {
            var device = _renderer.Device;
            var factory = device.ResourceFactory;

            BufferDescription vbDescription = new BufferDescription(
                4 * VertexPositionTextureColor.SizeInBytes,
                BufferUsage.VertexBuffer);
            vertexBuffer = AddDisposable(factory.CreateBuffer(vbDescription));

            BufferDescription ibDescription = new BufferDescription(
                4 * sizeof(ushort),
                BufferUsage.IndexBuffer);
            indexBuffer = AddDisposable(factory.CreateBuffer(ibDescription));

            var topLeft = new Vector2(0, 0);
            var topRight = new Vector2(0 + Size.X, 0);
            var bottomLeft = new Vector2(0, 0 + Size.Y);
            var bottomRight = new Vector2(0 + Size.X, 0 + Size.Y);

            VertexPositionTextureColor[] quadVertices =
            {
                new VertexPositionTextureColor(topLeft, new Vector2(0f, 0f), RgbaFloat.White),
                new VertexPositionTextureColor(topRight, new Vector2(1f, 0f), RgbaFloat.White),
                new VertexPositionTextureColor(bottomLeft, new Vector2(0f, 1f), RgbaFloat.White),
                new VertexPositionTextureColor(bottomRight, new Vector2(1f, 1f), RgbaFloat.White)
            };
            ushort[] quadIndices = { 0, 1, 2, 3 };

            device.UpdateBuffer(vertexBuffer, 0, quadVertices);
            device.UpdateBuffer(indexBuffer, 0, quadIndices);

            textCache = AddDisposable(new TextCache(device));
            Font = SystemFonts.CreateFont(FontName, FontSize, FontStyle);
            texture = AddDisposable(textCache.GetTextTexture(Content, Font, TextAlignment, Color, Size));
            textureView = AddDisposable(device.ResourceFactory.CreateTextureView(texture));

            textureSet = AddDisposable(device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                _renderer.Shader.TextureLayout,
                textureView,
                device.Aniso4xSampler)));
        }

        public void Recreate()
        {
            var device = _renderer.Device;

            if (texture != null)
            {
                RemoveAndDispose(ref textureView);
                RemoveAndDispose(ref texture);
                RemoveAndDispose(ref textCache);
            }

            textCache = AddDisposable(new TextCache(device));
            Font = SystemFonts.CreateFont(FontName, FontSize, FontStyle);
            texture = AddDisposable(textCache.GetTextTexture(Content, Font, TextAlignment, Color, Size));
            textureView = AddDisposable(device.ResourceFactory.CreateTextureView(texture));

            textureSet = AddDisposable(device.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                _renderer.Shader.TextureLayout,
                textureView,
                device.Aniso4xSampler)));
        }
        
        public void DrawBatched()
        {
            var cl = _renderer.CommandList;

            cl.UpdateBuffer(_renderer.Shader.ProjectionBuffer, 0, 
                Matrix4x4.CreateOrthographicOffCenter(
                    0, 
                    _renderer.Device.SwapchainFramebuffer.Width, 
                    _renderer.Device.SwapchainFramebuffer.Height, 
                    0, 0, 1));
            cl.UpdateBuffer(_renderer.Shader.WorldBuffer, 0, Matrix4x4.CreateTranslation(new Vector3(Position, 0)));

            cl.SetVertexBuffer(0, vertexBuffer);
            cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            cl.SetGraphicsResourceSet(0, _renderer.Shader.ProjViewSet);
            cl.SetGraphicsResourceSet(1, textureSet);
            
            cl.DrawIndexed(
                indexCount: (uint)4,
                instanceCount: 1,
                indexStart: (uint)0,
                vertexOffset: 0,
                instanceStart: 0);
        }

        /// <summary>
        /// Render this text in a single draw call
        /// </summary>
        public void Draw()
        {
            _renderer.BeginDraw();
            DrawBatched();
            _renderer.EndDraw();
        }
    }
}
