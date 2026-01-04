using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Interactables;

public class Btn02 : GameObject
{
    private readonly MeshMaterial _material;

    public Btn02(Transform3D transform)
    {
        UpdateEvent += OnUpdate;

        Texture? diffuseTex = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/btn_02/tex_diffuse01.png",
            new Texture.TextureFormat());
        if (diffuseTex == null)
        {
            throw new NullReferenceException("Btn02 texture was not found");
        }

        _material = CommonMaterials.CreateDiffuseLitMaterial(diffuseTex);
        AddComponent(transform);

        AddComponent(
            new StaticCollider(
                new Aabb(new Vector3(-0.75f, -0.4f, -0.75f), new Vector3(1.5f, 0.2f, 1.5f))));
        AddComponent(
            new CubeLandingArea(
                new Aabb(new Vector3(-0.5f, -0.2f, -0.5f), new Vector3(1f, 0.1f, 1f))));

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/btn_02/btn_02.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/btn_02/btn_02.glb.vert01",
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
        _material.SetProperty("material.specularCol", new Vector3(0.15f));
        _material.SetProperty("material.shininess", 8.0f);
    }
}