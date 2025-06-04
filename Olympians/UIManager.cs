using System;
using ImGuiNET;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace Olympians;

public class UIManager : IDisposable, IImguiWindowProvider
{
    private Game _game;

    private ImGuiController _imgui;

    private bool _showImguiWindow;

    public Action? OnImguiDraw { get; set; }

    public string WindowName => "Game Menu";

    public bool Show { get => true; set => _showImguiWindow = true; }

    public Dictionary<string, IImguiWindowProvider> WindowProviders { get; set; }

    public UIManager(Game game)
    {
        _showImguiWindow = true;
        
        _game = game;

        _imgui = new ImGuiController(_game.Renderer.GLContext, _game.MainWindow, _game.InputContext);

        WindowProviders = new Dictionary<string, IImguiWindowProvider>();
        WindowProviders.Add("UI", this);
    }

    public void Dispose()
    {
        _imgui?.Dispose();
    }

    public void Update(double deltaTime)
    {
        _imgui.Update((float)deltaTime);
    }

    public void Render(double deltaTime)
    {
        _imgui.Render();
    }

    public void DrawUI()
    {
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(0.0f, 0.0f), ImGuiCond.Always);
        ImGui.SetNextWindowSize(new System.Numerics.Vector2(_game.MainWindow.Size.X, 40.0f));
        ImGui.Begin(WindowName, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar);

        ImGui.End();

        if (OnImguiDraw != null)
            OnImguiDraw();
    }
}
