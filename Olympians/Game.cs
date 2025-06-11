using Silk.NET.Windowing;
using Silk.NET.Input;
using ImGuiNET;
using Silk.NET.OpenGL;
using System.Numerics;
using Silk.NET.Maths;
using Olympians.TestBeds;

namespace Olympians;

#nullable disable
public class Game : IDisposable, IImguiWindowProvider
{
    private IWindow _window;

    private IInputContext inputContext;

    private Renderer _renderer;

    private UIManager _uiManager;

    private bool _exit;

    private bool _showImguiWindow;

    private ITestBed _currentTestBed;

    private Dictionary<string, ITestBed> _testbedRegistry;

    public IWindow MainWindow { get => _window; }

    public Renderer Renderer { get => _renderer; }

    public IInputContext InputContext { get => inputContext; }

    public UIManager UI { get => _uiManager; }
    public bool Show { get => _showImguiWindow; set => _showImguiWindow = value; }

    public string WindowName => "Game";

    public Game()
    {
        WindowOptions windowOptions = WindowOptions.Default with
        {
            Size = new Vector2D<int>(1280, 768),
            Title = "Olympians"
        };

        _window = Window.Create(windowOptions);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClosing;
        _window.FramebufferResize += OnResize;

        _exit = false;
        _showImguiWindow = false;

        _testbedRegistry = new Dictionary<string, ITestBed>();
    }

    private void OnClosing()
    {
        if (_currentTestBed != null)
            _currentTestBed.Unload();

        _uiManager.Dispose();
    }

    public void Dispose()
    {
        Console.WriteLine("Exiting Game....!!");
    }

    public void Start()
    {
        _window.Run();
    }

    private void OnLoad()
    {
        inputContext = _window.CreateInput();

        for (int i = 0; i < inputContext.Keyboards.Count; i++)
        {
            inputContext.Keyboards[i].KeyDown += OnKeyDown;
        }

        _renderer = new Renderer(this);

        _uiManager = new UIManager(this);

        _uiManager.WindowProviders.Add("Game", this);
        _uiManager.WindowProviders.Add("Renderer", _renderer);

        _uiManager.OnImguiDraw += _renderer.DrawImgui;
        _uiManager.OnImguiDraw += DrawImgui;

        _renderer.EnableBlend();

        RegisterTestBed(new ColoredQuad());
        RegisterTestBed(new TexturedQuad());
    }

    private void DrawImgui()
    {
        //add to the ui game menu stuff we need
        if (_uiManager.WindowProviders.TryGetValue("UI", out IImguiWindowProvider imguiWindow))
        {
            ImGui.Begin(imguiWindow.WindowName);

            ImGui.SameLine();
            if (ImGui.Button("Game"))
                _showImguiWindow = true;

            if (ImGui.IsItemHovered())  //refers to the previous item
                ImGui.SetItemTooltip("Open game properties window");

            ImGui.SameLine();
            if (ImGui.Button("Exit"))
                _exit = true;

            ImGui.End();
        }

        if (_showImguiWindow)
        {
            ImGui.Begin(WindowName, ref _showImguiWindow, ImGuiWindowFlags.None);

            //draw testbeds
            foreach (var item in _testbedRegistry)
            {
                if (ImGui.Button(item.Key))
                {
                    if (_currentTestBed != null)
                        _currentTestBed.Unload();

                    _currentTestBed = item.Value;
                    _currentTestBed.Load(this);
                }
            }

            ImGui.End();
        }
    }

    private void OnUpdate(double deltaTime)
    {
        if (_currentTestBed != null)
            _currentTestBed.Update(deltaTime);

        _uiManager.Update(deltaTime);

        if (_exit)
            _window.Close();
    }

    private void OnRender(double deltaTime)
    {
        _uiManager.DrawUI();

        _renderer.BeginRender();

        if (_currentTestBed != null)
            _currentTestBed.Render(deltaTime);

        _uiManager.Render(deltaTime);
    }

    private void OnKeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
            _exit = true;
    }

    private void OnResize(Vector2D<int> d)
    {
        _renderer.Resize(d);
    }

    private void RegisterTestBed(ITestBed testBed)
    {
        _testbedRegistry.Add(testBed.Name, testBed);
    }
}
