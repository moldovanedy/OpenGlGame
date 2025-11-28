using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem.Components;

namespace OpenGlGame.SceneSystem;

public class GameObject
{
    public Renderer? SystemRenderer { get; set; }

    private readonly Dictionary<string, IGameObjectComponent> _components = [];

    public void EnterScene()
    {
        foreach (IGameObjectComponent component in _components.Values)
        {
            component.EnterScene();
        }
    }

    public void ExitScene()
    {
        foreach (IGameObjectComponent component in _components.Values)
        {
            component.ExitScene();
        }
    }

    public void Update(double deltaTime)
    {
        foreach (IGameObjectComponent component in _components.Values)
        {
            component.Update(deltaTime);
        }
    }

    public void AddComponent(IGameObjectComponent component)
    {
        _components.TryAdd(component.Name, component);
        component.Owner = this;
    }

    public IGameObjectComponent? GetComponent(string name)
    {
        return _components.GetValueOrDefault(name);
    }

    public bool RemoveComponent(string name)
    {
        if (_components.TryGetValue(name, out IGameObjectComponent? value))
        {
            bool result = _components.Remove(name);
            value.Owner = null;
            return result;
        }

        return false;
    }
}