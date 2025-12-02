using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;

namespace OpenGlGame.SceneSystem;

public static class CommonMaterials
{
    public static MeshMaterial CreateVertColorsMaterial()
    {
        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/vert_colors_lit")
            ?? throw new Exception("VertColorsMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.VertexColorsMesh);
    }

    public static MeshMaterial CreateAlbedoMaterial(Texture? texture = null)
    {
        if (texture == null)
        {
            texture = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (texture == null)
            {
                throw new NullReferenceException("AlbedoMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/albedo_lit")
            ?? throw new Exception("AlbedoMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.AlbedoTextureMesh,
            [texture]);
    }

    public static MeshMaterial CreateAlbedoTiledMaterial(Texture? texture = null)
    {
        if (texture == null)
        {
            texture = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (texture == null)
            {
                throw new NullReferenceException("AlbedoTiledMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/albedo_tiled_lit")
            ?? throw new Exception("AlbedoTiledMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.AlbedoTextureMesh,
            [texture]);
    }
}