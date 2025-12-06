using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.PhysicsEngine;

namespace OpenGlGame.SceneSystem;

public class SceneManager
{
    public record FrameParameters(int FramebufferWidth, int FramebufferHeight, double DeltaTime);

    private GameObject[] _currentGameObjects = [];
    private readonly Renderer _systemRenderer = new();

    public void SetLevel(GameObject[] gameObjects)
    {
        foreach (GameObject gameObject in _currentGameObjects)
        {
            gameObject.ExitScene();
            gameObject.SystemRenderer = null;
        }

        _currentGameObjects = gameObjects;
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.EnterScene();
            gameObject.SystemRenderer = _systemRenderer;
        }

        PhysicsSystem.Instance.RecreatePhysicsComponentsLists(gameObjects);
    }

    public void Update(FrameParameters frameParams)
    {
        if (_currentGameObjects.Length == 0)
        {
            return;
        }

        Player.Instance.Update(frameParams.DeltaTime);
        _systemRenderer.UpdateViewProjectionMatrix(Player.Instance.Camera.GetViewProjectionMatrix());

        foreach (GameObject gameObject in _currentGameObjects)
        {
            gameObject.Update(frameParams.DeltaTime);
        }

        _systemRenderer.Render(frameParams.FramebufferWidth, frameParams.FramebufferHeight);
    }

    public void PhysicsUpdate()
    {
        PhysicsSystem.Instance.PhysicsUpdate();
        Player.Instance.PhysicsUpdate();
    }
}