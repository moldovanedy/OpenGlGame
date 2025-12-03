using System;
using System.Collections.Generic;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Character;

public class Player : GameObject
{
    public static Player Instance
    {
        get
        {
            _instance ??= new Player();
            return _instance;
        }
    }

    private static Player? _instance;

    private readonly Transform3D _transform;
    private readonly RigidBody _rigidBody;

    private Camera.CameraMovement _currentMovingDirections;
    private bool _isOnGround;

    public Camera Camera { get; private set; }

    private Player()
    {
        UpdateEvent += OnUpdate;

        _transform = new Transform3D();
        AddComponent(_transform);

        Camera = new Camera();

        _rigidBody = new RigidBody
        {
            Mass = 70f,
            BoundingBox = new Aabb(new Vector3(-0.3f, -0.875f, -0.15f), new Vector3(0.6f, 1.75f, 0.3f))
        };
        AddComponent(_rigidBody);
    }

    public void PhysicsUpdate()
    {
        _isOnGround = false;
        List<StaticCollider> staticColliders = PhysicsSystem.Instance.GetCurrentStaticColliders();
        foreach (StaticCollider collider in staticColliders)
        {
            var colliderTransform = (Transform3D?)collider.Owner?.GetComponent(nameof(Transform3D));
            if (colliderTransform == null)
            {
                throw new Exception("StaticCollider did not have a Transform3D component");
            }

            Aabb colliderAabb = new(collider.BoundingBox, colliderTransform.GetModelMatrix());
            if (CheckIsOnGround(_rigidBody.InternalAaBoundingBox, colliderAabb))
            {
                _isOnGround = true;
                return;
            }
        }

        List<RigidBody> rigidBodies = PhysicsSystem.Instance.GetCurrentRigidBodies();
        foreach (RigidBody otherRigidBody in rigidBodies)
        {
            if (otherRigidBody == _rigidBody)
            {
                continue;
            }

            if (CheckIsOnGround(_rigidBody.InternalAaBoundingBox, otherRigidBody.InternalAaBoundingBox))
            {
                _isOnGround = true;
                return;
            }
        }
    }

    public void InputMovementChanged(Camera.CameraMovement currentState)
    {
        _currentMovingDirections = currentState;
    }

    private static bool CheckIsOnGround(Aabb playerAabb, Aabb colliderAabb)
    {
        const float EPSILON = 0.01f;

        bool horizontal =
            playerAabb.Start.X + playerAabb.Extents.X > colliderAabb.Start.X &&
            playerAabb.Start.X < colliderAabb.Start.X + colliderAabb.Extents.X &&
            playerAabb.Start.Z + playerAabb.Extents.Z > colliderAabb.Start.Z &&
            playerAabb.Start.Z < colliderAabb.Start.Z + colliderAabb.Extents.Z;

        bool vertical =
            playerAabb.Start.Y <= colliderAabb.End.Y + EPSILON && // feet not below the surface
            playerAabb.Start.Y >= colliderAabb.End.Y - EPSILON; // feet touching

        return horizontal && vertical;
    }

    private void OnUpdate(double deltaTime)
    {
        var playerRigidBody = (RigidBody?)GetComponent(nameof(RigidBody));
        if (playerRigidBody == null)
        {
            throw new Exception("Camera: player didn't have a RigidBody");
        }

        Camera.Update(deltaTime);

        Vector3 runningFrontVector = new(Camera.FrontVector.X, 0f, Camera.FrontVector.Z);
        Vector3 runningRightVector = new(Camera.RightVector.X, 0f, Camera.RightVector.Z);

        if (!_isOnGround)
        {
            return;
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Forward) != Camera.CameraMovement.None)
        {
            playerRigidBody.SetDirectVelocity(runningFrontVector * 6f);
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Backward) != Camera.CameraMovement.None)
        {
            playerRigidBody.SetDirectVelocity(-runningFrontVector * 6f);
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Left) != Camera.CameraMovement.None)
        {
            playerRigidBody.SetDirectVelocity(-runningRightVector * 6f);
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Right) != Camera.CameraMovement.None)
        {
            playerRigidBody.SetDirectVelocity(runningRightVector * 6f);
        }
    }
}