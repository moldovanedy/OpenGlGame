using OpenGlGame.Rendering;

namespace OpenGlGame.SceneSystem.Components;

public class PointLight : IGameObjectComponent
{
    public string Name => nameof(PointLight);

    public GameObject? Owner { get; set; }

    public Renderer.PointLightData LightData { get; set; }

    public void EnterScene()
    {
    }

    public void ExitScene()
    {
    }

    public void Update(double deltaTime)
    {
    }
}