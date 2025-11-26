#ifndef GAME_RENDERING_RESOURCES_VERTEX_BUFFERS_HPP
#define GAME_RENDERING_RESOURCES_VERTEX_BUFFERS_HPP

#include <vector>

#include "resource.hpp"
#include "vertex_attribute_descriptors.hpp"
#include "glad/glad.h"

namespace Rendering::Resources {
    class VertexBuffers : public Resource {
        uint32_t m_vao = 0;
        uint32_t m_vbo = 0;
        uint32_t m_ebo = 0;

        uint32_t m_vertexCount;
        uint32_t m_indexCount;
        GLenum m_primitiveTopology;

    public:
        explicit VertexBuffers(
            uint32_t resourceId,
            GLenum primitiveTopology,
            const float *vertices,
            uint32_t vertexCount,
            const VertexAttributeDescriptors &vertexAttributeDescriptors,
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
