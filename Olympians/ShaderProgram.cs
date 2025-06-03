using System;
using System.Text;
using Silk.NET.OpenGL;

namespace Olympians;

public readonly record struct ShaderInfo(string AssetsPath, string VertexName, string FragmentName);

public class ShaderProgram : IDisposable, IBindable
{

    private GL _gl;

    private uint _id;

    public ShaderProgram(GL gL, ShaderInfo shaderInfo)
    {
        _gl = gL;

        if (!string.IsNullOrEmpty(shaderInfo.VertexName) && !string.IsNullOrEmpty(shaderInfo.FragmentName))
        {
            uint vs = CompileFromFile(String.Format("{0}/{1}.glsl", shaderInfo.AssetsPath, shaderInfo.VertexName), ShaderType.VertexShader);
            uint fs = CompileFromFile(String.Format("{0}/{1}.glsl", shaderInfo.AssetsPath, shaderInfo.FragmentName), ShaderType.FragmentShader);

            if (vs > 0 && fs > 0)
            {
                _id = _gl.CreateProgram();

                Link(vs, fs);
            }
        }
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

    private void Link(uint vs, uint fs)
    {
        _gl.AttachShader(_id, vs);
        _gl.AttachShader(_id, fs);

        _gl.LinkProgram(_id);

        _gl.GetProgram(_id, ProgramPropertyARB.LinkStatus, out int status);
        if (status != (int)GLEnum.True)
            throw new Exception($"Program failed to link: {_gl.GetProgramInfoLog(_id)}");

        //no need to keep after program is linked
        _gl.DetachShader(_id, vs);
        _gl.DetachShader(_id, fs);
        _gl.DeleteShader(vs);
        _gl.DeleteShader(fs);       
    }

    private uint CompileFromFile(string filename, ShaderType shaderType)
    {
        if (!File.Exists(filename))
        {
            Console.WriteLine($"Shader file {filename} does not exists!");
            return 0;
        }

        string code = File.ReadAllText(filename);
        return CompileFromMemory(code, shaderType);
    }

    private uint CompileFromMemory(string code, ShaderType shaderType)
    {
        uint id = _gl.CreateShader(shaderType);
        _gl.ShaderSource(id, code);
        _gl.CompileShader(id);

        _gl.GetShader(id, ShaderParameterName.CompileStatus, out int status);
        if (status != (int)GLEnum.True)
            throw new Exception($"Shader failed to compile: {_gl.GetShaderInfoLog(id)}");

        return id;
    }
}
