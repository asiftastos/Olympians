using System;
using Silk.NET.OpenGL;

namespace Olympians;

public unsafe class BufferObject : IDisposable, IBindable
{
    private GL _gl;
    private uint _vbo;

    private BufferUsageARB _bufferUsage;

    private uint _byteSize;

    public uint ByteSize { get => _byteSize; }

    public BufferObject(GL gL, BufferUsageARB bufferUsage)
    {
        _gl = gL;

        _vbo = _gl.GenBuffer();

        _bufferUsage = bufferUsage;
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_vbo);
    }

    public void Bind()
    {
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
    }

    public void Reset()
    {
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    public void Data(IEnumerable<float> data, int size)
    {
        _byteSize = (uint)(size * sizeof(float));

        //fixed: don't let GC move this data or the pointer will be incorect as we use it
        fixed (float* d = data.ToArray())
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)_byteSize, d, _bufferUsage);
        }
    }
}
