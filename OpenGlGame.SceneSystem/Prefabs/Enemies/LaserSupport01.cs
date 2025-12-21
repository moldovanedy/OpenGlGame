using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Enemies;

public class LaserSupport01 : GameObject
{
    private readonly MeshMaterial _material;

    public LaserSupport01(Transform3D transform)
    {
        UpdateEvent += OnUpdate;

        AddComponent(transform);
        //no static collider, as it can be used to skip the lasers

        Texture? diffuseTex = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/laser_support_01/tex_diffuse01.png",
            new Texture.TextureFormat());
        if (diffuseTex == null)
        {
            throw new NullReferenceException("Laser support texture was not found");
        }

        _material = CommonMaterials.CreateDiffuseLitMaterial(diffuseTex);
        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/laser_support_01/laser_support_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/laser_support_01/laser_support_01.glb.vert01",
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
        _material.SetProperty("material.specularCol", new Vector3(0.2f));
        _material.SetProperty("material.shininess", 16.0f);
    }
}