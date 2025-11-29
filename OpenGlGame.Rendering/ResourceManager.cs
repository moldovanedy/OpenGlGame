using System;
using System.Collections.Generic;
using System.IO;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using StbImageSharp;

namespace OpenGlGame.Rendering;

public class ResourceManager : IDisposable
{
    public static ResourceManager Instance
    {
        get
        {
            _instance ??= new ResourceManager();
            return _instance;
        }
    }

    private static ResourceManager? _instance;

    private const int MAX_EXPENSIVE_RESOURCE_COUNT = 255;
    private const int MAX_SIMPLE_RESOURCE_COUNT = 4095;

    private readonly Dictionary<string, WeakReference> _shaders = new();
    private readonly Dictionary<string, WeakReference> _textures = new();
    private readonly Dictionary<string, WeakReference> _vertexBuffers = new();
    private readonly Dictionary<string, WeakReference> _vertexAttribDescriptors = new();

    private readonly Stack<uint> _freeShaderResIds = new();
    private readonly Stack<uint> _freeTextureResIds = new();
    private readonly Stack<uint> _freeVertBufResIds = new();
    private readonly Stack<uint> _freeVertAttribDescriptorsResIds = new();

    private ResourceManager()
    {
        _freeShaderResIds.Push(1);
        _freeTextureResIds.Push(1);
        _freeVertBufResIds.Push(1);
        _freeVertAttribDescriptorsResIds.Push(1);
    }

