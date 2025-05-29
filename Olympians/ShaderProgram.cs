using System;
using Silk.NET.OpenGL;

namespace Olympians;

public class ShaderProgram : IDisposable, IBindable
{
    private GL _gl;

    private uint _id;

    public ShaderProgram(GL gL)
    {
        _gl = gL;

        _id = _gl.CreateProgram();
    }

    public void Dispose()
    {
        _gl.UseProgram(0);
        _gl.DeleteProgram(_id);
    }

    public void Bind()
    {
        _gl.UseProgram(_id);
    }

    public void Reset()
    {
        _gl.UseProgram(0);
    }

    public void Uniform(string name, int textureunit)
    {
        int location = _gl.GetUniformLocation(_id, name);
        _gl.Uniform1(location, textureunit);
    }

    public void Link(Shader vertex, Shader fragment)
    {
        vertex.Attach(_id);
        fragment.Attach(_id);

        _gl.LinkProgram(_id);

        _gl.GetProgram(_id, ProgramPropertyARB.LinkStatus, out int status);
        if (status != (int)GLEnum.True)
            throw new Exception($"Program failed to link: {_gl.GetProgramInfoLog(_id)}");
    }
}
