using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Enemies;

public class Laser01Inner : GameObject
{
    private readonly Transform3D _transform;
    private readonly MeshMaterial _material;

    private const double ANIM_TIME = 0.1f;
    private double _timer;
    private bool _isShrinking = true;

    public Laser01Inner(Transform3D transform)
    {
        UpdateEvent += OnUpdate;

        _transform = transform;
        AddComponent(_transform);

        _material = CommonMaterials.CreateColoredUnlitMaterial(0xff, 0x4f, 0x42, 0xff);
        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/laser_01/laser_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/laser_01/laser_01.glb.vert01",
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

    private void OnUpdate(double deltaTime)
    {
        if (_isShrinking)
        {
            _transform.Scale -= new Vector3(1.5f * (float)deltaTime, 0f, 1.5f * (float)deltaTime);
        }
        else
        {
            _transform.Scale += new Vector3(1.5f * (float)deltaTime, 0f, 1.5f * (float)deltaTime);
        }

        _timer += deltaTime;
        if (_timer >= ANIM_TIME)
        {
            _timer = 0;
            _isShrinking = !_isShrinking;
        }

        _material.SetProperty("material.diffuseTex", 0);
        _material.SetProperty("material.specularCol", new Vector3(0.05f));
        _material.SetProperty("material.shininess", 4.0f);
    }
}