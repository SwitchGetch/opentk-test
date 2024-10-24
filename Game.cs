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

	Matrix4 view;
	Matrix4 projection;
	float transformSpeed = 1;

	Shader shader;

	Texture box;
	Texture container;
	Texture awesomeface;

	List<Cube> cubes;

	Camera camera;

	Stopwatch timer;
	float time;
	float deltaTime;
	float fps;
	float seconds;
	float frames;


	protected override void OnLoad()
	{
		base.OnLoad();

		GL.ClearColor(0.45f, 0.49f, 0.5f, 1.0f);

        CursorState = CursorState.Hidden;
		MousePosition = ClientSize / 2;

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
			new Cube(awesomeface.Handle) { Position = new Vector3(-2, 0, 5) }
		};

		camera = new Camera();
		camera.RotationSpeed = 0.5f;
		camera.MovingSpeed = 5;

		view = Matrix4.LookAt(camera.Position, camera.Direction, Vector3.UnitY);
		projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), ClientSize.X / ClientSize.Y, 0.1f, 100.0f);

		shader.SetVector2("u_resolution", (Vector2)ClientSize);

        GL.Enable(EnableCap.DepthTest);

        timer = new Stopwatch();
        timer.Start();
    }

	protected override void OnUpdateFrame(FrameEventArgs args)
	{
		base.OnUpdateFrame(args);

		TimeHandler(args.Time);
        MouseHandler();
		KeyboardHandler();

		cubes[0].Rotation += new Vector3(transformSpeed * deltaTime, 0, 0);
		cubes[1].Rotation += new Vector3(0, transformSpeed * deltaTime, 0);
		cubes[2].Rotation += new Vector3(0, 0, transformSpeed * deltaTime);

		view = camera.GetViewMatrix();

		shader.SetMartix4("view", ref view);
		shader.SetMartix4("projection", ref projection);

		shader.SetFloat("u_time", time);
    }

	protected override void OnRenderFrame(FrameEventArgs args)
	{
		base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		for (int i = 0; i < cubes.Count; i++)
		{
			cubes[i].Render();
		}

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

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

		transformSpeed += e.OffsetY;
    }

    private void TimeHandler(double argsTime)
	{
        time = (float)timer.Elapsed.TotalSeconds;
        deltaTime = (float)argsTime;
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
        Vector2 d = MousePosition - ClientSize / 2;
        MousePosition = ClientSize / 2;

		if (d.X == 0 && d.Y == 0) return;

		Vector2 angle = deltaTime * camera.RotationSpeed * new Vector2(d.Y, -d.X);
        camera.Direction *= Matrix3.CreateRotationX(angle.X) * Matrix3.CreateRotationY(angle.Y);
    }

	private void KeyboardHandler()
	{
		if (KeyboardState.IsKeyDown(Keys.Escape))
		{
			Close();
		}
	}
}