using System;
using OpenTK.Graphics.OpenGL;

namespace OpenGlGame.Rendering.Resources;

public class VertexBuffers : ResourceBase, IDisposable
{
    public int VertexCount { get; private set; }
    public int IndexCount { get; private set; }
    public PrimitiveType PrimitiveTopology { get; private set; }

    internal int GlVao { get; private set; }

    private int _vbo;
    private int _ebo = -1;

    internal VertexBuffers(
        uint resourceId,
        PrimitiveType primitiveTopology,
        VertexAttribDescriptors vertexAttribDescriptors,
        float[] vertexData,
        int vertexCount,
        uint[]? indexData = null,
        int indicesCount = 0) : base(resourceId)
    {
        PrimitiveTopology = primitiveTopology;
        VertexCount = vertexCount;
        IndexCount = indicesCount;

        // uint totalVertexByteSize = 0;
        // foreach (VertexAttribDescriptors.AttribDescriptor descriptor in vertexAttribDescriptors.Attributes)
        // {
        //     totalVertexByteSize += descriptor.NumSlotsUsed;
        // }

        GL.GenVertexArrays(1, out int vao);
        GlVao = vao;
        GL.GenBuffers(1, out _vbo);

        if (indexData != null)
        {
            GL.GenBuffers(1, out _ebo);
        }

        GL.BindVertexArray(GlVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(
            BufferTarget.ArrayBuffer,
            vertexData.Length * sizeof(float),
            vertexData,
            BufferUsageHint.StaticDraw);

        if (indexData != null)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                indexData.Length * sizeof(uint),
                indexData,
                BufferUsageHint.StaticDraw);
        }

        foreach (VertexAttribDescriptors.AttribDescriptor descriptor in vertexAttribDescriptors.Attributes)
        {
            GL.VertexAttribPointer(
                descriptor.Location,
                descriptor.NumSlotsUsed,
                descriptor.PrimitiveDataType,
                false,
                descriptor.Stride,
                descriptor.Offset);

            GL.EnableVertexAttribArray(descriptor.Location);
        }
    }

    public void Dispose()
    {
        int vao = GlVao;
        GL.DeleteVertexArrays(1, ref vao);
        GlVao = -1;

        GL.DeleteBuffers(1, ref _vbo);
        _vbo = -1;

        if (_ebo != -1)
        {
            GL.DeleteBuffers(1, ref _ebo);
            _ebo = -1;
        }

        GC.SuppressFinalize(this);
    }
}