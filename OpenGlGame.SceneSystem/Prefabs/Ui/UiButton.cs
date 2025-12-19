using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenGlGame.SceneSystem.Prefabs.Ui;

public class UiButton : GameObject
{
    private readonly MeshMaterial _material;
    private readonly Transform3D _transform;

    public Vector4 NormalColor { get; set; } = Vector4.One;
    public Vector4? HoverColor { get; set; }
    public Action? OnClick { get; set; }

    private bool _isPointerDown;

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

    public UiButton(Vector2 position, Vector2 scale, uint layer = 0)
    {
        UpdateEvent += OnUpdate;

        VertexBuffers buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/runtime_gen/ui_button.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffers(
                "/assets/models/runtime_gen/ui_button.vert01",
                PrimitiveType.Triangles,
                CommonVertexAttribDescriptors.UiMesh,
                _vertexData,
                4,
                _indices,
                6);

        _material = CommonMaterials.CreateUiPolygonMaterial();

        _transform = new Transform3D
        {
            Position = new Vector3(position.X, position.Y, 0f - 0.0001f * layer),
            Scale = new Vector3(scale.X, scale.Y, 1)
        };
        AddComponent(_transform);

        AddComponent(new MeshRenderer
        {
            VertBuffers = buffers,
            Material = _material,
            IsUi = true
        });
    }

    private void OnUpdate(double deltaTime)
    {
        Vector4 bounds = new(
            _transform.Position.X,
            _transform.Position.Y,
            _transform.Position.X + (_transform.Scale?.X ?? 0f),
            _transform.Position.Y + (_transform.Scale?.Y ?? 0f));

        bool isInside = IsPointInsideRect(
            SceneManager.Instance.NormalizedMousePosition + new Vector2(1f),
            bounds + new Vector4(1f));

        if (isInside)
        {
            _material.SetProperty("color", HoverColor ?? NormalColor);
        }
        else
        {
            _material.SetProperty("color", NormalColor);
        }

        if (isInside && _isPointerDown && SceneManager.Instance.CurrentlyPressedMouseButton == null)
        {
            OnClick?.Invoke();
        }

        _isPointerDown = SceneManager.Instance.CurrentlyPressedMouseButton == MouseButton.Left;
    }

    private static bool IsPointInsideRect(Vector2 point, Vector4 rect)
    {
        if (point.X >= rect.Z || point.X <= rect.X)
        {
            return false;
        }

        if (point.Y >= rect.W || point.Y <= rect.Y)
        {
            return false;
        }

        return true;
    }
}