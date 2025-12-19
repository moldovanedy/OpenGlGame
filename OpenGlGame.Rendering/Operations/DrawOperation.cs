using System;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.Rendering.Operations;

public class DrawOperation : OperationBase
{
    public VertexBuffers VertexBuffersDep { get; }
    public MeshMaterial MeshMaterialDep { get; }
    public Matrix4 ModelMatrix { get; }
    public bool IsUi { get; }

    public DrawOperation(
        VertexBuffers vertexBuffersDep,
        MeshMaterial material,
        Matrix4 modelMatrix,
        bool isUi)
    {
        VertexBuffersDep = vertexBuffersDep;
        MeshMaterialDep = material;
        ModelMatrix = modelMatrix;
        IsUi = isUi;
    }

    public override ulong ComputeOrderKey()
    {
        return
            ((IsUi ? 1UL : 0UL) << Renderer.RENDER_TARGET_OFFSET)
            | MeshMaterialDep.OrderKey
            | ((ulong)VertexBuffersDep.ResourceId << Renderer.VERTEX_BUFFER_OFFSET);
    }

    internal void Draw()
    {
        MeshMaterialDep.SetProperty("model", ModelMatrix);

        Matrix4 transInvMatrix = Matrix4.Transpose(Matrix4.Invert(ModelMatrix));
        MeshMaterialDep.SetProperty("transposedInverseModel", transInvMatrix);

        if (VertexBuffersDep.IndexCount == 0)
        {
            GL.DrawArrays(VertexBuffersDep.PrimitiveTopology, 0, VertexBuffersDep.VertexCount);
        }
        else
        {
            GL.DrawElements(
                VertexBuffersDep.PrimitiveTopology,
                VertexBuffersDep.IndexCount,
                DrawElementsType.UnsignedInt,
                IntPtr.Zero);
        }
    }
}