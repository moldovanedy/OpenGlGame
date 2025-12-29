using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.SceneSystem;

public static class CommonMaterials
{
    public static MeshMaterial CreateVertColorsUnlitMaterial(
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
    {
        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/vert_colors_unlit")
            ?? throw new Exception("VertColorsMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.VertexColorsMesh,
            hasAlphaBlending: hasAlphaBlending,
            hasBackfaceCulling: hasBackfaceCulling);
    }

    public static MeshMaterial CreateColoredUnlitMaterial(
        byte r, byte g, byte b, byte a,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
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
            [texture],
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateColoredLitMaterial(
        byte r, byte g, byte b, byte a,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
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
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/diffuse_lit")
            ?? throw new Exception("ColoredLitMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.DiffuseTextureMesh,
            [texture],
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateDiffuseUnlitMaterial(
        Texture? texture = null,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
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
            [texture],
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateDiffuseLitMaterial(
        Texture? texture = null,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
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
            [texture],
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateDiffuseTiledLitMaterial(
        Texture? texture = null,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
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
            [texture],
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateDiffuseSpecularTiledLitMaterial(
        Texture? diffuseTex = null,
        Texture? specularTex = null,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
    {
        if (diffuseTex == null)
        {
            diffuseTex = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (diffuseTex == null)
            {
                throw new NullReferenceException("DiffuseSpecularTiledLitMaterial: Default texture was not found");
            }
        }

        if (specularTex == null)
        {
            specularTex = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (specularTex == null)
            {
                throw new NullReferenceException("DiffuseSpecularTiledLitMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/diffuse_spec_tiled_lit")
            ?? throw new Exception("DiffuseSpecularTiledLitMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.DiffuseTextureMesh,
            [diffuseTex, specularTex],
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateDiffSpecNormTiledLitMaterial(
        Texture? diffuseTex = null,
        Texture? specularTex = null,
        Texture? normalTex = null,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
    {
        if (diffuseTex == null)
        {
            diffuseTex = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (diffuseTex == null)
            {
                throw new NullReferenceException("DiffSpecNormTiledLitMaterial: Default texture was not found");
            }
        }

        if (specularTex == null)
        {
            specularTex = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (specularTex == null)
            {
                throw new NullReferenceException("DiffSpecNormTiledLitMaterial: Default texture was not found");
            }
        }

        if (normalTex == null)
        {
            normalTex = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (normalTex == null)
            {
                throw new NullReferenceException("DiffSpecNormTiledLitMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/diffuse_spec_normal_tiled_lit")
            ?? throw new Exception("DiffSpecNormTiledLitMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.DiffuseNormalTextureMesh,
            [diffuseTex, specularTex, normalTex],
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateDiffNormTiledLitMaterial(
        Texture? diffuseTex = null,
        Texture? normalTex = null,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
    {
        if (diffuseTex == null)
        {
            diffuseTex = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (diffuseTex == null)
            {
                throw new NullReferenceException("DiffNormTiledLitMaterial: Default texture was not found");
            }
        }

        if (normalTex == null)
        {
            normalTex = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (normalTex == null)
            {
                throw new NullReferenceException("DiffNormTiledLitMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/diffuse_normal_tiled_lit")
            ?? throw new Exception("DiffNormTiledLitMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.DiffuseNormalTextureMesh,
            [diffuseTex, normalTex],
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateUiPolygonMaterial(
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
    {
        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/ui_polygon")
            ?? throw new Exception("UiPolygonMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.UiMesh,
            null,
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateUiTexAtlasMaterial(
        Texture? texture = null,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
    {
        if (texture == null)
        {
            texture = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
            if (texture == null)
            {
                throw new NullReferenceException("UiTexAtlasMaterial: Default texture was not found");
            }
        }

        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/ui_tex_atlas")
            ?? throw new Exception("UiTexAtlasMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.UiMesh,
            [texture],
            hasAlphaBlending,
            hasBackfaceCulling);
    }

    public static MeshMaterial CreateSkyboxMaterial()
    {
        return new MeshMaterial(
            ResourceManager.Instance.GetOrCreateShader("/assets/shaders/skybox")
            ?? throw new Exception("SkyboxMaterial: Could not find shader file"),
            CommonVertexAttribDescriptors.Skybox,
            [],
            false,
            false);
    }
}