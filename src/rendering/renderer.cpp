#include <cinttypes>
#include "glad/glad.h"

#include <rendering/renderer.hpp>

using namespace Rendering;

constexpr float ASPECT_RATIO = 16.0 / 9.0;

void Renderer::render(const RenderParams_t &params) {
    //TODO: sort render ops

    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

    //keep 16:9 aspect ratio
    const auto fbWidth = static_cast<float>(params.FramebufferWidth);
    const auto fbHeight = static_cast<float>(params.FramebufferHeight);
    const float actualAspectRatio = fbWidth / fbHeight;

    if (actualAspectRatio > ASPECT_RATIO) {
        const float newWidth = fbHeight * ASPECT_RATIO;
        const float offsetX = (fbWidth - newWidth) / 2.0f;
        glViewport(static_cast<int>(offsetX), 0, static_cast<int>(newWidth), static_cast<int>(fbHeight));
    } else if (actualAspectRatio < ASPECT_RATIO) {
        const float newHeight = fbWidth / ASPECT_RATIO;
        const float offsetY = (fbHeight - newHeight) / 2.0f;
        glViewport(0, static_cast<int>(offsetY), static_cast<int>(fbWidth), static_cast<int>(newHeight));
    }

    for (DrawOperation &operation: m_ops) {
        operation.changeState(
            ResourceManager::ResourceTypeFlags::eShader | ResourceManager::ResourceTypeFlags::eVertexBuffer);

        operation.renderMesh();
    }

    //restore
    glViewport(0, 0, params.FramebufferWidth, params.FramebufferHeight);
}

void Renderer::clearCommands() {
    m_ops.clear();
    m_lastUniqueKey = 0;
}

void Renderer::addCommand(const DrawOperation &drawOp) {
    m_ops.push_back(drawOp);
}

// bool Renderer::updateCommand(const uint64_t key) {
//     const Operations::RenderOperation *operation = nullptr;
//     size_t index = 0;
//
//     for (size_t i = 0; i < m_ops.size(); i++) {
//         if (m_ops[i].getUniqueKey() == key) {
//             operation = &m_ops[i];
//             index = i;
//             break;
//         }
//     }
//
//     if (operation == nullptr) {
//         return false;
//     }
//
//     m_ops[index] = Operations::RenderOperation(
//         [] {
//         },
//         m_ops[index].getUniqueKey());
//
//     m_needsSorting = true;
//     return true;
// }
