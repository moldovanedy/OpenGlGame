using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Ui;

public class UiLogo : GameObject
{
    private readonly MeshMaterial _material;

    public Vector4 Color { get; set; } = Vector4.One;

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

    public UiLogo(Vector2 position, Vector2 scale, uint layer = 0)
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
            "/assets/ui/logo.png",
            new Texture.TextureFormat(),
            false);
        if (iconTexAtlas == null)
        {
            throw new NullReferenceException("UI logo texture was not found");
        }

        _material = CommonMaterials.CreateUiTexAtlasMaterial(iconTexAtlas, true, false);

        var transform = new Transform3D
        {
            Position = new Vector3(position.X, position.Y, 0f - 0.0001f * layer),
            Scale = new Vector3(scale.X, scale.Y, 1)
        };
        AddComponent(transform);

        AddComponent(new MeshRenderer
        {
            VertBuffers = buffers,
            Material = _material,
            IsUi = true
        });
    }

    private void OnUpdate(double deltaTime)
    {
        _material.SetProperty("color", Color);
        _material.SetProperty("atlasOffsetAndSize", new Vector4(0f, 0f, 1f, 1f));
    }
}