using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Delimiters;

public class Wall01 : GameObject
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private MeshMaterial _material;

    public Wall01(Transform3D transform)
    {
        Texture? diffuseTex = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/wall_01/tex_diffuse01.png",
            new Texture.TextureFormat());
        if (diffuseTex == null)
        {
            throw new NullReferenceException("Wall texture was not found");
        }

        _material = CommonMaterials.CreateDiffuseTiledUnlitMaterial(diffuseTex);
        _material.SetProperty("tiling", new Vector2(transform.Scale?.X ?? 1f, transform.Scale?.Z ?? 1f));
        _material.SetProperty("offset", new Vector2(0f, 0f));

        AddComponent(transform);
        AddComponent(
            new StaticCollider(
                new Aabb(new Vector3(-0.5f, -0.2f, -0.5f), new Vector3(1f, 0.2f, 1f))));

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/wall_01/wall_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/wall_01/wall_01.glb.vert01",
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
}