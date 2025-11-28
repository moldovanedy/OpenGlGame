using System;
using OpenGlGame.ECS.Systems;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenGlGame;

public class Application : IDisposable
{
    private readonly Renderer _renderer = new();
    private readonly GlobalSystem _globalSystem;

    private VertexAttribDescriptors? _vertexAttributeDescriptors;
    private Shader? _shader;

    private VertexBuffers? _triangleVertexBuffers;
    private VertexBuffers? _indexedQuadVertexBuffers;

    public Application()
    {
        _globalSystem = new GlobalSystem();
    }

    public void Dispose()
    {
        ResourceManager.Instance.Dispose();
        GC.SuppressFinalize(this);
    }

    internal void Load()
    {
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.FramebufferSrgb);
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

        _globalSystem.StartDrawing();

        float[] triangleVertices =
        [
            // positions         // colors
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom left
            0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f // top 
        ];

        _vertexAttributeDescriptors =
            ResourceManager.Instance.CreateVertexAttribDescriptors(
                "triangle_with_vertex_colors",
                new VertexAttribDescriptors.AttribDescriptor(
                    0, 3, VertexAttribPointerType.Float, 24, 0),
                new VertexAttribDescriptors.AttribDescriptor(
                    1, 3, VertexAttribPointerType.Float, 24, 12)
            );

        _shader = ResourceManager.Instance.GetShader("/Assets/Shaders/basic_unlit");
        if (_shader == null)
        {
            throw new NullReferenceException("Shader could not compile");
        }

        float[] quadVertices =
        [
            // positions         // colors
            -0.25f, 0.75f, -0.01f, 1.0f, 0.0f, 0.0f,
            -0.25f, -0.25f, -0.01f, 1.0f, 0.0f, 0.0f,
            0.75f, -0.25f, -0.01f, 0.0f, 0.0f, 1.0f,
            0.75f, 0.75f, -0.01f, 0.0f, 0.0f, 1.0f
        ];

        uint[] indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        _triangleVertexBuffers =
            ResourceManager.Instance.CreateVertexBuffers(
                "triangle",
                PrimitiveType.Triangles,
                _vertexAttributeDescriptors,
                triangleVertices,
                3
            );

        _indexedQuadVertexBuffers =
            ResourceManager.Instance.CreateVertexBuffers(
                "indexed_quad",
                PrimitiveType.Triangles,
                _vertexAttributeDescriptors,
                quadVertices,
                4,
                indices,
                6
            );
    }

    internal void Update(FrameEventArgs e, GameWindow window)
    {
        if (
            _shader == null
            || _triangleVertexBuffers == null
            || _vertexAttributeDescriptors == null
            || _indexedQuadVertexBuffers == null)
        {
            throw new NullReferenceException("Resources missing");
        }

        _globalSystem.Update(
            new GlobalSystem.FrameParameters
            {
                Width = window.FramebufferSize.X,
                Height = window.FramebufferSize.Y
            }
        );

        // _renderer.AddCommand(new DrawOperation(_shader, _triangleVertexBuffers, _vertexAttributeDescriptors));
        // _renderer.AddCommand(
        //     new DrawOperation(
        //         _shader,
        //         _indexedQuadVertexBuffers,
        //         _vertexAttributeDescriptors));
        //
        // _renderer.Render(window.FramebufferSize.X, window.FramebufferSize.Y);

        window.SwapBuffers();
    }

    internal static void FramebufferResize(FramebufferResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}