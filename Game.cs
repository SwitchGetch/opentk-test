using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

public class Game : GameWindow
{
	public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

	public Vector2 WindowSize { get => ClientSize; }

	Color4 background;

	Shader shader;

	Texture box;
	Texture stone;
	Texture cross;

	Player player;
	List<Cube> cubes;
	Square crosshair;

	Camera camera;
	Camera camera2D;

	float time;
	float deltaTime;
	float fps;
	float seconds;
	float frames;


	protected override void OnLoad()
	{
		base.OnLoad();

		background = new Color4(0.27f, 0.28f, 0.3f, 1.0f);

		GL.ClearColor(background);

        CursorState = CursorState.Grabbed;	

        shader = new Shader("shader.vert", "shader.frag");

		box = new Texture("box.jpg");
		stone = new Texture("stone.jpg");
		cross = new Texture("crosshair.png");

		player = new Player();
		player.Position = new Vector3(0, 5, 0);
		player.Scale = new Vector3(0.5f, 1, 0.5f);
		player.MovingSpeed = 5;
		player.RotationSpeed = 0.001f;
		player.JumpingSpeed = 5;

		cubes = new List<Cube>()
		{
			new Cube(shader.Handle, stone.Handle) { Position = new Vector3(0, -0.5f, 0), Scale = new Vector3(20, 1, 20) },
        };

		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 1; y++)
			{
				for (int z = 0; z < 5; z++)
				{
					cubes.Add(new Cube(shader.Handle, box.Handle) {
						Position = new Vector3(4 * x - 8, 4 * y + 1, 4 * z - 8),
						Scale = new Vector3(2, 2, 2)});
				}
			}
		}

        camera = new Camera(shader.Handle);
		camera.Viewport = ClientSize;
		camera.FOV = MathHelper.DegreesToRadians(60.0f);

		camera2D = new Camera(shader.Handle, false);
		camera2D.Viewport = ClientSize;
		camera2D.LookAt(-Vector3.UnitZ);

        crosshair = new Square(shader.Handle, cross.Handle);
		crosshair.Position = new Vector3(0, 0, -1);
		crosshair.Scale = 50 * Vector2.One;

		shader.SetVector2("u_resolution", (Vector2)ClientSize);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

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

		/*for (int i = 1; i < cubes.Count; i++)
		{
			cubes[i].Position *= Matrix3.CreateRotationY(deltaTime);
			cubes[i].Position = new Vector3(cubes[i].Position.X, (float)Math.Sin(time) + 2, cubes[i].Position.Z);
		}*/

		UpdatePlayer();
    }

	protected override void OnRenderFrame(FrameEventArgs args)
	{
		base.OnRenderFrame(args);

		GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		camera.Use();
		for (int i = 0; i < cubes.Count; i++) cubes[i].Render();

		camera2D.Use();
		crosshair.Render();

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
		camera2D.Viewport = ClientSize;

		shader.SetVector2("u_resolution", ClientSize);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

		camera.FOV -= 0.05f * e.OffsetY;
    }

    private void TimeHandler(double argsTime)
	{
        deltaTime = (float)argsTime;
		time += deltaTime;
        frames++;

		shader.SetFloat("u_time", time);

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

        camera.Pitch += player.RotationSpeed * d.Y;
		camera.Yaw += player.RotationSpeed * -d.X;
    }

	private void KeyboardHandler()
	{
		if (KeyboardState.IsKeyDown(Keys.Escape)) Close();

		/*Vector3 d = new Vector3();
		if (KeyboardState.IsKeyDown(Keys.W)) d += camera.Forward;
		if (KeyboardState.IsKeyDown(Keys.S)) d -= camera.Forward;
		if (KeyboardState.IsKeyDown(Keys.A)) d += new Vector3(camera.Forward.Z, 0, -camera.Forward.X);
		if (KeyboardState.IsKeyDown(Keys.D)) d -= new Vector3(camera.Forward.Z, 0, -camera.Forward.X);
        if (KeyboardState.IsKeyDown(Keys.Space)) d += Vector3.UnitY;
        if (KeyboardState.IsKeyDown(Keys.LeftShift)) d -= Vector3.UnitY;

        if (d != Vector3.Zero)
		{
			camera.Position += deltaTime * camera.MovingSpeed * Vector3.Normalize(d);
		}*/
	}

	private void UpdatePlayer()
	{
		player.Speed += deltaTime * player.Acceleration;
		player.Position += deltaTime * player.Speed;

        Vector3 d = new Vector3();
        if (KeyboardState.IsKeyDown(Keys.W)) d += camera.Forward;
        if (KeyboardState.IsKeyDown(Keys.S)) d -= camera.Forward;
        if (KeyboardState.IsKeyDown(Keys.A)) d += new Vector3(camera.Forward.Z, 0, -camera.Forward.X);
        if (KeyboardState.IsKeyDown(Keys.D)) d -= new Vector3(camera.Forward.Z, 0, -camera.Forward.X);

        if (d != Vector3.Zero)
        {
            player.Position += deltaTime * player.MovingSpeed * Vector3.Normalize(d);
        }

		bool collision = false;

		for (int i = 0; i < cubes.Count; i++)
		{
			Vector3 playerMin = player.Position - 0.5f * player.Scale;
			Vector3 playerMax = player.Position + 0.5f * player.Scale;
			Vector3 cubeMin = cubes[i].Position - 0.5f * cubes[i].Scale;
			Vector3 cubeMax = cubes[i].Position + 0.5f * cubes[i].Scale;

			Vector3 min = new Vector3(Math.Min(playerMin.X, cubeMin.X), Math.Min(playerMin.Y, cubeMin.Y), Math.Min(playerMin.Z, cubeMin.Z));
			Vector3 max = new Vector3(Math.Max(playerMax.X, cubeMax.X), Math.Max(playerMax.Y, cubeMax.Y), Math.Max(playerMax.Z, cubeMax.Z));

			Vector3 common = player.Scale + cubes[i].Scale + min - max;

			if (common.X >= 0 && common.Y >= 0 && common.Z >= 0)
			{
				d = new Vector3();

				if (common.X <= common.Y && common.X <= common.Z)
				{
                    d.X = common.X * (playerMin.X == min.X ? -1 : 1);
					player.Speed.X = 0;
                }

				if (common.Y <= common.X && common.Y <= common.Z)
				{
                    d.Y = common.Y * (playerMin.Y == min.Y ? -1 : 1);
                    player.Speed.Y = 0;

                    player.Position *= Matrix3.CreateRotationY(deltaTime);
                }

				if (common.Z <= common.X && common.Z <= common.Y)
				{
                    d.Z = common.Z * (playerMin.Z == min.Z ? -1 : 1);
                    player.Speed.Z = 0;
                }

				player.Position += d;

                collision = true;
            }
		}

        if (collision && KeyboardState.IsKeyDown(Keys.Space)) player.Speed.Y = player.JumpingSpeed;

        camera.Position = player.Position + 0.25f * player.Scale.Y * Vector3.UnitY;
    }
}