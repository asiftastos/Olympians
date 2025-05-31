using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Extensions.ImGui;
using ImGuiNET;

namespace Olympians;

#nullable disable
public class Game : IDisposable
{
    private IWindow _window;

    private IInputContext inputContext;

    private Renderer _renderer;

    private ImGuiController _imgui;

    private bool _exit;

    private bool _showProperties;

    private VertexArrayObject _vao;

    private BufferObject _vbo;

    private IndexBufferObject _ebo;

    private Shader _simpleVertexShader;
    private Shader _simpleFragmentShader;
    private ShaderProgram _simpleShaderProgram;

    private Texture _texture;

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
        _showProperties = false;
    }

    private void OnClosing()
    {
        _imgui?.Dispose();
        _texture.Dispose();
        _simpleVertexShader.Dispose();
        _simpleFragmentShader.Dispose();
        _simpleShaderProgram.Dispose();
        _ebo.Dispose();
        _vbo.Dispose();
        _vao.Dispose();
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

        _renderer = new Renderer(_window);

        _imgui = new ImGuiController(_renderer.GLContext, _window, inputContext);

        for (int i = 0; i < inputContext.Keyboards.Count; i++)
        {
            inputContext.Keyboards[i].KeyDown += OnKeyDown;
        }

        _renderer.OnImguiDraw += DrawImgui;

        LoadTexturedQuad();

        _renderer.EnableBlend();
    }

    private void DrawImgui()
    {
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(0.0f, 0.0f), ImGuiCond.Always);
        ImGui.SetNextWindowSize(new System.Numerics.Vector2(_window.Size.X, 40.0f));
        ImGui.Begin("Game", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar);

        if (ImGui.Button("Properties"))
            _showProperties = true;

        if(_showProperties)
        {
            ImGui.Begin("Renderer properties", ref _showProperties);
            bool debugdraw = _renderer.DebugDraw;
            if (ImGui.Checkbox("Debug draw", ref debugdraw))
                _renderer.DebugDraw = debugdraw;
            if (ImGui.IsItemHovered())
                ImGui.SetItemTooltip("Enable/Disable debug drawing");

            ImGui.End();
        }


        ImGui.SameLine();
        if (ImGui.Button("Exit"))
            _exit = true;

        ImGui.End();
    }

    private void OnUpdate(double deltaTime)
    {
        _imgui.Update((float)deltaTime);

        if (_exit)
            _window.Close();
    }

    private void OnRender(double deltaTime)
    {
        _renderer.DrawImgui();

        _renderer.BeginRender();

        RenderTexturedQuad();

        _renderer.EndRender(_imgui);
    }

    private void OnKeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
            _window.Close();
    }

    private void OnResize(Vector2D<int> d)
    {
        _renderer.Resize(d);
    }

    private void LoadTexturedQuad()
    {
        _vao = new VertexArrayObject(_renderer.GLContext);
        _renderer.BindObject(_vao);

        float[] vertices =
        {
            0.5f, 0.5f, 0.0f, 1.0f, 1.0f,
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.0f,  0.0f, 1.0f
        };

        uint[] indices =
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        _vbo = new BufferObject(_renderer.GLContext);
        _renderer.BindObject(_vbo);
        _vbo.Data(vertices, vertices.Length);

        _ebo = new IndexBufferObject(_renderer.GLContext);
        _renderer.BindObject(_ebo);
        _ebo.Data(indices, indices.Length);

        _simpleVertexShader = new VertexShader(_renderer.GLContext);
        _simpleVertexShader.CompileFromFile("Assets/Shaders/simplevertex.glsl");

        _simpleFragmentShader = new FragmentShader(_renderer.GLContext);
        _simpleFragmentShader.CompileFromFile("Assets/Shaders/simplefragment.glsl");

        _simpleShaderProgram = new ShaderProgram(_renderer.GLContext);
        _simpleShaderProgram.Link(_simpleVertexShader, _simpleFragmentShader);

        _vao.EnableFloatAttribute(0, 3, 5, 0); //vertex data attribute
        _vao.EnableFloatAttribute(1, 2, 5, 3); //texture coordinate data attribute

        _texture = new Texture(_renderer.GLContext);
        _renderer.BindObject(_texture);
        _texture.LoadFromFile("Assets/Textures/silk.png");

        //always reset (unbind) VAO first, otherwise it will capture the other unbinds for himself
        _renderer.ResetObjects(new IBindable[] { _vao, _vbo, _ebo, _texture });

        _simpleShaderProgram.Uniform("uTexture", 0);
    }

    private void RenderTexturedQuad()
    {
        _renderer.BindObject(_vao);
        _renderer.BindObject(_simpleShaderProgram);
        _renderer.BindObject(_texture);
        _renderer.DrawIndexedTriangles(6);
    }
}
