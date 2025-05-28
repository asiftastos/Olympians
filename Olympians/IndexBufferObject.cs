using System;
using Silk.NET.OpenGL;

namespace Olympians;

public unsafe class IndexBufferObject : IDisposable, IBindable
{
    private GL _gl;

    private uint _ebo;

    public IndexBufferObject(GL gL)
    {
        _gl = gL;
        _ebo = _gl.GenBuffer();
    }

    public void Bind()
    {
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_ebo);
    }

    public void Data(IEnumerable<uint> data, int size)
    {
        fixed (uint* buf = data.ToArray())
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(size * sizeof(uint)), buf, BufferUsageARB.StaticDraw);
        }
    }

    public void Reset()
    {
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }
}
