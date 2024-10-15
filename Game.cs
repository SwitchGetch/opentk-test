using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

public class Game : GameWindow
{
	public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

	float[] vertices = {
    //    positions           colors
	 0.5f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,
     0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,
	-0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f,
	-0.5f,  0.5f, 0.0f,   1.0f, 1.0f, 1.0f,
	};

	uint[] indices = {
    0, 1, 3,
    1, 2, 3
	};

	int VertexBufferObject;
	int VertexArrayObject;
	int ElementBufferObject;

	Shader shader;

	Stopwatch timer = new Stopwatch();

	protected override void OnLoad()
	{
		base.OnLoad();

		GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

		shader = new Shader("shader.vert", "shader.frag");
		shader.Use();

		VertexBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
		GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

		VertexArrayObject = GL.GenVertexArray();
		GL.BindVertexArray(VertexArrayObject);
		int positionLocation = shader.GetAttribLocation("position");
		int colorLocation = shader.GetAttribLocation("color");
		GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
		GL.EnableVertexAttribArray(positionLocation);
		GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
		GL.EnableVertexAttribArray(colorLocation);

		ElementBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

		OnFramebufferResize(new FramebufferResizeEventArgs(ClientSize));

		timer.Start();
	}

	protected override void OnUpdateFrame(FrameEventArgs args)
	{
		base.OnUpdateFrame(args);

		float time = (float)timer.Elapsed.TotalSeconds;
		int timeLocation = GL.GetUniformLocation(shader.Handle, "u_time");
		GL.Uniform1(timeLocation, time);

		Title = time.ToString();

		if (KeyboardState.IsKeyDown(Keys.Escape))
		{
			Close();
		}
	}

	protected override void OnRenderFrame(FrameEventArgs args)
	{
		base.OnRenderFrame(args);

		GL.Clear(ClearBufferMask.ColorBufferBit);

		GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

		SwapBuffers();
	}

	protected override void OnUnload()
	{
		base.OnUnload();

		shader.Dispose();
	}

	protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
	{
		base.OnFramebufferResize(e);

		GL.Viewport(0, 0, e.Width, e.Height);

		int resolutionLocation = GL.GetUniformLocation(shader.Handle, "u_resolution");
		GL.Uniform2(resolutionLocation, (Vector2)ClientSize);
	}
}