using System;
using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.Prefabs.Ui;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels.Ui;

public class MainMenu : ILevelData
{
    public static Vector4 NormalButtonCol { get; } = new(0x20 / 255f, 0x20 / 255f, 0x20 / 255f, 1.0f);
    public static Vector4 NormalButtonHoverCol { get; } = new(0x30 / 255f, 0x30 / 255f, 0x30 / 255f, 1.0f);

    public static Vector4 DangerButtonCol { get; } = new(0x64 / 255f, 0.0f, 0.0f, 1.0f);
    public static Vector4 DangerButtonHoverCol { get; } = new(0x80 / 255f, 0.0f, 0.0f, 1.0f);

    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects = [];
        objects.AddRange([
            //temporary background
            new UiRect(
                new Vector2(-1.0f, -1.0f),
                new Vector2(2f, 2f))
            {
                Color = new Vector4(0x08 / 255f, 0x08 / 255f, 0x08 / 255f, 1.0f)
            },

            new UiButton(
                new Vector2(0.38f, -0.4f),
                new Vector2(0.6f, 0.25f),
                1)
            {
                NormalColor = NormalButtonCol,
                HoverColor = NormalButtonHoverCol,
                OnClick = () =>
                {
                    Player.Instance.RequestLoadingNextLevel(
                        new Transform3D
                        {
                            Position = new Vector3(1.74f, 0f, -4f)
                        });
                }
            },
            new UiButton(
                new Vector2(0.38f, -0.8f),
                new Vector2(0.6f, 0.25f),
                1)
            {
                NormalColor = DangerButtonCol,
                HoverColor = DangerButtonHoverCol,
                OnClick = () => { Environment.Exit(0); }
            }
        ]);

        objects.AddRange(
            AsciiSymbol.CreateText(
                "Play!",
                new Vector2(0.39f, -0.3f),
                new Vector2(0.05f, 0.05f * Renderer.ASPECT_RATIO),
                layer: 2));
        objects.AddRange(
            AsciiSymbol.CreateText(
                "Exit",
                new Vector2(0.39f, -0.7f),
                new Vector2(0.05f, 0.05f * Renderer.ASPECT_RATIO),
                layer: 2));

        return objects.ToArray();
    }
}