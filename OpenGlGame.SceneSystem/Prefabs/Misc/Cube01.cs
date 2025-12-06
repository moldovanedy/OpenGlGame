using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Misc;

public class Cube01 : GameObject
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private MeshMaterial _material;

    public Cube01(Transform3D transform)
    {
        _material = CommonMaterials.CreateDiffuseUnlitMaterial();

        AddComponent(transform);
        AddComponent(new RigidBody
        {
            Mass = 12.0f,
            BoundingBox = new Aabb(new Vector3(-1f, -1f, -1f), new Vector3(2f, 2f, 2f)),
            RestitutionCoefficient = 0.3f
        });

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
}