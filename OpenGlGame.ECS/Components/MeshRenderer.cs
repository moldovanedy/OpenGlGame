using OpenGlGame.Rendering.Resources;

namespace OpenGlGame.ECS.Components;

public struct MeshRenderer
{
    public VertexAttribDescriptors VertAttributesDescriptors { get; set; }
    public VertexBuffers VertBuffers { get; set; }
    public Shader Shader { get; set; }

    public MeshRenderer(VertexAttribDescriptors vertAttributesDescriptors, VertexBuffers vertBuffers, Shader shader)
    {
        VertAttributesDescriptors = vertAttributesDescriptors;
        VertBuffers = vertBuffers;
        Shader = shader;
    }
}