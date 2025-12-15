using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Ui;

public class AsciiSymbol : GameObject
{
    public const float FONT_ATLAS_WIDTH = 1024f;
    public const float FONT_ATLAS_HEIGHT = 384f;
    public const float FONT_GLYPH_SIZE = 64f;

    private readonly MeshMaterial _material;

    public Vector4 Color { get; set; } = Vector4.One;

    private readonly Vector2 _tileOffset;
    private readonly Vector2 _tileSize;

    private readonly float[] _vertexData =
    [
        //pos + tex coords
        0f, 1f, 0f, 1f, //top left
        0f, 0f, 0f, 0f, //bottom left
        1f, 0f, 1f, 0f, //bottom right
        1f, 1f, 1f, 1f //top right
    ];

    private readonly uint[] _indices =
    [
        0, 1, 2,
        0, 2, 3
    ];

    public AsciiSymbol(Vector2 position, Vector2 scale, byte asciiCode, uint layer = 0)
    {
        UpdateEvent += OnUpdate;

        VertexBuffers buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/runtime_gen/icon.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffers(
                "/assets/models/runtime_gen/icon.vert01",
                PrimitiveType.Triangles,
                CommonVertexAttribDescriptors.UiTexture,
                _vertexData,
                4,
                _indices,
                6);

        Texture? fontTexAtlas = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/ui/font.png",
            new Texture.TextureFormat());
        if (fontTexAtlas == null)
        {
            throw new NullReferenceException("UI icons texture was not found");
        }

        _material = CommonMaterials.CreateUiTexAtlasMaterial(fontTexAtlas, true);

        var transform = new Transform3D
        {
            Position = new Vector3(position.X, position.Y, 0f - 0.0001f * layer),
            Scale = new Vector3(scale.X, scale.Y, 1)
        };
        AddComponent(transform);

        AddComponent(new MeshRenderer
        {
            VertBuffers = buffers,
            Material = _material
        });

        asciiCode = (byte)(asciiCode % 128 - 32);
        int row = asciiCode % 16;
        int column = asciiCode / 16;

        _tileOffset = new Vector2(
            row * FONT_GLYPH_SIZE / FONT_ATLAS_WIDTH,
            column * FONT_GLYPH_SIZE / FONT_ATLAS_HEIGHT);
        _tileSize = new Vector2(
            FONT_GLYPH_SIZE / FONT_ATLAS_WIDTH,
            FONT_GLYPH_SIZE / FONT_ATLAS_HEIGHT);
    }

    private void OnUpdate(double deltaTime)
    {
        _material.SetProperty("color", Color);
        _material.SetProperty("atlasOffsetAndSize",
            new Vector4(_tileOffset.X, _tileOffset.Y, _tileSize.X, _tileSize.Y));
    }


    public static AsciiSymbol[] CreateText(
        string text,
        Vector2 position,
        Vector2 glyphSize,
        Vector4? color = null,
        uint layer = 0)
    {
        AsciiSymbol[] symbols = new AsciiSymbol[text.Length];
        for (int i = 0; i < text.Length; i++)
        {
            byte asciiCode;
            if (text[i] < 32 || text[i] > 127)
            {
                asciiCode = (byte)'?';
            }
            else
            {
                asciiCode = (byte)text[i];
            }

            symbols[i] = new AsciiSymbol(position, glyphSize, asciiCode, layer)
            {
                Color = color ?? Vector4.One
            };

            //apply some kerning
            position += new Vector2(glyphSize.X / 1.3f, 0f);
        }

        return symbols;
    }
}