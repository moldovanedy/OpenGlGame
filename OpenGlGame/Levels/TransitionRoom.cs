using System.Collections.Generic;
using System.Threading.Tasks;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.Prefabs.Delimiters;
using OpenGlGame.SceneSystem.Prefabs.Interactables;
using OpenGlGame.SceneSystem.Prefabs.Lights;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels;

public class TransitionRoom : ILevelData
{
    public static Vector3 InRoomSpawnPosition { get; } = new(0f, 0f, -4.101f);

    private Door01? _movingLeftDoor;
    private Door01? _movingRightDoor;
    private Btn01? _button;

    private readonly bool _isOutRoom;
    private readonly TransitionRoomCallbacks _callbackManager = new();

    public TransitionRoom(bool isOutRoom)
    {
        _isOutRoom = isOutRoom;
    }

    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects = [];
        AddSolidObjects(objects);
        AddButtonStand(objects);
        AddLights(objects);

        objects[0].UpdateEvent += deltaTime => { _callbackManager.Update(deltaTime); };
        if (!_isOutRoom)
        {
            Task.Run(async () =>
            {
                await Task.Delay(300);
                if (_movingLeftDoor != null && _movingRightDoor != null)
                {
                    MainThreadDispatcher.Enqueue(() =>
                    {
                        _callbackManager.OpenDoors(_movingLeftDoor, _movingRightDoor);
                    });
                }
            });
        }

        return objects.ToArray();
    }

    public static void OffsetObjects(GameObject[] objects, Vector3 offset)
    {
        foreach (GameObject obj in objects)
        {
            if (obj.GetComponent(nameof(Transform3D)) is Transform3D transform)
            {
                transform.Position += offset;
            }

            if (obj.GetComponent(nameof(PointLight)) is PointLight pointLight)
            {
                pointLight.LightData = new Renderer.PointLightData(
                    pointLight.LightData.Position + offset,
                    pointLight.LightData.LightColor,
                    pointLight.LightData.AmbientIntensity,
                    pointLight.LightData.DiffuseIntensity,
                    pointLight.LightData.SpecularIntensity,
                    pointLight.LightData.ConstantAttenuation,
                    pointLight.LightData.LinearAttenuation,
                    pointLight.LightData.QuadraticAttenuation
                );
            }
        }
    }

    private void AddSolidObjects(List<GameObject> objects)
    {
        objects.AddRange([
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(0.0f, -2.0f, 0.0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(4f, 1f, 4f)
                }),
            //major
            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(2.0f, 0.0f, 0.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(4f, 1f, 4f)
                }),
            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(-2.0f, 0.0f, 0.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(4f, 1f, 4f)
                }),
            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(0.0f, 2.0f, 0.0f),
                    Rotation = new Quaternion(0f, 0f, float.DegreesToRadians(180f)),
                    Scale = new Vector3(4f, 1f, 4f)
                }),

            //minor
            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(1.5f, 0.0f, 2.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(1f, 1f, 4f)
                }),
            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(-1.5f, 0.0f, 2.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(1f, 1f, 4f)
                }),
            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(0.0f, 1.5f, 2.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(2f, 1f, 1f)
                }),

            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(1.5f, 0.0f, -2.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(1f, 1f, 4f)
                }),
            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(-1.5f, 0.0f, -2.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(1f, 1f, 4f)
                }),
            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(0.0f, 1.5f, -2.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(2f, 1f, 1f)
                })
        ]);

        float inDoorsX = _isOutRoom ? TransitionRoomCallbacks.OPEN_DOOR_X : TransitionRoomCallbacks.CLOSED_DOOR_X;

        //doors
        Door01 inLeftDoor = new(
            new Transform3D
            {
                Position = new Vector3(inDoorsX, -0.499f, -1.924f),
                Rotation = new Quaternion(float.DegreesToRadians(90f), float.DegreesToRadians(-90f), 0f),
                Scale = new Vector3(2.998f, 0.15f, 1f)
            });
        Door01 inRightDoor = new(
            new Transform3D
            {
                Position = new Vector3(-inDoorsX, -0.499f, -1.924f),
                Rotation = new Quaternion(float.DegreesToRadians(90f), float.DegreesToRadians(90f), 0f),
                Scale = new Vector3(2.998f, 0.15f, 1f)
            });

        Door01 outLeftDoor = new(
            new Transform3D
            {
                Position = new Vector3(TransitionRoomCallbacks.CLOSED_DOOR_X, -0.499f, 1.926f),
                Rotation = new Quaternion(float.DegreesToRadians(-90f), float.DegreesToRadians(-90f), 0f),
                Scale = new Vector3(2.998f, 0.15f, 1f)
            });
        Door01 outRightDoor = new(
            new Transform3D
            {
                Position = new Vector3(-TransitionRoomCallbacks.CLOSED_DOOR_X, -0.499f, 1.926f),
                Rotation = new Quaternion(float.DegreesToRadians(-90f), float.DegreesToRadians(90f), 0f),
                Scale = new Vector3(2.998f, 0.15f, 1f)
            });

        if (_isOutRoom)
        {
            _movingLeftDoor = inLeftDoor;
            _movingRightDoor = inRightDoor;
        }
        else
        {
            _movingLeftDoor = outLeftDoor;
            _movingRightDoor = outRightDoor;
        }

        objects.AddRange([inLeftDoor, inRightDoor, outLeftDoor, outRightDoor]);
    }

    private void AddButtonStand(List<GameObject> objects)
    {
        _button = new Btn01(
            new Transform3D
            {
                Position = new Vector3(1.74f, -0.775f, 0f),
                Rotation = new Quaternion(0f, 0f, float.DegreesToRadians(35f)),
                Scale = new Vector3(1f, 1f, 1f)
            },
            !_isOutRoom)
        {
            PressedCallback = () =>
            {
                if (_movingRightDoor != null && _movingLeftDoor != null)
                {
                    _callbackManager.CloseDoors(_movingLeftDoor, _movingRightDoor);
                    Task.Run(async () =>
                    {
                        await Task.Delay(700);
                        MainThreadDispatcher.Enqueue(() =>
                        {
                            Vector3 pos = Vector3.Zero;
                            if (_button?.GetComponent(nameof(Transform3D)) is Transform3D btnTransform)
                            {
                                pos = btnTransform.Position;
                            }

                            Player.Instance.RequestLoadingNextLevel(pos);
                        });
                    });
                }
            }
        };

        objects.AddRange([
            new BtnStand01(
                new Transform3D
                {
                    Position = new Vector3(1.74f, -1.325f, 0f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            _button
        ]);
    }

    private static void AddLights(List<GameObject> objects)
    {
        Vector3 light1Pos = new(0.0f, 2.0f, 0.0f);
        var lightSupport1 = new LightSupport01(
            new Transform3D
            {
                Position = light1Pos
            });
        var light1 = new Light01(
            lightSupport1,
            new Renderer.PointLightData(
                light1Pos,
                new Vector3(1f, 1f, 1f),
                0.08f, 0.7f, 0.3f,
                1.0f, 0.35f, 0.44f));

        objects.AddRange([lightSupport1, light1]);
    }
}