using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenGlGame.Rendering;

namespace OpenGlGame.SceneSystem;

public class SceneManager
{
    public class WorldChunk
    {
        public GameObject[] GameObjects { get; }
        public Vector3 Center { get; }
        public Vector3 Extents { get; }

        public WorldChunk(GameObject[] gameObjects, Vector3 center, Vector3 extents)
        {
            GameObjects = gameObjects;
            Center = center;
            Extents = extents;
        }
    }

    public record FrameParameters(int FramebufferWidth, int FramebufferHeight, double DeltaTime);


    public Camera PlayerCamera { get; private set; } = new();

    private List<WorldChunk> _levelChunks = [];
    private readonly Renderer _systemRenderer = new();

    private readonly List<WorldChunk> _currentChunks = [];
    private int _currentChunkIndex;

    public void SetLevel(List<WorldChunk> allChunks)
    {
        if (allChunks.Count < 5)
        {
            throw new ArgumentException("The number of chunks must be at least 5.");
        }

        _levelChunks = allChunks;
        for (int i = 0; i < 3; i++)
        {
            LoadChunk(allChunks[i]);
        }
    }

    public void Update(FrameParameters frameParams)
    {
        switch (_levelChunks.Count)
        {
            case 0:
                return;
            case < 5:
                throw new Exception("Not enough level chunks");
        }

        PlayerCamera.Update(frameParams.DeltaTime);

        CheckChunkLoading(PlayerCamera.Position);
        _systemRenderer.UpdateViewProjectionMatrix(PlayerCamera.GetViewProjectionMatrix());

        foreach (WorldChunk chunk in _currentChunks)
        {
            foreach (GameObject gameObject in chunk.GameObjects)
            {
                gameObject.Update(frameParams.DeltaTime);
            }
        }

        _systemRenderer.Render(frameParams.FramebufferWidth, frameParams.FramebufferHeight);
    }

    private void LoadChunk(WorldChunk chunk)
    {
        _currentChunks.Add(chunk);
        foreach (GameObject gameObject in chunk.GameObjects)
        {
            gameObject.SystemRenderer = _systemRenderer;
            gameObject.EnterScene();
        }
    }

    private void UnloadChunk(WorldChunk chunk)
    {
        _currentChunks.Remove(chunk);
        foreach (GameObject gameObject in chunk.GameObjects)
        {
            gameObject.ExitScene();
            gameObject.SystemRenderer = null;
        }
    }

    private void CheckChunkLoading(Vector3 cameraPos)
    {
        if (
            !IsPointInsideAabb(
                cameraPos,
                _levelChunks[_currentChunkIndex].Center,
                _levelChunks[_currentChunkIndex].Extents))
        {
            if (_levelChunks.Count == 1)
            {
                throw new Exception("Unexpected condition: outside of the only chunk");
            }

            if (_currentChunkIndex != 0
                && IsPointInsideAabb(
                    cameraPos,
                    _levelChunks[_currentChunkIndex - 1].Center,
                    _levelChunks[_currentChunkIndex].Extents))
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
                         cameraPos,
                         _levelChunks[_currentChunkIndex + 1].Center,
                         _levelChunks[_currentChunkIndex].Extents))
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