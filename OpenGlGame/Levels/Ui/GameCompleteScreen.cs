using System;
using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Prefabs.Ui;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels.Ui;

public class GameCompleteScreen : ILevelData
{
    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects = [];

        objects.AddRange([
            //background
            new UiRect(
                new Vector2(-1f, -1f),
                new Vector2(2f, 2f * Renderer.ASPECT_RATIO),
                true,
                1)
            {
                Color = new Vector4(0.01f, 0.05f, 0.01f, 0.6f)
            },

            new UiButton(
                new Vector2(-0.25f, -0.3f),
                new Vector2(0.5f, 0.2f),
                2)
            {
                NormalColor = MainMenu.DangerButtonCol,
                HoverColor = MainMenu.DangerButtonHoverCol,
                OnClick = () => { Environment.Exit(0); }
            }
        ]);

        objects.AddRange(
            AsciiSymbol.CreateText(
                "Congratulations!",
                new Vector2(-0.65f, 0.45f),
                new Vector2(0.1f, 0.1f * Renderer.ASPECT_RATIO),
                kerning: 1.15f,
                layer: 2));
        objects.AddRange(
            AsciiSymbol.CreateText(
                "Game complete!",
                new Vector2(-0.55f, 0.2f),
                new Vector2(0.1f, 0.1f * Renderer.ASPECT_RATIO),
                kerning: 1.15f,
                layer: 2));

        objects.AddRange(
            AsciiSymbol.CreateText(
                "Exit",
                new Vector2(-0.125f, -0.26f),
                new Vector2(0.07f, 0.07f * Renderer.ASPECT_RATIO),
                kerning: 1.3f,
                layer: 3));

        return objects.ToArray();
    }
}