using OpenGlGame.SceneSystem;

namespace OpenGlGame.Levels;

public interface ILevelData
{
    public GameObject[] LoadAndGetGameObjects();
}