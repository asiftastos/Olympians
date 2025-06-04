using System;
using Silk.NET.OpenGL;

namespace Olympians;

public unsafe class IndexBufferObject : IDisposable, IBindable
{
    private GL _gl;

    private uint _ebo;

    private BufferUsageARB _bufferUsage;

    private uint _byteSize;

    public uint ByteSize { get => _byteSize; }

    public IndexBufferObject(GL gL, BufferUsageARB bufferUsage)
    {
        _gl = gL;
        _ebo = _gl.GenBuffer();
        _bufferUsage = bufferUsage;
    }

    public void Dispose()
    {
        _gl.DeleteBuffer(_ebo);
    }

    public void Bind()
    {
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
    }

    public void Reset()
    {
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }

    public void Data(IEnumerable<uint> data, int elementCount)
    {
        _byteSize = (uint)(elementCount * sizeof(uint));
        
        //fixed: don't let GC move this data or the pointer will be incorect as we use it
        fixed (uint* buf = data.ToArray())
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)_byteSize, buf, _bufferUsage);
        }
    }
}
