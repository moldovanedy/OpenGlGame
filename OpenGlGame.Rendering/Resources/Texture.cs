using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.Rendering.Resources;

public class Texture : ResourceBase
{
    public record TextureFormat(
        PixelInternalFormat InternalFormat,
        PixelFormat Format,
        int Width,
        int Height,
        PixelType IndividualPixelType,
        TextureWrapMode XWrapMode,
        TextureWrapMode YWrapMode,
        TextureMinFilter MinFilter,
        TextureMagFilter MagFilter
    );


    internal int GlTextureId { get; private set; }

    public Texture(
        uint resourceId,
        byte[] rawData,
        TextureFormat texFormat,
        Color4? wrapBorderColor = null) : base(resourceId)
    {
        GL.GenTextures(1, out int texId);
        GlTextureId = texId;
        GL.BindTexture(TextureTarget.Texture2D, texId);

        GL.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapS,
            [(int)texFormat.XWrapMode]);
        GL.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureWrapS,
            [(int)texFormat.YWrapMode]);

        GL.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMinFilter,
            [(int)texFormat.MinFilter]);
        GL.TexParameterI(
            TextureTarget.Texture2D,
            TextureParameterName.TextureMagFilter,
            [(int)texFormat.MagFilter]);

        if (texFormat.XWrapMode == TextureWrapMode.ClampToBorder ||
            texFormat.YWrapMode == TextureWrapMode.ClampToBorder)
        {
            wrapBorderColor ??= Color4.Black;
            float[] colorComponents =
            [
                wrapBorderColor.Value.R,
                wrapBorderColor.Value.G,
                wrapBorderColor.Value.B,
                wrapBorderColor.Value.A
            ];

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureBorderColor,
                colorComponents);
        }

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            texFormat.InternalFormat,
            texFormat.Width,
            texFormat.Height,
            0,
            texFormat.Format,
            texFormat.IndividualPixelType,
            rawData);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }
}