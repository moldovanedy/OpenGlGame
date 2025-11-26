#include <rendering/resources/vertex_attribute_descriptors.hpp>

Rendering::Resources::VertexAttributeDescriptors::VertexAttributeDescriptors(
    const uint32_t resourceId,
    const std::vector<VertexAttributeDescriptor_t> &descriptors) {
    m_resourceId = resourceId;

    m_descriptors = descriptors;
}