    public void Dispose()
    {
        foreach (WeakReference weakReference in _shaders.Values)
        {
            ((IDisposable?)weakReference.Target)?.Dispose();
        }

        foreach (WeakReference weakReference in _textures.Values)
        {
            ((IDisposable?)weakReference.Target)?.Dispose();
        }

        foreach (WeakReference weakReference in _vertexBuffers.Values)
        {
            ((IDisposable?)weakReference.Target)?.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    public Shader? GetShader(string id)
    {
        if (_shaders.TryGetValue(id, out WeakReference? shaderRef))
        {
            return shaderRef.Target as Shader;
        }

        //automatically handles dictionary addition
        return ReadShaderFromFiles(id);
    }

    public Texture? GetTexture(
        string id,
        Texture.TextureFormat? creationTexFormat = null,
        Color4? creationWrapBorderColor = null)
    {
        if (_textures.TryGetValue(id, out WeakReference? textureRef))
        {
            return textureRef.Target as Texture;
        }

        if (creationTexFormat == null)
        {
            throw new Exception("Texture was not found, but no creation parameters were provided.");
        }

        //automatically handles dictionary addition
        return ReadTextureFromFile(id, creationTexFormat, creationWrapBorderColor);
    }

    public VertexAttribDescriptors? GetVertexAttributeDescriptors(string id)
    {
        return _vertexAttribDescriptors.GetValueOrDefault(id)?.Target as VertexAttribDescriptors;
    }

    public Shader CreateShader(string id, string vertexShader, string fragmentShader)
    {
        if (_freeShaderResIds.Count == 0)
        {
            throw new Exception("Out of shader space!");
        }

        Shader shader = new(_freeShaderResIds.Pop(), vertexShader, fragmentShader);
        _shaders[id] = new WeakReference(shader);

        //if there are no free slots in between, but the shader storage hasn't reached full capacity, we specify that
        // there still is space
        if (_freeShaderResIds.Count == 0 && _shaders.Count < MAX_SIMPLE_RESOURCE_COUNT)
        {
            _freeShaderResIds.Push((uint)_shaders.Count);
        }

        return shader;
    }

    public Texture CreateTexture(
        string id,
        byte[] rawData,
        Texture.TextureFormat texFormat,
        Color4? wrapBorderColor = null)
    {
        if (_freeTextureResIds.Count == 0)
        {
            throw new Exception("Out of texture space!");
        }

        Texture texture = new(_freeTextureResIds.Pop(), rawData, texFormat, wrapBorderColor);
        _textures[id] = new WeakReference(texture);

        if (_freeTextureResIds.Count == 0 && _textures.Count < MAX_EXPENSIVE_RESOURCE_COUNT)
        {
            _freeTextureResIds.Push((uint)_textures.Count);
        }

        return texture;
    }

    public VertexBuffers CreateVertexBuffers(
        string id,
        PrimitiveType primitiveTopology,
        VertexAttribDescriptors vertexAttribDescriptors,
        float[] vertexData,
        int vertexCount,
        uint[]? indexData = null,
        int indicesCount = 0)
    {
        if (_freeVertBufResIds.Count == 0)
        {
            throw new Exception("Out of vertex buffer space!");
        }

        VertexBuffers vertexBuffers = new(
            _freeVertBufResIds.Pop(),
            primitiveTopology,
            vertexAttribDescriptors,
            vertexData,
            vertexCount,
            indexData,
            indicesCount);
        _vertexBuffers[id] = new WeakReference(vertexBuffers);

        if (_freeVertBufResIds.Count == 0 && _vertexBuffers.Count < MAX_SIMPLE_RESOURCE_COUNT)
        {
            _freeVertBufResIds.Push((uint)_vertexBuffers.Count);
        }

        return vertexBuffers;
    }

    public VertexAttribDescriptors CreateVertexAttribDescriptors(
        string id,
        params VertexAttribDescriptors.AttribDescriptor[] attributes)
    {
        if (_freeVertAttribDescriptorsResIds.Count == 0)
        {
            throw new Exception("Out of vertex attribute descriptor space!");
        }

        VertexAttribDescriptors vertexBuffers = new(
            _freeVertAttribDescriptorsResIds.Pop(),
            attributes);
        _vertexAttribDescriptors[id] = new WeakReference(vertexBuffers);

        if (_freeVertAttribDescriptorsResIds.Count == 0 &&
            _vertexAttribDescriptors.Count < MAX_EXPENSIVE_RESOURCE_COUNT)
        {
            _freeVertAttribDescriptorsResIds.Push((uint)_vertexAttribDescriptors.Count);
        }

        return vertexBuffers;
    }


    public Shader? ReadShaderFromFiles(string id)
    {
        if (!id.StartsWith('/'))
        {
            return null;
        }

        try
        {
            string vertSrc = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + id + ".vert");
            string fragSrc = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + id + ".frag");

            return CreateShader(id, vertSrc, fragSrc);
        }
        catch (IOException ex)
        {
#if DEBUG
            Console.WriteLine("Exception at reading shader file: " + ex);
#endif
            return null;
        }
    }

    public Texture? ReadTextureFromFile(string id, Texture.TextureFormat texFormat, Color4? wrapBorderColor = null)
    {
        if (!id.StartsWith('/'))
        {
            return null;
        }

        try
        {
            ColorComponents stbImageFormat;
            switch (texFormat.Format)
            {
                default:
                    throw new ArgumentException(
                        "Unknown texture format:  " + texFormat.Format,
                        nameof(texFormat));
                case PixelFormat.Rgba:
                    stbImageFormat = ColorComponents.RedGreenBlueAlpha;
                    break;
                case PixelFormat.Rgb:
                    stbImageFormat = ColorComponents.RedGreenBlue;
                    break;
                case PixelFormat.Alpha:
                case PixelFormat.Red:
                case PixelFormat.Green:
                case PixelFormat.Blue:
                    stbImageFormat = ColorComponents.Grey;
                    break;
                case PixelFormat.Rg:
                    stbImageFormat = ColorComponents.GreyAlpha;
                    break;
            }

            //flip the image vertically to align to OpenGL format
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult image = ImageResult.FromStream(
                File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + id),
                stbImageFormat);

            return CreateTexture(id, image.Data, texFormat, wrapBorderColor);
        }
        catch (IOException ex)
        {
#if DEBUG
            Console.WriteLine("Exception at reading texture file: " + ex);
#endif
            return null;
        }
    }
}