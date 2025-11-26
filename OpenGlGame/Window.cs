using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Operations;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenGlGame;

public class Window : GameWindow
{
    private VertexAttribDescriptors? _vertexAttributeDescriptors;
    private Shader? _shader;

    private VertexBuffers? _triangleVertexBuffers;
    private VertexBuffers? _indexedQuadVertexBuffers;

    public Window(bool isFullscreen, int width, int height)
        : base(
            GameWindowSettings.Default,
            new NativeWindowSettings
            {
                WindowState = isFullscreen ? WindowState.Fullscreen : WindowState.Normal,
                ClientSize = (width, height),
                MinimumClientSize = (600, 400),
                Title = "OpenGL game",
                NumberOfSamples = 4,
                Vsync = VSyncMode.On
            })
    {
        GL.Enable(EnableCap.Multisample);
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

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

        _shader = ResourceManager.Instance.GetShader("/Shaders/basic_unlit");
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

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        if (
            _shader == null
            || _triangleVertexBuffers == null
            || _vertexAttributeDescriptors == null
            || _indexedQuadVertexBuffers == null)
        {
            throw new NullReferenceException("Resources missing");
        }

        Renderer.Instance.ClearCommands();
        Renderer.Instance.AddCommand(new DrawOperation(_shader, _triangleVertexBuffers, _vertexAttributeDescriptors));
        Renderer.Instance.AddCommand(
            new DrawOperation(
                _shader,
                _indexedQuadVertexBuffers,
                _vertexAttributeDescriptors));

        Renderer.Instance.Render(FramebufferSize.X, FramebufferSize.Y);
        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
    }
}