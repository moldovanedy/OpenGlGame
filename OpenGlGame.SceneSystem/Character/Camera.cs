using System;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Character;

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

    private const float MOUSE_SENSITIVITY = 0.35f;

    private float _yaw = 90.0f;
    private float _pitch;

    public Vector3 FrontVector { get; private set; }
    public Vector3 UpVector { get; private set; }
    public Vector3 RightVector { get; private set; }
    public Vector3 WorldUpVector { get; } = new(0.0f, 1.0f, 0.0f);

    public Camera()
    {
        UpdateCameraVectors();
    }

    internal Matrix4 GetViewProjectionMatrix()
    {
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            float.DegreesToRadians(60), Renderer.ASPECT_RATIO, 0.01f, 100f).Transposed();
        Matrix4 view = Matrix4.LookAt(Position, Position + FrontVector, UpVector).Transposed();

        return projection * view;
    }

    internal void Update(double deltaTime)
    {
        Position = ((Transform3D?)Player.Instance.GetComponent(nameof(Transform3D)))?.Position ?? Position;
        UpdateCameraVectors();
    }

    public void InputLookDirectionChanged(float xOffset, float yOffset)
    {
        xOffset *= MOUSE_SENSITIVITY;
        yOffset *= MOUSE_SENSITIVITY;

        _yaw += xOffset;
        _pitch += yOffset;

        _pitch = Math.Clamp(_pitch, -89.0f, 89.0f);
    }

    private void UpdateCameraVectors()
    {
        float yawRad = float.DegreesToRadians(_yaw);
        float pitchRad = float.DegreesToRadians(_pitch);

        var front = new Vector3(
            MathF.Cos(yawRad) * MathF.Cos(pitchRad),
            MathF.Sin(pitchRad),
            MathF.Sin(yawRad) * MathF.Cos(pitchRad));

        FrontVector = front.Normalized();
        RightVector = Vector3.Cross(FrontVector, WorldUpVector).Normalized();
        UpVector = Vector3.Cross(RightVector, FrontVector).Normalized();
    }
}