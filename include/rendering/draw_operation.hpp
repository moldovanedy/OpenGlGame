#ifndef GAME_RENDERING_DRAW_OPERATION_HPP
#define  GAME_RENDERING_DRAW_OPERATION_HPP

#include "resource_manager.hpp"

namespace Rendering {
    class DrawOperation {
        const Resources::Shader *m_shaderRef;
        const Resources::VertexBuffers *m_vertexBuffersRef;
        const Resources::VertexAttributeDescriptors *m_vertexAttributeDescriptorsRef;

        uint64_t sortKey = 0;

    public:
        DrawOperation(
            const Resources::Shader &shader,
            const Resources::VertexBuffers &vertexBuffers,
            const Resources::VertexAttributeDescriptors &vertexAttributeDescriptors);

        [[nodiscard]] uint64_t getSortKey() const { return sortKey; }

        void changeState(ResourceManager::ResourceTypeFlags stateChanges) const;

        void renderMesh() const;
    };
}

#endif // GAME_RENDERING_DRAW_OPERATION_HPP
