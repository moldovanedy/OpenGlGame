using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Misc;

public class Flag : GameObject
{
    private readonly Aabb _boundingBox = new(new Vector3(-0.5f, -0.05f, -0.85f), new Vector3(1f, 0.1f, 1.7f));

    private readonly Transform3D _transform;
    private readonly MeshMaterial _material;

    private bool _hasStartedFlagRaising;
    private float _raisedMeters;

    public Flag(Transform3D transform)
    {
        UpdateEvent += OnUpdate;
        _transform = transform;
        AddComponent(transform);

        Texture? diffuseTex = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/finish_flag/tex_diffuse01.png",
            new Texture.TextureFormat());
        if (diffuseTex == null)
        {
            throw new NullReferenceException("Flag texture was not found");
        }

        _material = CommonMaterials.CreateWavingFlagMaterial(diffuseTex);
        AddComponent(new StaticCollider(_boundingBox));
        AddComponent(new Interactable(_boundingBox, OnFlagRaised));

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/finish_flag/flag.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/finish_flag/flag.glb.vert01",
                CommonVertexAttribDescriptors.DiffuseTextureMesh);
        if (buffers == null)
        {
            throw new NullReferenceException("Vertex buffers were not found");
        }

        AddComponent(
            new MeshRenderer
            {
                VertBuffers = buffers,
                Material = _material
            });
    }

    private void OnUpdate(double deltaTime)
    {
        //actual material properties
        _material.SetProperty("material.diffuseTex", 0);
        _material.SetProperty("material.specularCol", new Vector3(0.1f));
        _material.SetProperty("material.shininess", 8.0f);

        if (_hasStartedFlagRaising)
        {
            if (_raisedMeters < 2.5f)
            {
                float raiseValue = (float)deltaTime * 3f;
                _raisedMeters += raiseValue;

                _transform.Position = new Vector3(
                    _transform.Position.X,
                    _transform.Position.Y + raiseValue,
                    _transform.Position.Z);
            }
            else
            {
                Player.Instance.SignalGameCompletion();
                _hasStartedFlagRaising = false;
            }
        }
    }

    private void OnFlagRaised()
    {
        if (_hasStartedFlagRaising)
        {
            return;
        }

        _hasStartedFlagRaising = true;
    }
}