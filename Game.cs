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

    /*float[] vertices =
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
	};*/

    float[] vertices =
	{
		// передняя грань
		-0.5f, -0.5f,  0.5f,   0.0f, 0.0f,
		 0.5f, -0.5f,  0.5f,   1.0f, 0.0f,
		-0.5f,  0.5f,  0.5f,   0.0f, 1.0f,
		 0.5f,  0.5f,  0.5f,   1.0f, 1.0f,

		// задняя грань
        -0.5f, -0.5f, -0.5f,   1.0f, 0.0f,
         0.5f, -0.5f, -0.5f,   0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,   1.0f, 1.0f,
         0.5f,  0.5f, -0.5f,   0.0f, 1.0f,

		// левая грань
        -0.5f, -0.5f,  0.5f,   1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,   1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,   0.0f, 1.0f,

		// правая грань
         0.5f, -0.5f,  0.5f,   0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,   0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,   1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,   1.0f, 1.0f,

		// нижняя грань
        -0.5f, -0.5f,  0.5f,   0.0f, 1.0f,
         0.5f, -0.5f,  0.5f,   1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,   0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,   1.0f, 0.0f,

		// верхняя грань
        -0.5f,  0.5f,  0.5f,   0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,   1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,   0.0f, 1.0f,
         0.5f,  0.5f, -0.5f,   1.0f, 1.0f,
    };

	uint[] indices =
	{
		0, 1, 2,
		1, 2, 3,

		4, 5, 6,
		5, 6, 7,

		8, 9, 10,
		9, 10, 11,

		12, 13, 14,
		13, 14, 15,

		16, 17, 18,
		17, 18, 19,

		20, 21, 22,
		21, 22, 23
	};


    int VertexBufferObject;
	int VertexArrayObject;
	int ElementBufferObject;

	Matrix4 translation;
	Matrix4 scale;
	Matrix4 rotation;
	Matrix4 model;
	Matrix4 view;
	Matrix4 projection;
	float transformSpeed = 1;

	Camera camera;

	Shader shader;
	Texture texture;
	//Texture texture0;
	//Texture texture1;

	Stopwatch timer;
	float time;
	float deltaTime;
	float FPS;
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

		camera = new Camera();
		camera.RotationSpeed = 0.1f;

		translation = Matrix4.CreateTranslation(0, 0, 5);
		scale = Matrix4.Identity;
		rotation = Matrix4.Identity;
		model = Matrix4.Identity;
		view = Matrix4.LookAt(camera.Position, camera.Direction, Vector3.UnitY);
		projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), ClientSize.X / ClientSize.Y, 0.1f, 100.0f);

		texture = new Texture("box.jpg");
		texture.Use();
		shader.SetInt("texture0", 0);

        //texture0 = new Texture("container.jpg");
		//texture1 = new Texture("awesomeface.png");

		//texture0.Use(TextureUnit.Texture0);
		//texture1.Use(TextureUnit.Texture1);

		//shader.SetInt("texture0", 0);
		//shader.SetInt("texture1", 1);


        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);


        VertexBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
		GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        int positionLocation = shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        int texCoordLocation = shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);


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

		//camera.Direction *= Matrix3.CreateRotationY(transformSpeed * deltaTime);

        //translation = Matrix4.CreateTranslation((float)Math.Cos(time), (float)Math.Sin(time), 5);
		rotation *= Matrix4.CreateRotationY(transformSpeed * deltaTime);
		model = rotation * scale * translation;
		view = camera.GetViewMatrix();

		shader.SetMartix4("model", ref model);
		shader.SetMartix4("view", ref view);
		shader.SetMartix4("projection", ref projection);
		
		shader.SetFloat("u_time", (float)timer.Elapsed.TotalSeconds);

		if (KeyboardState.IsKeyDown(Keys.Escape))
		{
			Close();
		}
    }

	protected override void OnRenderFrame(FrameEventArgs args)
	{
		base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

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
			FPS = (int)(frames / (float)(time - seconds));
            seconds = (int)time;
			frames = 0;

            Title = "FPS: " + FPS;
        }
	}

	private void MouseHandler()
	{
        Vector2 d = MousePosition - ClientSize / 2;
        MousePosition = ClientSize / 2;

		Vector2 angle = deltaTime * camera.RotationSpeed * new Vector2(d.Y, -d.X);
        camera.Direction *= Matrix3.CreateRotationX(angle.X) * Matrix3.CreateRotationY(angle.Y);
    }
}