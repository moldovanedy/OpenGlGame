using System;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;

namespace OpenGlGame.SceneSystem.Prefabs.Misc;

public class Skybox : GameObject
{
    public Skybox()
    {
        Texture? diffuseTex = ResourceManager.Instance.GetOrCreateTexture(
            "/assets/models/rocky_01/tex_diffuse01.png",
            new Texture.TextureFormat());
        if (diffuseTex == null)
        {
            throw new NullReferenceException("Skybox texture was not found");
        }

        MeshMaterial material = CommonMaterials.CreateDiffuseLitMaterial(diffuseTex);

        VertexBuffers? buffers =
            ResourceManager.Instance.GetVertexBuffers(
                "/assets/models/rocky_01/rocky_platform_01.glb.vert01")
            ?? ResourceManager.Instance.CreateVertexBuffersFromFile(
                "/assets/models/rocky_01/rocky_platform_01.glb.vert01",
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