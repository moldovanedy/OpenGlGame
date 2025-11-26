using System;
using System.Collections.Generic;
using System.IO;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.Rendering;

public class ResourceManager
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
        _freeShaderResIds.Push(0);
        _freeTextureResIds.Push(0);
        _freeVertBufResIds.Push(0);
        _freeVertAttribDescriptorsResIds.Push(0);
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
        if (_freeShaderResIds.Count == 0 && _shaders.Count < 4096)
        {
            _freeShaderResIds.Push((uint)_shaders.Count);
        }

        return shader;
    }

    public Texture CreateTexture(string id)
    {
        if (_freeTextureResIds.Count == 0)
        {
            throw new Exception("Out of texture space!");
        }

        Texture texture = new(_freeTextureResIds.Pop());
        _textures[id] = new WeakReference(texture);

        if (_freeTextureResIds.Count == 0 && _textures.Count < 256)
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

        if (_freeVertBufResIds.Count == 0 && _vertexBuffers.Count < 4096)
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

        if (_freeVertAttribDescriptorsResIds.Count == 0 && _vertexAttribDescriptors.Count < 256)
        {
            _freeVertAttribDescriptorsResIds.Push((uint)_vertexAttribDescriptors.Count);
        }

        return vertexBuffers;
    }


    private Shader? ReadShaderFromFiles(string id)
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
}