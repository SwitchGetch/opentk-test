using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

public class Camera
{
    private static readonly float HalfPi = 1.57f;
    private static readonly float Pi = 3.14f;
    private static readonly float TwoPi = 6.28f;

    private bool isPerspective;
    public bool IsPerspective { get => isPerspective; set { isPerspective = value; SetProjectionMatrix(); } }

    private Vector2 viewport;
    private float fov;

    public Vector2 Viewport
    {
        get => viewport;
        set { viewport = value; SetProjectionMatrix(); }
    }

    public float FOV
    {
        get => fov;
        set
        {
            fov = value;

            float min = MathHelper.DegreesToRadians(30.0f);
            float max = MathHelper.DegreesToRadians(90.0f);

            if (fov < min) fov = min;
            else if (fov > max) fov = max;

            SetProjectionMatrix();
        }
    }

    private Vector3 position;

    public Vector3 Position
    {
        get => position;
        set { position = value; SetViewMatrix(); }
    }

    public Vector3 Direction { get; private set; }

    public Vector3 Forward { get; private set; }

    private float pitch;
    private float yaw;

    public float Pitch
    {
        get => pitch;
        set
        {
            pitch = value;

            if (pitch < -HalfPi) pitch = -HalfPi;
            else if (pitch > HalfPi) pitch = HalfPi;

            SetDirection();
        }
    }

    public float Yaw
    {
        get => yaw;
        set
        {
            yaw = value;

            if (yaw < -Pi) yaw += TwoPi;
            else if (yaw > Pi) yaw -= TwoPi;

            Forward = Vector3.UnitZ * Matrix3.CreateRotationY(yaw);

            SetDirection();
        }
    }

    private Matrix4 view;
    private Matrix4 projection;

    public Camera(bool IsPerspective = true)
    {
        isPerspective = IsPerspective;

        position = Vector3.Zero;
        Direction = Vector3.UnitZ;

        fov = MathHelper.DegreesToRadians(45.0f);

        Yaw = 0;
        Pitch = 0;
    }

    public void LookAt(Vector3 Target)
    {
        if (Target == Position) return;

        Direction = Vector3.Normalize(Target - Position);

        float l = (float)Math.Sqrt(Direction.X * Direction.X + Direction.Z * Direction.Z);
        float pitch = -(float)Math.Acos(l) * Math.Sign(Direction.Y);
        float yaw = (float)Math.Acos(Direction.Z / l) * Math.Sign(Direction.X);

        Forward = Vector3.UnitZ * Matrix3.CreateRotationY(yaw);

        //float lx = (float)Math.Sqrt(Direction.Y * Direction.Y + Direction.Z * Direction.Z);
        //if (lx > 0) pitch = (float)Math.Acos(Direction.Z / lx) * Math.Sign(Direction.Y);
        //else pitch = 0;

        //float ly = (float)Math.Sqrt(Direction.X * Direction.X + Direction.Z * Direction.Z);
        //if (ly > 0)
        //{
        //    yaw = (float)Math.Acos(Direction.Z / ly) * Math.Sign(-Direction.X);
        //    Forward = Vector3.UnitZ * Matrix3.CreateRotationY(yaw);
        //}

        SetViewMatrix();
    }

    public void Use()
    {
        int shader = GL.GetInteger(GetPName.CurrentProgram);

        GL.UniformMatrix4(GL.GetUniformLocation(shader, "view"), true, ref view);
        GL.UniformMatrix4(GL.GetUniformLocation(shader, "projection"), true, ref projection);
    }

    private void SetDirection()
    {
        Direction = Vector3.UnitZ * Matrix3.CreateRotationX(pitch) * Matrix3.CreateRotationY(yaw);

        SetViewMatrix();
    }

    private void SetViewMatrix()
    {
        view = Matrix4.LookAt(Position, Position + Direction, Vector3.UnitY);
    }

    private void SetProjectionMatrix()
    {
        projection = IsPerspective ? 
            Matrix4.CreatePerspectiveFieldOfView(FOV, viewport.X / viewport.Y, 0.1f, 100.0f) : 
            Matrix4.CreateOrthographicOffCenter(-0.5f * viewport.X, 0.5f * viewport.X, -0.5f * viewport.Y, 0.5f * viewport.Y, 0.1f, 100.0f);
	}
}