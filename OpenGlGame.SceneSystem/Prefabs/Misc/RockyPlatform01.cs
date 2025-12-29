using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Misc;

public class RockyPlatform01 : GameObject
{
    private readonly Aabb _boundingBox = new(new Vector3(-0.505f, -0.505f, -0.505f), new Vector3(1.01f, 1.01f, 1.01f));

    private readonly MeshMaterial _material;

    public RockyPlatform01(Transform3D transform)
    {
        UpdateEvent += OnUpdate;
        Texture? diffuseTex = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/rocky_01/tex_diffuse01.png",
            new Texture.TextureFormat());
        if (diffuseTex == null)
        {
            throw new NullReferenceException("RockyPlatform01 texture was not found");
        }

        AddComponent(transform);
        _material = CommonMaterials.CreateDiffuseLitMaterial(diffuseTex);

        AddComponent(new StaticCollider(_boundingBox));

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/rocky_01/rocky_platform_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/rocky_01/rocky_platform_01.glb.vert01",
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
        _material.SetProperty("material.diffuseTex", 0);
        _material.SetProperty("material.normalTex", 1);
        _material.SetProperty("material.specularCol", new Vector3(0.1f));
        _material.SetProperty("material.shininess", 8.0f);
    }
}