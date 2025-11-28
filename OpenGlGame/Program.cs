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
                WindowState = WindowState.Normal,
                ClientSize = (1280, 720),
                MinimumClientSize = (640, 360),
                Title = "OpenGL game",
                NumberOfSamples = 4,
                Vsync = VSyncMode.On
            });

        _window.Load += _app.Load;
        _window.RenderFrame += Update;
        _window.FramebufferResize += Application.FramebufferResize;
        _window.Run();

        _window.Load -= _app.Load;
        _window.RenderFrame -= Update;
        _window.FramebufferResize -= Application.FramebufferResize;

        _app.Dispose();
    }

    private static void Update(FrameEventArgs e)
    {
        _app.Update(e, _window);
    }
}