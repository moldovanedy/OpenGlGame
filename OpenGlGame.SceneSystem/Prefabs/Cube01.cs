using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;

namespace OpenGlGame.SceneSystem.Prefabs;

public class Cube01 : GameObject
{
    public Cube01()
    {
        AddComponent(new Transform3D());

        VertexAttribDescriptors vertAttributes = CommonVertexAttribDescriptors.AlbedoTextureMesh;
        Shader? shader = ResourceManager.Instance.GetOrCreateShader("/assets/shaders/albedo_lit");
        if (shader == null)
        {
            throw new NullReferenceException("Shader could not compile");
        }

        VertexBuffers? buffers =
            ResourceManager.Instance.GetOrCreateVertexBuffers(
                "/assets/models/cube_01/cube_01.glb.vert01",
                vertAttributes);
        if (buffers == null)
        {
            throw new NullReferenceException("Vertex buffers were not found");
        }

        Texture? defaultTexture = ResourceManager.Instance.GetOrCreateTexture("default_texture.rgba.raw");
        if (defaultTexture == null)
        {
            throw new NullReferenceException("Default texture was not found");
        }

        AddComponent(
            new MeshRenderer
            {
                VertBuffers = buffers,
                Shader = shader,
                VertAttributeDescriptors = vertAttributes,
                Textures = [defaultTexture]
            });
    }
}