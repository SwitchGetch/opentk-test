using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using OpenTK.Windowing.Common.Input;

public class Game : GameWindow
{
	public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

	public Vector2 WindowSize { get => ClientSize; }

	float transformSpeed = 1;

	Shader shader;

	Texture box;
	Texture container;
	Texture awesomeface;

	List<Cube> cubes;

	Camera camera;

	float time;
	float deltaTime;
	float fps;
	float seconds;
	float frames;


	protected override void OnLoad()
	{
		base.OnLoad();

		GL.ClearColor(0.45f, 0.49f, 0.5f, 1.0f);

        CursorState = CursorState.Grabbed;

        shader = new Shader("shader.vert", "shader.frag");
		shader.Use();

		box = new Texture("box.jpg");
		container = new Texture("container.jpg");
		awesomeface = new Texture("awesomeface.png");

		Cube.shader = shader.Handle;

		cubes = new List<Cube>()
		{
			new Cube(box.Handle) { Position = new Vector3(2, 0, 5) },
			new Cube(container.Handle) { Position = new Vector3(0, 0, 5) },
			new Cube(awesomeface.Handle) { Position = new Vector3(-2, 0, 5) },
        };

		Camera.shader = shader.Handle;
        camera = new Camera();
		camera.Viewport = ClientSize;
		camera.RotationSpeed = 0.001f;
		camera.MovingSpeed = 5;

		shader.SetVector2("u_resolution", (Vector2)ClientSize);

        GL.Enable(EnableCap.DepthTest);

		time = 0;
		deltaTime = 0;
		fps = 0;
		seconds = 0;
		frames = 0;
    }

	protected override void OnUpdateFrame(FrameEventArgs args)
	{
		base.OnUpdateFrame(args);

		TimeHandler(args.Time);
        MouseHandler();
		KeyboardHandler();

		for (int i = 0; i < cubes.Count; i++)
		{
			switch (i % 3)
			{
				case 0: cubes[i].Rotation.X += transformSpeed * deltaTime; break;
				case 1: cubes[i].Rotation.Y += transformSpeed * deltaTime; break;
				case 2: cubes[i].Rotation.Z += transformSpeed * deltaTime; break;
            }
		}

		shader.SetFloat("u_time", time);
    }

	protected override void OnRenderFrame(FrameEventArgs args)
	{
		base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		for (int i = 0; i < cubes.Count; i++) cubes[i].Render();

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

		camera.Viewport = ClientSize;

		shader.SetVector2("u_resolution", ClientSize);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

		transformSpeed += e.OffsetY;
    }

    private void TimeHandler(double argsTime)
	{
        deltaTime = (float)argsTime;
		time += deltaTime;
        frames++;

		if (seconds < (int)time)
		{
			fps = (int)(frames / (float)(time - seconds));
            seconds = (int)time;
			frames = 0;

            Title = "FPS: " + fps;
        }
	}

	private void MouseHandler()
	{
		Vector2 d = MouseState.Delta;

		if (d.X == 0 && d.Y == 0) return;

        camera.Pitch += camera.RotationSpeed * d.Y;
		camera.Yaw += camera.RotationSpeed * -d.X;
    }

	private void KeyboardHandler()
	{
		KeyboardState input = KeyboardState;

		if (input.IsKeyDown(Keys.Escape)) Close();

		Vector3 d = new Vector3();
		if (input.IsKeyDown(Keys.W)) d += camera.Forward;
		if (input.IsKeyDown(Keys.S)) d -= camera.Forward;
		if (input.IsKeyDown(Keys.A)) d += new Vector3(camera.Forward.Z, 0, -camera.Forward.X);
		if (input.IsKeyDown(Keys.D)) d -= new Vector3(camera.Forward.Z, 0, -camera.Forward.X);
		if (input.IsKeyDown(Keys.Space)) d += Vector3.UnitY;
		if (input.IsKeyDown(Keys.LeftShift)) d -= Vector3.UnitY;

		if (d != Vector3.Zero)
		{
			camera.Position += deltaTime * camera.MovingSpeed * Vector3.Normalize(d);
		}
	}
}