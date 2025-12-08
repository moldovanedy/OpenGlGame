using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenGlGame.SceneSystem.PhysicsEngine;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Prefabs.Lights;

public class LightSupport01 : GameObject
{
    public LightSupport01(Transform3D transform)
    {
        MeshMaterial material = CommonMaterials.CreateDiffuseLitMaterial();

        AddComponent(transform);
        AddComponent(
            new StaticCollider(
                new Aabb(new Vector3(-0.5f, -0.05f, -0.5f), new Vector3(1f, 0.1f, 1f))));

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/light_01/light_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/light_01/light_01.glb.vert01",
                CommonVertexAttribDescriptors.DiffuseTextureMesh);
        if (buffers == null)
        {
            throw new NullReferenceException("Vertex buffers were not found");
        }

        AddComponent(
            new MeshRenderer
            {
                VertBuffers = buffers,
                Material = material
            });
    }
}

public class Light01 : GameObject
{
    public Light01(LightSupport01 support, Renderer.PointLightData lightData)
    {
        byte r = (byte)(lightData.LightColor.X * 255.0f);
        byte g = (byte)(lightData.LightColor.Y * 255.0f);
        byte b = (byte)(lightData.LightColor.Z * 255.0f);
        MeshMaterial material = CommonMaterials.CreateColoredUnlitMaterial(r, g, b, 0xff);

        var supportTransform = (Transform3D?)support.GetComponent(nameof(Transform3D));
        if (supportTransform == null)
        {
            throw new NullReferenceException("Light support transform was not found");
        }

        AddComponent(
            new Transform3D
            {
                Position = new Vector3(
                    supportTransform.Position.X,
                    supportTransform.Position.Y - 0.05f,
                    supportTransform.Position.Z),
                Scale = new Vector3(0.85f, 0.05f, 0.85f)
            });

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers("/assets/models/cube_02/cube_02.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/cube_02/cube_02.glb.vert01",
                CommonVertexAttribDescriptors.DiffuseTextureMesh);
        if (buffers == null)
        {
            throw new NullReferenceException("Vertex buffers were not found");
        }

        AddComponent(
            new MeshRenderer
            {
                VertBuffers = buffers,
                Material = material
            });

        AddComponent(
            new PointLight
            {
                LightData = lightData
            });
    }
}