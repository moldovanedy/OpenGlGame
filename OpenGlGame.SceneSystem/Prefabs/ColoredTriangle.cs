using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Resources;
using OpenGlGame.SceneSystem.Components;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.SceneSystem.Prefabs;

public class ColoredTriangle : GameObject
{
    private const string VERT_BUFFERS_PATH = "/runtime_gen/triangle";

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private MeshMaterial _material;

    public ColoredTriangle()
    {
        _material = CommonMaterials.CreateVertColorsUnlitMaterial();
        AddComponent(new Transform3D());

        float[] triangleVertices =
        [
            // positions         // normals      //vertex colors
            -0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom left
            0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom right
            0.0f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f // top 
        ];

        VertexBuffers triangleVertexBuffers =
            ResourceManager.Instance.GetVertexBuffers(VERT_BUFFERS_PATH)
            ?? ResourceManager.Instance.CreateVertexBuffers(
                VERT_BUFFERS_PATH,
                PrimitiveType.Triangles,
                CommonVertexAttribDescriptors.VertexColorsMesh,
                triangleVertices,
                3
            );

        AddComponent(
            new MeshRenderer
            {
                VertBuffers = triangleVertexBuffers,
                Material = _material
            });
    }
}