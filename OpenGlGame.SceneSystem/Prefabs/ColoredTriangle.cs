using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.SceneSystem.Prefabs;

public class ColoredTriangle : GameObject
{
    public ColoredTriangle()
    {
        AddComponent(new Transform3D());

        float[] triangleVertices =
        [
            // positions         // colors
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom left
            0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f // top 
        ];

        VertexAttribDescriptors vertexAttributeDescriptors =
            ResourceManager.Instance.CreateVertexAttribDescriptors(
                "triangle_with_vertex_colors",
                new VertexAttribDescriptors.AttribDescriptor(
                    0, 3, VertexAttribPointerType.Float, 24, 0),
                new VertexAttribDescriptors.AttribDescriptor(
                    1, 3, VertexAttribPointerType.Float, 24, 12)
            );

        Shader? shader = ResourceManager.Instance.GetShader("/Assets/Shaders/basic_lit");
        if (shader == null)
        {
            throw new NullReferenceException("Shader could not compile");
        }

        VertexBuffers triangleVertexBuffers =
            ResourceManager.Instance.CreateVertexBuffers(
                "triangle",
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