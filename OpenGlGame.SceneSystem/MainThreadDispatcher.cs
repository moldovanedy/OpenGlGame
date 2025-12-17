using System;
using System.Collections.Concurrent;

namespace OpenGlGame.SceneSystem;

public static class MainThreadDispatcher
{
    private static readonly ConcurrentQueue<Action> Queue = new();

    public static void Enqueue(Action action)
    {
        Queue.Enqueue(action);
    }

    public static void ExecutePending()
    {
        while (Queue.TryDequeue(out Action? action))
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex);
            }
        }
    }
}