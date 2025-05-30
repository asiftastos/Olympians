using System.Drawing;
using ImGuiNET;
using Silk.NET.Core.Native;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace Olympians;

public unsafe class Renderer
{
    private GL _gl;

    private bool _debugDraw;

    public GL GLContext { get { return _gl; } }

    public bool DebugDraw { get { return _debugDraw; } set { _debugDraw = value; }}

    public Action? OnImguiDraw { get; set; }

    public Renderer(IWindow window)
    {
        Console.WriteLine("Initializing Renderer...!!");

        _gl = window.CreateOpenGL();

        Console.WriteLine("OpenGL version: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.Version)));
        Console.WriteLine("Vendor: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.Vendor)));
        Console.WriteLine("Renderer: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.Renderer)));
        Console.WriteLine("GLSL version: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.ShadingLanguageVersion)));

        _gl.ClearColor(Color.CornflowerBlue);

        _debugDraw = false;
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

    public void EndRender(ImGuiController imGui)
    {
        imGui.Render();
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

    public void DrawImgui()
    {
        if (OnImguiDraw != null)
            OnImguiDraw();

        if (_debugDraw)
        {
            _gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(500.0f, 300.0f), ImGuiCond.Once);
            ImGui.ShowMetricsWindow();
        }
        else
        {
            _gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);
        }
    }
}
