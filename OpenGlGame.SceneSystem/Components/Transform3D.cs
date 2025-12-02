using OpenTK.Mathematics;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace OpenGlGame.SceneSystem.Components;

public class Transform3D : IGameObjectComponent
{
    public string Name => nameof(Transform3D);

    public GameObject? Owner { get; set; }

    public Vector3 Position { get; set; }

    // public Vector3? Rotation { get; set; }
    public Quaternion? Rotation { get; set; }

    public Vector3? Scale { get; set; }

    public void EnterScene()
    {
    }

    public void ExitScene()
    {
    }

    public void Update(double deltaTime)
    {
    }

    public Matrix4 GetModelMatrix()
    {
        Matrix4 matrix;

        if (Rotation == null)
        {
            if (Scale == null)
            {
                matrix = Matrix4.CreateTranslation(Position);
            }
            else
            {
                matrix = Matrix4.CreateScale(Scale.Value) * Matrix4.CreateTranslation(Position);
            }
        }
        else
        {
            if (Scale == null)
            {
                matrix =
                    Matrix4.CreateFromQuaternion(Rotation.Value)
                    * Matrix4.CreateTranslation(Position)
                    ;
            }
            else
            {
                matrix =
                    Matrix4.CreateScale(Scale.Value)
                    * Matrix4.CreateFromQuaternion(Rotation.Value)
                    * Matrix4.CreateTranslation(Position)
                    ;
            }
        }

        return Matrix4.Transpose(matrix);
    }
}