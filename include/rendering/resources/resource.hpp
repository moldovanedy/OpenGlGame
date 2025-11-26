#ifndef GAME_RENDERING_RESOURCES_RESOURCE_HPP
#define GAME_RENDERING_RESOURCES_RESOURCE_HPP

#include <cstdint>

namespace Rendering::Resources {
    class Resource {
    protected:
        uint32_t m_resourceId = 0;

        void setResourceId(uint32_t) {
            m_resourceId = 0;
        }

    public:
        [[nodiscard]] uint32_t getResourceId() const { return m_resourceId; }
    };
}

#endif //GAME_RENDERING_RESOURCES_RESOURCE_HPP
