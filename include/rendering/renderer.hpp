#ifndef GAME_RENDERING_RENDERER_HPP
#define GAME_RENDERING_RENDERER_HPP

#include <vector>

#include "operations/render_operation.hpp"

namespace Rendering {
    class Renderer {
        std::vector<Operations::RenderOperation> m_ops;

        uint64_t m_lastUniqueKey = 0;
        bool m_needsSorting = false;

    public:
        void render();

        void clearCommands();

        void addCommand();

        /**
         * This should rarely be used. Oftentimes, it's more efficient to completely clear the command vector and
         * recreate it from scratch.
         * @param key 
         * @return 
         */
        bool updateCommand(uint64_t key);
    };
}

#endif //GAME_RENDERING_RENDERER_HPP
