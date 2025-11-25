#ifndef GAME_APPLICATION_HPP
#define GAME_APPLICATION_HPP

#include <memory>

#include "window.hpp"
#include "rendering/renderer.hpp"

class Application {
    const std::shared_ptr<Rendering::Renderer> m_renderer;
    const std::shared_ptr<Window> m_window;

public:
    explicit Application();

    void run() const;
};

#endif //GAME_APPLICATION_HPP
