using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Components;

namespace OpenGlGame.Levels.Level2;

public class Level2Callbacks
{
    public const float EM_LIGHT_AMBIENT_UPPER_LIMIT = 0.08f;
    public const float EM_LIGHT_AMBIENT_LOWER_LIMIT = 0.02f;

    public const float EM_LIGHT_DIFFUSE_UPPER_LIMIT = 0.7f;
    public const float EM_LIGHT_DIFFUSE_LOWER_LIMIT = 0.175f;

    public const float EM_LIGHT_SPECULAR_UPPER_LIMIT = 0.3f;
    public const float EM_LIGHT_SPECULAR_LOWER_LIMIT = 0.075f;

    private const float STOBE_TIME = 1.0f;

    private PointLight[] _emergencyLights = [];

    private double _emLightT;
    private bool _isEmLightIncreasing;

    internal void Update(double deltaTime)
    {
        if (_emLightT <= 0)
        {
            _isEmLightIncreasing = true;
        }
        else if (_emLightT >= STOBE_TIME)
        {
            _isEmLightIncreasing = false;
        }

        if (_isEmLightIncreasing)
        {
            _emLightT += deltaTime;
        }
        else
        {
            _emLightT -= deltaTime;
        }

        foreach (PointLight emergencyLight in _emergencyLights)
        {
            emergencyLight.LightData = new Renderer.PointLightData(
                emergencyLight.LightData.Position,
                emergencyLight.LightData.LightColor,
                float.Lerp(EM_LIGHT_AMBIENT_LOWER_LIMIT, EM_LIGHT_AMBIENT_UPPER_LIMIT, (float)_emLightT / STOBE_TIME),
                float.Lerp(EM_LIGHT_DIFFUSE_LOWER_LIMIT, EM_LIGHT_DIFFUSE_UPPER_LIMIT, (float)_emLightT / STOBE_TIME),
                float.Lerp(EM_LIGHT_SPECULAR_LOWER_LIMIT, EM_LIGHT_SPECULAR_UPPER_LIMIT, (float)_emLightT / STOBE_TIME),
                emergencyLight.LightData.ConstantAttenuation,
                emergencyLight.LightData.LinearAttenuation,
                emergencyLight.LightData.QuadraticAttenuation);
        }
    }

    internal void SetLights(GameObject[] emergencyLights)
    {
        List<PointLight> emergency = [];
        foreach (GameObject emergencyLight in emergencyLights)
        {
            if (emergencyLight.GetComponent(nameof(PointLight)) is PointLight pointLight)
            {
                emergency.Add(pointLight);
            }
        }

        _emergencyLights = emergency.ToArray();
    }
}