using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;

namespace OpenGlGame.SceneSystem;

public static class CommonMaterials
{
    public static MeshMaterial CreateVertColorsUnlitMaterial()
    {
        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/vert_colors_unlit")
            ?? throw new Exception("VertColorsMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.VertexColorsMesh);
    }

    public static MeshMaterial CreateDiffuseUnlitMaterial(Texture? texture = null)
    {
        if (texture == null)
        {
            texture = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (texture == null)
            {
                throw new NullReferenceException("DiffuseUnlitMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/diffuse_unlit")
            ?? throw new Exception("DiffuseUnlitMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.DiffuseTextureMesh,
            [texture]);
    }

    public static MeshMaterial CreateDiffuseTiledUnlitMaterial(Texture? texture = null)
    {
        if (texture == null)
        {
            texture = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (texture == null)
            {
                throw new NullReferenceException("DiffuseTiledUnlitMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/diffuse_tiled_unlit")
            ?? throw new Exception("DiffuseTiledUnlitMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.DiffuseTextureMesh,
            [texture]);
    }
}