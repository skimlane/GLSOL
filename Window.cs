using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace GLStuff
{
    public class Window : GameWindow
    {
        private readonly float[] _vertices =
        {
             -0.5f, -0.5f, 0.5f, 
             -0.5f, 0.5f, 0.5f, 
             0.5f,  0.5f, 0.5f,
             0.5f, -0.5f, 0.5f,

             -0.5f, -0.5f, -0.5f,
             -0.5f, 0.5f, -0.5f,
             0.5f,  0.5f, -0.5f,
             0.5f, -0.5f, -0.5f
        };

        private readonly uint[] _indices =
        {
            0, 1, 
            1, 2,
            2, 3,
            3, 0,
            
            4, 5,
            5, 6,
            6, 7,
            7, 4,

            0, 4,
            1, 5,
            2, 6,
            3, 7   
        };

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private int _elementBufferObject;

        private Shader _shader;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
  
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
 
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("C:/Users/Рома/source/repos/GLStuff/Shaders/shader.vert", "C:/Users/Рома/source/repos/GLStuff/Shaders/shader.frag");
            _shader.Use();

       }

        float _rotangle = 0f;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            var transform = Matrix4.Identity;
            transform = transform * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotangle));
            transform = transform * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(10f));
            _rotangle += 0.1f;
            _shader.SetMatrix4("transform", transform);

            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(PrimitiveType.Lines, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();

        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            this.Title = "framerate is " + (1.0 / this.RenderTime).ToString();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.X);
        }
        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            GL.DeleteProgram(_shader.Handle);

            base.OnUnload();
        }
    }
}