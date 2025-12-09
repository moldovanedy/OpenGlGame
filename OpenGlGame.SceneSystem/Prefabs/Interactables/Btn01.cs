using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Interactables;

public class Btn01 : GameObject
{
    private readonly Transform3D _transform;
    private MeshMaterial _material;

    private bool _isPressing;
    private double _pressingTime;

    public Btn01(Transform3D transform, bool hasAlpha = false, bool hasBackfaceCulling = true)
    {
        UpdateEvent += OnUpdate;

        _transform = transform;
        _material = CommonMaterials.CreateColoredLitMaterial(
            0x80, 0x00, 0x00, 0xff,
            hasAlpha,
            hasBackfaceCulling);
        AddComponent(transform);

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/btn_01/btn_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/btn_01/btn_01.glb.vert01",
                CommonVertexAttribDescriptors.DiffuseTextureMesh);
        if (buffers == null)
        {
            throw new NullReferenceException("Vertex buffers were not found");
        }

        AddComponent(
            new MeshRenderer
            {
                VertBuffers = buffers,
                Material = _material
            });
    }

    public void Press()
    {
        //this is a one-shot button: once pressed, it stays that way
        if (_isPressing || _pressingTime > 0.5f)
        {
            return;
        }

        _isPressing = true;
    }

    private void OnUpdate(double deltaTime)
    {
        if (_isPressing)
        {
            _pressingTime += deltaTime;

            Vector4 pressDirection =
                new Vector4(Player.Instance.Camera.WorldUpVector, 1.0f) * _transform.GetModelMatrix();
            _transform.Position +=
                Vector3.Cross(pressDirection.Xyz.Normalized(), new Vector3(0f, 0f, 0.05f))
                * (float)deltaTime;

            if (_pressingTime > 0.5f)
            {
                ShowAsPressed();
                _isPressing = false;
            }
        }

        //actual material properties
        _material.SetProperty("material.diffuseTex", 0);
        _material.SetProperty("material.specularCol", new Vector3(0.15f));
        _material.SetProperty("material.shininess", 8.0f);
    }

    private void ShowAsPressed()
    {
        _material = CommonMaterials.CreateColoredLitMaterial(
            0xff, 0x00, 0x00, 0xff,
            _material.HasAlphaBlending,
            _material.HasBackfaceCulling);

        if (GetComponent(nameof(MeshRenderer)) is MeshRenderer renderer)
        {
            renderer.Material = _material;
        }
    }
}