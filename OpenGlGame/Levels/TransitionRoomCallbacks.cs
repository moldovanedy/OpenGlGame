using System;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.Prefabs.Delimiters;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels;

public class TransitionRoomCallbacks
{
    public const float OPEN_DOOR_X = 1.499f;
    public const float CLOSED_DOOR_X = 0.499f;

    internal float YRotation { get; set; } = 0f;

    private bool _isClosingDoors;

    private Door01? _leftDoorRef;
    private Door01? _rightDoorRef;
    private float _baseLeftX;
    private float _baseRightX;
    private double _doorTimer;

    internal void Update(double deltaTime)
    {
        MoveDoorsStep(deltaTime);
    }

    internal void CloseDoors(Door01 leftDoor, Door01 rightDoor)
    {
        _leftDoorRef = leftDoor;
        _rightDoorRef = rightDoor;

        var leftDoorTransform = _leftDoorRef.GetComponent(nameof(Transform3D)) as Transform3D;
        var rightDoorTransform = _rightDoorRef.GetComponent(nameof(Transform3D)) as Transform3D;

        if (leftDoorTransform == null || rightDoorTransform == null)
        {
            return;
        }

        _baseLeftX = leftDoorTransform.Position.X;
        _baseRightX = rightDoorTransform.Position.X;
        _isClosingDoors = true;
    }

    internal void OpenDoors(Door01 leftDoor, Door01 rightDoor)
    {
        _leftDoorRef = leftDoor;
        _rightDoorRef = rightDoor;

        var leftDoorTransform = _leftDoorRef.GetComponent(nameof(Transform3D)) as Transform3D;
        var rightDoorTransform = _rightDoorRef.GetComponent(nameof(Transform3D)) as Transform3D;

        if (leftDoorTransform == null || rightDoorTransform == null)
        {
            return;
        }

        _baseLeftX = leftDoorTransform.Position.X;
        _baseRightX = rightDoorTransform.Position.X;
        _isClosingDoors = false;
    }

    private void MoveDoorsStep(double deltaTime)
    {
        if (_leftDoorRef == null || _rightDoorRef == null)
        {
            return;
        }

        var leftDoorTransform = _leftDoorRef.GetComponent(nameof(Transform3D)) as Transform3D;
        var rightDoorTransform = _rightDoorRef.GetComponent(nameof(Transform3D)) as Transform3D;

        if (leftDoorTransform == null || rightDoorTransform == null)
        {
            return;
        }

        _doorTimer += deltaTime;
        float desiredLeftPos =
            _isClosingDoors
                ? _baseLeftX - 1f * MathF.Cos(YRotation)
                : _baseLeftX + 1f * MathF.Cos(YRotation);
        float desiredRightPos =
            _isClosingDoors
                ? _baseRightX + 1f * MathF.Cos(YRotation)
                : _baseRightX - 1f * MathF.Cos(YRotation);

        if (_doorTimer < 0.5f)
        {
            leftDoorTransform.Position = new Vector3(
                float.Lerp(_baseLeftX, desiredLeftPos, (float)_doorTimer * 2f),
                leftDoorTransform.Position.Y,
                leftDoorTransform.Position.Z);
            rightDoorTransform.Position = new Vector3(
                float.Lerp(_baseRightX, desiredRightPos, (float)_doorTimer * 2f),
                rightDoorTransform.Position.Y,
                rightDoorTransform.Position.Z);
        }
        else
        {
            leftDoorTransform.Position =
                new Vector3(desiredLeftPos, leftDoorTransform.Position.Y, leftDoorTransform.Position.Z);
            rightDoorTransform.Position =
                new Vector3(desiredRightPos, rightDoorTransform.Position.Y, rightDoorTransform.Position.Z);

            _leftDoorRef = null;
            _rightDoorRef = null;
        }
    }
}