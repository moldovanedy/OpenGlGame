using System;
using System.Collections.Generic;

namespace OpenGlGame.ECS;

public class SceneRegistry
{
    private readonly Stack<int> _freeIds = [];
    private readonly Dictionary<Type, IComponentStore> _stores = new();
    private int _nextEntityId;

    public int CreateEntity()
    {
        return _freeIds.Count == 0 ? _nextEntityId++ : _freeIds.Pop();
    }

    public void DestroyEntity(int eId)
    {
        foreach (IComponentStore store in _stores.Values)
        {
            store.Remove(eId);
        }

        _freeIds.Push(eId);
    }

    public void AddComponent<T>(int id, T component) where T : struct
    {
        GetOrCreateStore<T>().Add(id, component);
    }

    public T GetComponent<T>(int id) where T : struct
    {
        return GetOrCreateStore<T>().Get(id);
    }

    public bool HasComponent<T>(int id) where T : struct
    {
        return GetOrCreateStore<T>().Has(id);
    }

    public void RemoveComponent<T>(int id) where T : struct
    {
        GetOrCreateStore<T>().Remove(id);
    }

    #region Views

    public ComponentView<T1> View<T1>() where T1 : struct
    {
        return new ComponentView<T1>(GetOrCreateStore<T1>());
    }

    public ComponentView<T1, T2> View<T1, T2>() where T1 : struct where T2 : struct
    {
        return new ComponentView<T1, T2>(GetOrCreateStore<T1>(), GetOrCreateStore<T2>());
    }

    public ComponentView<T1, T2, T3> View<T1, T2, T3>()
        where T1 : struct
        where T2 : struct
        where T3 : struct
    {
        return new ComponentView<T1, T2, T3>(
            GetOrCreateStore<T1>(),
            GetOrCreateStore<T2>(),
            GetOrCreateStore<T3>());
    }

    public ComponentView<T1, T2, T3, T4> View<T1, T2, T3, T4>()
        where T1 : struct
        where T2 : struct
        where T3 : struct
        where T4 : struct
    {
        return new ComponentView<T1, T2, T3, T4>(
            GetOrCreateStore<T1>(),
            GetOrCreateStore<T2>(),
            GetOrCreateStore<T3>(),
            GetOrCreateStore<T4>());
    }

    #endregion


    private ComponentStore<T> GetOrCreateStore<T>() where T : struct
    {
        Type t = typeof(T);
        if (!_stores.TryGetValue(t, out IComponentStore? s))
        {
            s = new ComponentStore<T>();
            _stores[t] = s;
        }

        return (ComponentStore<T>)s;
    }
}