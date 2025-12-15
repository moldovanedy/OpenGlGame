using System;
using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Prefabs.Ui;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels.Ui;

public class MainMenu : ILevelData
{
    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects = [];
        objects.AddRange([
            //temporary background
            new UiButton(
                new Vector2(-1.0f, -1.0f),
                new Vector2(2f, 2f))
            {
                NormalColor = new Vector4(0x10 / 255f, 0x10 / 255f, 0x10 / 255f, 1.0f)
            },

            new UiButton(
                new Vector2(0.38f, -0.4f),
                new Vector2(0.6f, 0.25f),
                1)
            {
                NormalColor = new Vector4(0x30 / 255f, 0x30 / 255f, 0x30 / 255f, 1.0f),
                HoverColor = new Vector4(0x40 / 255f, 0x40 / 255f, 0x40 / 255f, 1.0f),
                OnClick = () => { Player.Instance.RequestLoadingNextLevel(new Vector3(1.74f, 0f, -4f)); }
            },
            // new AsciiSymbol(
            //     new Vector2(0.39f, -0.3f),
            //     new Vector2(0.05f, 0.05f * Renderer.ASPECT_RATIO),
            //     (byte)'P',
            //     2
            // ),
            new UiButton(
                new Vector2(0.38f, -0.8f),
                new Vector2(0.6f, 0.25f),
                1)
            {
                NormalColor = new Vector4(0x64 / 255f, 0.0f, 0.0f, 1.0f),
                HoverColor = new Vector4(0x80 / 255f, 0.0f, 0.0f, 1.0f),
                OnClick = () => { Environment.Exit(0); }
            }
        ]);

        objects.AddRange(
            AsciiSymbol.CreateText(
                "Play!",
                new Vector2(0.39f, -0.3f),
                new Vector2(0.05f, 0.05f * Renderer.ASPECT_RATIO),
                null,
                2));
        objects.AddRange(
            AsciiSymbol.CreateText(
                "Exit",
                new Vector2(0.39f, -0.7f),
                new Vector2(0.05f, 0.05f * Renderer.ASPECT_RATIO),
                null,
                2));

        return objects.ToArray();
    }
}