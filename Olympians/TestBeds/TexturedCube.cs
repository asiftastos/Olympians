using System;
using System.Numerics;
using Silk.NET.OpenGL;

namespace Olympians.TestBeds;

public class TexturedCube : ITestBed
{
    private Game _game;
    private VertexArrayObject _vao;
    private BufferObject _vbo;
    private ShaderProgram _shader;
    private Texture _texture;

    private Transform _transform;

    private Matrix4x4 _view;
    private Matrix4x4 _projection;


    //camera
    private Vector3 _camPosition;
    private Vector3 _camTarget;
    private Vector3 _camDirection;
    private Vector3 _camRight;
    private Vector3 _camUp;


    public string Name => "Textured Cube";

    public void Load(Game game)
    {
        _game = game;

        float[] Vertices =
        {
            //X    Y      Z     U   V
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };

        _vao = new VertexArrayObject(_game.Renderer.GLContext);
        _game.Renderer.BindObject(_vao);

        _vbo = new BufferObject(_game.Renderer.GLContext, BufferUsageARB.StaticDraw);
        _game.Renderer.BindObject(_vbo);
        _vbo.Data(Vertices, Vertices.Length);

        _vao.EnableAttributes(new[]{
            new AttributeInfo{
                AttribIndex = 0,
                Size = 3,
                AttributeType = VertexAttribPointerType.Float,
                Stride = 5 * sizeof(float),
                Offset=  0
            },
            new AttributeInfo{
                AttribIndex = 1,
                Size = 2,
                AttributeType = VertexAttribPointerType.Float,
                Stride = 5 * sizeof(float),
                Offset = 3 * sizeof(float)
            }
        });

        _shader = new ShaderProgram(_game.Renderer.GLContext, new ShaderInfo
        {
            AssetsPath = "Assets/Shaders",
            VertexName = "cubevertex",
            FragmentName = "cubefragment"
        });

        _texture = new Texture(_game.Renderer.GLContext);
        _game.Renderer.BindObject(_texture);
        _texture.LoadFromFile("Assets/Textures/wall.jpg");

        //always reset (unbind) VAO first, otherwise it will capture the other unbinds for himself
        _game.Renderer.ResetObjects(new IBindable[] { _vao, _vbo, _texture });

        _transform = new Transform();

        _camPosition = new Vector3(0.0f, 2.0f, 3.0f);
        _camTarget = Vector3.Zero;
        _camDirection = Vector3.Normalize(_camPosition - _camTarget);
        _camRight = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, _camDirection));
        _camUp = Vector3.Normalize(Vector3.Cross(_camDirection, _camRight));
    }

    public void Render(double gametime)
    {
        _view = Matrix4x4.CreateLookAt(_camPosition, _camTarget, _camUp);
        _projection = Matrix4x4.CreatePerspectiveFieldOfView(DegreesToRadians(45.0f), (float)_game.MainWindow.FramebufferSize.X / (float)_game.MainWindow.FramebufferSize.Y, 0.1f, 100.0f);

        _game.Renderer.EnableDepth();

        _game.Renderer.BindObject(_vao);
        _game.Renderer.BindObject(_shader);
        _game.Renderer.BindObject(_texture);
        _shader.Uniform("uTexture0", 0);
        _shader.Uniform("uModel", _transform.ModelMatrix);
        _shader.Uniform("uView", _view);
        _shader.Uniform("uProjection", _projection);
        _game.Renderer.DrawTriangles(36);

        _game.Renderer.DisableDepth();
    }

    public void Unload()
    {
        _texture?.Dispose();
        _shader.Dispose();
        _vbo.Dispose();
        _vao.Dispose();
    }

    private float cubeRot = 0.0f;
    public void Update(double gametime)
    {
        var rotationSpeed = 5.0f;
        cubeRot += rotationSpeed * (float)gametime;
        _transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, DegreesToRadians(cubeRot));
    }

    private float DegreesToRadians(float degrees)
    {
        return MathF.PI / 180f * degrees;
    }
}
