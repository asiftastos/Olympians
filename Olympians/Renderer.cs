using System.Drawing;
using Silk.NET.Core.Native;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Olympians;

public unsafe class Renderer
{
    private GL _gl;

    public GL GLContext { get { return _gl; } }

    public Renderer(IWindow window)
    {
        Console.WriteLine("Initializing Renderer...!!");

        _gl = window.CreateOpenGL();

        Console.WriteLine("OpenGL version: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.Version)));
        Console.WriteLine("Vendor: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.Vendor)));
        Console.WriteLine("Renderer: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.Renderer)));
        Console.WriteLine("GLSL version: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.ShadingLanguageVersion)));

        _gl.ClearColor(Color.CornflowerBlue);
    }

    public void BindObject(IBindable bobj)
    {
        bobj.Bind();
    }

    public void ResetObjects(IEnumerable<IBindable> bobjs)
    {
        foreach (var obj in bobjs)
        {
            obj.Reset();
        }
    }

    public void Resize(Vector2D<int> newsize)
    {
        _gl.Viewport(newsize);
    }

    public void BeginRender()
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    public void DrawIndexedTriangles(uint elementCount)
    {
        _gl.DrawElements(PrimitiveType.Triangles, elementCount, DrawElementsType.UnsignedInt, (void*)0);
    }

    public void EnableBlend()
    {
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }
}
