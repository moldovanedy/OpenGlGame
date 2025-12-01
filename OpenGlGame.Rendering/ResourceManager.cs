using System;
using System.Collections.Generic;
using System.IO;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;
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

    public Shader? GetOrCreateShader(string id)
    {
        if (_shaders.TryGetValue(id, out WeakReference? shaderRef))
        {
            return shaderRef.Target as Shader;
        }

        //automatically handles dictionary addition
        return ReadShaderFromFiles(id);
    }

    public Texture? GetOrCreateTexture(
        string id,
        Texture.TextureFormat? creationTexFormat = null)
    {
        if (_textures.TryGetValue(id, out WeakReference? textureRef))
        {
            return textureRef.Target as Texture;
        }

        return
            creationTexFormat == null
                ? throw new Exception("Texture was not found, but no creation parameters were provided.")
                //automatically handles dictionary addition
                : ReadTextureFromFile(id, ref creationTexFormat);
    }

    public VertexBuffers? GetOrCreateVertexBuffers(
        string id,
        VertexAttribDescriptors? creationVertexAttribDescriptors = null)
    {
        if (_vertexBuffers.TryGetValue(id, out WeakReference? value))
        {
            return value.Target as VertexBuffers;
        }

        //this means the meshes are generated at runtime, so there is no use in trying to load them from the FS
        if (id.Contains("/runtime_gen/"))
        {
            return null;
        }

        if (creationVertexAttribDescriptors == null)
        {
            throw new Exception("Vertex buffers were not found, but no creation parameters were provided.");
        }

        try
        {
            //automatically handles dictionary addition
            ModelLoaderUtility.LoadModelFromFile(
                id.Substring(0, id.LastIndexOf('.')),
                creationVertexAttribDescriptors);
        }
        catch (Exception ex)
        {
#if DEBUG
            Console.WriteLine($"[DEBUG] Exception when trying to load vertex buffers with ID {id}: {ex}");
#endif

            return null;
        }

        //try again
        if (_vertexBuffers.TryGetValue(id, out value))
        {
            return value.Target as VertexBuffers;
        }

        return null;
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

        //if a value on the same ID already exists, overwrite it
        Shader? existingValue = null;
        if (_vertexBuffers.TryGetValue(id, out WeakReference? existingRef))
        {
            existingValue = existingRef.Target as Shader;
        }

        Shader shader = new(
            existingValue?.ResourceId ?? _freeShaderResIds.Pop(),
            vertexShader,
            fragmentShader);
        _shaders[id] = new WeakReference(shader);

        //if there are no free slots in between, but the shader storage hasn't reached full capacity, we specify that
        // there still is space
        if (_freeShaderResIds.Count == 0 && _shaders.Count < MAX_SIMPLE_RESOURCE_COUNT)
        {
            _freeShaderResIds.Push((uint)_shaders.Count + 1);
        }

        return shader;
    }

    public Texture CreateTexture(
        string id,
        byte[] rawData,
        Texture.TextureFormat texFormat)
    {
        if (_freeTextureResIds.Count == 0)
        {
            throw new Exception("Out of texture space!");
        }

        //if a value on the same ID already exists, overwrite it
        Texture? existingValue = null;
        if (_vertexBuffers.TryGetValue(id, out WeakReference? existingRef))
        {
            existingValue = existingRef.Target as Texture;
        }

        Texture texture = new(existingValue?.ResourceId ?? _freeTextureResIds.Pop(), rawData, texFormat);
        _textures[id] = new WeakReference(texture);

        if (_freeTextureResIds.Count == 0 && _textures.Count < MAX_EXPENSIVE_RESOURCE_COUNT)
        {
            _freeTextureResIds.Push((uint)_textures.Count + 1);
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

        //if a value on the same ID already exists, overwrite it
        VertexBuffers? existingValue = null;
        if (_vertexBuffers.TryGetValue(id, out WeakReference? existingRef))
        {
            existingValue = existingRef.Target as VertexBuffers;
        }

        VertexBuffers vertexBuffers = new(
            existingValue?.ResourceId ?? _freeVertBufResIds.Pop(),
            primitiveTopology,
            vertexAttribDescriptors,
            vertexData,
            vertexCount,
            indexData,
            indicesCount);
        _vertexBuffers[id] = new WeakReference(vertexBuffers);

        if (_freeVertBufResIds.Count == 0 && _vertexBuffers.Count < MAX_SIMPLE_RESOURCE_COUNT)
        {
            _freeVertBufResIds.Push((uint)_vertexBuffers.Count + 1);
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

        //if a value on the same ID already exists, overwrite it
        VertexAttribDescriptors? existingValue = null;
        if (_vertexBuffers.TryGetValue(id, out WeakReference? existingRef))
        {
            existingValue = existingRef.Target as VertexAttribDescriptors;
        }

        VertexAttribDescriptors vertexBuffers = new(
            existingValue?.ResourceId ?? _freeVertAttribDescriptorsResIds.Pop(),
            attributes);
        _vertexAttribDescriptors[id] = new WeakReference(vertexBuffers);

        if (_freeVertAttribDescriptorsResIds.Count == 0 &&
            _vertexAttribDescriptors.Count < MAX_EXPENSIVE_RESOURCE_COUNT)
        {
            _freeVertAttribDescriptorsResIds.Push((uint)_vertexAttribDescriptors.Count + 1);
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

    public Texture? ReadTextureFromFile(string id, ref Texture.TextureFormat texFormat)
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

            texFormat.Width = image.Width;
            texFormat.Height = image.Height;

            return CreateTexture(id, image.Data, texFormat);
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