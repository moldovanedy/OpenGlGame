using System;
using System.Collections.Generic;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.PhysicsEngine;

public class PhysicsSystem
{
    public struct RaycastHit
    {
        public float Distance;
        public Vector3 Normal;
        public Vector3 HitPoint;
    }


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

        if (!Player.FREE_ROAM_CAMERA)
        {
            var playerRigidBody = (RigidBody?)Player.Instance.GetComponent(nameof(RigidBody));
            if (playerRigidBody == null)
            {
                throw new Exception("Player did not have a RigidBody component");
            }

            _rigidBodies.Add(playerRigidBody);
        }

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

    public RaycastHit? RaycastTest(
        Vector3 origin,
        Vector3 normalizedRayDirection,
        bool isRayInsideAabb,
        out Aabb? collidedAabb)
    {
        foreach (RigidBody rigidBody in _rigidBodies)
        {
            RaycastHit? hit = AabbRaycastTest(origin, normalizedRayDirection, rigidBody.InternalAaBoundingBox);
            if (hit != null)
            {
                collidedAabb = rigidBody.InternalAaBoundingBox;
                return hit.Value;
            }
        }

        foreach (StaticCollider collider in _staticColliders)
        {
            var colliderTransform = (Transform3D?)collider.Owner?.GetComponent(nameof(Transform3D));
            if (colliderTransform == null)
            {
                throw new Exception("StaticCollider did not have a Transform3D component");
            }

            Aabb colliderAabb = new(collider.BoundingBox, colliderTransform.GetModelMatrix());

            RaycastHit? hit = AabbRaycastTest(origin, normalizedRayDirection, colliderAabb, isRayInsideAabb);
            if (hit != null)
            {
                collidedAabb = colliderAabb;
                return hit.Value;
            }
        }

        collidedAabb = null;
        return null;
    }

