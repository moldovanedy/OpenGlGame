using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.Rendering.Resources;

public class Texture : ResourceBase, IDisposable
{
    public class TextureFormat
    {
        public PixelInternalFormat InternalFormat { get; set; } = PixelInternalFormat.Rgba;
        public PixelFormat Format { get; set; } = PixelFormat.Rgba;
        public PixelType IndividualPixelType { get; set; } = PixelType.UnsignedByte;

        public int Width { get; set; }
        public int Height { get; set; }

        public TextureWrapMode XWrapMode { get; set; } = TextureWrapMode.Clamp;
        public TextureWrapMode YWrapMode { get; set; } = TextureWrapMode.Clamp;

        public TextureMinFilter MinFilter { get; set; } = TextureMinFilter.LinearMipmapLinear;
        public TextureMagFilter MagFilter { get; set; } = TextureMagFilter.Linear;

        public Color4? WrapBorderColor { get; set; }

        public TextureFormat()
        {
        }

        public TextureFormat(
            PixelInternalFormat internalFormat,
            PixelFormat format,
            int width,
            int height,
            TextureWrapMode xWrapMode,
            TextureWrapMode yWrapMode,
            TextureMinFilter minFilter,
            TextureMagFilter magFilter,
            Color4? wrapBorderColor = null)
        {
            InternalFormat = internalFormat;
            Format = format;
            Width = width;
            Height = height;
            XWrapMode = xWrapMode;
            YWrapMode = yWrapMode;
            MinFilter = minFilter;
            MagFilter = magFilter;
            WrapBorderColor = wrapBorderColor;
        }
    }


    internal int GlTextureId { get; private set; }

    internal Texture(
        uint resourceId,
        byte[] rawData,
        TextureFormat texFormat) : base(resourceId)
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

        //enable anisotropic filtering
        float anisotropyLevel = GL.GetFloat(GetPName.MaxTextureMaxAnisotropy);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxAnisotropy, anisotropyLevel);

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

    public void Dispose()
    {
        GL.DeleteTexture(GlTextureId);
        GC.SuppressFinalize(this);
    }

    ~Texture()
    {
        Dispose();
    }
}