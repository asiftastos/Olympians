using System;
using System.Numerics;
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
                Stride = 7 * sizeof(float),
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
    }

    public void Unload()
    {
        _simpleShaderProgram.Dispose();
        _ebo.Dispose();
        _vbo.Dispose();
        _vao.Dispose();
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
}
