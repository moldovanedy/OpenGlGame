using System.Collections.Generic;
using System.Linq;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenGlGame.SceneSystem;

public class SceneManager
{
    public static SceneManager Instance
    {
        get
        {
            _instance ??= new SceneManager();
            return _instance;
        }
    }

    private static SceneManager? _instance;

    public Vector2 NormalizedMousePosition { get; private set; }
    public MouseButton? CurrentlyPressedMouseButton { get; private set; }

    private readonly List<GameObject> _currentGameObjects = [];
    private PointLight[] _cachedPointLights = [];
    private readonly Renderer _systemRenderer = new();

    private SceneManager()
    {
    }

    public void SetLevel(GameObject[] gameObjects)
    {
        MainThreadDispatcher.Enqueue(() =>
        {
            foreach (GameObject gameObject in _currentGameObjects)
            {
                gameObject.ExitScene();
                gameObject.SystemRenderer = null;
            }

            _currentGameObjects.Clear();
            _currentGameObjects.AddRange(gameObjects);

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.EnterScene();
                gameObject.SystemRenderer = _systemRenderer;
            }

            List<PointLight> pointLights = [];
            foreach (GameObject gameObject in gameObjects)
            {
                IGameObjectComponent? pointLight = gameObject.GetComponent(nameof(PointLight));
                if (pointLight is PointLight light)
                {
                    pointLights.Add(light);
                }
            }

            _cachedPointLights = pointLights.ToArray();

            PhysicsSystem.Instance.RecreatePhysicsComponentsLists(gameObjects);
            PhysicsSystem.Instance.RecreateInteractablesList(gameObjects);
        });
    }

    public void Update(ref Renderer.RenderFrameData frameData)
    {
        if (_currentGameObjects.Count == 0)
        {
            return;
        }

        NormalizedMousePosition = frameData.NormalizedMousePosition;
        CurrentlyPressedMouseButton = frameData.CurrentlyPressedMouseButton;

        List<Renderer.PointLightData> pointLightsData = [];
        pointLightsData.AddRange(_cachedPointLights.Select(pointLight => pointLight.LightData));
        _systemRenderer.UpdatePointLights(pointLightsData.ToArray());

        Player.Instance.Update(frameData.DeltaTime);
        _systemRenderer.UpdateViewProjectionMatrix(Player.Instance.Camera.GetViewProjectionMatrix());

        foreach (GameObject gameObject in _currentGameObjects)
        {
            gameObject.Update(frameData.DeltaTime);
        }

        _systemRenderer.Render(ref frameData);
    }

    public static void PhysicsUpdate()
    {
        PhysicsSystem.Instance.PhysicsUpdate();
        Player.Instance.PhysicsUpdate();
    }
}