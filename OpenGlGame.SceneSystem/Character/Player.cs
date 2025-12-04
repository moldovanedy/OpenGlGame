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

    public const bool FREE_ROAM_CAMERA = false;

    private readonly Transform3D _transform;
    private readonly RigidBody _rigidBody;

    private Camera.CameraMovement _currentMovingDirections;
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

        if (!FREE_ROAM_CAMERA)
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
        if (FREE_ROAM_CAMERA)
        {
            return;
        }

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
        Camera.Update(deltaTime);

        if (FREE_ROAM_CAMERA)
        {
            Vector3 runningFrontVector = Camera.FrontVector;
            Vector3 runningRightVector = Camera.RightVector;

            if ((_currentMovingDirections & Camera.CameraMovement.Forward) != Camera.CameraMovement.None)
            {
                _transform.Position += runningFrontVector * (float)deltaTime * 6f;
            }

            if ((_currentMovingDirections & Camera.CameraMovement.Backward) != Camera.CameraMovement.None)
            {
                _transform.Position += -runningFrontVector * (float)deltaTime * 6f;
            }

            if ((_currentMovingDirections & Camera.CameraMovement.Left) != Camera.CameraMovement.None)
            {
                _transform.Position += -runningRightVector * (float)deltaTime * 6f;
            }

            if ((_currentMovingDirections & Camera.CameraMovement.Right) != Camera.CameraMovement.None)
            {
                _transform.Position += runningRightVector * (float)deltaTime * 6f;
            }
        }
        else
        {
            Vector3 runningFrontVector = new(Camera.FrontVector.X, 0f, Camera.FrontVector.Z);
            Vector3 runningRightVector = new(Camera.RightVector.X, 0f, Camera.RightVector.Z);


            if (!_isOnGround)
            {
                return;
            }


            if (_currentMovingDirections == Camera.CameraMovement.None)
            {
                _rigidBody.SetDirectVelocity(Vector3.Zero);
            }

            if ((_currentMovingDirections & Camera.CameraMovement.Forward) != Camera.CameraMovement.None)
            {
                _rigidBody.SetDirectVelocity(runningFrontVector * 6f);
            }

            if ((_currentMovingDirections & Camera.CameraMovement.Backward) != Camera.CameraMovement.None)
            {
                _rigidBody.SetDirectVelocity(-runningFrontVector * 6f);
            }

            if ((_currentMovingDirections & Camera.CameraMovement.Left) != Camera.CameraMovement.None)
            {
                _rigidBody.SetDirectVelocity(-runningRightVector * 6f);
            }

            if ((_currentMovingDirections & Camera.CameraMovement.Right) != Camera.CameraMovement.None)
            {
                _rigidBody.SetDirectVelocity(runningRightVector * 6f);
            }
        }
    }
}