using System;
using System.Numerics;

namespace Olympians;

public class Transform
{
    public Vector3 Position { get; set; } = new Vector3(0.0f, 0.0f, 0.0f);

    public float Scale { get; set; } = 1.0f;

    public Quaternion Rotation { get; set; } = Quaternion.Identity;

    public Matrix4x4 ViewMatrix => Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) * Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);
}
