using System;
using System.Runtime.InteropServices;
using Silk.NET.Core.Native;
using Silk.NET.Maths;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;

namespace Atrapo;

public unsafe class AtrapoBase
{
    protected IWindow? _window;
    protected Vk? _vk;

    protected Instance _instance;

    public void Run()
    {
        InitWindow();
        InitVulkan();
        MainLoop();
        CleanUp();
    }

    protected virtual void InitWindow()
    {
        var options = WindowOptions.DefaultVulkan with
        {
            Size = new Vector2D<int>(1280, 720),
            Title = "Atrapo"
        };

        _window = Window.Create(options);
        _window.Initialize();

        if (_window.VkSurface == null)
        {
            throw new Exception("Windowing platform does not support Vulkan");
        }
    }

    protected virtual void InitVulkan()
    {
        CreateInstance();
    }

    private void MainLoop()
    {
        _window!.Run();
    }

    protected virtual void CleanUp()
    {
        _vk!.Dispose();

        _window?.Dispose();
    }

    private void CreateInstance()
    {
        _vk = Vk.GetApi();

        var glfwExtensions = _window!.VkSurface!.GetRequiredExtensions(out var glfwExtensionsCount);
        Console.WriteLine($"Required Extensions: {glfwExtensionsCount}");
        for (uint i = 0; i < glfwExtensionsCount; i++)
        {
            byte* s = glfwExtensions[i];
            string extension = new string((sbyte*)s);
            Console.WriteLine($"Extension [{i}]: {extension}");
        }
    }
}
