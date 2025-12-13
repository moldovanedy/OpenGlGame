using System.Collections.Generic;
using System.Linq;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;

namespace OpenGlGame.SceneSystem;

public class SceneManager
{
    private GameObject[] _currentGameObjects = [];
    private PointLight[] _cachedPointLights = [];
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

        // List<Renderer.PointLightData> pointLightsData = [];
        // foreach (GameObject gameObject in gameObjects)
        // {
        //     IGameObjectComponent? pointLight = gameObject.GetComponent(nameof(PointLight));
        //     if (pointLight is PointLight light)
        //     {
        //         pointLightsData.Add(light.LightData);
        //     }
        // }
        //
        // _systemRenderer.UpdatePointLights(pointLightsData.ToArray());

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
    }

    public void Update(ref Renderer.RenderFrameData frameData)
    {
        if (_currentGameObjects.Length == 0)
        {
            return;
        }

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

    public void PhysicsUpdate()
    {
        PhysicsSystem.Instance.PhysicsUpdate();
        Player.Instance.PhysicsUpdate();
    }
}