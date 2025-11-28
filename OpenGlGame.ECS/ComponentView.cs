using System.Collections.Generic;
using OpenGlGame.ECS.Components;

namespace OpenGlGame.ECS;

public readonly struct ComponentView<T1> where T1 : struct
{
    private readonly ComponentStore<T1> _store1;

    internal ComponentView(ComponentStore<T1> store1)
    {
        _store1 = store1;
    }

    public IEnumerable<(int entity, T1 component1)> Each()
    {
        foreach ((int id, T1 comp) in _store1.View())
        {
            yield return (id, comp);
        }
    }
}

public readonly struct ComponentView<T1, T2> where T1 : struct where T2 : struct
{
    private readonly ComponentStore<T1> _store1;
    private readonly ComponentStore<T2> _store2;

    internal ComponentView(ComponentStore<T1> store1, ComponentStore<T2> store2)
    {
        _store1 = store1;
        _store2 = store2;
    }

    public IEnumerable<(int entity, T1 component1, T2 component2)> Each()
    {
        int smallestStoreIndex = _store1.Count < _store2.Count ? 1 : 2;

        if (smallestStoreIndex == 1)
        {
            foreach ((int id, T1 comp1) in _store1.View())
            {
                if (!_store2.Has(id))
                {
                    continue;
                }

                yield return (id, comp1, _store2.Get(id));
            }
        }
        else
        {
            foreach ((int id, T2 comp2) in _store2.View())
            {
                if (!_store1.Has(id))
                {
                    continue;
                }

                yield return (id, _store1.Get(id), comp2);
            }
        }
    }
}

public readonly struct ComponentView<T1, T2, T3>
    where T1 : struct
    where T2 : struct
    where T3 : struct
{
    private readonly ComponentStore<T1> _store1;
    private readonly ComponentStore<T2> _store2;
    private readonly ComponentStore<T3> _store3;

    internal ComponentView(ComponentStore<T1> store1, ComponentStore<T2> store2, ComponentStore<T3> store3)
    {
        _store1 = store1;
        _store2 = store2;
        _store3 = store3;
    }

    public IEnumerable<(int entity, T1 component1, T2 component2, T3 component3)> Each()
    {
        int smallestStoreIndex =
            _store1.Count < _store2.Count
                ? 1
                : _store2.Count < _store3.Count
                    ? 2
                    : 3;

        switch (smallestStoreIndex)
        {
            case 1:
            {
                foreach ((int id, T1 comp1) in _store1.View())
                {
                    if (!_store2.Has(id) || !_store3.Has(id))
                    {
                        continue;
                    }

                    yield return (
                        id,
                        comp1,
                        _store2.Get(id),
                        _store3.Get(id));
                }

                break;
            }
            case 2:
            {
                foreach ((int id, T2 comp2) in _store2.View())
                {
                    if (!_store1.Has(id) || !_store3.Has(id))
                    {
                        continue;
                    }

                    yield return (
                        id,
                        _store1.Get(id),
                        comp2,
                        _store3.Get(id));
                }

                break;
            }
            case 3:
            {
                foreach ((int id, T3 comp3) in _store3.View())
                {
                    if (!_store1.Has(id) || !_store2.Has(id))
                    {
                        continue;
                    }

                    yield return (
                        id,
                        _store1.Get(id),
                        _store2.Get(id),
                        comp3);
                }

                break;
            }
        }
    }
}

public readonly struct ComponentView<T1, T2, T3, T4>
    where T1 : struct
    where T2 : struct
    where T3 : struct
    where T4 : struct
{
    private readonly ComponentStore<T1> _store1;
    private readonly ComponentStore<T2> _store2;
    private readonly ComponentStore<T3> _store3;
    private readonly ComponentStore<T4> _store4;

    internal ComponentView(
        ComponentStore<T1> store1,
        ComponentStore<T2> store2,
        ComponentStore<T3> store3,
        ComponentStore<T4> store4)
    {
        _store1 = store1;
        _store2 = store2;
        _store3 = store3;
        _store4 = store4;
    }

    public IEnumerable<(int entity, T1 component1, T2 component2, T3 component3, T4 component4)>
        Each()
    {
        int smallestStoreIndex =
            _store1.Count < _store2.Count
                ? 1
                : _store2.Count < _store3.Count
                    ? 2
                    : _store3.Count < _store4.Count
                        ? 3
                        : 4;

        switch (smallestStoreIndex)
        {
            case 1:
            {
                foreach ((int id, T1 comp1) in _store1.View())
                {
                    if (!_store2.Has(id) || !_store3.Has(id) || !_store4.Has(id))
                    {
                        continue;
                    }

                    yield return (
                        id,
                        comp1,
                        _store2.Get(id),
                        _store3.Get(id),
                        _store4.Get(id));
                }

                break;
            }
            case 2:
            {
                foreach ((int id, T2 comp2) in _store2.View())
                {
                    if (!_store1.Has(id) || !_store3.Has(id) || !_store4.Has(id))
                    {
                        continue;
                    }

                    yield return (
                        id,
                        _store1.Get(id),
                        comp2,
                        _store3.Get(id),
                        _store4.Get(id));
                }

                break;
            }
            case 3:
            {
                foreach ((int id, T3 comp3) in _store3.View())
                {
                    if (!_store1.Has(id) || !_store2.Has(id) || !_store4.Has(id))
                    {
                        continue;
                    }

                    yield return (
                        id,
                        _store1.Get(id),
                        _store2.Get(id),
                        comp3,
                        _store4.Get(id));
                }

                break;
            }
            case 4:
            {
                foreach ((int id, T4 comp4) in _store4.View())
                {
                    if (!_store1.Has(id) || !_store2.Has(id) || !_store3.Has(id))
                    {
                        continue;
                    }

                    yield return (
                        id,
                        _store1.Get(id),
                        _store2.Get(id),
                        _store3.Get(id),
                        comp4);
                }

                break;
            }
        }
    }
}