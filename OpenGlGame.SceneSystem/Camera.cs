using System;
using OpenGlGame.Rendering;
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

    public Vector3 Position { get; private set; } = new(0.0f, 0.0f, -3.0f);

    // private const float MAX_SPEED_SPRINT = 10.0f;
    private const float MAX_SPEED_RUNNING = 6.0f;
    private const float MOUSE_SENSITIVITY = 0.35f;

    private CameraMovement _currentMovingDirections;

    private float _yaw = 90.0f;
    private float _pitch;

    private Vector3 _frontVector;
    private Vector3 _upVector;
    private Vector3 _rightVector;
    private readonly Vector3 _worldUpVector = new(0.0f, 1.0f, 0.0f);

    public Camera()
    {
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
        float velocity = (float)(MAX_SPEED_RUNNING * deltaTime);
        if ((_currentMovingDirections & CameraMovement.Forward) != CameraMovement.None)
        {
            Position += _frontVector * velocity;
        }

        if ((_currentMovingDirections & CameraMovement.Backward) != CameraMovement.None)
        {
            Position -= _frontVector * velocity;
        }

        if ((_currentMovingDirections & CameraMovement.Left) != CameraMovement.None)
        {
            Position -= _rightVector * velocity;
        }

        if ((_currentMovingDirections & CameraMovement.Right) != CameraMovement.None)
        {
            Position += _rightVector * velocity;
        }

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
}