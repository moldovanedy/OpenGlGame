namespace OpenGlGame;

internal static class Program
{
    private static void Main()
    {
        using var window = new Window(false, 800, 600);
        window.Run();
    }
}