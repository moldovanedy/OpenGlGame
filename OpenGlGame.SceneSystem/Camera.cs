using System;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem;

public class Camera
{
    [Flags]
    public enum CameraMovement
    {
        None = 0,
        Forward = 1,
        Backward = 2,
        Left = 4,
        Right = 8
    }

    private Player _playerRef;

    public Vector3 Position { get; private set; } = new(0.0f, 0.0f, -3.0f);

    // private const float MAX_SPEED_SPRINT = 10.0f;
    // private const float MAX_SPEED_RUNNING = 6.0f;
    // private const float MAX_SPEED_RUNNING_SIDEWAYS = 3.0f;
    private const float MOUSE_SENSITIVITY = 0.35f;

    private CameraMovement _currentMovingDirections;

    private float _yaw = 90.0f;
    private float _pitch;

    private Vector3 _frontVector;
    private Vector3 _upVector;
    private Vector3 _rightVector;
    private readonly Vector3 _worldUpVector = new(0.0f, 1.0f, 0.0f);

    public Camera(Player player)
    {
        _playerRef = player;
        UpdateCameraVectors();
    }

    internal Matrix4 GetViewProjectionMatrix()
    {
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(60), Renderer.ASPECT_RATIO, 0.01f, 100f).Transposed();
        Matrix4 view = Matrix4.LookAt(Position, Position + _frontVector, _upVector).Transposed();

        return projection * view;
    }

    internal void Update(double deltaTime)
    {
        //TODO: implement a Kinematic Character Controller, as the character being a RigidBody is very difficult to
        // control
        var playerRigidBody = (RigidBody?)_playerRef.GetComponent(nameof(RigidBody));
        if (playerRigidBody == null)
        {
            throw new Exception("Camera: player didn't have a RigidBody");
        }

        if ((_currentMovingDirections & CameraMovement.Forward) != CameraMovement.None)
        {
            playerRigidBody.AddForce(_frontVector * playerRigidBody.Mass * (float)(deltaTime * 60f) * 5.0f);
        }

        if ((_currentMovingDirections & CameraMovement.Backward) != CameraMovement.None)
        {
            playerRigidBody.AddForce(-_frontVector * playerRigidBody.Mass * (float)(deltaTime * 60f) * 5.0f);
        }

        if ((_currentMovingDirections & CameraMovement.Left) != CameraMovement.None)
        {
            playerRigidBody.AddForce(-_rightVector * playerRigidBody.Mass * (float)(deltaTime * 60f) * 5.0f);
        }

        if ((_currentMovingDirections & CameraMovement.Right) != CameraMovement.None)
        {
            playerRigidBody.AddForce(_rightVector * playerRigidBody.Mass * (float)(deltaTime * 60f) * 5.0f);
        }

        //slow down the player 
        if (_currentMovingDirections == CameraMovement.None)
        {
            Vector3 horizontalVelocity =
                new(playerRigidBody.DerivedVelocity.X, 0f, playerRigidBody.DerivedVelocity.Z);
            Vector3 dragAcceleration =
                -horizontalVelocity * ComputeDecelerationRate(horizontalVelocity);
            Vector3 dragForce = dragAcceleration * playerRigidBody.Mass * (float)(deltaTime * 60f);
            playerRigidBody.AddForce(dragForce);
        }

        Position = ((Transform3D?)_playerRef.GetComponent(nameof(Transform3D)))?.Position ?? Position;
        UpdateCameraVectors();
    }

    public void InputMovementChanged(CameraMovement currentState)
    {
        _currentMovingDirections = currentState;
    }

    public void InputLookDirectionChanged(float xOffset, float yOffset)
    {
        xOffset *= MOUSE_SENSITIVITY;
        yOffset *= MOUSE_SENSITIVITY;

        _yaw += xOffset;
        _pitch += yOffset;

        _pitch = Math.Clamp(_pitch, -89.0f, 89.0f);
        // UpdateCameraVectors();
    }

    private void UpdateCameraVectors()
    {
        float yawRad = float.DegreesToRadians(_yaw);
        float pitchRad = float.DegreesToRadians(_pitch);

        var front = new Vector3(
            MathF.Cos(yawRad) * MathF.Cos(pitchRad),
            MathF.Sin(pitchRad),
            MathF.Sin(yawRad) * MathF.Cos(pitchRad));

        _frontVector = front.Normalized();
        _rightVector = Vector3.Cross(_frontVector, _worldUpVector).Normalized();
        _upVector = Vector3.Cross(_rightVector, _frontVector).Normalized();
    }

    private float ComputeDecelerationRate(Vector3 velocity)
    {
        return 120f * (float)Math.Clamp(1.0 / (velocity.Length * 15.0), 0.5f, 20f);
    }
}