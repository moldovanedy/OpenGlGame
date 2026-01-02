using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Misc;

public class FlagPole : GameObject
{
    private readonly Aabb _boundingBox = new(new Vector3(-0.05f, 0f, -0.05f), new Vector3(0.1f, 4f, 0.1f));

    private readonly MeshMaterial _material;

    public FlagPole(Transform3D transform)
    {
        UpdateEvent += OnUpdate;
        AddComponent(transform);
        _material = CommonMaterials.CreateColoredLitMaterial(0xff, 0xff, 0xff, 0xff);

        AddComponent(new StaticCollider(_boundingBox));

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/finish_flag/flag_pole.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/finish_flag/flag_pole.glb.vert01",
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
        _material.SetProperty("material.specularCol", new Vector3(0.5f));
        _material.SetProperty("material.shininess", 128.0f);
    }
}