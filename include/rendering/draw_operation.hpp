#ifndef GAME_RENDERING_DRAW_OPERATION_HPP
#define  GAME_RENDERING_DRAW_OPERATION_HPP

#include "resource_manager.hpp"

namespace Rendering {
    namespace Resources {
        class VertexBuffers;
    }

    class DrawOperation {
        const Resources::Shader *m_shaderRef;
        const Resources::VertexBuffers *m_vertexBuffersRef;

    public:
        DrawOperation(const Resources::Shader &shader, const Resources::VertexBuffers &vertexBuffers);

        void changeState(ResourceManager::ResourceTypeFlags stateChanges) const;

        void renderMesh() const;
    };
}

#endif // GAME_RENDERING_DRAW_OPERATION_HPP
