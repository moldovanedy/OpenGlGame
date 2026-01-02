using System;
using System.Timers;
using OpenGlGame.Levels;
using OpenGlGame.Levels.Level1;
using OpenGlGame.Levels.Level2;
using OpenGlGame.Levels.Level3;
using OpenGlGame.Levels.Ui;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenGlGame;

public class Application : IDisposable
{
    public static int CurrentLevelIndex { get; private set; } = -1;

    private GameWindow? _windowRef;
    private Timer? _physicsUpdateTimer;

    private readonly ILevelData[] _levels =
    [
        new MainMenu(),
        new Level1Data(),
        new Level2Data(),
        new Level3Data()
    ];

    //Wayland does not allow a window to control its own window mode...
    private bool _isWayland;
    private bool _isFullscreenToggleRequested;
    private bool _isFullscreen;

    private int _windowedWidth = (int)(600f * Renderer.ASPECT_RATIO);
    private int _windowedHeight = 600;
    private int _fbWidth = (int)(600f * Renderer.ASPECT_RATIO);
    private int _fbHeight = 600;

    private bool _isPaused;

    public void Dispose()
    {
        Player.Instance.ExitScene();
        _defaultTexture = null;

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
        SetLevel(0);

        Player.Instance.EnterScene();
        Player.Instance.NextLevelRequestedEvent += () =>
        {
            if (CurrentLevelIndex >= _levels.Length - 1)
            {
                Console.WriteLine("ERROR: Can't advance past the last level");
                return;
            }

            SetLevel(++CurrentLevelIndex);
        };
        Player.Instance.PlayerDeadEvent += () =>
        {
            GameOverScreen gameOverScreen = new();
            gameOverScreen.RetryRequestedEvent += () =>
            {
                Player.Instance.ExitScene();
                Player.Instance.EnterScene();
                SetLevel(1);
            };

            GameObject[] gameOverUiObj = gameOverScreen.LoadAndGetGameObjects();
            foreach (GameObject uiObj in gameOverUiObj)
            {
                SceneManager.Instance.AddObject(uiObj);
            }

            if (_windowRef != null)
            {
                _windowRef.CursorState = CursorState.Normal;
            }
        };
        Player.Instance.GameCompleteEvent += () =>
        {
            GameCompleteScreen gameCompleteScreen = new();
            gameCompleteScreen.RetryRequestedEvent += () =>
            {
                Player.Instance.ExitScene();
                Player.Instance.EnterScene();
                SetLevel(1);
            };

            GameObject[] gameCompleteUiObj = gameCompleteScreen.LoadAndGetGameObjects();
            foreach (GameObject uiObj in gameCompleteUiObj)
            {
                SceneManager.Instance.AddObject(uiObj);
            }

            if (_windowRef != null)
            {
                _windowRef.CursorState = CursorState.Normal;
            }
        };

        _physicsUpdateTimer = new Timer(PhysicsSystem.PHYSICS_TICK_SECONDS * 1000.0);
        _physicsUpdateTimer.Elapsed += PhysicsUpdate;
        _physicsUpdateTimer.Start();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="levelIndex">Always level number. 0 for the main menu.</param>
    internal void SetLevel(int levelIndex)
    {
        GameObject[] combined;
        //actual levels
        if (levelIndex > 0)
        {
            GameObject[] levelObjects = _levels[levelIndex].LoadAndGetGameObjects();
            GameObject[] hudObjects = new Hud().LoadAndGetGameObjects();

            combined = new GameObject[levelObjects.Length + hudObjects.Length];
            Array.Copy(levelObjects, combined, levelObjects.Length);
            Array.Copy(hudObjects, 0, combined, levelObjects.Length, hudObjects.Length);

            if (_windowRef != null)
            {
                _windowRef.CursorState = CursorState.Grabbed;
            }
        }
        //main menu
        else
        {
            combined = _levels[levelIndex].LoadAndGetGameObjects();

            if (_windowRef != null)
            {
                _windowRef.CursorState = CursorState.Normal;
            }
        }

        SceneManager.Instance.SetLevel(combined);
        CurrentLevelIndex = levelIndex;
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

        _fbWidth = window.FramebufferSize.X;
        _fbHeight = window.FramebufferSize.Y;

        MainThreadDispatcher.ExecutePending();

        if (_isPaused)
        {
            return;
        }

        //useful for debugging
        // SceneManager.PhysicsUpdate();

        Renderer.RenderFrameData frameData = new(
            window.FramebufferSize.X,
            window.FramebufferSize.Y,
            Player.Instance.Camera.Position,
            e.Time,
            GetNormalizedMouseCoords(),
            _pressedMouseButton);

        SceneManager.Instance.Update(ref frameData);
        window.SwapBuffers();
    }

    internal void PhysicsUpdate(object? sender, ElapsedEventArgs e)
    {
        if (_isPaused)
        {
            return;
        }

        SceneManager.PhysicsUpdate();
    }

    internal void FocusChanged(FocusedChangedEventArgs e)
    {
        _isPaused = !e.IsFocused;
    }

    #region Input

    private float _lastMouseX;
    private float _lastMouseY;
    private bool _isFirstMouse = true;
    private MouseButton? _pressedMouseButton;

    private Camera.CameraMovement _playerDirection;
    private Camera.CameraMovement _pendingContradictoryMovements;

    internal void OnKeyUp(KeyboardKeyEventArgs e)
    {
        _playerDirection |= _pendingContradictoryMovements;

        switch (e.Key)
        {
            default:
                return;
            case Keys.Escape:
                Environment.Exit(0);
                break;
            case Keys.F11:
                _isFullscreenToggleRequested = true;
                break;
            //debug-only reload feature
#if DEBUG
            case Keys.F5:
                SetLevel(CurrentLevelIndex);
                break;
            case Keys.F12:
            {
                SetLevel(CurrentLevelIndex);
                if (Player.Instance.GetComponent(nameof(Transform3D)) is not Transform3D transform)
                {
                    break;
                }

                if (Player.Instance.GetComponent(nameof(RigidBody)) is not RigidBody rigidBody)
                {
                    break;
                }

                transform.Position = Player.SpawnPosition;
                rigidBody.ResetPositions(transform.Position);
            }
                break;
#endif
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
            case Keys.Space:
                Player.Instance.InputJumpChanged(false);
                break;
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
            case Keys.Space:
                Player.Instance.InputJumpChanged(true);
                break;
        }

        Player.Instance.InputMovementChanged(_playerDirection);
    }

    internal void OnMouseMove(MouseMoveEventArgs e)
    {
        if (_isPaused)
        {
            return;
        }

        if (_isFirstMouse)
        {
            _lastMouseX = e.X;
            _lastMouseY = e.Y;
            _isFirstMouse = false;
        }

        float xOffset = e.X - _lastMouseX;
        //Y-axis reversed
        float yOffset = _lastMouseY - e.Y;

        _lastMouseX = e.X;
        _lastMouseY = e.Y;

        if (CurrentLevelIndex != 0)
        {
            Player.Instance.Camera.InputLookDirectionChanged(xOffset, yOffset);
        }
    }

    internal void OnMouseDown(MouseButtonEventArgs e)
    {
        _pressedMouseButton = e.Button;
        if (e.Button == MouseButton.Left)
        {
            Player.Instance.InputInteractButtonChanged(true);
        }
    }

    internal void OnMouseUp(MouseButtonEventArgs e)
    {
        _pressedMouseButton = null;
        if (e.Button == MouseButton.Left)
        {
            Player.Instance.InputInteractButtonChanged(false);
        }
    }

    #endregion

    internal static void FramebufferResize(FramebufferResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    //this is needed so that the texture doesn't get disposed of by the fact that it's not used anywhere
    // ReSharper disable once NotAccessedField.Local
    private static Texture? _defaultTexture;

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
        _defaultTexture = ResourceManager.Instance.CreateTexture(
            "default_texture.rgba.raw",
            rawData,
            texFormat
        );
    }

    private Vector2 GetNormalizedMouseCoords()
    {
        //keep 16:9 aspect ratio
        float actualAspectRatio = _fbWidth / (float)_fbHeight;
        float normalizedX;
        float normalizedY;

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (actualAspectRatio > Renderer.ASPECT_RATIO)
        {
            float newWidth = _fbHeight * Renderer.ASPECT_RATIO;
            float offsetX = (_fbWidth - newWidth) / 2.0f;

            if (_lastMouseX < offsetX || _lastMouseX > newWidth + offsetX)
            {
                return new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            }

            normalizedX = float.Lerp(-1.0f, 1.0f, (_lastMouseX - offsetX) / (_fbWidth - offsetX * 2f));
            normalizedY = float.Lerp(1.0f, -1.0f, _lastMouseY / _fbHeight);
        }
        else
        {
            float newHeight = _fbWidth / Renderer.ASPECT_RATIO;
            float offsetY = (_fbHeight - newHeight) / 2.0f;

            if (_lastMouseY < offsetY || _lastMouseY > newHeight + offsetY)
            {
                return new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            }

            normalizedX = float.Lerp(-1.0f, 1.0f, _lastMouseX / _fbWidth);
            normalizedY = float.Lerp(1.0f, -1.0f, (_lastMouseY - offsetY) / (_fbHeight - offsetY * 2f));
        }

        return new Vector2(normalizedX, normalizedY);
    }
}