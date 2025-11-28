using System;
using System.Collections.Generic;
using System.Numerics;
using OpenGlGame.ECS.Components;

namespace OpenGlGame.ECS.Systems;

public class WorldManagerSystem
{
    public class WorldChunk
    {
        public static Vector3 Extents { get; } = new(50, 25, 50);

        public object[][] StoredEntities { get; }
        public Vector3 Center { get; }

        internal List<int> RuntimeEntities { get; set; } = [];

        public WorldChunk(object[][] storedEntities, Vector3 center)
        {
            StoredEntities = storedEntities;
            Center = center;
        }
    }

    private readonly SceneRegistry _registry;

    private List<WorldChunk>? _levelChunks;
    private int _currentChunkIndex;

    public WorldManagerSystem(SceneRegistry registry)
    {
        _registry = registry;
    }


    public void SetLevel(List<WorldChunk> allChunks)
    {
        _levelChunks = allChunks;
        LoadChunk(allChunks[0]);
    }

    public void Update()
    {
        if (_levelChunks == null)
        {
            return;
        }

        if (_levelChunks.Count < 5)
        {
            throw new Exception("Not enough level chunks");
        }

        if (
            !IsPointInsideAabb(
                CameraSystem.Instance.PlayerPosition,
                _levelChunks[_currentChunkIndex].Center,
                WorldChunk.Extents))
        {
            if (_levelChunks.Count == 1)
            {
                throw new Exception("Unexpected condition: outside of the only chunk");
            }

            if (_currentChunkIndex != 0
                && IsPointInsideAabb(
                    CameraSystem.Instance.PlayerPosition,
                    _levelChunks[_currentChunkIndex - 1].Center,
                    WorldChunk.Extents))
            {
                _currentChunkIndex--;
                if (_currentChunkIndex - 2 < _levelChunks.Count)
                {
                    LoadChunk(_levelChunks[_currentChunkIndex - 2]);
                }

                UnloadChunk(_levelChunks[_currentChunkIndex + 1]);
            }
            else if (_currentChunkIndex < _levelChunks.Count - 1
                     && IsPointInsideAabb(
                         CameraSystem.Instance.PlayerPosition,
                         _levelChunks[_currentChunkIndex + 1].Center,
                         WorldChunk.Extents))
            {
                _currentChunkIndex++;
                if (_currentChunkIndex + 2 < _levelChunks.Count)
                {
                    LoadChunk(_levelChunks[_currentChunkIndex + 2]);
                }

                UnloadChunk(_levelChunks[_currentChunkIndex - 1]);
            }
            else
            {
                throw new Exception("Unexpected condition: outside of adjacent chunks");
            }
        }
    }

    private void LoadChunk(WorldChunk chunk)
    {
        foreach (object[] entity in chunk.StoredEntities)
        {
            int id = _registry.CreateEntity();
            chunk.RuntimeEntities.Add(id);

            foreach (object component in entity)
            {
                switch (component)
                {
                    case Transform3D transform3D:
                        _registry.AddComponent(id, transform3D);
                        break;
                    case MeshRenderer meshRenderer:
                        _registry.AddComponent(id, meshRenderer);
                        break;
                }
            }
        }
    }

    private void UnloadChunk(WorldChunk chunk)
    {
        foreach (int id in chunk.RuntimeEntities)
        {
            _registry.DestroyEntity(id);
        }

        chunk.RuntimeEntities.Clear();
    }

    private static bool IsPointInsideAabb(Vector3 point, Vector3 cubeCenter, Vector3 cubeSize)
    {
        if (cubeCenter.X - cubeSize.X / 2.0 > point.X)
        {
            return false;
        }

        if (cubeCenter.X + cubeSize.X / 2.0 < point.X)
        {
            return false;
        }

        if (cubeCenter.Y - cubeSize.Y / 2.0 > point.Y)
        {
            return false;
        }

        if (cubeCenter.Y + cubeSize.Y / 2.0 < point.Y)
        {
            return false;
        }

        if (cubeCenter.Z - cubeSize.Z / 2.0 > point.Z)
        {
            return false;
        }

        if (cubeCenter.Z + cubeSize.Z / 2.0 < point.Z)
        {
            return false;
        }

        return true;
    }
}