using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using Newtonsoft.Json;

public class Game : GameWindow
{
    public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

    public Vector2 WindowSize { get => ClientSize; }

    Color4 background;

    Shader shader;

    Texture box;
    Texture stone;
    Texture sky;
    Texture cross;

    Player player;
    List<Cube> cubes = new List<Cube>();
    Square crosshair;
    Cube skybox;

    List<Cube> bulletTrails = new List<Cube>();
    List<Cube> bulletTraces = new List<Cube>();
    float trailFadeOut = 1.0f;
    float traceFadeOut = 1.0f;
    float trailOffset = 0.0f;
    float traceOffset = 1.0f;
    bool renderTrails = true;
    bool renderTraces = true;

    float spreadAngle = 0.1f;
    float maxBulletDist = 1000.0f;
    int bulletCountForClick = 1;
    float ShotInterval = 0.1f;
    float ShotTimer = 0;
    bool ShotAbility = true;

    Camera camera;
    Camera camera2D;

    float time = 0;
    float deltaTime = 0;
    float fps = 0;
    float seconds = 0;
    float frames = 0;

    Random random = new Random();


    protected override void OnLoad()
    {
        base.OnLoad();

        background = new Color4(0.27f, 0.28f, 0.3f, 1.0f);

        GL.ClearColor(background);

        CursorState = CursorState.Grabbed;

        shader = new Shader("shader.vert", "shader.frag");
        shader.Use();

        box = new Texture("box.jpg");
        stone = new Texture("stone.jpg");
        sky = new Texture("sky.jpg");
        cross = new Texture("crosshair.png");

        player = new Player();
        player.Position = new Vector3(0, 5, 0);
        player.Scale = new Vector3(0.5f, 1, 0.5f);
        player.MovingSpeed = 5;
        player.RotationSpeed = 0.001f;
        player.JumpingSpeed = 5;

        cubes.Add(new Cube() { texture = stone.Handle, Position = new Vector3(0, -0.5f, 0), Scale = new Vector3(40, 1, 40) });
        //cubes.Add(new Cube() { texture = sky.Handle, Position = new Vector3(0, 39.5f, 0), Scale = new Vector3(40, 1, 40) });
        //cubes.Add(new Cube() { texture = sky.Handle, Position = new Vector3(-20.5f, 19.5f, 0), Scale = new Vector3(1, 39, 40) });
        //cubes.Add(new Cube() { texture = sky.Handle, Position = new Vector3(20.5f, 19.5f, 0), Scale = new Vector3(1, 39, 40) });
        //cubes.Add(new Cube() { texture = sky.Handle, Position = new Vector3(0, 19.5f, -20.5f), Scale = new Vector3(40, 39, 1) });
        //cubes.Add(new Cube() { texture = sky.Handle, Position = new Vector3(0, 19.5f, 20.5f), Scale = new Vector3(40, 39, 1) });

        for (int x = -16; x <= 16; x += 4)
        {
            for (int y = 0; y <= 0; y += 4)
            {
                for (int z = -16; z <= 16; z += 4)
                {
                    Vector3 position = new Vector3();
                    Vector3 scale = new Vector3();

                    if ((Math.Abs(x) % 8 == 0) != (Math.Abs(z) % 8 == 0))
                    {
                        position = new Vector3(x, y + 0.5f, z);
                        scale = Vector3.One;
                    }
                    else
                    {
                        position = new Vector3(x, y + 1, z);
                        scale = Vector3.One * 2;
                    }

                    cubes.Add(new Cube() { texture = box.Handle, Position = position, Scale = scale });
                }
            }
        }

        camera = new Camera();
        camera.Viewport = ClientSize;
        camera.FOV = MathHelper.DegreesToRadians(60.0f);

        camera2D = new Camera(false);
        camera2D.Viewport = ClientSize;
        camera2D.LookAt(-Vector3.UnitZ);

        crosshair = new Square();
        crosshair.texture = cross.Handle;
        crosshair.Position = new Vector3(0, 0, -1);
        crosshair.Scale = 50 * Vector2.One;

        skybox = new Cube();
        skybox.texture = sky.Handle;
        skybox.Scale = 100 * Vector3.One;

        shader.SetVector2("resolution", (Vector2)ClientSize);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _ = Task.Run(Client.ConnectToServer);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        TimeHandler(args.Time);
        MouseHandler();
        KeyboardHandler();

        UpdateBullets();

        /*for (int i = 1; i < cubes.Count; i++)
		{
			cubes[i].Speed = cubes[i].Position * Matrix3.CreateRotationY(deltaTime) - cubes[i].Position;
		}*/

        UpdatePlayer();

        skybox.Position = player.Position;

        Client.SetPlayerTransform(player.Position, player.Scale, player.Rotation);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        camera.Use();

        skybox.Render();

        for (int i = 0; i < cubes.Count; i++) cubes[i].Render();

        for (int i = 0; i < Client.Players.Count; i++)
        {
            try
            {
                Client.Players[i].Render();
            }
            catch
            {
                continue;
            }
        }

        if (renderTraces)
        {
            for (int i = bulletTraces.Count - 1; i >= 0; i--)
            {
                bulletTraces[i].Render();
            }

            for (int i = Client.bulletTraces.Count - 1; i >= 0; i--)
            {
                try
                {
                    Client.bulletTraces[i].Render();
                }
                catch
                {
                    continue;
                }
            }
        }

        if (renderTrails)
        {
            for (int i = bulletTrails.Count - 1; i >= 0; i--)
            {
                bulletTrails[i].Render();
            }

            for (int i = Client.bulletTrails.Count - 1; i >= 0; i--)
            {
                try
                {
                    Client.bulletTrails[i].Render();
                }
                catch
                {
                    continue;
                }
            }
        }

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

        shader.SetVector2("resolution", ClientSize);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        switch (e.Button)
        {
            case MouseButton.Left:

                if (ShotAbility)
                {
                    for (int i = 0; i < bulletCountForClick; i++)
                    {
                        Shoot(Spread(camera.Direction, spreadAngle));
                    }

                    ShotAbility = false;
                }

                break;
        }
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

        shader.SetFloat("time", time);

        if (seconds < (int)time)
        {
            fps = (int)(frames / (float)(time - seconds));
            seconds = (int)time;
            frames = 0;

            Title = $"FPS: {fps}";
        }

        ShotTimer += deltaTime;

        if (ShotTimer >= ShotInterval)
        {
            ShotTimer = 0;
            ShotAbility = true;
        }
    }

