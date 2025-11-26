#include <iostream>
#include <rendering/resources/vertex_buffers.hpp>

#include "glm/ext/scalar_uint_sized.hpp"

using namespace Rendering::Resources;

VertexBuffers::VertexBuffers(
    const uint32_t resourceId,
    const GLenum primitiveTopology,
    const float *vertices,
    const uint32_t vertexCount,
    const VertexAttributeDescriptors &vertexAttributeDescriptors,
    const uint32_t *indices,
    const uint32_t indexCount
) : m_vertexCount(vertexCount), m_indexCount(indexCount), m_primitiveTopology(primitiveTopology) {
    m_resourceId = resourceId;

    uint32_t totalVertexByteSize = 0;
    for (const VertexAttributeDescriptor_t &attributeDescriptor: vertexAttributeDescriptors.getVector()) {
        totalVertexByteSize += attributeDescriptor.NumBytesUsed;
    }

    glGenVertexArrays(1, &m_vao);
    glGenBuffers(1, &m_vbo);

    if (indices != nullptr) {
        glGenBuffers(1, &m_ebo);
    }

    glBindVertexArray(m_vao);

    glBindBuffer(GL_ARRAY_BUFFER, m_vbo);
    glBufferData(
        GL_ARRAY_BUFFER,
        //TODO: find out why does it not work without the +4 at the end
        totalVertexByteSize * vertexCount + 4,
        vertices,
        GL_STATIC_DRAW);

    if (indices != nullptr) {
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, m_ebo);
        glBufferData(
            GL_ELEMENT_ARRAY_BUFFER,
            static_cast<uint32_t>(sizeof(indices[0])) * indexCount,
            indices,
            GL_STATIC_DRAW);
    }

    for (const VertexAttributeDescriptor_t &attributeDescriptor: vertexAttributeDescriptors.getVector()) {
        glVertexAttribPointer(
            attributeDescriptor.Location,
            static_cast<int32_t>(attributeDescriptor.NumBytesUsed) / 3,
            attributeDescriptor.DataType,
            GL_FALSE,
            static_cast<int32_t>(attributeDescriptor.Stride),
            reinterpret_cast<void *>(attributeDescriptor.Offset)
        );
        glEnableVertexAttribArray(attributeDescriptor.Location);
    }

    //this is for safety, so we don't accidentally modify the VBO later
    glBindBuffer(GL_ARRAY_BUFFER, 0);
}

VertexBuffers::~VertexBuffers() {
    glDeleteVertexArrays(1, &m_vao);
    glDeleteBuffers(1, &m_vbo);

    if (m_ebo != 0) {
        glDeleteBuffers(1, &m_ebo);
    }
}
