using System;
using System.Collections.Generic;
using OpenGlGame.Data;
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
    private readonly List<Interactable> _interactables = [];

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

    public List<Interactable> GetCurrentInteractables()
    {
        return _interactables;
    }

    public void RecreatePhysicsComponentsLists(GameObject[] currentGameObjects)
    {
        _rigidBodies.Clear();
        _staticColliders.Clear();

        if (!DebuggingData.IS_CAMERA_FREE_ROAM)
        {
            var playerRigidBody = (RigidBody?)Player.Instance.GetComponent(nameof(RigidBody));
            if (playerRigidBody == null)
            {
                throw new Exception("Player did not have a RigidBody component");
            }

            _rigidBodies.Add(playerRigidBody);
        }

        foreach (GameObject gameObject in currentGameObjects)
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

    public void RecreateInteractablesList(GameObject[] currentGameObjects)
    {
        _interactables.Clear();

        foreach (GameObject gameObject in currentGameObjects)
        {
            var interactable = (Interactable?)gameObject.GetComponent(nameof(Interactable));
            if (interactable != null)
            {
                _interactables.Add(interactable);
            }
        }
    }

    public void PhysicsUpdate()
    {
        lock (_rigidBodies)
        lock (_staticColliders)
        {
            foreach (RigidBody rigidBody in _rigidBodies)
            {
                if (rigidBody.IsAffectedByGravity)
                {
                    ApplyGravity(rigidBody);
                }

                if (rigidBody.Owner != Player.Instance)
                {
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
                        // Console.WriteLine("RigidBody collided with RigidBody");
                        ResolveCollision(rigidBody, otherRigidBody);
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
        const float COEFFICIENT_OF_FRICTION = 0.4f;

        Vector3 tangent =
            rigidBody.DerivedVelocity
            - Vector3.Dot(rigidBody.DerivedVelocity, normalVector) * normalVector;
        if (tangent.Length < 1e-5f)
        {
            return;
        }

        Vector3 tangentDir = tangent / tangent.Length;
        float normalForce = rigidBody.Mass * -GravitationalAccelerationVector.Y;
        float frictionMagnitude = COEFFICIENT_OF_FRICTION * normalForce * (float)PHYSICS_TICK_SECONDS * 5f;

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
    }

    private static void ResolveCollision(RigidBody rigidBody1, RigidBody rigidBody2)
    {
        //AI-generated... physics is hard

        var rigidBody1Transform = (Transform3D?)rigidBody1.Owner?.GetComponent(nameof(Transform3D));
        if (rigidBody1Transform == null)
        {
            throw new Exception("RigidBody1 did not have a Transform3D component");
        }

        var rigidBody2Transform = (Transform3D?)rigidBody2.Owner?.GetComponent(nameof(Transform3D));
        if (rigidBody2Transform == null)
        {
            throw new Exception("RigidBody2 did not have a Transform3D component");
        }

        // Make sure AABBs represent the current transforms (they may have been moved by earlier collision resolution this tick)
        rigidBody1.InternalAaBoundingBox = new Aabb(rigidBody1.BoundingBox, rigidBody1Transform.GetModelMatrix());
        rigidBody2.InternalAaBoundingBox = new Aabb(rigidBody2.BoundingBox, rigidBody2Transform.GetModelMatrix());

        Aabb a = rigidBody1.InternalAaBoundingBox;
        Aabb b = rigidBody2.InternalAaBoundingBox;

        // Compute overlap along each axis (MTV candidate)
        float overlapX1 = b.End.X - a.Start.X;
        float overlapX2 = a.End.X - b.Start.X;
        float overlapX = MathF.Min(overlapX1, overlapX2);

        float overlapY1 = b.End.Y - a.Start.Y;
        float overlapY2 = a.End.Y - b.Start.Y;
        float overlapY = MathF.Min(overlapY1, overlapY2);

        float overlapZ1 = b.End.Z - a.Start.Z;
        float overlapZ2 = a.End.Z - b.Start.Z;
        float overlapZ = MathF.Min(overlapZ1, overlapZ2);

        // Choose the axis with the smallest penetration depth
        int axis = 0; // 0:X 1:Y 2:Z
        float penetration = overlapX;
        if (overlapY < penetration)
        {
            penetration = overlapY;
            axis = 1;
        }

        if (overlapZ < penetration)
        {
            penetration = overlapZ;
            axis = 2;
        }

        // Determine collision normal direction using centers
        Vector3 centerA = (a.Start + a.End) * 0.5f;
        Vector3 centerB = (b.Start + b.End) * 0.5f;

        Vector3 normal;
        const float EPSILON = 0.0001f;

        switch (axis)
        {
            case 0:
                normal = centerA.X < centerB.X ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
                break;
            case 1:
                normal = centerA.Y < centerB.Y ? new Vector3(0, -1, 0) : new Vector3(0, 1, 0);
                break;
            default:
                normal = centerA.Z < centerB.Z ? new Vector3(0, 0, -1) : new Vector3(0, 0, 1);
                break;
        }

        penetration = MathF.Max(penetration, 0f);

        // --- 1) Positional correction (split by inverse mass) to separate the bodies ---
        float invMass1 = rigidBody1.Mass > 0f ? 1f / rigidBody1.Mass : 0f;
        float invMass2 = rigidBody2.Mass > 0f ? 1f / rigidBody2.Mass : 0f;
        float invMassSum = invMass1 + invMass2;

        if (invMassSum <= 0f || penetration <= 0f)
        {
            // If both are "infinite mass", we can't resolve meaningfully.
            return;
        }

        // Small slop prevents jitter from tiny penetrations; percent controls how aggressively we correct.
        const float SLOP = 0.001f;
        const float PERCENT = 0.8f;

        float correctedPenetration = MathF.Max(penetration - SLOP, 0f);
        Vector3 correction = correctedPenetration / invMassSum * PERCENT * normal;

        rigidBody1.InternalPosition += correction * invMass1;
        rigidBody2.InternalPosition -= correction * invMass2;

        rigidBody1Transform.Position = rigidBody1.InternalPosition;
        rigidBody2Transform.Position = rigidBody2.InternalPosition;

        // Refresh AABBs after position changes
        rigidBody1.InternalAaBoundingBox = new Aabb(rigidBody1.BoundingBox, rigidBody1Transform.GetModelMatrix());
        rigidBody2.InternalAaBoundingBox = new Aabb(rigidBody2.BoundingBox, rigidBody2Transform.GetModelMatrix());

        // --- 2) Velocity (impulse) resolution based on masses and current velocities ---
        const float DT = (float)PHYSICS_TICK_SECONDS;

        Vector3 v1 = rigidBody1.DerivedVelocity;
        Vector3 v2 = rigidBody2.DerivedVelocity;

        Vector3 relativeVelocity = v1 - v2;
        float velAlongNormal = Vector3.Dot(relativeVelocity, normal);

        // If they're separating along the normal, don't apply impulse (positional correction already handled overlap).
        if (velAlongNormal > 0f)
        {
            return;
        }

        float restitution = MathF.Min(rigidBody1.RestitutionCoefficient, rigidBody2.RestitutionCoefficient);

        float j = -(1f + restitution) * velAlongNormal;
        j /= invMassSum;

        Vector3 impulse = j * normal;

        v1 += impulse * invMass1;
        v2 -= impulse * invMass2;

        // Verlet note:
        // velocity is derived from (pos - prevPos) / dt, so enforce the new velocities by updating PreviousPosition.
        // Also add a tiny bias along the normal to reduce "sticking" when bodies remain in contact.
        Vector3 bias = normal * EPSILON;

        rigidBody1.InternalPreviousPosition = rigidBody1.InternalPosition - v1 * DT + bias;
        rigidBody2.InternalPreviousPosition = rigidBody2.InternalPosition - v2 * DT - bias;
    }
}