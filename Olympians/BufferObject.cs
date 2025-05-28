using System;
using Silk.NET.OpenGL;

namespace Olympians;

public unsafe class BufferObject : IDisposable, IBindable
{
    private GL _gl;
    private uint _vbo;

    public BufferObject(GL gL)
    {
        _gl = gL;

        _vbo = _gl.GenBuffer();
    }

    public void Bind()
    {
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_vbo);
    }

    public void Data(IEnumerable<float> data, int size)
    {
        fixed (float* d = data.ToArray())
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(size * sizeof(float)), d, BufferUsageARB.StaticDraw);
        }
    }

    public void Reset()
    {
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }
}
