using OpenGlGame.SceneSystem.PhysicsEngine;

namespace OpenGlGame.SceneSystem.Components;

/// <summary>
/// Represents an object that the player can interact with.
/// </summary>
public class CubeLandingArea : IPhysicsObjectComponent
{
    public string Name => nameof(CubeLandingArea);
    public GameObject? Owner { get; set; }

    public Aabb BoundingBox { get; set; }

    public bool IsOccupied { get; set; }

    public CubeLandingArea(Aabb boundingBox)
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