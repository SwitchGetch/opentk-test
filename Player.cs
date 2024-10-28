using OpenTK.Mathematics;

public class Player : Cube
{
    public Vector3 Speed;
    public Vector3 Acceleration;

	public float MovingSpeed;
    public float JumpingSpeed;
	public float RotationSpeed;

	public Player(int shader = 0, int texture = 0) : base(shader, texture)
    {
        Speed = Vector3.Zero;
        Acceleration = -10 * Vector3.UnitY;

        MovingSpeed = 1;
        JumpingSpeed = 1;
        RotationSpeed = 1;
    }
}