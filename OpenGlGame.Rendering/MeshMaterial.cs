using System.Collections.Generic;
using OpenGlGame.Rendering.Resources;
using OpenTK.Mathematics;

namespace OpenGlGame.Rendering;

public class MeshMaterial
{
    internal const int RENDER_STATE_OFFSET = 48;
    internal const ulong RENDER_STATE_MASK = 0x0fUL << 48;

    internal const int SHADER_PROGRAM_OFFSET = 36;
    internal const ulong SHADER_PROGRAM_MASK = 0x0fffUL << 36;

    internal const int TEXTURE_ATTACHMENTS_HASH_OFFSET = 20;
    internal const ulong TEXTURE_ATTACHMENTS_HASH_MASK = 0xffffUL << 20;

    internal const int VERTEX_ATTRIBUTE_FORMAT_OFFSET = 12;
    internal const ulong VERTEX_ATTRIBUTE_FORMAT_MASK = 0xffUL << 12;

    public ulong OrderKey { get; private set; }

    public Shader UsedShader { get; private set; }

    public VertexAttribDescriptors VertexAttributes { get; private set; }

    public Texture[]? Textures { get; private set; }

    public bool HasAlphaBlending { get; private set; }

    public bool HasBackfaceCulling { get; private set; }

    private readonly Dictionary<string, object> _properties = [];

    public MeshMaterial(
        Shader shader,
        VertexAttribDescriptors vertexAttributes,
        Texture[]? textures = null,
        bool hasAlphaBlending = false,
        bool hasBackfaceCulling = true)
    {
        UsedShader = shader;
        VertexAttributes = vertexAttributes;
        Textures = textures;
        HasAlphaBlending = hasAlphaBlending;
        HasBackfaceCulling = hasBackfaceCulling;

        OrderKey = ComputeOrderKey();
    }

    protected void AddProperty(string name, object value)
    {
        _properties[name] = value;
    }

    public T? GetProperty<T>(string propertyName)
    {
        if (_properties.TryGetValue(propertyName, out object? value))
        {
            return (T)value;
        }

        return default;
    }

    public void SetProperty<T>(string propertyName, T value) where T : notnull
    {
        _properties[propertyName] = value;
    }

    internal void UpdateProperties()
    {
        foreach (KeyValuePair<string, object> property in _properties)
        {
            switch (property.Value)
            {
                case int intValue:
                    UsedShader.SetUniformScalarI(property.Key, intValue);
                    break;
                case float floatValue:
                    UsedShader.SetUniformScalar(property.Key, floatValue);
                    break;
                case Vector2 vec2:
                    UsedShader.SetUniformVec2(property.Key, ref vec2);
                    break;
                case Vector2i vec2I:
                    UsedShader.SetUniformVec2I(property.Key, ref vec2I);
                    break;
                case Vector3 vec3:
                    UsedShader.SetUniformVec3(property.Key, ref vec3);
                    break;
                case Vector3i vec3I:
                    UsedShader.SetUniformVec3I(property.Key, ref vec3I);
                    break;
                case Vector4 vec4:
                    UsedShader.SetUniformVec4(property.Key, ref vec4);
                    break;
                case Vector4i vec4I:
                    UsedShader.SetUniformVec4I(property.Key, ref vec4I);
                    break;
                case Matrix3 mat3:
                    UsedShader.SetUniformMat3(property.Key, ref mat3);
                    break;
                case Matrix4 mat4:
                    UsedShader.SetUniformMat4(property.Key, ref mat4);
                    break;
            }
        }
    }


    private ulong ComputeOrderKey()
    {
        ushort textureHash = 0;
        if (Textures != null)
        {
            if (Textures.Length == 1)
            {
                textureHash = (ushort)Textures[0].ResourceId;
            }
            else
            {
                unchecked
                {
                    uint h = 0xBEEF;

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (Texture tex in Textures)
                    {
                        h ^= tex.ResourceId + 0x9E3779B9 + (h << 6) + (h >> 2);
                    }

                    textureHash = (ushort)(h & 0xFFFF);
                }
            }
        }

        return
            ((HasAlphaBlending ? 1UL : 0UL) << (RENDER_STATE_OFFSET + 1))
            | ((HasBackfaceCulling ? 1UL : 0UL) << RENDER_STATE_OFFSET)
            | ((ulong)UsedShader.ResourceId << SHADER_PROGRAM_OFFSET)
            | ((ulong)textureHash << TEXTURE_ATTACHMENTS_HASH_OFFSET)
            | (VertexAttributes.ResourceId << VERTEX_ATTRIBUTE_FORMAT_OFFSET);
    }
}