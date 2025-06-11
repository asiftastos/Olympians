using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Silk.NET.Core.Native;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace Olympians;

public unsafe class Renderer: IImguiWindowProvider
{
    private Game _game;

    private GL _gl;

    private bool _debugDraw;

    private Matrix4x4 _ortho;

    private bool _showImguiWindow;

    public GL GLContext { get { return _gl; } }

    public bool DebugDraw { get { return _debugDraw; } set { _debugDraw = value; }}

    public Matrix4x4 Ortho { get { return _ortho; } }

    public bool Show { get => _showImguiWindow; set => _showImguiWindow = value; }

    public string WindowName => "Renderer";

    public Renderer(Game game)
    {
        _game = game;

        Console.WriteLine("Initializing Renderer...!!");

        _gl = _game.MainWindow.CreateOpenGL();

        Console.WriteLine("OpenGL version: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.Version)));
        Console.WriteLine("Vendor: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.Vendor)));
        Console.WriteLine("Renderer: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.Renderer)));
        Console.WriteLine("GLSL version: " + SilkMarshal.PtrToString((nint)_gl.GetString(GLEnum.ShadingLanguageVersion)));

        _gl.ClearColor(Color.CornflowerBlue);

        _debugDraw = false;

        //0,0 is in the center of the window
        _ortho = Matrix4x4.CreateOrthographic(_game.MainWindow.FramebufferSize.X, _game.MainWindow.FramebufferSize.Y, 0.1f, 1.0f);
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

        _ortho = Matrix4x4.CreateOrthographic(newsize.X, newsize.Y, 0.1f, 1.0f);
    }

    public void BeginRender()
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void EndRender(ImGuiController imGui)
    {
    }

    public void DrawTriangles(uint elementCount)
    {
        _gl.DrawArrays(PrimitiveType.Triangles, 0, elementCount);
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

    public void DisableBlend()
    {
        _gl.Disable(EnableCap.Blend);
    }

    public void EnableDepth()
    {
        _gl.Enable(EnableCap.DepthTest);
    }

    public void DisableDepth()
    {
        _gl.Disable(EnableCap.DepthTest);
    }

    public void DrawImgui()
    {
        if (_game.UI.WindowProviders.TryGetValue("UI", out IImguiWindowProvider windowProvider))
        {
            ImGui.Begin(windowProvider.WindowName);

            ImGui.SameLine();
            if (ImGui.Button("Renderer"))
                _showImguiWindow = true;

            if (ImGui.IsItemHovered())  //refers to the previous item
                ImGui.SetItemTooltip("Open renderer properties window");

            ImGui.End();
        }

        if (_showImguiWindow)
        {
            ImGui.Begin(WindowName, ref _showImguiWindow, ImGuiWindowFlags.None);

            ImGui.Checkbox("Enable debug draw", ref _debugDraw);

            ImGui.End();
        }

        if (_debugDraw)
        {
            _gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Line);

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(500.0f, 300.0f), ImGuiCond.Once);
            if (ImGui.Begin("Debug"))
            {
                ImGui.Text($"FPS: {ImGui.GetIO().Framerate}");

                ImGui.End();
            }
            //ImGui.ShowMetricsWindow();
        }
        else
        {
            _gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);
        }
    }
}
