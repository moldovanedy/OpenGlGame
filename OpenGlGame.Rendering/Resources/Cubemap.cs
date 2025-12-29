using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.Rendering.Resources;

public class Cubemap : ResourceBase, IDisposable
{
    internal int GlCubemapId { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="rawDataTextures">In this order: +X, -X, +Y, -Y, +Z, -Z</param>
    /// <param name="texFormat"></param>
    /// <param name="shouldSetMipmaps"></param>
    public Cubemap(
        uint resourceId,
        byte[][] rawDataTextures,
        Texture.TextureFormat texFormat,
        bool shouldSetMipmaps = true) : base(resourceId)
    {
        if (!shouldSetMipmaps)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (texFormat.MinFilter)
            {
                case TextureMinFilter.LinearMipmapLinear:
                case TextureMinFilter.LinearMipmapNearest:
                    texFormat.MinFilter = TextureMinFilter.Linear;
                    break;
                case TextureMinFilter.NearestMipmapLinear:
                case TextureMinFilter.NearestMipmapNearest:
                    texFormat.MinFilter = TextureMinFilter.Nearest;
                    break;
            }
        }

        GL.GenTextures(1, out int texId);
        GlCubemapId = texId;
        GL.BindTexture(TextureTarget.TextureCubeMap, texId);

        if (texFormat.XWrapMode == TextureWrapMode.ClampToBorder ||
            texFormat.YWrapMode == TextureWrapMode.ClampToBorder)
        {
            texFormat.WrapBorderColor ??= Color4.Black;
            float[] colorComponents =
            [
                texFormat.WrapBorderColor.Value.R,
                texFormat.WrapBorderColor.Value.G,
                texFormat.WrapBorderColor.Value.B,
                texFormat.WrapBorderColor.Value.A
            ];

            GL.TexParameter(
                TextureTarget.TextureCubeMap,
                TextureParameterName.TextureBorderColor,
                colorComponents);
        }

        for (int i = 0; i < 6; i++)
        {
            GL.TexImage2D(
                TextureTarget.TextureCubeMapPositiveX + i,
                0,
                texFormat.InternalFormat,
                texFormat.Width,
                texFormat.Height,
                0,
                texFormat.Format,
                texFormat.IndividualPixelType,
                rawDataTextures[i]);
        }

        GL.TexParameter(
            TextureTarget.TextureCubeMap,
            TextureParameterName.TextureWrapS,
            (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(
            TextureTarget.TextureCubeMap,
            TextureParameterName.TextureWrapT,
            (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(
            TextureTarget.TextureCubeMap,
            TextureParameterName.TextureWrapR,
            (int)TextureWrapMode.ClampToEdge);

        GL.TexParameter(
            TextureTarget.TextureCubeMap,
            TextureParameterName.TextureMinFilter,
            (int)texFormat.MinFilter);
        GL.TexParameter(
            TextureTarget.TextureCubeMap,
            TextureParameterName.TextureMagFilter,
            (int)texFormat.MagFilter);

        if (shouldSetMipmaps)
        {
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
        }
    }

    public void Dispose()
    {
        GL.DeleteTexture(GlCubemapId);
        GC.SuppressFinalize(this);
    }

    ~Cubemap()
    {
        Dispose();
    }
}