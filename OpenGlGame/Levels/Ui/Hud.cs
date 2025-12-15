using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Prefabs.Ui;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels.Ui;

public class Hud : ILevelData
{
    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects = [];

        objects.AddRange([
            new Icon(new Vector2(-0.02f, -0.02f), new Vector2(0.04f, 0.04f * Renderer.ASPECT_RATIO))
            {
                TileOffset = new Vector2(32f / Icon.ICON_ATLAS_WIDTH, 0f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(32f / Icon.ICON_ATLAS_WIDTH, 32f / Icon.ICON_ATLAS_HEIGHT)
            }
        ]);

        return objects.ToArray();
    }
}