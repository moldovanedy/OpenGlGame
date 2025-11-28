using System;
using System.Collections.Generic;

namespace OpenGlGame.ECS;

internal interface IComponentStore
{
    void Remove(int entityId);
}

internal class ComponentStore<T> : IComponentStore where T : struct
{
    private readonly List<T> _dense = [];
    private readonly List<int> _denseEntity = [];

    //redundancy for fast access
    private readonly Dictionary<int, int> _sparse = [];

    public int Count => _dense.Count;

    public void Add(int entityId, T component)
    {
        if (_sparse.ContainsKey(entityId))
        {
            throw new Exception($"Entity {entityId} already has component {typeof(T).Name}");
        }

        int index = _dense.Count;
        _sparse[entityId] = index;
        _dense.Add(component);
        _denseEntity.Add(entityId);
    }

    public T Get(int entityId)
    {
        return _dense[_sparse[entityId]];
    }

    public bool Has(int entityId)
    {
        return _sparse.ContainsKey(entityId);
    }

    public void Remove(int entityId)
    {
        if (!_sparse.TryGetValue(entityId, out int index))
        {
            return;
        }

        int lastIndex = _dense.Count - 1;

        //swap-remove to keep arrays dense
        _dense[index] = _dense[lastIndex];
        _denseEntity[index] = _denseEntity[lastIndex];

        _sparse[_denseEntity[index]] = index;

        _dense.RemoveAt(lastIndex);
        _denseEntity.RemoveAt(lastIndex);
        _sparse.Remove(entityId);
    }

    public IEnumerable<(int EntityId, T Component)> View()
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        for (int i = 0; i < _dense.Count; i++)
        {
            yield return (_denseEntity[i], _dense[i]);
        }
    }
}