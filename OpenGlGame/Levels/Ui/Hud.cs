using System;
using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Prefabs.Ui;
using OpenTK.Mathematics;

namespace OpenGlGame.Levels.Ui;

public class Hud : ILevelData
{
    private static readonly Vector4 MainColor = new(180f / 255f, 134f / 255f, 30f / 255f, 1f);
    private static readonly Vector4 LowHealthColor = new(205f / 255f, 19f / 255f, 19f / 255f, 1f);

    private AsciiSymbol[]? _healthGlyphs;
    private int _lastNormalizedHealth = 100;

    public GameObject[] LoadAndGetGameObjects()
    {
        int normalizedHealth = (int)Math.Ceiling(Player.Instance.Health);
        List<GameObject> objects = [];

        objects.AddRange([
            new Icon(new Vector2(-0.02f, -0.02f), new Vector2(0.04f, 0.04f * Renderer.ASPECT_RATIO))
            {
                TileOffset = new Vector2(32f / Icon.ICON_ATLAS_WIDTH, 0f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(32f / Icon.ICON_ATLAS_WIDTH, 32f / Icon.ICON_ATLAS_HEIGHT),
                Color = MainColor
            },
            new Icon(new Vector2(-0.96f, -0.96f), new Vector2(0.05f, 0.05f * Renderer.ASPECT_RATIO))
            {
                TileOffset = new Vector2(0f / Icon.ICON_ATLAS_WIDTH, 0f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(32f / Icon.ICON_ATLAS_WIDTH, 32f / Icon.ICON_ATLAS_HEIGHT),
                Color = MainColor
            }
        ]);

        _healthGlyphs = AsciiSymbol.CreateText(
            normalizedHealth.ToString(),
            new Vector2(-0.91f, -0.96f),
            new Vector2(0.05f, 0.05f * Renderer.ASPECT_RATIO),
            normalizedHealth > 20 ? MainColor : LowHealthColor);
        objects.AddRange(_healthGlyphs);

        objects[0].UpdateEvent += OnUpdate;
        return objects.ToArray();
    }

    private void OnUpdate(double deltaTime)
    {
        int normalizedHealth = (int)Math.Ceiling(Player.Instance.Health);

        if (_lastNormalizedHealth == normalizedHealth || _healthGlyphs == null)
        {
            return;
        }

        _lastNormalizedHealth = normalizedHealth;

        MainThreadDispatcher.Enqueue(() =>
        {
            string healthString = _lastNormalizedHealth.ToString();
            if (healthString.Length < 3)
            {
                for (int i = healthString.Length; i < 3; i++)
                {
                    healthString += ' ';
                }
            }

            for (int i = 0; i < _healthGlyphs.Length; i++)
            {
                _healthGlyphs[i].SetGlyph((byte)healthString[i]);
                _healthGlyphs[i].Color = _lastNormalizedHealth > 20 ? MainColor : LowHealthColor;
            }
        });
    }
}