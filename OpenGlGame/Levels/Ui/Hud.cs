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

    private UiRect[]? _damageIndicators;
    private float _damageIndicatorTimer;
    private const float DAMAGE_TIMER_MAX = 0.5f;

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

        _damageIndicators =
        [
            new UiRect(
                new Vector2(-0.12f, 0.1f * Renderer.ASPECT_RATIO),
                new Vector2(0.24f, 0.02f * Renderer.ASPECT_RATIO),
                true,
                2)
            {
                Color = Vector4.Zero
            },
            new UiRect(
                new Vector2(0.1f, -0.1f * Renderer.ASPECT_RATIO),
                new Vector2(0.02f, 0.2f * Renderer.ASPECT_RATIO),
                true,
                2)
            {
                Color = Vector4.Zero
            },
            new UiRect(
                new Vector2(-0.1f, -0.1f * Renderer.ASPECT_RATIO),
                new Vector2(0.22f, 0.02f * Renderer.ASPECT_RATIO),
                true,
                2)
            {
                Color = Vector4.Zero
            },
            new UiRect(
                new Vector2(-0.12f, -0.1f * Renderer.ASPECT_RATIO),
                new Vector2(0.02f, 0.2f * Renderer.ASPECT_RATIO),
                true,
                2)
            {
                Color = Vector4.Zero
            }
        ];

        objects.AddRange(_healthGlyphs);
        objects.AddRange(_damageIndicators);

        objects[0].UpdateEvent += OnUpdate;
        return objects.ToArray();
    }

    private void OnUpdate(double deltaTime)
    {
        if (_damageIndicatorTimer > 0.0f)
        {
            _damageIndicatorTimer -= (float)deltaTime;
        }
        else
        {
            _damageIndicatorTimer = 0f;
        }

        MainThreadDispatcher.Enqueue(() =>
        {
            //damage indicator
            if (_damageIndicators != null)
            {
                foreach (UiRect indicator in _damageIndicators)
                {
                    float alpha = _damageIndicatorTimer * (1f / DAMAGE_TIMER_MAX);
                    indicator.Color = new Vector4(LowHealthColor.X, LowHealthColor.Y, LowHealthColor.Z, alpha * 0.5f);
                }
            }
        });

        int normalizedHealth = (int)Math.Ceiling(Player.Instance.Health);
        if (_lastNormalizedHealth == normalizedHealth)
        {
            return;
        }

        _lastNormalizedHealth = normalizedHealth;
        _damageIndicatorTimer = DAMAGE_TIMER_MAX;

        MainThreadDispatcher.Enqueue(() =>
        {
            //health indicator
            if (_healthGlyphs != null)
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
            }
        });
    }
}