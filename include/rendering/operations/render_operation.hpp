#ifndef GAME_RENDERING_RENDER_OPERATION_HPP
#define GAME_RENDERING_RENDER_OPERATION_HPP

#include <functional>

namespace Rendering::Operations {
    struct OpKey {
        long LowBits = 0;
        long HighBits = 0;
    };

    class RenderOperation {
        OpKey m_opKey = OpKey{};
        uint64_t m_uniqueKey;
        std::function<void()> m_renderFunction;

    public:
        explicit RenderOperation(const std::function<void()> &renderFunction, uint64_t uniqueKey);

        [[nodiscard]] OpKey getOpKey() const { return m_opKey; }

        [[nodiscard]] uint64_t getUniqueKey() const { return m_uniqueKey; }

        void execute() const {
            m_renderFunction();
        }
    };
}

#endif //GAME_RENDERING_RENDER_OPERATION_HPP
