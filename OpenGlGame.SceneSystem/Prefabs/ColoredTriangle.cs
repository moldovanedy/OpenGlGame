using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.SceneSystem.Prefabs;

public class ColoredTriangle : GameObject
{
    private const string VERT_BUFFERS_PATH = "/runtime_gen/triangle";

    public ColoredTriangle()
    {
        AddComponent(new Transform3D());

        float[] triangleVertices =
        [
            // positions         // normals      //vertex colors
            -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom left
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom right
            0.0f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f // top 
        ];

        VertexAttribDescriptors vertexAttributeDescriptors = CommonVertexAttribDescriptors.VertexColorsMesh;
        Shader? shader = ResourceManager.Instance.GetOrCreateShader("/assets/shaders/vert_colors_lit");
        if (shader == null)
        {
            throw new NullReferenceException("Shader could not compile");
        }

        VertexBuffers triangleVertexBuffers =
            ResourceManager.Instance.GetOrCreateVertexBuffers(VERT_BUFFERS_PATH)
            ?? ResourceManager.Instance.CreateVertexBuffers(
                VERT_BUFFERS_PATH,
                PrimitiveType.Triangles,
                vertexAttributeDescriptors,
                triangleVertices,
                3
            );

        AddComponent(
            new MeshRenderer
            {
                VertBuffers = triangleVertexBuffers,
                Shader = shader,
                VertAttributeDescriptors = vertexAttributeDescriptors
            });
    }
}