using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.Prefabs.Delimiters;
using OpenGlGame.SceneSystem.Prefabs.Lights;
using OpenGlGame.SceneSystem.Prefabs.Misc;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels.Level3;

public class Level3Data : ILevelData
{
    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects = [];

        TransitionRoom transInRoom = new(false);
        transInRoom.SetYRotation(float.DegreesToRadians(180f));

        GameObject[] transitionInRoom = transInRoom.LoadAndGetGameObjects();
        TransitionRoom.OffsetObjects(transitionInRoom, TransitionRoom.InRoomSpawnPosition);
        TransitionRoom.RotateObjectsAroundPoint(
            transitionInRoom,
            TransitionRoom.InRoomSpawnPosition,
            Quaternion.FromAxisAngle(Vector3.UnitY, float.DegreesToRadians(180f)));

        objects.AddRange(transitionInRoom);

        AddDelimiters(objects);
        AddFinishFlag(objects);
        AddDecoRocks(objects);
        AddLights(objects);

        return objects.ToArray();
    }

    private static void AddDelimiters(List<GameObject> objects)
    {
        objects.AddRange([
            //exit
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(0f, -2.0f, -6.3f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(2f, 1f, 0.4f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(1f, -0.5f, -6.3f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(0.4f, 1f, 3f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(-1f, -0.5f, -6.3f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(0.4f, 1f, 3f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(0f, 1.0f, -6.3f),
                    Rotation = new Quaternion(float.DegreesToRadians(180f), 0f, 0f),
                    Scale = new Vector3(2f, 1f, 0.4f)
                }),

            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(0f, -2.25f, -6.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(4f, 1f, 0.5f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(1.5f, 0.0f, -6.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(1f, 1f, 4f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(-1.5f, 0.0f, -6.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(1f, 1f, 4f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(0.0f, 1.5f, -6.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(2f, 1f, 1f)
                }),

            //platforms
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-2.5f, -2.5f, -7.75f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(3f, 0.5f, 2.5f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(0.5f, -2.5f, -7.75f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(3f, 0.5f, 2.5f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-7.5f, -1.5f, -7.75f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(4f, 0.5f, 2.5f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-11.5f, 0f, -7.75f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(2f, 0.5f, 2.5f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-15.5f, 1.5f, -7.75f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(2f, 0.5f, 2.5f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-11.5f, 3f, -7.0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(2f, 0.5f, 1.25f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-7.5f, 4.5f, -7.0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(2f, 0.5f, 2.25f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-4.5f, 6f, -7.0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(1.75f, 0.5f, 2.0f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-7.5f, 7.5f, -7.0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(1.5f, 0.5f, 1.25f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-10.5f, 9f, -7.0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(1.5f, 0.5f, 1.0f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-13.5f, 10.5f, -7.0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(1.25f, 0.5f, 1.25f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-15.5f, 12f, -7.0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(1.25f, 0.5f, 1.25f)
                }),
            new RockyPlatform01(
                new Transform3D
                {
                    Position = new Vector3(-18f, 13.5f, -7.0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(1.75f, 0.5f, 1.25f)
                }),

            //big walls
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(17.0f, -35f, -6.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(30f, 1f, 100f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(-17.0f, -35f, -6.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(30f, 1f, 100f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(0.0f, 8.5f, -6.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(4f, 1f, 13f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(0.0f, -43.75f, -6.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(4f, 1f, 82.5f)
                }),

            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(0.0f, 15f, 3.5f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(64f, 1f, 20f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(32.0f, -35f, 3.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(20f, 1f, 100f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(-32.0f, -35f, 3.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(20f, 1f, 100f)
                }),
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(0.0f, -35f, 13.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, 0f),
                    Scale = new Vector3(64f, 1f, 100f)
                }),

            //floor
            new RockyWall01(
                new Transform3D
                {
                    Position = new Vector3(0f, -85f, 0f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(364f, 1f, 300f)
                })
        ]);
    }

    private static void AddFinishFlag(List<GameObject> objects)
    {
        objects.AddRange([
            new FlagPole(
                new Transform3D
                {
                    Position = new Vector3(-10f, 15f, -2f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new Flag(
                new Transform3D
                {
                    Position = new Vector3(-10f, 16f, -1.1f),
                    Rotation = new Quaternion(0f, 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(1f, 1f, 1f)
                })
        ]);
    }

    private static void AddDecoRocks(List<GameObject> objects)
    {
        objects.AddRange([
            new DecoRock01(
                new Transform3D
                {
                    Position = new Vector3(20f, -75f, -70f),
                    Rotation = new Quaternion(0f, 0f, 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new DecoRock01(
                new Transform3D
                {
                    Position = new Vector3(60f, -75f, -90f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(45f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new DecoRock01(
                new Transform3D
                {
                    Position = new Vector3(120f, -75f, -60f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(52f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new DecoRock01(
                new Transform3D
                {
                    Position = new Vector3(10f, -75f, -120f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(32f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new DecoRock01(
                new Transform3D
                {
                    Position = new Vector3(-30f, -78f, -100f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(-42f), 0f),
                    Scale = new Vector3(0.7f, 0.7f, 0.7f)
                }),
            new DecoRock01(
                new Transform3D
                {
                    Position = new Vector3(-10f, -78f, -70f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(-42f), 0f),
                    Scale = new Vector3(0.7f, 0.7f, 0.7f)
                }),
            new DecoRock01(
                new Transform3D
                {
                    Position = new Vector3(-110f, -75f, -120f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(-118f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new DecoRock01(
                new Transform3D
                {
                    Position = new Vector3(-130f, -75f, -20f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(-38f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),

            new DecoRock02(
                new Transform3D
                {
                    Position = new Vector3(80f, -73f, -50f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(28f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new DecoRock02(
                new Transform3D
                {
                    Position = new Vector3(160f, -73f, -110f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(162f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new DecoRock02(
                new Transform3D
                {
                    Position = new Vector3(100f, -78f, -100f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(117f), 0f),
                    Scale = new Vector3(0.7f, 0.7f, 0.7f)
                }),
            new DecoRock02(
                new Transform3D
                {
                    Position = new Vector3(140f, -78f, -20f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(5f), 0f),
                    Scale = new Vector3(0.7f, 0.7f, 0.7f)
                }),
            new DecoRock02(
                new Transform3D
                {
                    Position = new Vector3(-40f, -75f, -50f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(-68f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new DecoRock02(
                new Transform3D
                {
                    Position = new Vector3(-70f, -82f, -70f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(61f), 0f),
                    Scale = new Vector3(0.5f, 0.5f, 0.5f)
                }),
            new DecoRock02(
                new Transform3D
                {
                    Position = new Vector3(-75f, -82f, -120f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(3f), 0f),
                    Scale = new Vector3(0.5f, 0.5f, 0.5f)
                }),
            new DecoRock02(
                new Transform3D
                {
                    Position = new Vector3(-100f, -75f, -60f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(132f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new DecoRock02(
                new Transform3D
                {
                    Position = new Vector3(-150f, -75f, -100f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(2f), 0f),
                    Scale = new Vector3(1f, 1f, 1f)
                })
        ]);
    }

    private static void AddLights(List<GameObject> objects)
    {
        Vector3 lightPos = new(-50f, 25f, -45f);
        objects.Add(
            new Light02(
                new Transform3D
                {
                    Position = lightPos
                },
                new Renderer.PointLightData(
                    lightPos,
                    new Vector3(1f, 1f, 1f),
                    0.2f,
                    0.65f,
                    0.2f,
                    0.3f, 0.0f, 0.0f)));
    }
}