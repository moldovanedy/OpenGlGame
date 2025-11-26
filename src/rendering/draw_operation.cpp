#include <rendering/draw_operation.hpp>

using namespace Rendering;

DrawOperation::DrawOperation(
    const Resources::Shader &shader,
    const Resources::VertexBuffers &vertexBuffers)
    : m_shaderRef(&shader),
      m_vertexBuffersRef(&vertexBuffers) {
}

void DrawOperation::renderMesh() const {
    if (m_vertexBuffersRef->getIndexCount() == 0) {
        glDrawArrays(m_vertexBuffersRef->getPrimitiveTopology(), 0, m_vertexBuffersRef->getVertexCount());
    } else {
        glDrawElements(
            m_vertexBuffersRef->getPrimitiveTopology(),
            m_vertexBuffersRef->getIndexCount(),
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
