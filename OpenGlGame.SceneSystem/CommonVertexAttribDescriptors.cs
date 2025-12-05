using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.SceneSystem;

public static class CommonVertexAttribDescriptors
{
    public static readonly VertexAttribDescriptors DiffuseTextureMesh =
        ResourceManager.Instance.GetVertexAttributeDescriptors(
            "/assets/vertex_attributes/runtime_gen/diffuse_texture_mesh")
        ?? ResourceManager.Instance.CreateVertexAttribDescriptors(
            "/assets/vertex_attributes/runtime_gen/diffuse_texture_mesh",
            new VertexAttribDescriptors.AttribDescriptor(
                0,
                3,
                VertexAttribPointerType.Float,
                8 * sizeof(float),
                0),
            new VertexAttribDescriptors.AttribDescriptor(
                1,
                3,
                VertexAttribPointerType.Float,
                8 * sizeof(float),
                3 * sizeof(float)),
            new VertexAttribDescriptors.AttribDescriptor(
                2,
                2,
                VertexAttribPointerType.Float,
                8 * sizeof(float),
                6 * sizeof(float))
        );

    public static readonly VertexAttribDescriptors VertexColorsMesh =
        ResourceManager.Instance.GetVertexAttributeDescriptors(
            "/assets/vertex_attributes/runtime_gen/vertex_colors_mesh")
        ?? ResourceManager.Instance.CreateVertexAttribDescriptors(
            "/assets/vertex_attributes/runtime_gen/vertex_colors_mesh",
            new VertexAttribDescriptors.AttribDescriptor(
                0,
                3,
                VertexAttribPointerType.Float,
                9 * sizeof(float),
                0),
            new VertexAttribDescriptors.AttribDescriptor(
                1,
                3,
                VertexAttribPointerType.Float,
                9 * sizeof(float),
                3 * sizeof(float)),
            new VertexAttribDescriptors.AttribDescriptor(
                2,
                3,
                VertexAttribPointerType.Float,
                9 * sizeof(float),
                6 * sizeof(float))
        );
}