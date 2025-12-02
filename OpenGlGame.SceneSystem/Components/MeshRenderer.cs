using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Operations;
using OpenGlGame.Rendering.Resources;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Components;

public class MeshRenderer : IGameObjectComponent
{
    public string Name => "MeshRenderer";

    public GameObject? Owner
    {
        get => _owner;
        set
        {
            _owner = value;
            _ownerTransform = value?.GetComponent(nameof(Transform3D)) as Transform3D;
        }
    }

    private GameObject? _owner;

    public MeshMaterial? Material { get; set; }

    public VertexBuffers? VertBuffers { get; set; }

    private Transform3D? _ownerTransform;

    public void EnterScene()
    {
    }

    public void ExitScene()
    {
    }

    public void Update(double deltaTime)
    {
        if (VertBuffers != null && Material != null)
        {
            _owner?.SystemRenderer?.AddCommand(
                new DrawOperation(
                    VertBuffers,
                    Material,
                    _ownerTransform?.GetModelMatrix() ?? Matrix4.Identity));
        }
    }
}