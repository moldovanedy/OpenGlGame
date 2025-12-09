using System;
using System.Collections.Generic;
using OpenGlGame.Data;
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
    private bool _isJumpPressed;

    private bool _isOnGround;

    public Camera Camera { get; private set; }

    private Player()
    {
        UpdateEvent += OnUpdate;

        _transform = new Transform3D();

        //spawn position for debugging
        // _transform.Position = new Vector3(20.5f, 3.5f, 20.5f);

        AddComponent(_transform);

        Camera = new Camera();

        if (!DebuggingData.IS_CAMERA_FREE_ROAM)
        {
            _rigidBody = new RigidBody
            {
                Mass = 70f,
                BoundingBox = new Aabb(new Vector3(-0.3f, -0.875f, -0.15f), new Vector3(0.6f, 1.75f, 0.3f)),
                RestitutionCoefficient = 0f
            };
            AddComponent(_rigidBody);
        }
    }

    public void PhysicsUpdate()
    {
        if (DebuggingData.IS_CAMERA_FREE_ROAM)
        {
            return;
        }

        MovePlayer();

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

    public void InputJumpChanged(bool isPressingJump)
    {
        _isJumpPressed = isPressingJump;
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

    private float _hotTimeFrontSeconds;
    private float _hotTimeBackSeconds;
    private float _hotTimeLeftSeconds;
    private float _hotTimeRightSeconds;
    private const float MAX_HOT_TIME = 0.5f;

    private void OnUpdate(double deltaTime)
    {
        Camera.Update(deltaTime);

        if (!DebuggingData.IS_CAMERA_FREE_ROAM)
        {
            return;
        }

        float speed = _isJumpPressed ? 18f : 6f;

        Vector3 runningFrontVector = Camera.FrontVector;
        Vector3 runningRightVector = Camera.RightVector;

        if ((_currentMovingDirections & Camera.CameraMovement.Forward) != Camera.CameraMovement.None)
        {
            _transform.Position += runningFrontVector * (float)deltaTime * speed;
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Backward) != Camera.CameraMovement.None)
        {
            _transform.Position += -runningFrontVector * (float)deltaTime * speed;
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Left) != Camera.CameraMovement.None)
        {
            _transform.Position += -runningRightVector * (float)deltaTime * speed;
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Right) != Camera.CameraMovement.None)
        {
            _transform.Position += runningRightVector * (float)deltaTime * speed;
        }
    }

    private void MovePlayer()
    {
        //normalize so the camera pitch is not considered
        Vector3 runningFrontVector = new Vector3(Camera.FrontVector.X, 0f, Camera.FrontVector.Z).Normalized();
        Vector3 runningRightVector = new Vector3(Camera.RightVector.X, 0f, Camera.RightVector.Z).Normalized();

        const float JUMP_FORCE = 6f;
        const float MOVE_FRONTAL_VELOCITY = 6f;
        const float MOVE_LATERAL_VELOCITY = 3f;

        Vector3 finalVelocity = _rigidBody.DerivedVelocity / (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
        if (_isJumpPressed && _isOnGround)
        {
            finalVelocity += Camera.WorldUpVector * (JUMP_FORCE - finalVelocity.Y);
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Forward) != Camera.CameraMovement.None)
        {
            finalVelocity += runningFrontVector * ComputeVelocity(_hotTimeFrontSeconds, MOVE_FRONTAL_VELOCITY);
            if (_hotTimeFrontSeconds <= MAX_HOT_TIME)
            {
                _hotTimeFrontSeconds += (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
            }
        }
        else if (_hotTimeFrontSeconds > 0f)
        {
            finalVelocity += runningFrontVector * ComputeVelocity(_hotTimeFrontSeconds, MOVE_FRONTAL_VELOCITY);
            _hotTimeFrontSeconds -= (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Backward) != Camera.CameraMovement.None)
        {
            finalVelocity += -runningFrontVector * ComputeVelocity(_hotTimeBackSeconds, MOVE_FRONTAL_VELOCITY);
            if (_hotTimeBackSeconds <= MAX_HOT_TIME)
            {
                _hotTimeBackSeconds += (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
            }
        }
        else if (_hotTimeBackSeconds > 0f)
        {
            finalVelocity += -runningFrontVector * ComputeVelocity(_hotTimeBackSeconds, MOVE_FRONTAL_VELOCITY);
            _hotTimeBackSeconds -= (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Left) != Camera.CameraMovement.None)
        {
            finalVelocity += -runningRightVector * ComputeVelocity(_hotTimeLeftSeconds, MOVE_LATERAL_VELOCITY);
            if (_hotTimeLeftSeconds <= MAX_HOT_TIME)
            {
                _hotTimeLeftSeconds += (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
            }
        }
        else if (_hotTimeLeftSeconds > 0f)
        {
            finalVelocity += -runningRightVector * ComputeVelocity(_hotTimeLeftSeconds, MOVE_LATERAL_VELOCITY);
            _hotTimeLeftSeconds -= (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
        }

        if ((_currentMovingDirections & Camera.CameraMovement.Right) != Camera.CameraMovement.None)
        {
            finalVelocity += runningRightVector * ComputeVelocity(_hotTimeRightSeconds, MOVE_LATERAL_VELOCITY);
            if (_hotTimeRightSeconds <= MAX_HOT_TIME)
            {
                _hotTimeRightSeconds += (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
            }
        }
        else if (_hotTimeRightSeconds > 0f)
        {
            finalVelocity += runningRightVector * ComputeVelocity(_hotTimeRightSeconds, MOVE_LATERAL_VELOCITY);
            _hotTimeRightSeconds -= (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
        }

        if (_isOnGround)
        {
            finalVelocity -=
                finalVelocity * new Vector3(Math.Clamp(20.0f / finalVelocity.Length, 0f, 1.5f) * 0.05f);

            finalVelocity.X = Math.Clamp(finalVelocity.X, -6f, 6f);
            finalVelocity.Z = Math.Clamp(finalVelocity.Z, -6f, 6f);

            _rigidBody.SetDirectVelocity(finalVelocity);
        }
    }


    private static float ComputeVelocity(float hotTime, float maxSpeed)
    {
        hotTime /= MAX_HOT_TIME;
        float sineInOutResult = -(MathF.Cos(MathF.PI * hotTime) - 1.0f) / 2.0f;

        return sineInOutResult * maxSpeed * 0.1f;
    }
}