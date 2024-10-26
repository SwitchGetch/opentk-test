using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using OpenTK.Windowing.Common.Input;

public class Player : Cube
{
    public Vector3 Speed;
    public Vector3 Acceleration;

    public Player()
    {
        Speed = Vector3.Zero;
        Acceleration = -10 * Vector3.UnitY;
    }
}