    public static RaycastHit? AabbRaycastTest(
        Vector3 origin,
        Vector3 normalizedRayDirection,
        Aabb colliderToTest,
        bool isRayInsideAabb = false)
    {
        float tMin = float.NegativeInfinity;
        float tMax = float.PositiveInfinity;

        Vector3 hitNormal = Vector3.Zero;

        if (Math.Abs(normalizedRayDirection.X) < 1e-8f)
        {
            //ray is parallel, check if it's outside the AABB
            if (origin.X < colliderToTest.Start.X || origin.X > colliderToTest.End.X)
                return null;
        }
        else
        {
            float t1 = (colliderToTest.Start.X - origin.X) / normalizedRayDirection.X;
            float t2 = (colliderToTest.End.X - origin.X) / normalizedRayDirection.X;

            float enter = MathF.Min(t1, t2);
            float exit = MathF.Max(t1, t2);

            if (enter > tMin)
            {
                tMin = enter;
                hitNormal = new Vector3(t1 > t2 ? 1 : -1, 0, 0); // which face we hit
            }

            tMax = MathF.Min(tMax, exit);
            if (tMin > tMax)
            {
                return null;
            }
        }

        if (Math.Abs(normalizedRayDirection.Y) < 1e-8f)
        {
            if (origin.Y < colliderToTest.Start.Y || origin.Y > colliderToTest.End.Y)
            {
                return null;
            }
        }
        else
        {
            float t1 = (colliderToTest.Start.Y - origin.Y) / normalizedRayDirection.Y;
            float t2 = (colliderToTest.End.Y - origin.Y) / normalizedRayDirection.Y;

            float enter = MathF.Min(t1, t2);
            float exit = MathF.Max(t1, t2);

            if (enter > tMin)
            {
                tMin = enter;
                hitNormal = new Vector3(0, t1 > t2 ? 1 : -1, 0);
            }

            tMax = MathF.Min(tMax, exit);
            if (tMin > tMax)
            {
                return null;
            }
        }

        if (Math.Abs(normalizedRayDirection.Z) < 1e-8f)
        {
            if (origin.Z < colliderToTest.Start.Z || origin.Z > colliderToTest.End.Z)
            {
                return null;
            }
        }
        else
        {
            float t1 = (colliderToTest.Start.Z - origin.Z) / normalizedRayDirection.Z;
            float t2 = (colliderToTest.End.Z - origin.Z) / normalizedRayDirection.Z;

            float enter = MathF.Min(t1, t2);
            float exit = MathF.Max(t1, t2);

            if (enter > tMin)
            {
                tMin = enter;
                hitNormal = new Vector3(0, 0, t1 > t2 ? 1 : -1);
            }

            tMax = MathF.Min(tMax, exit);
            if (tMin > tMax)
            {
                return null;
            }
        }

        //if the hit is behind the ray origin
        if (tMin < 0)
        {
            return null;
        }

        float tHit = isRayInsideAabb ? tMax : tMin;
        return new RaycastHit
        {
            Distance = tHit,
            Normal = hitNormal.Normalized(),
            HitPoint = origin + normalizedRayDirection * tHit
        };
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
        //AI-generated... physics is hard

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

        // Sync AABB with the current transform in case it was moved by another collision this frame
        rigidBody.InternalAaBoundingBox = new Aabb(rigidBody.BoundingBox, rigidBodyTransform.GetModelMatrix());

        Aabb colliderAabb = new(staticCollider.BoundingBox, colliderTransform.GetModelMatrix());
        Aabb rigidBodyAabb = rigidBody.InternalAaBoundingBox;

        // Capture velocity BEFORE modifying position
        Vector3 velocity = rigidBody.DerivedVelocity;

        // Calculate previous frame AABB to determine entry direction
        // We approximate the previous AABB by shifting the current one back by the displacement
        Vector3 displacement = rigidBody.InternalPosition - rigidBody.InternalPreviousPosition;
        Vector3 prevStart = rigidBodyAabb.Start - displacement;
        Vector3 prevEnd = rigidBodyAabb.End - displacement;

        // Check which axes were overlapping in the previous frame
        bool wasOverlapX = prevEnd.X > colliderAabb.Start.X && prevStart.X < colliderAabb.End.X;
        bool wasOverlapY = prevEnd.Y > colliderAabb.Start.Y && prevStart.Y < colliderAabb.End.Y;
        bool wasOverlapZ = prevEnd.Z > colliderAabb.Start.Z && prevStart.Z < colliderAabb.End.Z;

        float overlapX1 = colliderAabb.End.X - rigidBodyAabb.Start.X;
        float overlapX2 = rigidBodyAabb.End.X - colliderAabb.Start.X;
        float overlapX = MathF.Min(overlapX1, overlapX2);

        float overlapY1 = colliderAabb.End.Y - rigidBodyAabb.Start.Y;
        float overlapY2 = rigidBodyAabb.End.Y - colliderAabb.Start.Y;
        float overlapY = MathF.Min(overlapY1, overlapY2);

        float overlapZ1 = colliderAabb.End.Z - rigidBodyAabb.Start.Z;
        float overlapZ2 = rigidBodyAabb.End.Z - colliderAabb.Start.Z;
        float overlapZ = MathF.Min(overlapZ1, overlapZ2);

        // Identify the collision axis.
        // We prioritize axes that were NOT overlapping previously (meaning we entered the collider along this axis).
        int axis = -1; // 0: X, 1: Y, 2: Z

        // Pure axis entry checks
        if (!wasOverlapX && wasOverlapY && wasOverlapZ)
        {
            axis = 0;
        }
        else if (wasOverlapX && !wasOverlapY && wasOverlapZ)
        {
            axis = 1;
        }
        else if (wasOverlapX && wasOverlapY && !wasOverlapZ)
        {
            axis = 2;
        }

        // If no single-axis entry detected (e.g. corner hit or already inside), fall back to MTV (smallest overlap)
        if (axis == -1)
        {
            float minOverlap = MathF.Min(overlapX, MathF.Min(overlapY, overlapZ));
            if (MathF.Abs(minOverlap - overlapX) < 1e-6f)
            {
                axis = 0;
            }
            else if (MathF.Abs(minOverlap - overlapY) < 1e-6f)
            {
                axis = 1;
            }
            else
            {
                axis = 2;
            }
        }

        Vector3 correctionVector;
        float correctionAmount;
        const float EPSILON = 0.0001f;

        // Use centers to reliably determine which side we came from, preventing tunnelling
        Vector3 prevCenter = (prevStart + prevEnd) * 0.5f;
        Vector3 colliderCenter = (colliderAabb.Start + colliderAabb.End) * 0.5f;

        switch (axis)
        {
            // X
            case 0:
            {
                bool pushPositive = prevCenter.X > colliderCenter.X;
                // If we were previously inside on this axis (fallback case), trust the shortest path
                if (wasOverlapX)
                {
                    pushPositive = overlapX1 < overlapX2;
                }

                correctionAmount = pushPositive ? overlapX1 : overlapX2;
                correctionVector = pushPositive
                    ? new Vector3(correctionAmount + EPSILON, 0, 0)
                    : new Vector3(-correctionAmount - EPSILON, 0, 0);
                break;
            }
            // Y
            case 1:
            {
                bool pushPositive = prevCenter.Y > colliderCenter.Y;
                if (wasOverlapY)
                {
                    pushPositive = overlapY1 < overlapY2;
                }

                correctionAmount = pushPositive ? overlapY1 : overlapY2;
                correctionVector = pushPositive
                    ? new Vector3(0, correctionAmount + EPSILON, 0)
                    : new Vector3(0, -correctionAmount - EPSILON, 0);
                break;
            }
            // Z
            default:
            {
                bool pushPositive = prevCenter.Z > colliderCenter.Z;
                if (wasOverlapZ)
                {
                    pushPositive = overlapZ1 < overlapZ2;
                }

                correctionAmount = pushPositive ? overlapZ1 : overlapZ2;
                correctionVector = pushPositive
                    ? new Vector3(0, 0, correctionAmount + EPSILON)
                    : new Vector3(0, 0, -correctionAmount - EPSILON);
                break;
            }
        }

        // Apply position correction
        rigidBody.InternalPosition += correctionVector;
        rigidBodyTransform.Position = rigidBody.InternalPosition;

        // IMPORTANT: Update AABB immediately so later calculations (or other colliders) see the corrected position
        rigidBody.InternalAaBoundingBox = new Aabb(rigidBody.BoundingBox, rigidBodyTransform.GetModelMatrix());

        // Reflect velocity
        Vector3 normal = correctionVector.Normalized();
        float velocityAlongNormal = Vector3.Dot(velocity, normal);

        // Only reflect if moving towards the collider
        if (velocityAlongNormal < 0)
        {
            Vector3 newVelocity = velocity - (1f + rigidBody.RestitutionCoefficient) * velocityAlongNormal * normal;

            // Update PreviousPosition for Verlet integration: PrevPos = Pos - NewVel * dt
            // This ensures the bounce/slide is respected in the next frame
            rigidBody.InternalPreviousPosition = rigidBody.InternalPosition - newVelocity;
        }


        // var rigidBodyTransform = (Transform3D?)rigidBody.Owner?.GetComponent(nameof(Transform3D));
        // if (rigidBodyTransform == null)
        // {
        //     throw new Exception("RigidBody did not have a Transform3D component");
        // }
        //
        // var colliderTransform = (Transform3D?)staticCollider.Owner?.GetComponent(nameof(Transform3D));
        // if (colliderTransform == null)
        // {
        //     throw new Exception("StaticCollider did not have a Transform3D component");
        // }
        //
        // Aabb colliderAabb = new(staticCollider.BoundingBox, colliderTransform.GetModelMatrix());
        // Aabb rigidBodyAabb = rigidBody.InternalAaBoundingBox;
        //
        // float overlapX1 = colliderAabb.End.X - rigidBodyAabb.Start.X;
        // float overlapX2 = rigidBodyAabb.End.X - colliderAabb.Start.X;
        // float overlapX = MathF.Min(overlapX1, overlapX2);
        //
        // float overlapY1 = colliderAabb.End.Y - rigidBodyAabb.Start.Y;
        // float overlapY2 = rigidBodyAabb.End.Y - colliderAabb.Start.Y;
        // float overlapY = MathF.Min(overlapY1, overlapY2);
        //
        // float overlapZ1 = colliderAabb.End.Z - rigidBodyAabb.Start.Z;
        // float overlapZ2 = rigidBodyAabb.End.Z - colliderAabb.Start.Z;
        // float overlapZ = MathF.Min(overlapZ1, overlapZ2);
        //
        // float minOverlap = MathF.Min(overlapX, MathF.Min(overlapY, overlapZ));
        //
        // float correctionAmount = minOverlap + 0.0001f;
        // Vector3 correctionVector = Vector3.Zero;
        //
        // if (MathF.Abs(minOverlap - overlapX) < 1e-6f)
        // {
        //     //push along X; if overlapX1 is smaller, rigidbody is to the right, push positive.
        //     correctionVector = overlapX1 < overlapX2
        //         ? new Vector3(correctionAmount, 0, 0)
        //         : new Vector3(-correctionAmount, 0, 0);
        // }
        // else if (MathF.Abs(minOverlap - overlapY) < 1e-6f)
        // {
        //     correctionVector = overlapY1 < overlapY2
        //         ? new Vector3(0, correctionAmount, 0)
        //         : new Vector3(0, -correctionAmount, 0);
        // }
        // else if (MathF.Abs(minOverlap - overlapZ) < 1e-6f)
        // {
        //     correctionVector = overlapZ1 < overlapZ2
        //         ? new Vector3(0, 0, correctionAmount)
        //         : new Vector3(0, 0, -correctionAmount);
        // }
        //
        // rigidBody.InternalPosition += correctionVector;
        // rigidBodyTransform.Position = rigidBody.InternalPosition;
        //
        // //reflect velocity (for pushback)
        // Vector3 normal = correctionVector.Normalized();
        // Vector3 velocity = rigidBody.DerivedVelocity;
        //
        // float velocityAlongNormal = Vector3.Dot(velocity, normal);
        // Vector3 newVelocity = velocity - (1f + rigidBody.RestitutionCoefficient) * velocityAlongNormal * normal;
        // rigidBody.InternalPreviousPosition = rigidBody.InternalPosition - newVelocity;
    }
}