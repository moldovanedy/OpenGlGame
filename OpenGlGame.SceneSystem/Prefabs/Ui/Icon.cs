using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Ui;

public class Icon : GameObject
{
    public const float ICON_ATLAS_WIDTH = 256f;
    public const float ICON_ATLAS_HEIGHT = 256f;

    private readonly MeshMaterial _material;
    private readonly Transform3D _transform;

    public Vector4 Color { get; set; } = Vector4.One;
    public Vector2 TileOffset { get; set; } = Vector2.Zero;
    public Vector2 TileSize { get; set; } = Vector2.One;

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

    public Icon(Vector2 position, Vector2 scale, uint layer = 0)
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

        Texture? iconTexAtlas = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/ui/icons.png",
            new Texture.TextureFormat());
        if (iconTexAtlas == null)
        {
            throw new NullReferenceException("UI icons texture was not found");
        }

        _material = CommonMaterials.CreateUiTexAtlasMaterial(iconTexAtlas, true);

        _transform = new Transform3D
        {
            Position = new Vector3(position.X, position.Y, 0f - 0.0001f * layer),
            Scale = new Vector3(scale.X, scale.Y, 1)
        };
        AddComponent(_transform);

        AddComponent(new MeshRenderer
        {
            VertBuffers = buffers,
            Material = _material
        });
    }

    private void OnUpdate(double deltaTime)
    {
        _material.SetProperty("color", Color);
        _material.SetProperty("atlasOffsetAndSize", new Vector4(TileOffset.X, TileOffset.Y, TileSize.X, TileSize.Y));
    }
}