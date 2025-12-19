using System;
using System.Collections.Generic;
using OpenGlGame.Rendering;
using OpenGlGame.SceneSystem;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Mathematics;

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
    private double _emLightTimer;
    private bool _isEmLightIncreasing;

    public const float LASER_X1_UPPER_LIMIT = -18f;
    public const float LASER_X1_LOWER_LIMIT = -10f;

    public const float LASER_X2_UPPER_LIMIT = -12f;
    public const float LASER_X2_LOWER_LIMIT = -20f;

    private const float LASER_TIME = 2.0f;
    private GameObject[] _firstMovingLaserObjects = [];
    private GameObject[] _secondMovingLaserObjects = [];
    private double _laserTimer;
    private bool _isLaserIncreasing;

    internal void Update(double deltaTime)
    {
        if (_emLightTimer <= 0)
        {
            _isEmLightIncreasing = true;
        }
        else if (_emLightTimer >= STOBE_TIME)
        {
            _isEmLightIncreasing = false;
        }

        if (_isEmLightIncreasing)
        {
            _emLightTimer += deltaTime;
        }
        else
        {
            _emLightTimer -= deltaTime;
        }

        foreach (PointLight emergencyLight in _emergencyLights)
        {
            emergencyLight.LightData = new Renderer.PointLightData(
                emergencyLight.LightData.Position,
                emergencyLight.LightData.LightColor,
                float.Lerp(
                    EM_LIGHT_AMBIENT_LOWER_LIMIT,
                    EM_LIGHT_AMBIENT_UPPER_LIMIT,
                    (float)_emLightTimer / STOBE_TIME),
                float.Lerp(
                    EM_LIGHT_DIFFUSE_LOWER_LIMIT,
                    EM_LIGHT_DIFFUSE_UPPER_LIMIT,
                    (float)_emLightTimer / STOBE_TIME),
                float.Lerp(
                    EM_LIGHT_SPECULAR_LOWER_LIMIT,
                    EM_LIGHT_SPECULAR_UPPER_LIMIT,
                    (float)_emLightTimer / STOBE_TIME),
                emergencyLight.LightData.ConstantAttenuation,
                emergencyLight.LightData.LinearAttenuation,
                emergencyLight.LightData.QuadraticAttenuation);
        }


        if (_laserTimer <= 0)
        {
            _isLaserIncreasing = true;
        }
        else if (_laserTimer >= LASER_TIME)
        {
            _isLaserIncreasing = false;
        }

        if (_isLaserIncreasing)
        {
            _laserTimer += deltaTime;
        }
        else
        {
            _laserTimer -= deltaTime;
        }

        foreach (GameObject laserObject in _firstMovingLaserObjects)
        {
            if (laserObject.GetComponent(nameof(Transform3D)) is not Transform3D transform)
            {
                continue;
            }

            transform.Position = new Vector3(
                float.Lerp(
                    LASER_X1_LOWER_LIMIT,
                    LASER_X1_UPPER_LIMIT,
                    EaseCubicInOut((float)_laserTimer / LASER_TIME)),
                transform.Position.Y,
                transform.Position.Z);
        }

        foreach (GameObject laserObject in _secondMovingLaserObjects)
        {
            if (laserObject.GetComponent(nameof(Transform3D)) is not Transform3D transform)
            {
                continue;
            }

            transform.Position = new Vector3(
                float.Lerp(
                    LASER_X2_LOWER_LIMIT,
                    LASER_X2_UPPER_LIMIT,
                    EaseCubicInOut((float)_laserTimer / LASER_TIME)),
                transform.Position.Y,
                transform.Position.Z);
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

    internal void SetMovingLasers(GameObject[] firstLaserObjects, GameObject[] secondLaserObjects)
    {
        _firstMovingLaserObjects = firstLaserObjects;
        _secondMovingLaserObjects = secondLaserObjects;
    }

    internal static float EaseCubicInOut(float t)
    {
        return t < 0.5f
            ? 4f * t * t * t
            : 1f - MathF.Pow(-2f * t + 2f, 3f) / 2f;
    }
}