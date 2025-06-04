using System;

namespace Olympians;

public interface IImguiWindowProvider
{
    bool Show { get; set; }
    
    string WindowName { get; }
}
