using System.Numerics;

namespace OpenGlGame.ECS.Components;

public struct Transform3D
{
    public Vector3 Position { get; set; }

    public Vector3? Rotation { get; set; }

    public Vector3? Scale { get; set; }

    public Transform3D(Vector3 position, Vector3? rotation = null, Vector3? scale = null)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }

    public Matrix4x4 GetModelMatrix()
    {
        if (Rotation == null)
        {
            if (Scale == null)
            {
                return Matrix4x4.CreateTranslation(Position);
            }

            return
                Matrix4x4.CreateScale(Scale.Value)
                * Matrix4x4.CreateTranslation(Position);
        }

        if (Scale == null)
        {
            return
                Matrix4x4.CreateRotationX(Rotation.Value.X)
                * Matrix4x4.CreateRotationY(Rotation.Value.Y)
                * Matrix4x4.CreateRotationZ(Rotation.Value.Z)
                * Matrix4x4.CreateTranslation(Position);
        }

        return
            Matrix4x4.CreateScale(Scale.Value)
            * Matrix4x4.CreateRotationX(Rotation.Value.X)
            * Matrix4x4.CreateRotationY(Rotation.Value.Y)
            * Matrix4x4.CreateRotationZ(Rotation.Value.Z)
            * Matrix4x4.CreateTranslation(Position);
    }
}