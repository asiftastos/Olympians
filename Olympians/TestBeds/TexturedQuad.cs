using System;
using System.Numerics;
using Silk.NET.OpenGL;

namespace Olympians.TestBeds;

public class TexturedQuad : ITestBed
{
    private Game _game;
    private VertexArrayObject _vao;
    private BufferObject _vbo;
    private IndexBufferObject _ebo;
    private ShaderProgram _simpleShaderProgram;
    private Transform _transform;
    private Texture _texture;

    public string Name => "Textured Quad";

    public void Load(Game game)
    {
        _game = game;
        _vao = new VertexArrayObject(_game.Renderer.GLContext);
        _game.Renderer.BindObject(_vao);

        float[] vertices =
        {
            100.0f, 100.0f, 0.0f, 1.0f, 1.0f,
            100.0f, -100.0f, 0.0f, 1.0f, 0.0f,
            -100.0f, -100.0f, 0.0f, 0.0f, 0.0f,
            -100.0f, 100.0f, 0.0f,  0.0f, 1.0f
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
                Stride = 5 * sizeof(float),
                Offset = 0,
                AttributeType = VertexAttribPointerType.Float
            },
            new AttributeInfo{
                AttribIndex = 1,
                Size = 2,
                Stride = 5 * sizeof(float),
                Offset = 3 * sizeof(float),
                AttributeType = VertexAttribPointerType.Float
            }
        });

        _simpleShaderProgram = new ShaderProgram(_game.Renderer.GLContext, new ShaderInfo
        {
            AssetsPath = "Assets/Shaders",
            VertexName = "simplevertex",
            FragmentName = "simplefragment"
        });


        _texture = new Texture(_game.Renderer.GLContext);
        _game.Renderer.BindObject(_texture);
        _texture.LoadFromFile("Assets/Textures/silk.png");

        //always reset (unbind) VAO first, otherwise it will capture the other unbinds for himself
        _game.Renderer.ResetObjects(new IBindable[] { _vao, _vbo, _ebo, _texture });

        _transform = new Transform
        {
            Position = new Vector3(100.0f, 0.0f, 0.0f)
        };
    }

    public void Render(double gametime)
    {
        _game.Renderer.BindObject(_vao);
        _game.Renderer.BindObject(_simpleShaderProgram);
        _game.Renderer.BindObject(_texture);
        _simpleShaderProgram.Uniform("uTexture", 0);
        _simpleShaderProgram.Uniform("view", _transform.ModelMatrix * _game.Renderer.Ortho); //multiplication in reverse order of the shader code
        _game.Renderer.DrawIndexedTriangles(6);
    }

    public void Unload()
    {
        _texture?.Dispose();
        _simpleShaderProgram.Dispose();
        _ebo.Dispose();
        _vbo.Dispose();
        _vao.Dispose();
    }

    public void Update(double gametime)
    {
    }
}
