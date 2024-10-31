﻿using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

public class Cube
{
	public readonly int VertexBufferObject;
	public readonly int VertexArrayObject;
	public readonly int ElementBufferObject;

	private readonly float[] vertices =
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

	private readonly uint[] indices =
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

    private Vector3 position;
    private Vector3 rotation;
    private Vector3 scale;

    public Vector3 Position { get => position; set { position = value; SetModelMatrix(); } }
	public Vector3 Rotation { get => rotation; set { rotation = value; SetModelMatrix(); } }
    public Vector3 Scale { get => scale; set { scale = value; SetModelMatrix(); } }

    private Matrix4 model;

    public Vector3 Speed;
    public Vector3 Acceleration;

    public int shader;
	public int texture;

	public Cube(int shader = 0, int texture = 0)
	{
		this.shader = shader;
		this.texture = texture;

		GL.UseProgram(shader);

		VertexArrayObject = GL.GenVertexArray();
		GL.BindVertexArray(VertexArrayObject);

		VertexBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
		GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

		ElementBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

		GL.BindVertexArray(0);

		GL.UseProgram(0);


		Position = Vector3.Zero;
		Rotation = Vector3.Zero;
		Scale = Vector3.One;

		Speed = Vector3.Zero;
		Acceleration = Vector3.Zero;
	}

	public void Render()
	{
		SetModelMatrix();

		GL.UseProgram(shader);
		GL.BindVertexArray(VertexArrayObject);
		GL.BindTexture(TextureTarget.Texture2D, texture);

		GL.UniformMatrix4(GL.GetUniformLocation(shader, "model"), true, ref model);

		GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

		GL.BindVertexArray(0);
		GL.BindTexture(TextureTarget.Texture2D, 0);
		GL.UseProgram(0);
	}

	private void SetModelMatrix()
	{
        model =
		Matrix4.CreateRotationX(Rotation.X) *
		Matrix4.CreateRotationY(Rotation.Y) *
		Matrix4.CreateRotationZ(Rotation.Z) *
		Matrix4.CreateScale(Scale) *
		Matrix4.CreateTranslation(Position);
	}
}