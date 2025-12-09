using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Interactables;

public class BtnStand01 : GameObject
{
    private readonly MeshMaterial _material;

    public BtnStand01(Transform3D transform, bool hasAlpha = false, bool hasBackfaceCulling = true)
    {
        UpdateEvent += OnUpdate;
        Texture? diffuseTex = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/btn_stand_01/tex_diffuse01.png",
            new Texture.TextureFormat());
        if (diffuseTex == null)
        {
            throw new NullReferenceException("Button stand texture was not found");
        }

        _material = CommonMaterials.CreateDiffuseLitMaterial(diffuseTex, hasAlpha, hasBackfaceCulling);

        AddComponent(transform);
        AddComponent(
            new StaticCollider(
                new Aabb(new Vector3(-0.5f, -0.675f, -0.5f), new Vector3(1f, 1.35f, 1f))));

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/btn_stand_01/btn_stand_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/btn_stand_01/btn_stand_01.glb.vert01",
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
        _material.SetProperty("material.specularCol", new Vector3(0.2f));
        _material.SetProperty("material.shininess", 16.0f);
    }
}