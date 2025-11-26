#ifndef GAME_RENDERING_RENDERER_HPP
#define GAME_RENDERING_RENDERER_HPP

#include <vector>
#include <cstdint>

#include "draw_operation.hpp"

namespace Rendering {
    class Renderer {
    public:
        struct RenderParams_t {
            int FramebufferWidth;
            int FramebufferHeight;
        };

    private:
        std::vector<DrawOperation> m_ops;

        uint64_t m_lastUniqueKey = 0;

    public:
        void render(const RenderParams_t &params);

        void clearCommands();

        void addCommand(const DrawOperation &drawOp);

        // bool updateCommand(uint64_t key);
    };
}

#endif //GAME_RENDERING_RENDERER_HPP
