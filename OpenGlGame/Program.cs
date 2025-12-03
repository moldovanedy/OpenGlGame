using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace OpenGlGame;

internal static class Program
{
    private static Application _app = null!;
    private static GameWindow _window = null!;

    private static void Main()
    {
        _app = new Application();

        _window = new GameWindow(
            GameWindowSettings.Default,
            new NativeWindowSettings
            {
                WindowState = WindowState.Fullscreen,
                ClientSize = (1280, 720),
                MinimumClientSize = (640, 360),
                Title = "OpenGL game",
                NumberOfSamples = 4,
                Vsync = VSyncMode.On
            });
        _window.CursorState = CursorState.Grabbed;
        _app.SetWindowRef(_window);

        _window.Load += _app.Load;
        _window.RenderFrame += Update;
        _window.FramebufferResize += Application.FramebufferResize;
        _window.FocusedChanged += _app.FocusChanged;

        _window.KeyDown += _app.OnKeyDown;
        _window.KeyUp += _app.OnKeyUp;
        _window.MouseMove += _app.OnMouseMove;

        _window.Run();

        _window.Load -= _app.Load;
        _window.RenderFrame -= Update;
        _window.FramebufferResize -= Application.FramebufferResize;
        _window.FocusedChanged -= _app.FocusChanged;

        _window.KeyDown -= _app.OnKeyDown;
        _window.KeyUp -= _app.OnKeyUp;
        _window.MouseMove -= _app.OnMouseMove;

        _app.Dispose();
    }


    private static void Update(FrameEventArgs e)
    {
        _app.Update(e, _window);
    }
}