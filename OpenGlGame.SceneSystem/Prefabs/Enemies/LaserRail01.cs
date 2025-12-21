using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Enemies;

public class LaserRail01 : GameObject
{
    private readonly MeshMaterial _material;

    public LaserRail01(Transform3D transform)
    {
        UpdateEvent += OnUpdate;

        AddComponent(transform);
        //no static collider, as it can be used to skip the lasers

        _material = CommonMaterials.CreateColoredLitMaterial(0x10, 0x10, 0x10, 0xff);
        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/laser_rail_01/laser_rail_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/laser_rail_01/laser_rail_01.glb.vert01",
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
        _material.SetProperty("material.diffuseTex", 0);
        _material.SetProperty("material.specularCol", new Vector3(0.5f));
        _material.SetProperty("material.shininess", 64.0f);
    }
}