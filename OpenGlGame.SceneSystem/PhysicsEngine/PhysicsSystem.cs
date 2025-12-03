using System;
using System.Collections.Generic;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.PhysicsEngine;

public class PhysicsSystem
{
    public static readonly Vector3 GravitationalAccelerationVector = new(0.0f, -9.81f, 0.0f);

    public static PhysicsSystem Instance
    {
        get
        {
            _instance ??= new PhysicsSystem();
            return _instance;
        }
    }

    private static PhysicsSystem? _instance;

    public const double PHYSICS_TICK_SECONDS = 1.0 / 60.0;

    private readonly List<RigidBody> _rigidBodies = [];
    private readonly List<StaticCollider> _staticColliders = [];

    private PhysicsSystem()
    {
    }

    public List<RigidBody> GetCurrentRigidBodies()
    {
        return _rigidBodies;
    }

    public List<StaticCollider> GetCurrentStaticColliders()
    {
        return _staticColliders;
    }

    public void RecreatePhysicsComponentsLists(List<SceneManager.WorldChunk> currentChunks)
    {
        _rigidBodies.Clear();
        _staticColliders.Clear();

        var playerRigidBody = (RigidBody?)Player.Instance.GetComponent(nameof(RigidBody));
        if (playerRigidBody == null)
        {
            throw new Exception("Player did not have a RigidBody component");
        }

        _rigidBodies.Add(playerRigidBody);

        foreach (SceneManager.WorldChunk currentChunk in currentChunks)
        {
            foreach (GameObject gameObject in currentChunk.GameObjects)
            {
                var rigidBody = (RigidBody?)gameObject.GetComponent(nameof(RigidBody));
                if (rigidBody != null)
                {
                    _rigidBodies.Add(rigidBody);
                }

                var staticCollider = (StaticCollider?)gameObject.GetComponent(nameof(StaticCollider));
                if (staticCollider != null)
                {
                    _staticColliders.Add(staticCollider);
                }
            }
        }
    }

    public void PhysicsUpdate()
    {
        foreach (RigidBody rigidBody in _rigidBodies)
        {
            if (rigidBody.IsAffectedByGravity)
            {
                ApplyGravity(rigidBody);
                ApplyFriction(rigidBody);
            }
        }

        //apply forces
        foreach (RigidBody rigidBody in _rigidBodies)
        {
            VerletIntegrate(rigidBody);
        }

        //collision detection
        foreach (RigidBody rigidBody in _rigidBodies)
        {
            var objectTransform = (Transform3D?)rigidBody.Owner?.GetComponent(nameof(Transform3D));
            if (objectTransform == null)
            {
                throw new Exception("RigidBody did not have a Transform3D component");
            }

            foreach (RigidBody otherRigidBody in _rigidBodies)
            {
                if (otherRigidBody == rigidBody)
                {
                    continue;
                }

                if (Aabb.DoBoxesIntersect(rigidBody.InternalAaBoundingBox, otherRigidBody.InternalAaBoundingBox))
                {
                    Console.WriteLine("RigidBody collided with RigidBody");
                }
            }

            foreach (StaticCollider staticCollider in _staticColliders)
            {
                var colliderTransform = (Transform3D?)staticCollider.Owner?.GetComponent(nameof(Transform3D));
                if (colliderTransform == null)
                {
                    throw new Exception("StaticCollider did not have a Transform3D component");
                }

                Aabb colliderAabb = new(staticCollider.BoundingBox, colliderTransform.GetModelMatrix());

                if (Aabb.DoBoxesIntersect(rigidBody.InternalAaBoundingBox, colliderAabb))
                {
                    ResolveCollision(rigidBody, staticCollider);
                }
            }
        }
    }

    private static void ApplyGravity(RigidBody rigidBody)
    {
        //weight force
        rigidBody.AddForce(rigidBody.Mass * GravitationalAccelerationVector);
    }

