using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem.Components;

namespace OpenGlGame.SceneSystem.Prefabs.Lights;

public class Light02 : GameObject
{
    public Light02(Transform3D transform, Renderer.PointLightData lightData)
    {
        AddComponent(transform);

        AddComponent(
            new PointLight
            {
                LightData = lightData
            });
    }
}