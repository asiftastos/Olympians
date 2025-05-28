using System;
using Silk.NET.OpenGL;

namespace Olympians;

public abstract class Shader : IDisposable
{
    protected GL _gl;
    protected uint _id;

    protected Shader(GL gL)
    {
        _gl = gL;
    }

    public void Dispose()
    {
        _gl.DeleteShader(_id);
    }

    public void CompileFromFile(string filename)
    {
        string code = File.ReadAllText(filename);
        CompileFromMemory(code);
    }

    public void CompileFromMemory(string code)
    {
        _gl.ShaderSource(_id, code);
        _gl.CompileShader(_id);

        _gl.GetShader(_id, ShaderParameterName.CompileStatus, out int status);
        if (status != (int)GLEnum.True)
            throw new Exception($"Shader failed to compile: {_gl.GetShaderInfoLog(_id)}");
    }

    public void Attach(uint programID)
    {
        _gl.AttachShader(programID, _id);
    }
}
