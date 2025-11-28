using System;
using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.Prefabs;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace OpenGlGame;

public class Application : IDisposable
{
    private readonly SceneManager _sceneManager = new();

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

        ColoredTriangle tri1 = new();
        if (tri1.GetComponent(nameof(Transform3D)) is Transform3D transform1)
        {
            transform1.Position = new Vector3(-0.5f, 1.0f, 2.0f);
            transform1.Scale = new Vector3(1.5f, 0.75f, 1.5f);
        }

        ColoredTriangle tri2 = new();
        if (tri2.GetComponent(nameof(Transform3D)) is Transform3D transform2)
        {
            transform2.Position = new Vector3(3.0f, 1.0f, 1.1f);
            transform2.Rotation = new Vector3(0.0f, float.DegreesToRadians(-45.0f), 0.0f);
        }

        ColoredTriangle tri3 = new();
        if (tri3.GetComponent(nameof(Transform3D)) is Transform3D transform3)
        {
            transform3.Position = new Vector3(-4.0f, -0.5f, 1.15f);
            transform3.Rotation = new Vector3(0.0f, 0.0f, float.DegreesToRadians(-135.0f));
            transform3.Scale = new Vector3(1.0f, 1.5f, 1.0f);
        }

        SceneManager.WorldChunk chunk1 = new(
            [tri1, tri2, tri3],
            Vector3.Zero);

        List<SceneManager.WorldChunk> chunks =
        [
            chunk1,
            new([], Vector3.Zero),
            new([], Vector3.Zero),
            new([], Vector3.Zero),
            new([], Vector3.Zero)
        ];
        _sceneManager.SetLevel(chunks);
    }

    internal void Update(FrameEventArgs e, GameWindow window)
    {
        _sceneManager.Update(
            new SceneManager.FrameParameters(
                window.FramebufferSize.X,
                window.FramebufferSize.Y));

        window.SwapBuffers();
    }

    internal static void FramebufferResize(FramebufferResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}