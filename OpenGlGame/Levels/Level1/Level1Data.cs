using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.Prefabs.Misc;
using OpenGlGame.SceneSystem.Prefabs.Delimiters;
using OpenGlGame.SceneSystem.Prefabs.Lights;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels.Level1;

public class Level1Data : ILevelData
{
    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects = [];
        AddMoveTraining(objects);
        AddJumpTraining(objects);
        AddLights(objects);

        GameObject[] transitionRoom = new TransitionRoom().LoadAndGetGameObjects();
        TransitionRoom.OffsetObjects(transitionRoom, new Vector3(18.0f, 4.25f, 27.101f));
        objects.AddRange(transitionRoom);
        return objects.ToArray();
    }

    private static void AddMoveTraining(List<GameObject> objects)
    {
        objects.AddRange([
            new Cube01(
                new Transform3D
                {
                    Position = new Vector3(1.5f, 1.0f, 15.0f)
                }),

            //move training floor
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(0.0f, -1.75f, 10.0f),
                    Scale = new Vector3(8f, 1f, 30f)
                }
            ),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(10.0f, -1.75f, 21.0f),
                    Scale = new Vector3(12f, 1f, 8f)
                }
            ),

            //walls
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(0f, 0.25f, -5.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), float.DegreesToRadians(180f),
                        float.DegreesToRadians(0f)),
                    Scale = new Vector3(8f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(-4f, 0.25f, 10.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(30f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(4f, 0.25f, 6.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(22f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(6f, 0.25f, 25.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(20f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(10f, 0.25f, 17.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(180f)),
                    Scale = new Vector3(12f, 1f, 4f)
                })
        ]);
    }

    private static void AddJumpTraining(List<GameObject> objects)
    {
        objects.AddRange([
            //steps
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(16f, -1.25f, 18.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(3f, 1f, 1f)
                }),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(17.5f, -0.75f, 18.5f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(3f, 1f, 3f)
                }),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(19f, -0.25f, 18.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(3f, 1f, 1f)
                }),
            //corner
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(20.5f, 0.25f, 18.5f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(3f, 1f, 3f)
                }),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(20.5f, 0.75f, 20.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(3f, 1f, 1f)
                }),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(20.5f, 1.25f, 20.5f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(1f, 1f, 3f)
                }),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(20.5f, 1.75f, 21.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(3f, 1f, 1f)
                }),
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(19.0f, 2.25f, 23.0f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(4f, 1f, 6f)
                }),

            //filler
            new Floor01(
                new Transform3D
                {
                    Position = new Vector3(17.5f, 2.25f, 20.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(0f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(1f, 1f, 3f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(19.0f, 1.75f, 20.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(17.5f, 0.75f, 20.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(3f, 1f, 3f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(16f, 0.25f, 22.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(5f, 1f, 4f)
                }),

            //walls
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(19.0f, 4.25f, 17.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(6f, 1f, 10f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(22.0f, 4.25f, 21.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(8f, 1f, 10f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(20.5f, 5.25f, 25.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(3f, 1f, 8f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(14.0f, 4.25f, 25.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(4f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(16.5f, 5.75f, 25.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(1f, 1f, 7f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(18.0f, 7.25f, 25.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, 0f),
                    Scale = new Vector3(2f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(14.0f, 4.25f, 17.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), float.DegreesToRadians(180f), 0f),
                    Scale = new Vector3(4f, 1f, 4f)
                }),

            //transition seams
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(17.0f, 3.75f, 25.05f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(0.1f, 1f, 3f)
                },
                hasBackfaceCulling: false),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(19.0f, 3.75f, 25.05f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(0.1f, 1f, 3f)
                },
                hasBackfaceCulling: false),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(18.0f, 5.25f, 25.05f),
                    Rotation = new Quaternion(float.DegreesToRadians(180f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(0.1f, 1f, 2f)
                },
                hasBackfaceCulling: false),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(18.0f, 2.25f, 25.05f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(-90f), 0f),
                    Scale = new Vector3(0.1f, 1f, 2f)
                },
                hasBackfaceCulling: false),

            //roof
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(0.0f, 2.25f, 6.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(180f), 0f, 0f),
                    Scale = new Vector3(8f, 1f, 22f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(4.0f, 2.25f, 21.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(180f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(8f, 1f, 16f)
                }),

            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(12.0f, 4.25f, 21.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(8f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(14.0f, 6.25f, 21.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(180f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(8f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(16.0f, 7.75f, 21.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(8f, 1f, 3f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(19.0f, 9.25f, 21.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(180f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(8f, 1f, 6f)
                })
        ]);
    }

    private static void AddLights(List<GameObject> objects)
    {
        //the first lights cover a distance of 50, the third one a distance of 32

        Vector3 light1Pos = new(-1.0f, 2.25f, 3.0f);
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
                1.0f, 0.09f, 0.032f));

        Vector3 light2Pos = new(7.0f, 2.25f, 22.0f);
        var lightSupport2 = new LightSupport01(
            new Transform3D
            {
                Position = light2Pos
            });
        var light2 = new Light01(
            lightSupport2,
            new Renderer.PointLightData(
                light2Pos,
                new Vector3(1f, 1f, 1f),
                0.08f, 0.7f, 0.3f,
                1.0f, 0.09f, 0.032f));

        Vector3 light3Pos = new(18.0f, 9.25f, 22.0f);
        var lightSupport3 = new LightSupport01(
            new Transform3D
            {
                Position = light3Pos
            });
        var light3 = new Light01(
            lightSupport3,
            new Renderer.PointLightData(
                light3Pos,
                new Vector3(0f, 1f, 1f),
                0.08f, 0.5f, 0.2f,
                1.0f, 0.14f, 0.07f));

        objects.AddRange([lightSupport1, light1, lightSupport2, light2, lightSupport3, light3]);
    }
}