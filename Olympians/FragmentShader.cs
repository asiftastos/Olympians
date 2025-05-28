using System;
using Silk.NET.OpenGL;

namespace Olympians;

public class FragmentShader : Shader
{
    public FragmentShader(Silk.NET.OpenGL.GL gL) : base(gL)
    {
        _id = _gl.CreateShader(ShaderType.FragmentShader);
    }
}
