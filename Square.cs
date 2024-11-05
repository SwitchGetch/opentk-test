using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

public class Square
{
	public static readonly int VertexBufferObject;
	public static readonly int VertexArrayObject;
	public static readonly int ElementBufferObject;

	private static readonly float[] vertices =
	{
		-0.5f, -0.5f, 0.0f,  0.0f, 0.0f,
		 0.5f, -0.5f, 0.0f,  1.0f, 0.0f,
		-0.5f,  0.5f, 0.0f,  0.0f, 1.0f,
		 0.5f,  0.5f, 0.0f,  1.0f, 1.0f
	};

	private static readonly uint[] indices =
	{
		0, 1, 2,
		1, 2, 3
	};

	private Vector3 position;
	private Vector2 scale;
	private float rotation;

	public Vector3 Position { get => position; set { position = value; SetModelMatrix(); } }
	public Vector2 Scale { get => scale; set { scale = value; SetModelMatrix(); } }
	public float Rotation { get => rotation; set { rotation = value; SetModelMatrix(); } }

	private Matrix4 model;

	public int texture;

	private float opacity;
	public float Opacity { get => opacity; set => opacity = Math.Clamp(value, 0.0f, 1.0f); }

	static Square()
	{
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
    }

	public Square()
	{
		texture = 0;
		opacity = 1;

		Position = Vector3.Zero;
		Scale = Vector2.One;
		Rotation = 0;
	}

	public void Render()
	{
		GL.BindVertexArray(VertexArrayObject);
		GL.BindTexture(TextureTarget.Texture2D, texture);

        GL.UniformMatrix4(GL.GetUniformLocation(GL.GetInteger(GetPName.CurrentProgram), "model"), true, ref model);
		GL.Uniform1(GL.GetUniformLocation(GL.GetInteger(GetPName.CurrentProgram), "alpha"), opacity);

		GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
	}

	private void SetModelMatrix()
	{
		model =
			Matrix4.CreateRotationZ(rotation) *
			Matrix4.CreateScale(new Vector3(scale)) *
			Matrix4.CreateTranslation(position);
	}
}