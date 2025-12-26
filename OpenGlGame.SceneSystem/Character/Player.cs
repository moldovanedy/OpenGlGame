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

    public static Vector3 SpawnPosition { get; } = new(0f, -1.12f, -4.101f);
    public static Vector3 PlayerSize { get; } = new(0.6f, 1.75f, 0.3f);

    public Camera Camera { get; private set; }
    public event Action? NextLevelRequestedEvent;
    public event Action? PlayerDeadEvent;

    public float Health { get; private set; } = 100;
    public bool IsDead { get; private set; }
    public bool IsNearInteractable { get; private set; }
    public bool HasPickedObjectUp { get; internal set; }

    private readonly Transform3D _transform;
    private readonly RigidBody _rigidBody;

    private Camera.CameraMovement _currentMovingDirections;
    private bool _isJumpPressed;
    private bool _isInteractButtonPressed;
    private bool _isCurrentlyInteracting;

    private bool _isOnGround;

    private Player()
    {
        UpdateEvent += OnUpdate;
        EnterSceneEvent += OnEnterScene;

        _transform = new Transform3D();
        AddComponent(_transform);

        Camera = new Camera();

        if (!DebuggingData.IS_CAMERA_FREE_ROAM)
        {
            _rigidBody = new RigidBody
            {
                Mass = 70f,
                BoundingBox = new Aabb(new Vector3(-0.3f, -0.875f, -0.15f), PlayerSize),
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

        if (IsDead)
        {
            //stop player
            _rigidBody.InternalPosition = _transform.Position;
            _rigidBody.InternalPreviousPosition = _transform.Position;
            return;
        }

        MovePlayer();
        _isOnGround = CheckIsOnGround();
    }

    public void InputMovementChanged(Camera.CameraMovement currentState)
    {
        if (IsDead)
        {
            return;
        }

        _currentMovingDirections = currentState;
    }

    public void InputJumpChanged(bool isPressingJump)
    {
        if (IsDead)
        {
            return;
        }

        _isJumpPressed = isPressingJump;
    }

    public void InputInteractButtonChanged(bool isPressingInteractButton)
    {
        if (IsDead)
        {
            return;
        }

        _isInteractButtonPressed = isPressingInteractButton;
        _isCurrentlyInteracting = false;
    }

    public void RequestLoadingNextLevel(Vector3 teleportButtonCurrentPosition)
    {
        //1.74 is the X offset of the teleport button from the transition room center
        float deltaX = _transform.Position.X - teleportButtonCurrentPosition.X + 1.74f;
        float deltaZ = _transform.Position.Z - teleportButtonCurrentPosition.Z;

        NextLevelRequestedEvent?.Invoke();
        _transform.Position = new Vector3(SpawnPosition.X + deltaX, SpawnPosition.Y, SpawnPosition.Z + deltaZ);

        _rigidBody.InternalPosition = _transform.Position;
        _rigidBody.InternalPreviousPosition = _transform.Position;
    }

    public void DecreaseHealth(float amount)
    {
        //prevent further decrease
        if (Health <= 0)
        {
            return;
        }

        Health -= amount;

        if (Health <= 0)
        {
            IsDead = true;
            PlayerDeadEvent?.Invoke();
        }
    }


    private void CheckInteractableHit(out Interactable? objectInRange)
    {
        objectInRange = null;
        if (_isCurrentlyInteracting)
        {
            return;
        }

        List<Interactable> interactables = PhysicsSystem.Instance.GetCurrentInteractables();
        foreach (Interactable interactable in interactables)
        {
            var interactableTransform = (Transform3D?)interactable.Owner?.GetComponent(nameof(Transform3D));
            if (interactableTransform == null)
            {
                throw new Exception("Interactable did not have a Transform3D component");
            }

            Aabb colliderAabb = new(interactable.BoundingBox, interactableTransform.GetModelMatrix());
            PhysicsSystem.RaycastHit? hit = PhysicsSystem.AabbRaycastTest(
                Camera.Position,
                Camera.FrontVector.Normalized(),
                colliderAabb);

            if (hit != null && hit.Value.Distance < 1.5f)
            {
                objectInRange = interactable;
                if (_isInteractButtonPressed)
                {
                    interactable.InteractCallback();
                    _isCurrentlyInteracting = true;
                }

                return;
            }
        }
    }

    private static bool CheckIsOnGround(Aabb playerAabb, Aabb colliderAabb)
    {
        const float EPSILON = 0.01f;
        bool isRaycastHit = false;

        Vector3[] testPositions =
        [
            Instance._transform.Position,
            Instance._transform.Position + new Vector3(PlayerSize.X / 2f - EPSILON, 0f, 0f),
            Instance._transform.Position + new Vector3(-PlayerSize.X / 2f + EPSILON, 0f, 0f),
            Instance._transform.Position + new Vector3(0f, 0f, PlayerSize.Z / 2f - EPSILON),
            Instance._transform.Position + new Vector3(0f, 0f, -PlayerSize.Z / 2f + EPSILON)
        ];

        foreach (Vector3 position in testPositions)
        {
            PhysicsSystem.RaycastHit? hit = PhysicsSystem.AabbRaycastTest(
                position,
                new Vector3(0f, -1f, 0f),
                colliderAabb);

            if (hit != null)
            {
                isRaycastHit = hit.Value.Distance < PlayerSize.Y / 2f + EPSILON;
            }

            if (isRaycastHit)
            {
                break;
            }
        }

        bool horizontal =
            playerAabb.Start.X + playerAabb.Extents.X > colliderAabb.Start.X &&
            playerAabb.Start.X < colliderAabb.Start.X + colliderAabb.Extents.X &&
            playerAabb.Start.Z + playerAabb.Extents.Z > colliderAabb.Start.Z &&
            playerAabb.Start.Z < colliderAabb.Start.Z + colliderAabb.Extents.Z;

        bool vertical =
            playerAabb.Start.Y <= colliderAabb.End.Y + EPSILON && // feet not below the surface
            playerAabb.Start.Y >= colliderAabb.End.Y - EPSILON; // feet touching

        return (horizontal && vertical) || isRaycastHit;
    }

    private float _hotTimeFrontSeconds;
    private float _hotTimeBackSeconds;
    private float _hotTimeLeftSeconds;
    private float _hotTimeRightSeconds;
    private const float MAX_HOT_TIME = 0.5f;

    private void OnUpdate(double deltaTime)
    {
        Camera.Update(deltaTime);

        CheckInteractableHit(out Interactable? objectInRange);
        IsNearInteractable = objectInRange?.Owner != null;

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

    private void OnEnterScene()
    {
        IsDead = false;
        Health = 100;
        _transform.Position = new Vector3(SpawnPosition.X, SpawnPosition.Y + 0.5f, SpawnPosition.Z);

        _rigidBody.InternalPosition = _transform.Position;
        _rigidBody.InternalPreviousPosition = _transform.Position;

        _hotTimeFrontSeconds = 0f;
        _hotTimeBackSeconds = 0f;
        _hotTimeLeftSeconds = 0f;
        _hotTimeRightSeconds = 0f;
        _currentMovingDirections = Camera.CameraMovement.None;

        _isJumpPressed = false;
        _isInteractButtonPressed = false;
        _isOnGround = false;

        Camera.ResetCameraOrientation();
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

    private bool CheckIsOnGround()
    {
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
                return true;
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
                return true;
            }
        }

        return false;
    }

    private static float ComputeVelocity(float hotTime, float maxSpeed)
    {
        hotTime /= MAX_HOT_TIME;
        float sineInOutResult = -(MathF.Cos(MathF.PI * hotTime) - 1.0f) / 2.0f;

        return sineInOutResult * maxSpeed * 0.1f;
    }
}