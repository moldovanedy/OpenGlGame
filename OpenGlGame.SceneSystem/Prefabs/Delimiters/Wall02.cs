using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Delimiters;

public class Wall02 : GameObject
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private MeshMaterial _material;

    public Wall02(Transform3D transform, bool hasAlpha = false, bool hasBackfaceCulling = true)
    {
        Texture? diffuseTex = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/wall_02/tex_diffuse01.png",
            new Texture.TextureFormat());
        if (diffuseTex == null)
        {
            throw new NullReferenceException("Wall texture was not found");
        }

        _material = CommonMaterials.CreateDiffuseTiledLitMaterial(diffuseTex, hasAlpha, hasBackfaceCulling);
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