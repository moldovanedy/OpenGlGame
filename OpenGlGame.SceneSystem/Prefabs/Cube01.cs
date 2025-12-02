using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;

namespace OpenGlGame.SceneSystem.Prefabs;

public class Cube01 : GameObject
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private MeshMaterial _material;

    public Cube01()
    {
        _material = CommonMaterials.CreateAlbedoMaterial();

        AddComponent(new Transform3D());

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/cube_01/cube_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/cube_01/cube_01.glb.vert01",
                CommonVertexAttribDescriptors.AlbedoTextureMesh);
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