    private static void ApplyFriction(RigidBody rigidBody)
    {
        Vector3 normalVector = new(0f, 1f, 0f);
        const float COEFFICIENT_OF_FRICTION = 0.5f;

        Vector3 tangent =
            rigidBody.DerivedVelocity
            - Vector3.Dot(rigidBody.DerivedVelocity, normalVector) * normalVector;
        if (tangent.Length < 1e-5f)
        {
            return;
        }

        Vector3 tangentDir = tangent / tangent.Length;
        float normalForce = rigidBody.Mass * -GravitationalAccelerationVector.Y * (float)PHYSICS_TICK_SECONDS;
        float frictionMagnitude = COEFFICIENT_OF_FRICTION * normalForce;

        rigidBody.AddForce(-tangentDir * frictionMagnitude);
    }

    public static Vector3 GetSurfaceNormal(StaticCollider staticCollider)
    {
        Transform3D colliderTransform =
            (Transform3D?)staticCollider.Owner?.GetComponent(nameof(Transform3D))
            ?? new Transform3D();

        //TODO: get the actual normal
        Vector3 surfaceLocalNormal = new(0, 1, 0);
        Matrix4 modelMatrix = colliderTransform.GetModelMatrix();
        Matrix3 transposedInvertedMatrix =
            new Matrix3(modelMatrix.Row0.Xyz, modelMatrix.Row1.Xyz, modelMatrix.Row2.Xyz)
                .Inverted()
                .Transposed();

        return Vector3.Normalize(transposedInvertedMatrix * surfaceLocalNormal);
    }

    private static void VerletIntegrate(RigidBody rigidBody)
    {
        var objectTransform = (Transform3D?)rigidBody.Owner?.GetComponent(nameof(Transform3D));
        if (objectTransform == null)
        {
            throw new Exception("RigidBody did not have a Transform3D component");
        }

        Vector3 acceleration = rigidBody.InternalAccumulatedForces / rigidBody.Mass;
        Vector3 newPos =
            rigidBody.InternalPosition +
            (rigidBody.InternalPosition - rigidBody.InternalPreviousPosition) +
            acceleration * (float)(PHYSICS_TICK_SECONDS * PHYSICS_TICK_SECONDS);

        rigidBody.InternalPreviousPosition = rigidBody.InternalPosition;
        rigidBody.InternalPosition = newPos;
        objectTransform.Position = rigidBody.InternalPosition;

        rigidBody.InternalAccumulatedForces = Vector3.Zero;
        rigidBody.InternalAaBoundingBox =
            new Aabb(
                rigidBody.BoundingBox,
                objectTransform.GetModelMatrix());
    }

    private static void ResolveCollision(RigidBody rigidBody, StaticCollider staticCollider)
    {
        var rigidBodyTransform = (Transform3D?)rigidBody.Owner?.GetComponent(nameof(Transform3D));
        if (rigidBodyTransform == null)
        {
            throw new Exception("RigidBody did not have a Transform3D component");
        }

        var colliderTransform = (Transform3D?)staticCollider.Owner?.GetComponent(nameof(Transform3D));
        if (colliderTransform == null)
        {
            throw new Exception("StaticCollider did not have a Transform3D component");
        }

        Aabb colliderAabb = new(staticCollider.BoundingBox, colliderTransform.GetModelMatrix());
        float bounceBackOffset =
            MathF.Abs(rigidBody.InternalAaBoundingBox.Start.Y) - Math.Abs(colliderAabb.End.Y) +
            0.00001f;
        Vector3 colliderNormal = GetSurfaceNormal(staticCollider);

        Vector3 velocity = rigidBody.DerivedVelocity;
        rigidBody.InternalPosition += colliderNormal * bounceBackOffset;
        rigidBodyTransform.Position = rigidBody.InternalPosition;

        float reflectedVelocity = Vector3.Dot(velocity, colliderNormal);
        Vector3 newVelocity =
            velocity - (1f + rigidBody.RestitutionCoefficient) * reflectedVelocity * colliderNormal;

        rigidBody.InternalPreviousPosition = rigidBody.InternalPosition - newVelocity;
    }
}