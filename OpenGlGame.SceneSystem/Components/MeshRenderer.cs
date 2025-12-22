using OpenGlGame.Rendering;
using OpenGlGame.Rendering.Operations;
using OpenGlGame.Rendering.Resources;
using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.Components;

public class MeshRenderer : IGameObjectComponent
{
    public string Name => "MeshRenderer";

    public GameObject? Owner { get; set; }

    public MeshMaterial? Material { get; set; }

    public VertexBuffers? VertBuffers { get; set; }

    public bool IsUi { get; set; }

    private Transform3D? _ownerTransform;

    public void EnterScene()
    {
        _ownerTransform = Owner?.GetComponent(nameof(Transform3D)) as Transform3D;
    }

    public void ExitScene()
    {
    }

    public void Update(double deltaTime)
    {
        if (VertBuffers == null || Material == null)
        {
            return;
        }

        Owner?.SystemRenderer?.AddCommand(
            new DrawOperation(
                VertBuffers,
                Material,
                _ownerTransform?.GetModelMatrix() ?? Matrix4.Identity,
                IsUi));
    }
}