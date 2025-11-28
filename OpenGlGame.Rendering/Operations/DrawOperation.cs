using System;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.Rendering.Operations;

public class DrawOperation : OperationBase
{
    public Shader ShaderDep { get; }
    public VertexBuffers VertexBuffersDep { get; }
    public VertexAttribDescriptors VertexAttribsDep { get; }
    public Matrix4 ModelMatrix { get; }

    public DrawOperation(
        Shader shaderDep,
        VertexBuffers vertexBuffersDep,
        VertexAttribDescriptors vertexAttribsDep,
        Matrix4 modelMatrix)
    {
        ShaderDep = shaderDep;
        VertexBuffersDep = vertexBuffersDep;
        VertexAttribsDep = vertexAttribsDep;
        ModelMatrix = modelMatrix;
    }

    public override ulong ComputeOrderKey()
    {
        return
            ((ulong)ShaderDep.ResourceId << Renderer.SHADER_PROGRAM_OFFSET)
            | ((ulong)VertexAttribsDep.ResourceId << Renderer.VERTEX_ATTRIBUTE_FORMAT_OFFSET)
            | ((ulong)VertexBuffersDep.ResourceId << Renderer.VERTEX_BUFFER_OFFSET);
    }

    internal void Draw()
    {
        Matrix4 modelMatrix = ModelMatrix;
        ShaderDep.SetUniformMat4("model", ref modelMatrix);

        Matrix4 transInvMatrix = Matrix4.Transpose(Matrix4.Invert(modelMatrix));
        ShaderDep.SetUniformMat4("transposedInverseModel", ref transInvMatrix);

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