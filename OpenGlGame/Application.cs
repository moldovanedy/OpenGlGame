using System;
using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.Prefabs;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace OpenGlGame;

public class Application : IDisposable
{
    private readonly SceneManager _sceneManager = new();
    private GameWindow? _windowRef;

    //Wayland does not allow a window to control its own window mode...
    private bool _isWayland;
    private bool _isFullscreenToggleRequested;
    private bool _isFullscreen;

    private int _windowedWidth = 800;
    private int _windowedHeight = 600;

    public void Dispose()
    {
        ResourceManager.Instance.Dispose();
        GC.SuppressFinalize(this);
    }

    internal void SetWindowRef(GameWindow window)
    {
        _windowRef = window;
    }

    internal void Load()
    {
        _isWayland = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE")?.ToLower() == "wayland";
        if (_windowRef != null)
        {
            _windowedWidth = _windowRef.ClientSize.X;
            _windowedHeight = _windowRef.ClientSize.Y;
            _isFullscreen = _windowRef.WindowState == WindowState.Fullscreen;
        }

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
            transform3.Position = new Vector3(0.0f, -1.5f, 0.0f);
            transform3.Rotation = new Vector3(float.DegreesToRadians(90.0f), 0.0f, 0.0f);
            transform3.Scale = new Vector3(30.0f, 30.0f, 30.0f);
        }

        SceneManager.WorldChunk chunk1 = new(
            [tri1, tri2, tri3],
            Vector3.Zero,
            new Vector3(500, 500, 500));

        List<SceneManager.WorldChunk> chunks =
        [
            chunk1,
            new([], Vector3.Zero, Vector3.One),
            new([], Vector3.Zero, Vector3.One),
            new([], Vector3.Zero, Vector3.One),
            new([], Vector3.Zero, Vector3.One)
        ];
        _sceneManager.SetLevel(chunks);
    }

    internal void Update(FrameEventArgs e, GameWindow window)
    {
        if (_isFullscreenToggleRequested)
        {
            if (_isFullscreen)
            {
                window.WindowState = WindowState.Normal;
                window.ClientSize = new Vector2i(_windowedWidth, _windowedHeight);
            }
            else
            {
                _windowedWidth = window.ClientSize.X;
                _windowedHeight = window.ClientSize.Y;

                window.WindowState = _isWayland ? WindowState.Maximized : WindowState.Fullscreen;
            }

            _isFullscreen = !_isFullscreen;
            _isFullscreenToggleRequested = false;
        }

        _sceneManager.Update(
            new SceneManager.FrameParameters(
                window.FramebufferSize.X,
                window.FramebufferSize.Y,
                e.Time));

        window.SwapBuffers();
    }

    #region Input

    private Camera.CameraMovement _playerDirection;

    internal void OnKeyUp(KeyboardKeyEventArgs e)
    {
        switch (e.Key)
        {
            default:
                return;
            case Keys.F11:
                _isFullscreenToggleRequested = true;
                break;
            case Keys.W:
            case Keys.Up:
            {
                _playerDirection &= ~Camera.CameraMovement.Forward;
                break;
            }
            case Keys.S:
            case Keys.Down:
            {
                _playerDirection &= ~Camera.CameraMovement.Backward;
                break;
            }
            case Keys.A:
            case Keys.Left:
            {
                _playerDirection &= ~Camera.CameraMovement.Left;
                break;
            }
            case Keys.D:
            case Keys.Right:
            {
                _playerDirection &= ~Camera.CameraMovement.Right;
                break;
            }
        }

        _sceneManager.PlayerCamera.InputMovementChanged(_playerDirection);
    }

    internal void OnKeyDown(KeyboardKeyEventArgs e)
    {
        switch (e.Key)
        {
            default:
                return;
            case Keys.W:
            case Keys.Up:
            {
                if ((_playerDirection & Camera.CameraMovement.Backward) != Camera.CameraMovement.None)
                {
                    break;
                }

                _playerDirection |= Camera.CameraMovement.Forward;
                break;
            }
            case Keys.S:
            case Keys.Down:
            {
                if ((_playerDirection & Camera.CameraMovement.Forward) != Camera.CameraMovement.None)
                {
                    break;
                }

                _playerDirection |= Camera.CameraMovement.Backward;
                break;
            }
            case Keys.A:
            case Keys.Left:
            {
                if ((_playerDirection & Camera.CameraMovement.Right) != Camera.CameraMovement.None)
                {
                    break;
                }

                _playerDirection |= Camera.CameraMovement.Left;
                break;
            }
            case Keys.D:
            case Keys.Right:
            {
                if ((_playerDirection & Camera.CameraMovement.Left) != Camera.CameraMovement.None)
                {
                    break;
                }

                _playerDirection |= Camera.CameraMovement.Right;
                break;
            }
        }

        _sceneManager.PlayerCamera.InputMovementChanged(_playerDirection);
    }

    private float _lastX;
    private float _lastY;
    private bool _isFirstMouse = true;

    internal void OnMouseMove(MouseMoveEventArgs e)
    {
        if (_isFirstMouse)
        {
            _lastX = e.X;
            _lastY = e.Y;
            _isFirstMouse = false;
        }

        float xOffset = e.X - _lastX;
        //Y-axis reversed
        float yOffset = _lastY - e.Y;

        _lastX = e.X;
        _lastY = e.Y;

        _sceneManager.PlayerCamera.InputLookDirectionChanged(xOffset, yOffset);
    }

    #endregion

    internal static void FramebufferResize(FramebufferResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}