using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using TheRender.Entities;
using TheRender.OpenTK.Entities;
using TheRender.OpenTK.Scenes;
using TheRender.OpenTK.Services;
using TheRender.Services;
using TheRender.Services.Interfaces;

namespace TheRender.OpenTK
{
    public class Window : GameWindow
    {
        private readonly int countTask = 4;
        private readonly int cellWidth = 512;
        private readonly int cellHeight = 512;

        private readonly int vertexSize = 2;
        private readonly int colorSize = 3;
         
        private readonly ITraceService tracingService;
        private readonly ShaderService shaderService;

        private Matrix4 matrix;
        private int vertexBufferHandle;
        private int colorBufferHandle;

        private float pointSize;
        private float[] arrayVertexs;
        private float[] arrayColors;

        public Window() : base(512, 512, GraphicsMode.Default, "TheRender")
        {
            tracingService = new RayTracingService(cellWidth, cellHeight, countTask);
            tracingService.AddDefaultScene();
            
            shaderService = new ShaderService();

            arrayVertexs = new float[cellWidth * cellHeight * vertexSize];
            arrayColors = new float[cellWidth * cellHeight * colorSize];

            colorBufferHandle = GL.GenBuffer();
            vertexBufferHandle = GL.GenBuffer();

            UpdateVertexes();
            UpdateColors(tracingService.GetPixels());
        }

        public void Start()
        {
            tracingService.Run();
            Run(60);
        }

        private void UpdateVertexes()
        {
            var arrayVertexesIterator = 0;

            var widthStep = Width / (float)cellWidth;
            var heightStep = Height / (float)cellHeight;
            pointSize = Math.Max(1.0f, Math.Max(widthStep, heightStep) * 2.0f);

            for (var cellWidthIterator = 0; cellWidthIterator < cellWidth; cellWidthIterator++)
            {
                for (var cellHeightIterator = 0; cellHeightIterator < cellHeight; cellHeightIterator++)
                {
                    arrayVertexs[arrayVertexesIterator + 0] = widthStep * cellWidthIterator;
                    arrayVertexs[arrayVertexesIterator + 1] = heightStep * cellHeightIterator;
                    arrayVertexesIterator += vertexSize;
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * arrayVertexs.Length, arrayVertexs, BufferUsageHint.DynamicDraw);
        }

        private void UpdateColors(PixelEntity[,] pixels)
        {
            int arrayColorsIterator = 0;

            for (int cellWidthIterator = 0; cellWidthIterator < cellWidth; cellWidthIterator++)
            {
                for (int cellHeightIterator = 0; cellHeightIterator < cellHeight; cellHeightIterator++)
                {
                    arrayColors[arrayColorsIterator + 0] = pixels[cellWidthIterator, cellHeightIterator].Color.R;
                    arrayColors[arrayColorsIterator + 1] = pixels[cellWidthIterator, cellHeightIterator].Color.G;
                    arrayColors[arrayColorsIterator + 2] = pixels[cellWidthIterator, cellHeightIterator].Color.B;
                    arrayColorsIterator += colorSize;
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * arrayColors.Length, arrayColors, BufferUsageHint.DynamicDraw);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
        }
        
        public string GetPixelColor(int positionX, int positionY)
        {
            try
            {
                var pixels = tracingService.GetPixels();
                var x = (int) (positionX / (Width / (float) cellWidth));
                var y = (int) (positionY / (Height / (float) cellHeight));

                if (x >= 0 && x < pixels.GetLength(0) && y >= 0 && y < pixels.GetLength(1))
                {
                    var color = pixels[x, y].Color;
                    return $"[{x},{y}] {color.R:F3} {color.G:F3} {color.B:F3}";
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{exception.Message} {exception.StackTrace}");
            }
            
            return null;
        }

        protected override void OnResize(EventArgs e)
        {
            MakeCurrent();
            GL.Viewport(0, 0, Width, Height);
            UpdateVertexes();
        }
        
        protected override void Dispose(bool manual)
        {
            tracingService.Dispose();
            base.Dispose(manual);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            UpdateColors(tracingService.GetPixels());
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            MakeCurrent();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            matrix = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, -1.0f, 1.0f);

            GL.EnableVertexAttribArray((int)ShaderAttributeEntity.Vertex);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferHandle);
            GL.VertexAttribPointer((int)ShaderAttributeEntity.Vertex, vertexSize, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray((int)ShaderAttributeEntity.Color);
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferHandle);
            GL.VertexAttribPointer((int)ShaderAttributeEntity.Color, colorSize, VertexAttribPointerType.Float, false, 0, 0);

            GL.UniformMatrix4(shaderService.MatrixHandle, false, ref matrix);
            GL.Uniform1(shaderService.PointSizeHandle, pointSize);
            GL.DrawArrays(PrimitiveType.Points, 0, cellWidth * cellHeight);

            GL.DisableVertexAttribArray((int)ShaderAttributeEntity.Color);
            GL.DisableVertexAttribArray((int)ShaderAttributeEntity.Vertex);

            Context.SwapBuffers();
        }
    }
}
