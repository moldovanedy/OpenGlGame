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
            // positions         // normals      //texture coords
            -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, // bottom left
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom right
            0.0f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.5f, 1.0f // top 
        ];

        VertexAttribDescriptors vertexAttributeDescriptors =
            ResourceManager.Instance.CreateVertexAttribDescriptors(
                "simple_one_texture_model",
                new VertexAttribDescriptors.AttribDescriptor(
                    0,
                    3,
                    VertexAttribPointerType.Float,
                    8 * sizeof(float),
                    0),
                new VertexAttribDescriptors.AttribDescriptor(
                    1,
                    3,
                    VertexAttribPointerType.Float,
                    8 * sizeof(float),
                    3 * sizeof(float)),
                new VertexAttribDescriptors.AttribDescriptor(
                    2,
                    2,
                    VertexAttribPointerType.Float,
                    8 * sizeof(float),
                    6 * sizeof(float))
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