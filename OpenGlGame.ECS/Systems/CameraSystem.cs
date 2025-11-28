using System.Numerics;

namespace OpenGlGame.ECS.Systems;

public class CameraSystem
{
    public static CameraSystem Instance
    {
        get
        {
            _instance ??= new CameraSystem();
            return _instance;
        }
    }

    private static CameraSystem? _instance;

    public Vector3 PlayerPosition { get; set; }
}