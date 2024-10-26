using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;

public class Camera
{
    private static readonly float HalfPi = 1.57f;
    private static readonly float Pi = 3.14f;
    private static readonly float TwoPi = 6.28f;

    public static int shader;

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

    public float MovingSpeed;
    public float RotationSpeed;

    public Camera()
    {
        Position = Vector3.Zero;
        Direction = Vector3.UnitZ;

        FOV = MathHelper.DegreesToRadians(45.0f);

        Yaw = 0;
        Pitch = 0;

        MovingSpeed = 1;
        RotationSpeed = 1;
    }

    public void LookAt(Vector3 Target)
    {
        if (Target == Position) return;

        Direction = Vector3.Normalize(Target - Position);

        // рассчитать углы

        SetViewMatrix();
    }

    private void SetDirection()
    {
        Direction = Vector3.UnitZ * Matrix3.CreateRotationX(pitch) * Matrix3.CreateRotationY(yaw);

        SetViewMatrix();
    }

    private void SetViewMatrix()
    {
        view = Matrix4.LookAt(Position, Position + Direction, Vector3.UnitY);

        GL.UniformMatrix4(GL.GetUniformLocation(shader, "view"), true, ref view);
    }

    private void SetProjectionMatrix()
    {
        projection = Matrix4.CreatePerspectiveFieldOfView(FOV, viewport.X / viewport.Y, 0.1f, 100.0f);

        GL.UniformMatrix4(GL.GetUniformLocation(shader, "projection"), true, ref projection);
    }
}

//public class Camera
//{
//    private readonly float HalfPi = (float)Math.PI / 2;
//    private readonly float Pi = (float)Math.PI;
//    private readonly float TwoPi = (float)Math.PI * 2;

//    public Vector3 Position = new Vector3();
//    public float MoveSpeed = 5;
//    public float RotateSpeed = 0.25f;

//    private Vector3 MoveDirection = new Vector3(0, 0, 1);
//	private Vector3 direction = new Vector3(0, 0, 1);

//    public Vector3 Direction
//    {
//        get => direction;
//        set
//        {
//            direction = value;
//            direction.Normalize();

//            float dot = direction.X * MoveDirection.X + direction.Z * MoveDirection.Z;
//            angle.X = (float)Math.Acos(dot) * Math.Sign(direction.Y);
//            angle.Y = (float)Math.Acos(MoveDirection.Z) * Math.Sign(-direction.X);
//		}
//    }

//	private Vector2 angle = new Vector2();

//    public Vector2 Angle
//    {
//        get => angle;
//        set
//        {
//            angle = value;

//            if (angle.X > HalfPi) angle.X = HalfPi;
//            else if (angle.X < -HalfPi) angle.X = -HalfPi;
//            if (angle.Y > Pi) angle.Y -= TwoPi;
//            else if (angle.Y < -Pi) angle.Y += TwoPi;

//            Vector4 d = new Vector4(0, 0, 1, 1) * Matrix4.CreateRotationY(angle.Y);
//            MoveDirection = new Vector3(d);
//            d *= Matrix4.CreateRotationX(angle.X);
//            direction = new Vector3(d);
//		}
//    }

//	public string Stats
//    {
//        get
//        {
//            return
//                $"\n Position:\n X: {Position.X:F7}\n Y: {Position.Y:F7}\n Z: {Position.Z:F7}" +
//                $"\n\n Direction:\n X: {Direction.X:F7}\n Y: {Direction.Y:F7}\n Z: {Direction.Z:F7}" +
//                $"\n\n Move Direction:\n X: {MoveDirection.X:F7}\n Z: {MoveDirection.Z:F7}" +
//                $"\n\n Rotate Angle:\n X: {Angle.X:F7}\n Y: {Angle.Y:F7}" +
//                $"\n\n Move Speed: {MoveSpeed}\n\n Rotate Speed: {RotateSpeed}";
//        }
//    }

//    private void AngleSetter() { }

//    public void Move()
//    {
//        Vector3 D = new Vector3();

//        if (Keyboard.IsKeyPressed(Keyboard.Key.W)) D += MoveDirection;
//        if (Keyboard.IsKeyPressed(Keyboard.Key.S)) D -= MoveDirection;
//        if (Keyboard.IsKeyPressed(Keyboard.Key.A)) D += new Vector3(-MoveDirection.Z, 0, MoveDirection.X);
//        if (Keyboard.IsKeyPressed(Keyboard.Key.D)) D -= new Vector3(-MoveDirection.Z, 0, MoveDirection.X);
//        if (Keyboard.IsKeyPressed(Keyboard.Key.Space)) D.Y++;
//        if (Keyboard.IsKeyPressed(Keyboard.Key.LShift)) D.Y--;

//        Position += Game.DeltaTime * MoveSpeed * Vector.Normalize(D);
//    }

//    public void Rotate()
//    {
//        if (MouseMove.Difference.X == 0 && MouseMove.Difference.Y == 0) return;

//        Angle += -Game.DeltaTime * RotateSpeed * new Vector2f(MouseMove.Difference.Y, MouseMove.Difference.X);


//		/*float k = -Game.DeltaTime * RotateSpeed;

//        if (MouseMove.Difference.X != 0)
//        {
//            float ay = k * MouseMove.Difference.X;
//            Direction = Vector.Rotate(Direction, ay, Axis.Y);
//            angle.Y += ay;
//        }

//        if (MouseMove.Difference.Y != 0)
//        {
//            float ax = k * MouseMove.Difference.Y;
//            Direction = Vector.Rotate(Direction, ax, Axis.X);
//            angle.X += ax;
//        }*/
//    }
//}