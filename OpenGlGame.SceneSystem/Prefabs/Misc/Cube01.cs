using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Misc;

public class Cube01 : GameObject
{
    private readonly Aabb _boundingBox = new(new Vector3(-0.555f, -0.555f, -0.555f), new Vector3(1.11f, 1.11f, 1.11f));

    private readonly MeshMaterial _material;
    private readonly RigidBody _rigidBody;

    private bool _isPickedUp;
    private float _pickUpTimer;
    private bool _isEasingOnOutGraph;

    public Cube01(Transform3D transform)
    {
        UpdateEvent += OnUpdate;
        Texture? diffuseTex = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/cube_01/tex_diffuse01.png",
            new Texture.TextureFormat());
        if (diffuseTex == null)
        {
            throw new NullReferenceException("Cube01 texture was not found");
        }

        AddComponent(transform);
        _material = CommonMaterials.CreateDiffuseLitMaterial(diffuseTex);

        _rigidBody = new RigidBody
        {
            Mass = 12.0f,
            BoundingBox = _boundingBox,
            RestitutionCoefficient = 0.3f
        };
        AddComponent(_rigidBody);

        AddComponent(new Interactable(_boundingBox, OnInteractWith));

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/cube_01/cube_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/cube_01/cube_01.glb.vert01",
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

        if (Player.Instance.GetComponent(nameof(RigidBody)) is not RigidBody playerRigidBody)
        {
            return;
        }

        if (!_isEasingOnOutGraph)
        {
            if (_pickUpTimer < 0.25f)
            {
                _pickUpTimer += (float)deltaTime;
            }
            else
            {
                _pickUpTimer = 0.25f;
                _isEasingOnOutGraph = true;
            }
        }
        else
        {
            if (_pickUpTimer > 0f)
            {
                _pickUpTimer -= (float)deltaTime;
            }
            else
            {
                _pickUpTimer = 0f;
            }
        }

        if (_isPickedUp)
        {
            Vector3 velocity = playerRigidBody.InternalPosition - playerRigidBody.InternalPreviousPosition;
            velocity +=
                new Vector3(0f, float.Lerp(0f, 6.0f, _pickUpTimer / 0.5f), 0f)
                * (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
            velocity /= (float)PhysicsSystem.PHYSICS_TICK_SECONDS;
            _rigidBody.SetDirectVelocity(velocity);
        }
    }

    private void OnInteractWith()
    {
        //if the player already has an object and it's not this one, abort
        if (Player.Instance.HasPickedObjectUp && !_isPickedUp)
        {
            return;
        }

        _pickUpTimer = 0f;
        _isEasingOnOutGraph = false;

        _rigidBody.IsAffectedByGravity = _isPickedUp;
        _isPickedUp = !_isPickedUp;

        Player.Instance.HasPickedObjectUp = _isPickedUp;
    }
}