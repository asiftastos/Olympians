using System;
using System.Runtime.InteropServices;
using Silk.NET.Core;
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
        _vk!.DestroyInstance(_instance, null);
        _vk!.Dispose();

        _window?.Dispose();
    }

    private void CreateInstance()
    {
        _vk = Vk.GetApi();

        // Get the required instance extensions from glfw
        var glfwExtensions = _window!.VkSurface!.GetRequiredExtensions(out var glfwExtensionsCount);
        Console.WriteLine($"Required Extensions: {glfwExtensionsCount}");
        for (uint i = 0; i < glfwExtensionsCount; i++)
        {
            byte* s = glfwExtensions[i];
            string extension = new string((sbyte*)s);
            Console.WriteLine($"Extension [{i}]: {extension}");
        }

        //Enumerate the extension properties for the instance
        //We can check if we want if the extensions from GLFW retreived above are in this list and so are supported
        uint extensionPropertiesCount = 0;
        if (_vk.EnumerateInstanceExtensionProperties(String.Empty, ref extensionPropertiesCount, null) == Result.Success)
        {
            Console.WriteLine($"Number of instance extension properties: {extensionPropertiesCount}");

            ExtensionProperties[] extensions = new ExtensionProperties[extensionPropertiesCount];
            if (_vk.EnumerateInstanceExtensionProperties(new ReadOnlySpan<byte>(), &extensionPropertiesCount, new Span<ExtensionProperties>(extensions)) == Result.Success)
            {
                foreach (var item in extensions)
                {
                    Console.WriteLine($"{new string((sbyte*)item.ExtensionName)}");
                }
            }
        }


        //  Create the instance
        ApplicationInfo applicationInfo = new()
        {
            SType = StructureType.ApplicationInfo,
            PApplicationName = (byte*)SilkMarshal.StringToPtr("Hello triangle", NativeStringEncoding.LPUTF8Str),
            ApplicationVersion = new Version32(1, 0, 0),
            PEngineName = (byte*)SilkMarshal.StringToPtr("Atrapo", NativeStringEncoding.LPUTF8Str),
            EngineVersion = new Version32(1, 0, 0),
            ApiVersion = Vk.Version13
        };

        InstanceCreateInfo instanceCreateInfo = new()
        {
            SType = StructureType.InstanceCreateInfo,
            PApplicationInfo = &applicationInfo,
            EnabledExtensionCount = glfwExtensionsCount,
            PpEnabledExtensionNames = glfwExtensions,
            EnabledLayerCount = 0
        };

        if (_vk.CreateInstance(in instanceCreateInfo, null, out _instance) != Result.Success)
        {
            throw new Exception("Failed to create vulkan instance");
        }
        else
        {
            Console.WriteLine("Vulkan instance created");
        }

        SilkMarshal.FreeString((IntPtr)applicationInfo.PApplicationName, NativeStringEncoding.LPUTF8Str);
        SilkMarshal.FreeString((IntPtr)applicationInfo.PEngineName, NativeStringEncoding.LPUTF8Str);
    }
}
