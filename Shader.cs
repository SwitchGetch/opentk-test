﻿using OpenTK.Graphics.OpenGL4;

public class Shader : IDisposable
{
	public int Handle { get; private set; }
	int VertexShader;
	int FragmentShader;

	public Shader(string vertexShaderPath, string fragmentShaderPath)
	{
		string VertexShaderSource = File.ReadAllText(vertexShaderPath);

		string FragmentShaderSource = File.ReadAllText(fragmentShaderPath);

		VertexShader = GL.CreateShader(ShaderType.VertexShader);
		GL.ShaderSource(VertexShader, VertexShaderSource);

		FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
		GL.ShaderSource(FragmentShader, FragmentShaderSource);

		GL.CompileShader(VertexShader);
		GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int vertexShaderCompileSuccess);
		if (vertexShaderCompileSuccess == 0)
		{
			string infoLog = GL.GetShaderInfoLog(VertexShader);
			Console.WriteLine(infoLog);
		}

		GL.CompileShader(FragmentShader);
		GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int fragmentShaderCompileSuccess);
		if (fragmentShaderCompileSuccess == 0)
		{
			string infoLog = GL.GetShaderInfoLog(FragmentShader);
			Console.WriteLine(infoLog);
		}

		Handle = GL.CreateProgram();

		GL.AttachShader(Handle, VertexShader);
		GL.AttachShader(Handle, FragmentShader);

		GL.LinkProgram(Handle);

		GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int programCompileSuccess);
		if (programCompileSuccess == 0)
		{
			string infoLog = GL.GetProgramInfoLog(Handle);
			Console.WriteLine(infoLog);
		}

		GL.DetachShader(Handle, VertexShader);
		GL.DetachShader(Handle, FragmentShader);
		GL.DeleteShader(FragmentShader);
		GL.DeleteShader(VertexShader);
	}

	public void Use()
	{
		GL.UseProgram(Handle);
	}

	public int GetAttribLocation(string attribName)
	{
		return GL.GetAttribLocation(Handle, attribName);
	}

	private bool disposedValue = false;

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			GL.DeleteProgram(Handle);

			disposedValue = true;
		}
	}

	~Shader()
	{
		if (disposedValue == false)
		{
			Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
		}
	}


	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
}