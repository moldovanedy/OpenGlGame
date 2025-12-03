using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.PhysicsEngine;

namespace OpenGlGame.SceneSystem;

public class SceneManager
{
    public class WorldChunk
    {
        public GameObject[] GameObjects { get; }
        public Aabb AaBoundingBox { get; }

        public WorldChunk(GameObject[] gameObjects, Aabb aaBoundingBox)
        {
            GameObjects = gameObjects;
            AaBoundingBox = aaBoundingBox;
        }
    }

    public record FrameParameters(int FramebufferWidth, int FramebufferHeight, double DeltaTime);

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

        Player.Instance.Update(frameParams.DeltaTime);

        CheckChunkLoading(Player.Instance.Camera.Position);
        _systemRenderer.UpdateViewProjectionMatrix(Player.Instance.Camera.GetViewProjectionMatrix());

        foreach (WorldChunk chunk in _currentChunks)
        {
            foreach (GameObject gameObject in chunk.GameObjects)
            {
                gameObject.Update(frameParams.DeltaTime);
            }
        }

        _systemRenderer.Render(frameParams.FramebufferWidth, frameParams.FramebufferHeight);
    }

    public void PhysicsUpdate()
    {
        PhysicsSystem.Instance.PhysicsUpdate();
        Player.Instance.PhysicsUpdate();
    }

    private void LoadChunk(WorldChunk chunk)
    {
        _currentChunks.Add(chunk);
        foreach (GameObject gameObject in chunk.GameObjects)
        {
            gameObject.SystemRenderer = _systemRenderer;
            gameObject.EnterScene();
        }

        PhysicsSystem.Instance.RecreatePhysicsComponentsLists(_currentChunks);
    }

    private void UnloadChunk(WorldChunk chunk)
    {
        _currentChunks.Remove(chunk);
        foreach (GameObject gameObject in chunk.GameObjects)
        {
            gameObject.ExitScene();
            gameObject.SystemRenderer = null;
        }

        PhysicsSystem.Instance.RecreatePhysicsComponentsLists(_currentChunks);
    }

    private void CheckChunkLoading(Vector3 cameraPos)
    {
        if (!Aabb.IsPointInside(_levelChunks[_currentChunkIndex].AaBoundingBox, cameraPos))
        {
            if (_levelChunks.Count == 1)
            {
                throw new Exception("Unexpected condition: outside of the only chunk");
            }

            if (_currentChunkIndex != 0
                && !Aabb.IsPointInside(_levelChunks[_currentChunkIndex - 1].AaBoundingBox, cameraPos))
            {
                _currentChunkIndex--;
                if (_currentChunkIndex - 2 < _levelChunks.Count)
                {
                    LoadChunk(_levelChunks[_currentChunkIndex - 2]);
                }

                UnloadChunk(_levelChunks[_currentChunkIndex + 1]);
            }
            else if (_currentChunkIndex < _levelChunks.Count - 1
                     && !Aabb.IsPointInside(_levelChunks[_currentChunkIndex + 1].AaBoundingBox, cameraPos))
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
}