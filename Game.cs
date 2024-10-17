using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using StbImageSharp;

public class Game : GameWindow
{
	public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

    float[] vertices =
	{
    //Position          Texture coordinates
     0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
     0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
    -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
    -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
	};

    uint[] indices = {
    0, 1, 3,
    1, 2, 3
	};

	int VertexBufferObject;
	int VertexArrayObject;
	int ElementBufferObject;

	Matrix4 transform;

	Shader shader;
	Texture texture0;
	Texture texture1;

	Stopwatch timer = new Stopwatch();

	protected override void OnLoad()
	{
		base.OnLoad();

		GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

		shader = new Shader("shader.vert", "shader.frag");
		shader.Use();

		transform = Matrix4.Identity;
		shader.SetMartix4("transform", ref transform);

		texture0 = new Texture("container.jpg");
        texture1 = new Texture("awesomeface.png");

        texture0.Use();
        texture1.Use(TextureUnit.Texture1);

        shader.SetInt("texture0", 0);
        shader.SetInt("texture1", 1);

        VertexBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
		GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

		VertexArrayObject = GL.GenVertexArray();
		GL.BindVertexArray(VertexArrayObject);

		int positionLocation = shader.GetAttribLocation("aPosition");
		GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
		GL.EnableVertexAttribArray(positionLocation);

		int texCoordLocation = shader.GetAttribLocation("aTexCoord");
		GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
		GL.EnableVertexAttribArray(texCoordLocation);

        ElementBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        shader.SetVector2("u_resolution", (Vector2)ClientSize);

        timer.Start();
	}

	protected override void OnUpdateFrame(FrameEventArgs args)
	{
		base.OnUpdateFrame(args);

		transform *= Matrix4.CreateRotationZ((float)args.Time);
		shader.SetMartix4("transform", ref transform);

        float time = (float)timer.Elapsed.TotalSeconds;
		
		shader.SetFloat("u_time", time);

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

		shader.SetVector2("u_resolution", new Vector2(e.Width, e.Height));
	}
}