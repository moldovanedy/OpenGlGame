using System;
using OpenGlGame.SceneSystem.PhysicsEngine;

namespace OpenGlGame.SceneSystem.Components;

/// <summary>
/// Represents an object that the player can interact with.
/// </summary>
public class Interactable : IPhysicsObjectComponent
{
    public string Name => nameof(Interactable);
    public GameObject? Owner { get; set; }

    public Aabb BoundingBox { get; set; }

    public Action InteractCallback { get; set; }

    public Interactable(Aabb boundingBox, Action interactCallback)
    {
        BoundingBox = boundingBox;
        InteractCallback = interactCallback;
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