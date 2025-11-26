#ifndef GAME_RENDERING_RESOURCES_VERTEX_BUFFERS_HPP
#define GAME_RENDERING_RESOURCES_VERTEX_BUFFERS_HPP

#include <vector>

#include "resource.hpp"
#include "glad/glad.h"

namespace Rendering::Resources {
    class VertexBuffers : public Resource {
    public:
        struct ShaderVertexAttributeDescriptor_t {
            /**
             * The location of this descriptor.
             */
            uint32_t Location;
            /**
             * The number of bytes of this attribute. It must be smaller than 16 and is generally a multiple of 4.
             */
            uint32_t NumBytesUsed;
            /**
             * The data type of the primitive values, usually GL_FLOAT.
             */
            GLenum DataType;
            uint32_t Stride;
            /**
             * The offset (in bytes) in the data buffer.
             */
            uint32_t Offset;
        };

    private:
        uint32_t m_vao = 0;
        uint32_t m_vbo = 0;
        uint32_t m_ebo = 0;

        uint32_t m_vertexCount;
        uint32_t m_indexCount;
        GLenum m_primitiveTopology;

    public:
        explicit VertexBuffers(
            GLenum primitiveTopology,
            const float *vertices,
            uint32_t vertexCount,
            const std::vector<ShaderVertexAttributeDescriptor_t> &attributeDescriptors,
            const uint32_t *indices = nullptr,
            uint32_t indexCount = 0
        );

        ~VertexBuffers();

        VertexBuffers(VertexBuffers &) = delete;

        VertexBuffers &operator=(VertexBuffers &) = delete;

        VertexBuffers(VertexBuffers &&) = delete;

        VertexBuffers &operator=(VertexBuffers &&) = delete;

        [[nodiscard]] uint32_t getVertexCount() const { return m_vertexCount; }

        [[nodiscard]] uint32_t getIndexCount() const { return m_indexCount; }

        [[nodiscard]] GLenum getPrimitiveTopology() const { return m_primitiveTopology; }

        [[nodiscard]] uint32_t getGlVao() const { return m_vao; }
    };
}

#endif //GAME_RENDERING_RESOURCES_VERTEX_BUFFERS_HPP
