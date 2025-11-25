#include <application.hpp>

Application::Application()
    : m_renderer(std::make_shared<Rendering::Renderer>()),
      m_window(std::make_shared<Window>(false)) {
}

void Application::run() const {
    while (!m_window->shouldCloseWindow()) {
        m_renderer->render();
        m_window->swapBuffers();

        glfwWaitEventsTimeout(0.01);
    }
}
