using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using OpenTK.Windowing.Common.Input;

public class Cube
{
	public static readonly int VertexBufferObject;
	public static readonly int VertexArrayObject;
	public static readonly int ElementBufferObject;

	private static readonly float[] vertices =
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

	private static readonly uint[] indices =
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

	public static int shader;

	static Cube()
	{
		VertexArrayObject = GL.GenVertexArray();
		GL.BindVertexArray(VertexArrayObject);

		VertexBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
		GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

		ElementBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

		GL.BindVertexArray(0);
	}

	public Vector3 Position;
	public Vector3 Rotation;
	public Vector3 Scale;

	private Matrix4 model;

	public int texture;

	public Cube(int texture = 0)
	{
		Position = Vector3.Zero;
		Rotation = Vector3.Zero;
		Scale = Vector3.One;

		model = Matrix4.Identity;

		this.texture = texture;
	}

	public void Render()
	{
		model =
			Matrix4.CreateRotationX(Rotation.X) *
			Matrix4.CreateRotationY(Rotation.Y) *
			Matrix4.CreateRotationZ(Rotation.Z) *
			Matrix4.CreateScale(Scale) *
			Matrix4.CreateTranslation(Position);

		GL.UniformMatrix4(GL.GetUniformLocation(shader, "model"), true, ref model);

		GL.BindVertexArray(VertexArrayObject);
		GL.BindTexture(TextureTarget.Texture2D, texture);

		GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

		GL.BindVertexArray(0);
		GL.BindTexture(TextureTarget.Texture2D, 0);
	}
}