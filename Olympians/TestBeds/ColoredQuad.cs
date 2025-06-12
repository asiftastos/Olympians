using System;
using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;

namespace Olympians.TestBeds;

public class ColoredQuad : ITestBed
{
    private Game _game;
    private VertexArrayObject _vao;
    private BufferObject _vbo;
    private IndexBufferObject _ebo;
    private ShaderProgram _simpleShaderProgram;
    private Transform _transform;

    private int _verticesCount;
    private int _indicesCount;
    private int _elementsCount;
    private int _elementSize;

    private int _stride;

    private bool _showImguiWindow;

    public string Name => "Colored Quad";

    public void Load(Game game)
    {
        _game = game;

        _vao = new VertexArrayObject(_game.Renderer.GLContext);
        _game.Renderer.BindObject(_vao);

        float[] vertices =
        {
            100.0f, 100.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            100.0f, -100.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            -100.0f, -100.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f,
            -100.0f, 100.0f, 0.0f,  0.0f, 1.0f, 1.0f, 1.0f
        };

        uint[] indices =
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        _verticesCount = vertices.Length;
        _indicesCount = indices.Length;
        _elementSize = 7;
        _elementsCount = (int)(_verticesCount / _elementSize);
        _stride = _elementSize * sizeof(float);

        _vbo = new BufferObject(_game.Renderer.GLContext, BufferUsageARB.StaticDraw);
        _game.Renderer.BindObject(_vbo);
        _vbo.Data(vertices, vertices.Length);

        _ebo = new IndexBufferObject(_game.Renderer.GLContext, BufferUsageARB.StaticDraw);
        _game.Renderer.BindObject(_ebo);
        _ebo.Data(indices, indices.Length);

        _vao.EnableAttributes(new[]{
            new AttributeInfo{
                AttribIndex = 0,
                Size = 3,
                Stride = (uint)_stride,
                Offset = 0,
                AttributeType = VertexAttribPointerType.Float
            },
            new AttributeInfo{
                AttribIndex = 1,
                Size = 4,
                Stride = 7 * sizeof(float),
                Offset = 3 * sizeof(float),
                AttributeType = VertexAttribPointerType.Float
            }
        });

        _simpleShaderProgram = new ShaderProgram(_game.Renderer.GLContext, new ShaderInfo
        {
            AssetsPath = "Assets/Shaders",
            VertexName = "colorvertex",
            FragmentName = "colorfragment"
        });


        //always reset (unbind) VAO first, otherwise it will capture the other unbinds for himself
        _game.Renderer.ResetObjects(new IBindable[] { _vao, _vbo, _ebo });

        _transform = new Transform
        {
            Position = new Vector3(100.0f, 0.0f, 0.0f)
        };

        _showImguiWindow = true;
        _game.UI.OnImguiDraw += DrawImgui;
    }

    public void Unload()
    {
        _simpleShaderProgram.Dispose();
        _ebo.Dispose();
        _vbo.Dispose();
        _vao.Dispose();

        _showImguiWindow = false;
        _game.UI.OnImguiDraw -= DrawImgui;
    }

    public void Render(double gametime)
    {
        _game.Renderer.BindObject(_vao);
        _game.Renderer.BindObject(_simpleShaderProgram);
        _simpleShaderProgram.Uniform("view", _transform.ModelMatrix * _game.Renderer.Ortho); //multiplication in reverse order of the shader code
        _game.Renderer.DrawIndexedTriangles(6);
    }

    public void Update(double gametime)
    {

    }

    private void DrawImgui()
    {
        if (!_showImguiWindow)
            return;

        ImGui.Begin(Name, ref _showImguiWindow, ImGuiWindowFlags.AlwaysAutoResize);
        
        ImGui.LabelText("Number of Vertices", $"{_verticesCount}");
        ImGui.LabelText("Number of Indices", $"{_indicesCount}");
        ImGui.LabelText("Number of Elements", $"{_elementsCount}");
        ImGui.LabelText("Element size", $"{_elementSize} Bytes");
        ImGui.LabelText("Stride", $"{_stride} Bytes");
        ImGui.LabelText("Buffer size", $"{_stride * _elementsCount} Bytes");
        
        ImGui.End();
    }
}
