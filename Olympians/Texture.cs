using System;
using Silk.NET.OpenGL;
using StbImageSharp;

namespace Olympians;

public unsafe class Texture : IBindable, IDisposable
{
    private GL _gl;
    private uint _id;

    public Texture(GL gL)
    {
        _gl = gL;
        _id = _gl.GenTexture();
    }

    public void Dispose()
    {
        _gl.DeleteTexture(_id);
    }

    public void Bind()
    {
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _id);
    }

    public void Reset()
    {
        _gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void LoadFromFile(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        LoadFromMemory(data);
    }

    public void LoadFromMemory(byte[] data)
    {
        ImageResult result = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);

        fixed (byte* pData = result.Data)
            _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)result.Width, (uint)result.Height,
                            0, PixelFormat.Rgba, PixelType.UnsignedByte, pData);

        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.Nearest);
    }
}
