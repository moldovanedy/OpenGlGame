using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Ui;

public class UiRect : GameObject
{
    private readonly MeshMaterial _material;

    public Vector4 Color { get; set; } = Vector4.One;

    private readonly float[] _vertexData =
    [
        0f, 1f, //top left
        0f, 0f, //bottom left
        1f, 0f, //bottom right
        1f, 1f //top right
    ];

    private readonly uint[] _indices =
    [
        0, 1, 2,
        0, 2, 3
    ];

    public UiRect(Vector2 position, Vector2 scale, bool hasAlphaBlending = false, uint layer = 0)
    {
        UpdateEvent += OnUpdate;

        VertexBuffers buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/runtime_gen/ui_rect.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffers(
                "/assets/models/runtime_gen/ui_rect.vert01",
                PrimitiveType.Triangles,
                CommonVertexAttribDescriptors.UiMesh,
                _vertexData,
                4,
                _indices,
                6);

        //if it has alpha, we also need to disable backface culling
        _material = CommonMaterials.CreateUiPolygonMaterial(hasAlphaBlending, !hasAlphaBlending);
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
    }
}