using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs;

public class Floor01 : GameObject
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private MeshMaterial _material;

    public Floor01(Transform3D transform)
    {
        Texture? albedo = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/floor_01/tex_albedo01.png",
            new Texture.TextureFormat());
        if (albedo == null)
        {
            throw new NullReferenceException("Wall texture was not found");
        }

        _material = CommonMaterials.CreateAlbedoTiledMaterial(albedo);
        _material.SetProperty("tiling", new Vector2(transform.Scale?.X ?? 1f, transform.Scale?.Z ?? 1f));
        _material.SetProperty("offset", new Vector2(0f, 0f));

        AddComponent(transform);

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/floor_01/floor_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/floor_01/floor_01.glb.vert01",
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