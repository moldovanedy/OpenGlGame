using System;
using System.Collections.Generic;
using System.Timers;
using OpenGlGame.Chunks.Chunk01;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.PhysicsEngine;
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
    private Timer? _physicsUpdateTimer;

    //Wayland does not allow a window to control its own window mode...
    private bool _isWayland;
    private bool _isFullscreenToggleRequested;
    private bool _isFullscreen;

    private int _windowedWidth = 800;
    private int _windowedHeight = 600;
    private bool _isPaused;

    public void Dispose()
    {
        Player.Instance.ExitScene();

        _physicsUpdateTimer?.Stop();
        _physicsUpdateTimer = null;
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

        GenerateRuntimeAssets();

        List<SceneManager.WorldChunk> chunks =
        [
            new LevelChunk01(),
            new(new Aabb(Vector3.Zero, Vector3.One)),
            new(new Aabb(Vector3.Zero, Vector3.One)),
            new(new Aabb(Vector3.Zero, Vector3.One)),
            new(new Aabb(Vector3.Zero, Vector3.One))
        ];
        _sceneManager.SetLevel(chunks);

        Player.Instance.EnterScene();

        _physicsUpdateTimer = new Timer(PhysicsSystem.PHYSICS_TICK_SECONDS * 1000.0);
        _physicsUpdateTimer.Elapsed += PhysicsUpdate;
        _physicsUpdateTimer.Start();
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

        if (e.Time > 0.2f || _isPaused)
        {
            return;
        }

        //useful for debugging
        _sceneManager.PhysicsUpdate();

        _sceneManager.Update(
            new SceneManager.FrameParameters(
                window.FramebufferSize.X,
                window.FramebufferSize.Y,
                e.Time));

        window.SwapBuffers();
    }

    internal void PhysicsUpdate(object? sender, ElapsedEventArgs e)
    {
        // if (_isPaused)
        // {
        //     return;
        // }
        //
        // _sceneManager.PhysicsUpdate();
    }

    internal void FocusChanged(FocusedChangedEventArgs e)
    {
        _isPaused = !e.IsFocused;
    }

    #region Input

    private Camera.CameraMovement _playerDirection;
    private Camera.CameraMovement _pendingContradictoryMovements;

    internal void OnKeyUp(KeyboardKeyEventArgs e)
    {
        _playerDirection |= _pendingContradictoryMovements;

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
                _pendingContradictoryMovements &= ~Camera.CameraMovement.Forward;
                break;
            }
            case Keys.S:
            case Keys.Down:
            {
                _playerDirection &= ~Camera.CameraMovement.Backward;
                _pendingContradictoryMovements &= ~Camera.CameraMovement.Backward;
                break;
            }
            case Keys.A:
            case Keys.Left:
            {
                _playerDirection &= ~Camera.CameraMovement.Left;
                _pendingContradictoryMovements &= ~Camera.CameraMovement.Left;
                break;
            }
            case Keys.D:
            case Keys.Right:
            {
                _playerDirection &= ~Camera.CameraMovement.Right;
                _pendingContradictoryMovements &= ~Camera.CameraMovement.Right;
                break;
            }
        }

        Player.Instance.InputMovementChanged(_playerDirection);
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
                    _pendingContradictoryMovements |= Camera.CameraMovement.Forward;
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
                    _pendingContradictoryMovements |= Camera.CameraMovement.Backward;
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
                    _pendingContradictoryMovements |= Camera.CameraMovement.Left;
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
                    _pendingContradictoryMovements |= Camera.CameraMovement.Right;
                    break;
                }

                _playerDirection |= Camera.CameraMovement.Right;
                break;
            }
        }

        Player.Instance.InputMovementChanged(_playerDirection);
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

        Player.Instance.Camera.InputLookDirectionChanged(xOffset, yOffset);
    }

    #endregion

    internal static void FramebufferResize(FramebufferResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    private static void GenerateRuntimeAssets()
    {
        byte[] rawData = new byte[2 * 2 * 4];
        for (int i = 0; i < rawData.Length; i++)
        {
            rawData[i] = 0xff;
        }

        var texFormat = new Texture.TextureFormat
        {
            Width = 2,
            Height = 2,
            Format = PixelFormat.Rgba,
            InternalFormat = PixelInternalFormat.Rgba
        };

        //generate blank, default texture
        ResourceManager.Instance.CreateTexture(
            "default_texture.rgba.raw",
            rawData,
            texFormat
        );
    }
}