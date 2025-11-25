#include "glad/glad.h"

#include <rendering/renderer.hpp>

using namespace Rendering;

void Renderer::render() {
    //TODO: sort render ops

    glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

    for (Operations::RenderOperation &operation: m_ops) {
        operation.execute();
    }
}

void Renderer::clearCommands() {
    m_ops.clear();
    m_lastUniqueKey = 0;
    m_needsSorting = false;
}

//TODO
void Renderer::addCommand() {
    m_ops.emplace_back(
        [] {
        },
        ++m_lastUniqueKey
    );

    m_needsSorting = true;
}

bool Renderer::updateCommand(const uint64_t key) {
    const Operations::RenderOperation *operation = nullptr;
    size_t index = 0;

    for (size_t i = 0; i < m_ops.size(); i++) {
        if (m_ops[i].getUniqueKey() == key) {
            operation = &m_ops[i];
            index = i;
            break;
        }
    }

    if (operation == nullptr) {
        return false;
    }

    m_ops[index] = Operations::RenderOperation(
        [] {
        },
        m_ops[index].getUniqueKey());

    m_needsSorting = true;
    return true;
}
