using System;
using System.Reflection.Metadata;
using Silk.NET.Core;
using Silk.NET.OpenGL;

namespace Olympians;

public unsafe struct VertexArrayObject : IDisposable, IBindable
{
    private GL _gl;
    private uint _vao;

    public VertexArrayObject(GL gL)
    {
        _gl = gL;
        _vao = _gl.GenVertexArray();
    }

    public void Bind()
    {
        _gl.BindVertexArray(_vao);
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
    }

    public void EnableFloatAttribute(uint index, int size, uint stride, int offset)
    {
        _gl.EnableVertexAttribArray(index);
        _gl.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, stride * sizeof(float), offset * sizeof(float));
    }

    public void Reset()
    {
        _gl.BindVertexArray(0);
    }
}
