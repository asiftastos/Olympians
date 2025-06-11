using System;

namespace Olympians;

public interface ITestBed
{
    string Name { get; }

    void Load(Game game);
    void Unload();

    void Update(double gametime);
    void Render(double gametime);
}
