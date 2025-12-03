using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem;

public class Player : GameObject
{
    private readonly Transform3D _transform;

    public Player()
    {
        UpdateEvent += OnUpdate;

        _transform = new Transform3D();
        AddComponent(_transform);

        AddComponent(new RigidBody
        {
            Mass = 70f,
            BoundingBox = new Aabb(new Vector3(-0.3f, -0.875f, -0.15f), new Vector3(0.6f, 1.75f, 0.3f))
        });
    }

    private void OnUpdate(double obj)
    {
        // Console.WriteLine("POS: " + _transform.Position);
    }
}