    private void MouseHandler()
    {
        if (MouseState.IsButtonDown(MouseButton.Right))
        {
            if (ShotAbility)
            {
                for (int i = 0; i < bulletCountForClick; i++)
                {
                    Shoot(Spread(camera.Direction, spreadAngle));
                }

                ShotAbility = false;
            }
        }

        Vector2 d = MouseState.Delta;

        if (d.X == 0 && d.Y == 0) return;

        camera.Pitch += player.RotationSpeed * d.Y;
        camera.Yaw += player.RotationSpeed * -d.X;
    }

    private void KeyboardHandler()
    {
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();
    }

    private void UpdateBullets()
    {
        for (int i = 0; i < bulletTrails.Count; i++)
        {
            bulletTrails[i].Opacity -= deltaTime / trailFadeOut;

            if (bulletTrails[i].Opacity <= 0)
            {
                bulletTrails.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < Client.bulletTrails.Count; i++)
        {
            try
            {
                Client.bulletTrails[i].Opacity -= deltaTime / trailFadeOut;

                if (Client.bulletTrails[i].Opacity <= 0)
                {
                    Client.bulletTrails.RemoveAt(i);
                    i--;
                }
            }
            catch
            {
                continue;
            }
        }

        for (int i = 0; i < bulletTraces.Count; i++)
        {
            bulletTraces[i].Opacity -= deltaTime / traceFadeOut;

            if (bulletTraces[i].Opacity <= 0)
            {
                bulletTraces.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < Client.bulletTraces.Count; i++)
        {
            try
            {
                Client.bulletTraces[i].Opacity -= deltaTime / traceFadeOut;

                if (Client.bulletTraces[i].Opacity <= 0)
                {
                    Client.bulletTraces.RemoveAt(i);
                    i--;
                }
            }
            catch
            {
                continue;
            }
        }
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

        bool jumbAbility = false;

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
                if (common.X <= common.Y && common.X <= common.Z)
                {
                    if (playerMin.X == min.X)
                    {
                        player.Position -= new Vector3(common.X, 0, 0);
                    }
                    else
                    {
                        player.Position += new Vector3(common.X, 0, 0);
                    }

                    player.Speed.X = 0;
                }

                if (common.Y <= common.X && common.Y <= common.Z)
                {
                    if (playerMin.Y == min.Y)
                    {
                        player.Position -= new Vector3(0, common.Y, 0);
                    }
                    else
                    {
                        player.Position += new Vector3(0, common.Y, 0);

                        jumbAbility = true;
                    }

                    player.Speed.Y = 0;
                }

                if (common.Z <= common.X && common.Z <= common.Y)
                {
                    if (playerMin.Z == min.Z)
                    {
                        player.Position -= new Vector3(0, 0, common.Z);
                    }
                    else
                    {
                        player.Position += new Vector3(0, 0, common.Z);
                    }

                    player.Speed.Z = 0;
                }
            }
        }

        //jumbAbility = true;

        if (jumbAbility && KeyboardState.IsKeyDown(Keys.Space)) player.Speed.Y = player.JumpingSpeed;

        camera.Position = player.Position + 0.25f * player.Scale.Y * Vector3.UnitY;
    }

