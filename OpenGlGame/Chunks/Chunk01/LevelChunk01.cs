using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenGlGame.SceneSystem.Prefabs;
using OpenTK.Mathematics;

namespace OpenGlGame.Chunks.Chunk01;

public class LevelChunk01 : SceneManager.WorldChunk
{
    public LevelChunk01()
        : base(new Aabb(new Vector3(-250f, -50f, -250f), new Vector3(500f, 100f, 500f)))
    {
        GameObject[] objects =
        [
            new Cube01(
                new Transform3D
                {
                    Position = new Vector3(0.0f, 2.0f, 15.0f)
                }),

            #region Move training

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
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(180f)),
                    Scale = new Vector3(8f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(-4f, 0.25f, 10.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(30f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(4f, 0.25f, 6.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(22f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(6f, 0.25f, 25.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, 0f),
                    Scale = new Vector3(20f, 1f, 4f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(10f, 0.25f, 17.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(180f)),
                    Scale = new Vector3(12f, 1f, 4f)
                }),

            #endregion

            #region Jump training

            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(16f, 0.25f, 22.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(-90f)),
                    Scale = new Vector3(5f, 1f, 4f)
                }),

            //steps
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(16f, -1.25f, 18.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(3f, 1f, 1f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(17.5f, -0.75f, 18.5f),
                    Rotation = new Quaternion(0f, float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(3f, 1f, 3f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(19f, -0.25f, 18.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(3f, 1f, 1f)
                }),
            //corner
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(20.5f, 0.25f, 18.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-180f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(3f, 1f, 3f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(20.5f, 0.75f, 20.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, 0f),
                    Scale = new Vector3(3f, 1f, 1f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(20.5f, 1.25f, 20.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-180f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(1f, 1f, 3f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(20.5f, 1.75f, 21.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, 0f),
                    Scale = new Vector3(3f, 1f, 1f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(19.0f, 2.25f, 23.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(-180f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(4f, 1f, 6f)
                }),

            //filler
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(17.5f, 2.25f, 20.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(-180f), float.DegreesToRadians(90f), 0f),
                    Scale = new Vector3(1f, 1f, 3f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(19.0f, 1.75f, 20.5f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, float.DegreesToRadians(90f)),
                    Scale = new Vector3(1f, 1f, 1f)
                }),
            new Wall01(
                new Transform3D
                {
                    Position = new Vector3(17.5f, 0.75f, 20.0f),
                    Rotation = new Quaternion(float.DegreesToRadians(90f), 0f, 0f),
                    Scale = new Vector3(3f, 1f, 3f)
                }),

            #endregion
        ];

        GameObjects = objects;
    }
}