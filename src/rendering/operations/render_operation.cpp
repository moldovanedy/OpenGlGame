#include <cstdint>
#include <rendering/operations/render_operation.hpp>

using namespace Rendering::Operations;

RenderOperation::RenderOperation(const std::function<void()> &renderFunction, const uint64_t uniqueKey)
    : m_uniqueKey(uniqueKey), m_renderFunction(renderFunction) {
}
