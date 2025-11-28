using OpenGlGame.ECS.Components;
using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Operations;

namespace OpenGlGame.ECS.Systems;

public class DrawingSystem
{
    private readonly Renderer _renderer;
    private readonly SceneRegistry _registry;

    public DrawingSystem(Renderer rendererRef, SceneRegistry registryRef)
    {
        _renderer = rendererRef;
        _registry = registryRef;
    }

    public void Update(int width, int height)
    {
        foreach ((int _, Transform3D _, MeshRenderer mesh) in _registry.View<Transform3D, MeshRenderer>().Each())
        {
            _renderer.AddCommand(new DrawOperation(mesh.Shader, mesh.VertBuffers, mesh.VertAttributesDescriptors));
        }

        _renderer.Render(width, height);
    }
}