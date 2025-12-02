using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.Prefabs;
using OpenTK.Mathematics;

namespace OpenGlGame.Chunks.Chunk01;

public class LevelChunk01 : SceneManager.WorldChunk
{
    private static readonly GameObject[] Objects =
    [
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
        )
    ];

    public LevelChunk01() : base(Objects, Vector3.Zero, new Vector3(500f, 100f, 500f))
    {
    }
}