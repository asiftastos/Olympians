using Silk.NET.OpenGL;

namespace Olympians;

public readonly record struct AttributeInfo(uint AttribIndex, int Size, uint Stride, int Offset, VertexAttribPointerType AttributeType);

public unsafe struct VertexArrayObject : IDisposable, IBindable
{
    private GL _gl;
    private uint _vao;

    public VertexArrayObject(GL gL)
    {
        _gl = gL;
        _vao = _gl.GenVertexArray();
    }

    public void Dispose()
    {
        _gl.DeleteVertexArray(_vao);
    }

    public void Bind()
    {
        _gl.BindVertexArray(_vao);
    }

    public void Reset()
    {
        _gl.BindVertexArray(0);
    }

    public void EnableAttributes(IEnumerable<AttributeInfo> attributes)
    {
        foreach (var item in attributes)
        {
            _gl.EnableVertexAttribArray(item.AttribIndex);
            _gl.VertexAttribPointer(item.AttribIndex, item.Size, item.AttributeType, false, item.Stride, (void*)item.Offset);
        }
    }
}
