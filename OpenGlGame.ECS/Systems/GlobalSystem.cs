using System;
using System.Numerics;
using OpenGlGame.ECS.Components;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.ECS.Systems;

public class GlobalSystem
{
    public struct FrameParameters
    {
        public int Width;
        public int Height;
    }


    private readonly SceneRegistry _sceneRegistry;
    private readonly Renderer _renderer;

    private readonly DrawingSystem _drawingSystem;
    private readonly WorldManagerSystem _worldManagerSystem;

    private bool _canDraw;

    public GlobalSystem()
    {
        _sceneRegistry = new SceneRegistry();
        _renderer = new Renderer();

        _drawingSystem = new DrawingSystem(_renderer, _sceneRegistry);
        _worldManagerSystem = new WorldManagerSystem(_sceneRegistry);
    }

    public void StartDrawing()
    {
        _canDraw = true;

        WorldManagerSystem.WorldChunk chunk1 = CreateSceneChunk();
        var chunk2 = new WorldManagerSystem.WorldChunk([[]], new Vector3(0, 0, 50));
        var chunk3 = new WorldManagerSystem.WorldChunk([[]], new Vector3(50, 0, 50));
        var chunk4 = new WorldManagerSystem.WorldChunk([[]], new Vector3(100, 0, 50));
        var chunk5 = new WorldManagerSystem.WorldChunk([[]], new Vector3(100, 0, 100));

        _worldManagerSystem.SetLevel([chunk1, chunk2, chunk3, chunk4, chunk5]);
    }

    //TODO: this is a temporary test scene, actual levels need to be imported somehow
    private static WorldManagerSystem.WorldChunk CreateSceneChunk()
    {
        VertexAttribDescriptors vertexAttributeDescriptors =
            ResourceManager.Instance.CreateVertexAttribDescriptors(
                "triangle_with_vertex_colors",
                new VertexAttribDescriptors.AttribDescriptor(
                    0, 3, VertexAttribPointerType.Float, 24, 0),
                new VertexAttribDescriptors.AttribDescriptor(
                    1, 3, VertexAttribPointerType.Float, 24, 12)
            );

        Shader? shader = ResourceManager.Instance.GetShader("/Assets/Shaders/basic_unlit");
        if (shader == null)
        {
            throw new NullReferenceException("Shader could not compile");
        }

        float[] triangleVertices =
        [
            // positions         // colors
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom left
            0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f // top 
        ];

        VertexBuffers triangleVertexBuffers =
            ResourceManager.Instance.CreateVertexBuffers(
                "triangle",
                PrimitiveType.Triangles,
                vertexAttributeDescriptors,
                triangleVertices,
                3
            );

        object[][] objects =
        [
            [
                new Transform3D(new Vector3(0.5f, 0f, 1.0f)),
                new MeshRenderer(vertexAttributeDescriptors, triangleVertexBuffers, shader)
            ],
            [
                new Transform3D(new Vector3(-0.5f, 1.0f, 2.5f)),
                new MeshRenderer(vertexAttributeDescriptors, triangleVertexBuffers, shader)
            ],
            [
                new Transform3D(new Vector3(-1.5f, 1.5f, 3.0f)),
                new MeshRenderer(vertexAttributeDescriptors, triangleVertexBuffers, shader)
            ]
        ];

        return new WorldManagerSystem.WorldChunk(objects, new Vector3(0, 0, 0));
    }

    public void Update(FrameParameters parameters)
    {
        if (_canDraw)
        {
            _drawingSystem.Update(parameters.Width, parameters.Height);
        }

        _worldManagerSystem.Update();
    }
}