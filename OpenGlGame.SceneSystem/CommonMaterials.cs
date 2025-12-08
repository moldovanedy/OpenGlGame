using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;

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

    public static MeshMaterial CreateColoredUnlitMaterial(byte r, byte g, byte b, byte a)
    {
        byte[] rawData = new byte[2 * 2 * 4];
        for (int i = 0; i < rawData.Length; i += 4)
        {
            rawData[i] = r;
            rawData[i + 1] = g;
            rawData[i + 2] = b;
            rawData[i + 3] = a;
        }

        var texFormat = new Texture.TextureFormat
        {
            Width = 2,
            Height = 2,
            Format = PixelFormat.Rgba,
            InternalFormat = PixelInternalFormat.Rgba
        };

        string hexCode = $"{r:X2}{g:X2}{b:X2}{a:X2}";
        //generate colored texture
        Texture texture = ResourceManager.Instance.CreateTexture(
            $"default_texture__{hexCode}.rgba.raw",
            rawData,
            texFormat
        );

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/diffuse_unlit")
            ?? throw new Exception("ColoredUnlitMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.DiffuseTextureMesh,
            [texture]);
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

    public static MeshMaterial CreateDiffuseLitMaterial(Texture? texture = null)
    {
        if (texture == null)
        {
            texture = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (texture == null)
            {
                throw new NullReferenceException("DiffuseLitMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/diffuse_lit")
            ?? throw new Exception("DiffuseLitMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.DiffuseTextureMesh,
            [texture]);
    }

    public static MeshMaterial CreateDiffuseTiledLitMaterial(Texture? texture = null)
    {
        if (texture == null)
        {
            texture = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (texture == null)
            {
                throw new NullReferenceException("DiffuseTiledLitMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/diffuse_tiled_lit")
            ?? throw new Exception("DiffuseTiledLitMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.DiffuseTextureMesh,
            [texture]);
    }
}