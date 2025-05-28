using System;
using Silk.NET.OpenGL;

namespace Olympians;

public class VertexShader : Shader
{
    public VertexShader(GL gL) : base(gL)
    {
        _id = _gl.CreateShader(ShaderType.VertexShader);
    }
}
