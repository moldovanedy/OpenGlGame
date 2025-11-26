#ifndef GAME_RENDERING_RESOURCES_VERTEX_ATTRIBUTE_DESCRIPTOR_HPP
#define GAME_RENDERING_RESOURCES_VERTEX_ATTRIBUTE_DESCRIPTOR_HPP

#include "glad/glad.h"
#include <vector>
#include <stdexcept>
#include "resource.hpp"

namespace Rendering::Resources {
    struct VertexAttributeDescriptor_t {
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
        /**
         * The size in bytes from the attribute of this instance to the same attribute of the next instance
         */
        uint32_t Stride;
        /**
         * The offset (in bytes) in the data buffer.
         */
        uint32_t Offset;
    };

    class VertexAttributeDescriptors : public Resource {
        std::vector<VertexAttributeDescriptor_t> m_descriptors;

    public:
        explicit VertexAttributeDescriptors(
            uint32_t resourceId,
            const std::vector<VertexAttributeDescriptor_t> &descriptors);

        [[nodiscard]] std::vector<VertexAttributeDescriptor_t> getVector() const { return m_descriptors; }

        [[nodiscard]] VertexAttributeDescriptor_t getAtIndex(const size_t index) const {
            if (index >= m_descriptors.size()) {
                throw std::invalid_argument("VertexAttributeDescriptor: index out of range");
            }

            return m_descriptors[index];
        }
    };
}

#endif //GAME_RENDERING_RESOURCES_VERTEX_ATTRIBUTE_DESCRIPTOR_HPP
