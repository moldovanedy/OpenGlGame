using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Interactables;

public class Btn02 : GameObject
{
    private readonly Transform3D _transform;
    private MeshMaterial _material;

    public Btn02(Transform3D transform, bool hasAlpha = false, bool hasBackfaceCulling = true)
    {
        UpdateEvent += OnUpdate;

        _transform = transform;
        _material = CommonMaterials.CreateColoredLitMaterial(
            0xc0, 0xc0, 0xc0, 0xff,
            hasAlpha,
            hasBackfaceCulling);
        AddComponent(transform);

        AddComponent(
            new StaticCollider(
                new Aabb(new Vector3(-0.75f, -0.1f, -0.75f), new Vector3(1.5f, 0.2f, 1.5f))));

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