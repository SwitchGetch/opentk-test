﻿using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

public class Square
{
	public readonly int VertexBufferObject;
	public readonly int VertexArrayObject;
	public readonly int ElementBufferObject;

	private readonly float[] vertices =
	{
		-0.5f, -0.5f,  0.0f, 0.0f,
		 0.5f, -0.5f,  1.0f, 0.0f,
		-0.5f,  0.5f,  0.0f, 1.0f,
		 0.5f,  0.5f,  1.0f, 1.0f
	};

	private readonly uint[] indices =
	{
		0, 1, 2,
		1, 2, 3
	};

	private Vector2 position;
	private float rotation;
	private Vector2 scale;

	public Vector2 Position { get => position; set { position = value; SetModelMatrix(); } }
	public float Rotation { get => rotation; set { rotation = value; SetModelMatrix(); } }
	public Vector2 Scale { get => scale; set { scale = value; SetModelMatrix(); } }

	private Matrix4 model;

	public int shader;
	public int texture;

	public Square(int shader = 0, int texture = 0)
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
		GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

		ElementBufferObject = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
		GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));

		GL.BindVertexArray(0);

		GL.UseProgram(0);


		Position = Vector2.Zero;
		Rotation = 0;
		Scale = Vector2.One;
	}

	public void Render()
	{
		GL.UseProgram(shader);
		GL.BindVertexArray(VertexArrayObject);
		GL.BindTexture(TextureTarget.Texture2D, texture);

		GL.UniformMatrix4(GL.GetUniformLocation(shader, "model"), true, ref model);

		GL.Disable(EnableCap.DepthTest);
		GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

		GL.BindVertexArray(0);
		GL.BindTexture(TextureTarget.Texture2D, 0);
		GL.UseProgram(0);
	}

	private void SetModelMatrix()
	{
		model =
			Matrix4.CreateRotationZ(rotation) *
			Matrix4.CreateScale(new Vector3(scale.X, scale.Y, 0)) *
			Matrix4.CreateTranslation(new Vector3(position.X, position.Y, 0));
	}
}