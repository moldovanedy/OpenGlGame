using System;
using OpenGlGame.Rendering.Resources;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.Rendering.Operations;

public class DrawOperation : OperationBase
{
    public Shader ShaderDep { get; private set; }
    public VertexBuffers VertexBuffersDep { get; private set; }
    public VertexAttribDescriptors VertexAttribsDep { get; private set; }

    public DrawOperation(Shader shaderDep, VertexBuffers vertexBuffersDep, VertexAttribDescriptors vertexAttribsDep)
    {
        ShaderDep = shaderDep;
        VertexBuffersDep = vertexBuffersDep;
        VertexAttribsDep = vertexAttribsDep;
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