    private void Shoot(Vector3 Direction)
    {
        int iNearest = -1;
        float tNearest = -1;

        for (int i = 0; i < cubes.Count; i++)
        {
            Vector3 tMin = (cubes[i].Position - 0.5f * cubes[i].Scale - camera.Position) / Direction;
            Vector3 tMax = (cubes[i].Position + 0.5f * cubes[i].Scale - camera.Position) / Direction;

            if (tMin.X > tMax.X) (tMin.X, tMax.X) = (tMax.X, tMin.X);
            if (tMin.Y > tMax.Y) (tMin.Y, tMax.Y) = (tMax.Y, tMin.Y);
            if (tMin.Z > tMax.Z) (tMin.Z, tMax.Z) = (tMax.Z, tMin.Z);

            float tEnter = new float[] { tMin.X, tMin.Y, tMin.Z }.Max();
            float tExit = new float[] { tMax.X, tMax.Y, tMax.Z }.Min();

            if (tEnter <= tExit && tExit > 0)
            {
                if (iNearest != -1)
                {
                    if (tNearest > tEnter)
                    {
                        iNearest = i;
                        tNearest = tEnter;
                    }
                }
                else
                {
                    iNearest = i;
                    tNearest = tEnter;
                }
            }
        }

        Vector3 hit = camera.Position + Direction * (iNearest != -1 ? tNearest : maxBulletDist);
        Vector3 dir = Vector3.Normalize(hit - player.Position);
        float dist = Vector3.Distance(player.Position, hit);

        Cube trail = new Cube();
        trail.Scale = new Vector3(0.05f, 0.05f, maxBulletDist);
        trail.Rotation = FindRotation(dir);
        trail.Position = player.Position + 0.5f * maxBulletDist * dir;
        //trail.Opacity += trailOffset / trailFadeOut;
        trail.Opacity = 0.5f;

        if (iNearest != -1)
        {
            trail.Scale = new Vector3(0.05f, 0.05f, dist);
            trail.Position = player.Position + 0.5f * dist * dir;

            Cube trace = new Cube();
            trace.Scale = new Vector3(0.1f);
            trace.Position = hit;
            trace.Opacity += traceOffset / traceFadeOut;

            bulletTraces.Add(trace);
            Client.newTraces.Add(trace);
        }

        bulletTrails.Add(trail);
        Client.newTrails.Add(trail);
    }

    private Vector3 Spread(Vector3 Direction, float Angle)
    {
        return Direction *
            Matrix3.CreateRotationX(((float)random.NextDouble() - 0.5f) * Angle) *
            Matrix3.CreateRotationY(((float)random.NextDouble() - 0.5f) * Angle) *
            Matrix3.CreateRotationZ(((float)random.NextDouble() - 0.5f) * Angle);
    }

    private Vector3 FindRotation(Vector3 Direction)
    {
        float l = (float)Math.Sqrt(Direction.X * Direction.X + Direction.Z * Direction.Z);
        float pitch = -(float)Math.Acos(l) * Math.Sign(Direction.Y);
        float yaw = (float)Math.Acos(Direction.Z / l) * Math.Sign(Direction.X);

        return new Vector3(pitch, yaw, 0);
    }
}