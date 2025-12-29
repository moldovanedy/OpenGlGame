using System;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Components;

/// <summary>
/// Represents a dynamic object affected by physics. Will participate in collision detection.
/// </summary>
public class RigidBody : IPhysicsObjectComponent
{
    public string Name => nameof(RigidBody);
    public GameObject? Owner { get; set; }

    public bool IsAffectedByGravity { get; set; } = true;

    public float Mass { get; set; } = 1.0f;

    /// <summary>
    /// A.K.A. the bounciness of this object, from 0 to 1.
    /// </summary>
    public float RestitutionCoefficient { get; set; } = 0.06f;

    public Aabb BoundingBox { get; set; }


    internal Vector3 InternalAccumulatedForces { get; set; }
    internal Aabb InternalAaBoundingBox { get; set; }

    internal Vector3 InternalPosition { get; set; }
    internal Vector3 InternalPreviousPosition { get; set; }

    public Vector3 DerivedVelocity => InternalPosition - InternalPreviousPosition;

    public void EnterScene()
    {
        var objectTransform = (Transform3D?)Owner?.GetComponent(nameof(Transform3D));
        if (objectTransform == null)
        {
            throw new Exception("RigidBody: no Transform3D component in EnterScene");
        }

        InternalPosition = objectTransform.Position;
        InternalPreviousPosition = objectTransform.Position;
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

    public void AddForce(Vector3 force)
    {
        InternalAccumulatedForces += force;
    }

    public void SetDirectVelocity(Vector3 velocity)
    {
        InternalPosition = InternalPreviousPosition + (float)PhysicsSystem.PHYSICS_TICK_SECONDS * velocity;
    }

    public void ResetPositions(Vector3 newPosition)
    {
        InternalPosition = newPosition;
        InternalPreviousPosition = newPosition;
    }
}