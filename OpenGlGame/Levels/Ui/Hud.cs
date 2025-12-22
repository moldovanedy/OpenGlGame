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
    private static readonly Vector4 CollimatorActiveColor = new(79f / 255f, 225f / 255f, 64f / 255f, 1f);

    private AsciiSymbol[]? _healthGlyphs;
    private int _lastNormalizedHealth = 100;

    private Icon? _collimatorSight;

    private UiRect[]? _damageIndicators;
    private float _damageIndicatorTimer;
    private const float DAMAGE_TIMER_MAX = 0.5f;

    private GameObject[]? _interactionMessageObjects;
    private float _fadeTimer;
    private const float FADE_TIMER_MAX = 0.15f;

    public GameObject[] LoadAndGetGameObjects()
    {
        int normalizedHealth = (int)Math.Ceiling(Player.Instance.Health);
        List<GameObject> objects = [];

        _collimatorSight =
            new Icon(new Vector2(-0.02f, -0.02f), new Vector2(0.04f, 0.04f * Renderer.ASPECT_RATIO))
            {
                TileOffset = new Vector2(32f / Icon.ICON_ATLAS_WIDTH, 0f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(32f / Icon.ICON_ATLAS_WIDTH, 32f / Icon.ICON_ATLAS_HEIGHT),
                Color = MainColor
            };

        objects.AddRange([
            _collimatorSight,
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

        List<GameObject> interactionMessageObjects =
        [
            //background
            new UiRect(
                new Vector2(-0.95f, -0.1f),
                new Vector2(0.3f, 0.2f),
                true,
                1)
            {
                Color = new Vector4(0.03f, 0.03f, 0.03f, 0.8f)
            },
            new Icon(
                new Vector2(-0.96f, -0.06f),
                new Vector2(0.07f, 0.07f * Renderer.ASPECT_RATIO),
                2)
            {
                TileOffset = new Vector2(0f / Icon.ICON_ATLAS_WIDTH, 64f / Icon.ICON_ATLAS_HEIGHT),
                TileSize = new Vector2(64f / Icon.ICON_ATLAS_WIDTH, 64f / Icon.ICON_ATLAS_HEIGHT)
            }
        ];

        interactionMessageObjects.AddRange(
            AsciiSymbol.CreateText(
                "Press to",
                new Vector2(-0.88f, 0.03f),
                new Vector2(0.03f, 0.03f * Renderer.ASPECT_RATIO),
                layer: 2));
        interactionMessageObjects.AddRange(
            AsciiSymbol.CreateText(
                "interact",
                new Vector2(-0.88f, -0.05f),
                new Vector2(0.03f, 0.03f * Renderer.ASPECT_RATIO),
                layer: 2));

        _interactionMessageObjects = interactionMessageObjects.ToArray();
        objects.AddRange(_interactionMessageObjects);

        objects[0].UpdateEvent += OnUpdate;
        return objects.ToArray();
    }

    private void OnUpdate(double deltaTime)
    {
        UpdateDamageAndHealth((float)deltaTime);
        UpdateIndicators((float)deltaTime);
    }

    private void UpdateDamageAndHealth(float deltaTime)
    {
        if (_damageIndicatorTimer > 0.0f)
        {
            _damageIndicatorTimer -= deltaTime;
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

    private void UpdateIndicators(float deltaTime)
    {
        if (Player.Instance.IsNearInteractable)
        {
            if (_fadeTimer < FADE_TIMER_MAX)
            {
                _fadeTimer += deltaTime;
            }
            else
            {
                _fadeTimer = FADE_TIMER_MAX;
            }
        }
        else
        {
            if (_fadeTimer > 0.0f)
            {
                _fadeTimer -= deltaTime;
            }
            else
            {
                _fadeTimer = 0f;
            }
        }

        MainThreadDispatcher.Enqueue(() =>
        {
            //collimator
            if (_collimatorSight != null)
            {
                _collimatorSight.Color = Player.Instance.IsNearInteractable ? CollimatorActiveColor : MainColor;
            }

            //interaction indicator
            if (_interactionMessageObjects != null)
            {
                for (int i = 0; i < _interactionMessageObjects.Length; i++)
                {
                    float alpha = _fadeTimer * (1f / FADE_TIMER_MAX);
                    //the background is the only one that's semi-transparent
                    if (i == 0)
                    {
                        alpha *= 0.8f;
                    }

                    switch (_interactionMessageObjects[i])
                    {
                        case UiRect uiRect:
                            uiRect.Color = new Vector4(uiRect.Color.X, uiRect.Color.Y, uiRect.Color.Z, alpha);
                            break;
                        case AsciiSymbol glyph:
                            glyph.Color = new Vector4(glyph.Color.X, glyph.Color.Y, glyph.Color.Z, alpha);
                            break;
                        case Icon icon:
                            icon.Color = new Vector4(icon.Color.X, icon.Color.Y, icon.Color.Z, alpha);
                            break;
                    }
                }
            }
        });
    }
}