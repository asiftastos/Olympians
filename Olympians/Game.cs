using Silk.NET.Windowing;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace Olympians;

#nullable disable
public class Game : IDisposable
{
    private IWindow _window;

    private Renderer _renderer;

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
        _window.Resize += OnResize;
    }

    private void OnClosing()
    {
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
        IInputContext inputContext = _window.CreateInput();
        for (int i = 0; i < inputContext.Keyboards.Count; i++)
        {
            inputContext.Keyboards[i].KeyDown += OnKeyDown;
        }

        _renderer = new Renderer(_window);

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

        _simpleShaderProgram.UniformTexture("uTexture", 0);

        _renderer.EnableBlend();
    }

    private void OnUpdate(double deltaTime)
    {

    }

    private void OnRender(double deltaTime)
    {
        _renderer.BeginRender();

        _renderer.BindObject(_vao);
        _renderer.BindObject(_simpleShaderProgram);
        _renderer.BindObject(_texture);
        _renderer.DrawIndexedTriangles(6);
    }

    private void OnKeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Escape)
            _window.Close();
    }

    private void OnResize(Vector2D<int> d)
    {
        throw new NotImplementedException();
    }
}
