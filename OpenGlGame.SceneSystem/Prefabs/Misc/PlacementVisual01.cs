using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Misc;

public class PlacementVisual01 : GameObject
{
    private readonly Transform3D _transform;
    private readonly MeshMaterial _material;

    private double _timer;

    public PlacementVisual01(Transform3D transform)
    {
        UpdateEvent += OnUpdate;

        _transform = transform;
        AddComponent(transform);

        _material = CommonMaterials.CreateColoredUnlitMaterial(
            0x00, 0xff, 0x70, 0x25,
            true,
            false);

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/placement_visual_01/placement_visual_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/placement_visual_01/placement_visual_01.glb.vert01",
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
        //actual material properties
        _material.SetProperty("material.specularCol", new Vector3(0.1f));
        _material.SetProperty("material.shininess", 8.0f);

        _timer += deltaTime;
        double progress = _timer % 3;

        switch (progress)
        {
            case < 1:
                _transform.Rotation = Quaternion.Slerp(
                    new Quaternion(0f, float.DegreesToRadians(0f), 0f),
                    new Quaternion(0f, float.DegreesToRadians(120f), 0f),
                    (float)progress);
                break;
            case < 2:
                _transform.Rotation = Quaternion.Slerp(
                    new Quaternion(0f, float.DegreesToRadians(120f), 0f),
                    new Quaternion(0f, float.DegreesToRadians(240f), 0f),
                    (float)(progress - 1));
                break;
            default:
                _transform.Rotation = Quaternion.Slerp(
                    new Quaternion(0f, float.DegreesToRadians(240f), 0f),
                    new Quaternion(0f, float.DegreesToRadians(360f), 0f),
                    (float)(progress - 2));
                break;
        }

        float progressInSecond = (float)(_timer % 1);
        if (progressInSecond < 0.5f)
        {
            _transform.Scale = Vector3.Lerp(
                new Vector3(1f, 1f, 1f),
                new Vector3(1.25f, 1.25f, 1.25f),
                progressInSecond * 2);
        }
        else
        {
            _transform.Scale = Vector3.Lerp(
                new Vector3(1.25f, 1.25f, 1.25f),
                new Vector3(1f, 1f, 1f),
                (progressInSecond - 0.5f) * 2);
        }
    }
}