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

    public static readonly VertexAttribDescriptors DiffuseNormalTextureMesh =
        ResourceManager.Instance.GetVertexAttributeDescriptors(
            "/assets/vertex_attributes/runtime_gen/diffuse_normal_texture_mesh")
        ?? ResourceManager.Instance.CreateVertexAttribDescriptors(
            "/assets/vertex_attributes/runtime_gen/diffuse_normal_texture_mesh",
            new VertexAttribDescriptors.AttribDescriptor(
                0,
                3,
                VertexAttribPointerType.Float,
                14 * sizeof(float),
                0),
            new VertexAttribDescriptors.AttribDescriptor(
                1,
                3,
                VertexAttribPointerType.Float,
                14 * sizeof(float),
                3 * sizeof(float)),
            new VertexAttribDescriptors.AttribDescriptor(
                2,
                2,
                VertexAttribPointerType.Float,
                14 * sizeof(float),
                6 * sizeof(float)),
            new VertexAttribDescriptors.AttribDescriptor(
                3,
                3,
                VertexAttribPointerType.Float,
                14 * sizeof(float),
                8 * sizeof(float)),
            new VertexAttribDescriptors.AttribDescriptor(
                4,
                3,
                VertexAttribPointerType.Float,
                14 * sizeof(float),
                11 * sizeof(float))
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

    public static readonly VertexAttribDescriptors UiMesh =
        ResourceManager.Instance.GetVertexAttributeDescriptors(
            "/assets/vertex_attributes/runtime_gen/ui_mesh")
        ?? ResourceManager.Instance.CreateVertexAttribDescriptors(
            "/assets/vertex_attributes/runtime_gen/ui_mesh",
            new VertexAttribDescriptors.AttribDescriptor(
                0,
                2,
                VertexAttribPointerType.Float,
                2 * sizeof(float),
                0)
        );

    public static readonly VertexAttribDescriptors UiTexture =
        ResourceManager.Instance.GetVertexAttributeDescriptors(
            "/assets/vertex_attributes/runtime_gen/ui_texture")
        ?? ResourceManager.Instance.CreateVertexAttribDescriptors(
            "/assets/vertex_attributes/runtime_gen/ui_texture",
            new VertexAttribDescriptors.AttribDescriptor(
                0,
                2,
                VertexAttribPointerType.Float,
                4 * sizeof(float),
                0),
            new VertexAttribDescriptors.AttribDescriptor(
                1,
                2,
                VertexAttribPointerType.Float,
                4 * sizeof(float),
                2 * sizeof(float))
        );
}