namespace OpenGlGame.SceneSystem.Components;

public interface IGameObjectComponent
{
    string Name { get; }

    GameObject? Owner { get; set; }

    void EnterScene();
    void ExitScene();
    void Update(double deltaTime);
}