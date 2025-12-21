using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Prefabs.Ui;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels.Ui;

public class PlayInstructions : ILevelData
{
    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects =
        [
            //background
            new UiRect(
                new Vector2(0.6f, -0.5f),
                new Vector2(0.35f, 1.0f),
                true,
                1)
            {
                Color = new Vector4(0.08f, 0.08f, 0.08f, 0.8f)
            }
        ];

        AddIcons(objects);
        AddTexts(objects);

        return objects.ToArray();
    }

    private static void AddIcons(List<GameObject> objects)
    {
        objects.AddRange([
            new Icon(
                new Vector2(0.62f, 0.37f),
                new Vector2(0.1f, 0.1f * Renderer.ASPECT_RATIO),
                2)
            {
                TileOffset = new Vector2(64f / Icon.ICON_ATLAS_WIDTH, 0f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(64f / Icon.ICON_ATLAS_WIDTH, 64f / Icon.ICON_ATLAS_HEIGHT)
            },
            new Icon(
                new Vector2(0.62f, 0.2f),
                new Vector2(0.1f, 0.1f * Renderer.ASPECT_RATIO),
                2)
            {
                TileOffset = new Vector2(128f / Icon.ICON_ATLAS_WIDTH, 0f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(64f / Icon.ICON_ATLAS_WIDTH, 64f / Icon.ICON_ATLAS_HEIGHT)
            },

            new Icon(
                new Vector2(0.62f, 0.0f),
                new Vector2(0.1f, 0.1f * Renderer.ASPECT_RATIO),
                2)
            {
                TileOffset = new Vector2(192f / Icon.ICON_ATLAS_WIDTH, 0f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(64f / Icon.ICON_ATLAS_WIDTH, 64f / Icon.ICON_ATLAS_HEIGHT)
            },
            new Icon(
                new Vector2(0.62f, -0.23f),
                new Vector2(0.1f, 0.1f * Renderer.ASPECT_RATIO),
                2)
            {
                TileOffset = new Vector2(0f / Icon.ICON_ATLAS_WIDTH, 64f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(64f / Icon.ICON_ATLAS_WIDTH, 64f / Icon.ICON_ATLAS_HEIGHT)
            },
            new Icon(
                new Vector2(0.64f, -0.41f),
                new Vector2(0.06f, 0.06f * Renderer.ASPECT_RATIO),
                2)
            {
                TileOffset = new Vector2(64f / Icon.ICON_ATLAS_WIDTH, 64f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(32f / Icon.ICON_ATLAS_WIDTH, 32f / Icon.ICON_ATLAS_HEIGHT)
            }
        ]);
    }

    private static void AddTexts(List<GameObject> objects)
    {
        objects.AddRange(
            AsciiSymbol.CreateText(
                "Move",
                new Vector2(0.75f, 0.3f),
                new Vector2(0.03f, 0.03f * Renderer.ASPECT_RATIO),
                layer: 2));

        objects.AddRange(
            AsciiSymbol.CreateText(
                "Jump",
                new Vector2(0.75f, 0.055f),
                new Vector2(0.03f, 0.03f * Renderer.ASPECT_RATIO),
                kerning: 1.15f,
                layer: 2));

        objects.AddRange(
            AsciiSymbol.CreateText(
                "Interact/",
                new Vector2(0.72f, -0.13f),
                new Vector2(0.03f, 0.03f * Renderer.ASPECT_RATIO),
                layer: 2));
        objects.AddRange(
            AsciiSymbol.CreateText(
                "Pick up",
                new Vector2(0.72f, -0.19f),
                new Vector2(0.03f, 0.03f * Renderer.ASPECT_RATIO),
                layer: 2));

        objects.AddRange(
            AsciiSymbol.CreateText(
                "Quit",
                new Vector2(0.75f, -0.38f),
                new Vector2(0.03f, 0.03f * Renderer.ASPECT_RATIO),
                layer: 2));
    }
}