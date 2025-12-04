using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs;

public class Wall02 : GameObject
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private MeshMaterial _material;

    public Wall02(Transform3D transform)
    {
        Texture? albedo = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/wall_02/tex_albedo01.png",
            new Texture.TextureFormat());
        if (albedo == null)
        {
            throw new NullReferenceException("Wall texture was not found");
        }

        _material = CommonMaterials.CreateAlbedoTiledMaterial(albedo);
        _material.SetProperty("tiling", new Vector2(transform.Scale?.X ?? 1f, transform.Scale?.Z ?? 1f));
        _material.SetProperty("offset", new Vector2(0f, 0f));

        AddComponent(transform);
        AddComponent(
            new StaticCollider(
                new Aabb(new Vector3(-0.5f, -0.01f, -0.5f), new Vector3(1f, 0.02f, 1f))));

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/wall_02/wall_02.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/wall_02/wall_02.glb.vert01",
                CommonVertexAttribDescriptors.AlbedoTextureMesh);
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