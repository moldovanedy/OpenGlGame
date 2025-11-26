#include <rendering/draw_operation.hpp>

using namespace Rendering;

constexpr uint64_t RENDER_PASS_OFFSET = 1ULL << 60ULL;
constexpr uint64_t RENDER_PASS_MASK = 0x0fULL << 60ULL;

constexpr uint64_t RENDER_TARGET_OFFSET = 1ULL << 56ULL;
constexpr uint64_t RENDER_TARGET_MASK = 0x0fULL << 56ULL;

constexpr uint64_t SHADER_PROGRAM_OFFSET = 1ULL << 44ULL;
constexpr uint64_t SHADER_PROGRAM_MASK = 0x0fffULL << 44ULL;

constexpr uint64_t BLENDING_OFFSET = 1ULL << 40ULL;
constexpr uint64_t BLENDING_MASK = 0x0fULL << 40ULL;

constexpr uint64_t TEXTURE_ATTACHMENT_OFFSET = 1ULL << 32ULL;
constexpr uint64_t TEXTURE_ATTACHMENT_MASK = 0xffULL << 32ULL;

constexpr uint64_t VERTEX_ATTRIBUTE_FORMAT_OFFSET = 1ULL << 24ULL;
constexpr uint64_t VERTEX_ATTRIBUTE_FORMAT_MASK = 0xffULL << 24ULL;

constexpr uint64_t UNIFORM_BUFFER_OFFSET = 1ULL << 12ULL;
constexpr uint64_t UNIFORM_BUFFER_MASK = 0x0fffULL << 12ULL;

constexpr uint64_t VERTEX_BUFFER_OFFSET = 1ULL;
constexpr uint64_t VERTEX_BUFFER_MASK = 0x0fffULL;

DrawOperation::DrawOperation(
    const Resources::Shader &shader,
    const Resources::VertexBuffers &vertexBuffers,
    const Resources::VertexAttributeDescriptors &vertexAttributeDescriptors)
    : m_shaderRef(&shader),
      m_vertexBuffersRef(&vertexBuffers),
      m_vertexAttributeDescriptorsRef(&vertexAttributeDescriptors) {
}

void DrawOperation::renderMesh() const {
    if (m_vertexBuffersRef->getIndexCount() == 0) {
        glDrawArrays(
            m_vertexBuffersRef->getPrimitiveTopology(),
            0,
            static_cast<int>(m_vertexBuffersRef->getVertexCount()));
    } else {
        glDrawElements(
            m_vertexBuffersRef->getPrimitiveTopology(),
            static_cast<int>(m_vertexBuffersRef->getIndexCount()),
            GL_UNSIGNED_INT,
            nullptr);
    }
}

void DrawOperation::changeState(const ResourceManager::ResourceTypeFlags stateChanges) const {
    if ((stateChanges & ResourceManager::ResourceTypeFlags::eShader) != ResourceManager::ResourceTypeFlags::eNone) {
        glUseProgram(m_shaderRef->getGlId());
    }

    if ((stateChanges & ResourceManager::ResourceTypeFlags::eVertexBuffer) !=
        ResourceManager::ResourceTypeFlags::eNone) {
        glBindVertexArray(m_vertexBuffersRef->getGlVao());
    }
}
