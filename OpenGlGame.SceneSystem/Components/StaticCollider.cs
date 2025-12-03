using OpenGlGame.SceneSystem.PhysicsEngine;

namespace OpenGlGame.SceneSystem.Components;

/// <summary>
/// Represents a static object that can't be moved but will participate in collision detection.
/// </summary>
public class StaticCollider : IPhysicsObjectComponent
{
    public string Name => nameof(StaticCollider);
    public GameObject? Owner { get; set; }

    public Aabb BoundingBox { get; set; }

    public StaticCollider(Aabb boundingBox)
    {
        BoundingBox = boundingBox;
    }

    public void EnterScene()
    {
    }

    public void ExitScene()
    {
    }

    public void Update(double deltaTime)
    {
    }

    public void PhysicsUpdate()
    {
    }
}