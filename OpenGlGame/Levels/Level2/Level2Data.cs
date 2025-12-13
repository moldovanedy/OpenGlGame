using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.Prefabs.Delimiters;
using OpenGlGame.SceneSystem.Prefabs.Lights;
using OpenGlGame.SceneSystem.Prefabs.Misc;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels.Level2;

public class Level2Data : ILevelData
{
    private readonly Level2Callbacks _callbacks = new();

    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects = [];

        GameObject[] transitionRoom = new TransitionRoom(false).LoadAndGetGameObjects();
        TransitionRoom.OffsetObjects(transitionRoom, TransitionRoom.InRoomSpawnPosition);
        objects.AddRange(transitionRoom);

        AddStage1Delimiters(objects);
        AddStage2Delimiters(objects);
        AddPuzzleCubes(objects);
        AddLights(objects);

        objects[0].UpdateEvent += deltaTime => { _callbacks.Update(deltaTime); };
        return objects.ToArray();
    }

    private static void AddStage1Delimiters(List<GameObject> objects)
    {
        objects.AddRange([
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(0f, -2f, -2.05f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(-90f), 0f),
                    Scale = new Vector3(0.1f, 1f, 2f)
                }),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(0f, -2f, 8f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(4f, 1f, 20f)
                }),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(-10f, -2f, 21f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(24f, 1f, 6f)
                }),

            //walls
            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(0f, 1.0f, -2.05f),
                    Rotation = new Quaternion(float.DegreesToRadians(180f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(0.1f, 1f, 2f)
                }),
            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(1f, 0f, -2.05f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(0.1f, 1f, 4f)
                }),
            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(-1f, 0f, -2.05f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(0.1f, 1f, 4f)
                }),
            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(0f, 1.5f, -2.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, 0f),
                    Scale = new Vector3(2f, 1f, 1f)
                }),
            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(1.5f, 0f, -2f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, 0f),
                    Scale = new Vector3(1f, 1f, 4f)
                }),
            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(-1.5f, 0f, -2f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, 0f),
                    Scale = new Vector3(1f, 1f, 4f)
                }),

            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(2f, 0f, 11f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(26f, 1f, 4f)
                }),
            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(-2f, 0f, 8f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(20f, 1f, 4f)
                }),
            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(-10f, 0f, 24f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(24f, 1f, 4f)
                }),
            new Wall03(
                new Transform3D
                {
                    Position = new Vector3(-12f, 0f, 18f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(180f)),
                    Scale = new Vector3(20f, 1f, 4f)
                }),

            //ceiling
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(0f, 2f, 8f),
                    Rotation = new Quaternion(0f, 0f, float.DegreesToRadians(180f)),
                    Scale = new Vector3(4f, 1f, 20f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(-10f, 2f, 21f),
                    Rotation = new Quaternion(0f, 0f, float.DegreesToRadians(180f)),
                    Scale = new Vector3(24f, 1f, 6f)
                })
        ]);
    }

    private static void AddStage2Delimiters(List<GameObject> objects)
    {
        objects.AddRange([
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(-29.5f, -2f, 21f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(15f, 1f, 10f)
                }),
            new Wall02(
                new Transform3D
                {
                    Position = new Vector3(-29.5f, 0.5f, 16f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, 0f),
                    Scale = new Vector3(15f, 1f, 5f)
                }),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(-29.5f, 3f, 7f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(15f, 1f, 18f)
                }),

            //walls
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(-22f, 4f, 8f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(20f, 1f, 12f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(-22f, 4f, 25f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(2f, 1f, 12f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(-22f, 6f, 21f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(6f, 1f, 8f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(-29.5f, 4f, 26f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(0f)),
                    Scale = new Vector3(15f, 1f, 12f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(-37f, 4f, 12f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(28f, 1f, 12f)
                }),

            //ceiling
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(-29.5f, 10f, 12f),
                    Rotation = new Quaternion(float.DegreesToRadians(0f), 0f, float.DegreesToRadians(180f)),
                    Scale = new Vector3(15f, 1f, 28f)
                })
        ]);
    }

    private static void AddPuzzleCubes(List<GameObject> objects)
    {
        objects.AddRange([
            new Cube01(
                new Transform3D
                {
                    Position = new Vector3(-35f, -1.5f, 22f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                })
        ]);
    }

    private void AddLights(List<GameObject> objects)
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
                Level2Callbacks.EM_LIGHT_AMBIENT_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_DIFFUSE_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_SPECULAR_UPPER_LIMIT,
                1.0f, 0.14f, 0.07f));

        Vector3 light2Pos = new(0.0f, 2.0f, 10.0f);
        var light2Support = new LightSupport01(
            new Transform3D
            {
                Position = light2Pos
            });
        var light2 = new Light01(
            light2Support,
            new Renderer.PointLightData(
                light2Pos,
                new Vector3(1f, 0f, 0f),
                Level2Callbacks.EM_LIGHT_AMBIENT_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_DIFFUSE_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_SPECULAR_UPPER_LIMIT,
                1.0f, 0.22f, 0.20f));

        Vector3 light3Pos = new(0.0f, 2.0f, 21.0f);
        var light3Support = new LightSupport01(
            new Transform3D
            {
                Position = light3Pos
            });
        var light3 = new Light01(
            light3Support,
            new Renderer.PointLightData(
                light3Pos,
                new Vector3(1f, 0f, 0f),
                Level2Callbacks.EM_LIGHT_AMBIENT_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_DIFFUSE_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_SPECULAR_UPPER_LIMIT,
                1.0f, 0.22f, 0.20f));

        Vector3 light4Pos = new(-10.0f, 2.0f, 21.0f);
        var light4Support = new LightSupport01(
            new Transform3D
            {
                Position = light4Pos
            });
        var light4 = new Light01(
            light4Support,
            new Renderer.PointLightData(
                light4Pos,
                new Vector3(1f, 0f, 0f),
                Level2Callbacks.EM_LIGHT_AMBIENT_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_DIFFUSE_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_SPECULAR_UPPER_LIMIT,
                1.0f, 0.22f, 0.20f));

        Vector3 light5Pos = new(-30.0f, 10.0f, 21.0f);
        var light5Support = new LightSupport01(
            new Transform3D
            {
                Position = light5Pos
            });
        var light5 = new Light01(
            light5Support,
            new Renderer.PointLightData(
                light5Pos,
                new Vector3(1f, 1f, 1f),
                Level2Callbacks.EM_LIGHT_AMBIENT_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_DIFFUSE_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_SPECULAR_UPPER_LIMIT,
                1.0f, 0.14f, 0.07f));

        Vector3 light6Pos = new(-30.0f, 10.0f, 5.0f);
        var light6Support = new LightSupport01(
            new Transform3D
            {
                Position = light6Pos
            });
        var light6 = new Light01(
            light6Support,
            new Renderer.PointLightData(
                light6Pos,
                new Vector3(1f, 1f, 1f),
                Level2Callbacks.EM_LIGHT_AMBIENT_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_DIFFUSE_UPPER_LIMIT,
                Level2Callbacks.EM_LIGHT_SPECULAR_UPPER_LIMIT,
                1.0f, 0.14f, 0.07f));

        objects.AddRange([
            light1, lightSupport1,

            light2, light2Support,
            light3, light3Support,
            light4, light4Support,

            light5, light5Support,
            light6, light6Support
        ]);

        _callbacks.SetLights([light2, light3, light4]);
    }
}