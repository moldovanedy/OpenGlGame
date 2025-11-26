#ifndef GAME_WINDOW_HPP
#define GAME_WINDOW_HPP

// this is needed because GLAD needs to be included before GLFW, otherwise we'll have compilation errors
// ReSharper disable once CppUnusedIncludeDirective
#include "glad/glad.h"
#include "GLFW/glfw3.h"

class Window {
    int m_width = 800;
    int m_height = 600;
    GLFWwindow *m_glfwWindow = nullptr;

public:
    explicit Window(bool isFullscreen, int width = 800, int height = 600);

    ~Window();

    Window(const Window &) = delete;

    Window &operator=(const Window &) = delete;

    Window(Window &&) noexcept;

    Window &operator=(Window &&) noexcept;

    [[nodiscard]] bool shouldCloseWindow() const;

    [[nodiscard]] int getFramebufferWidth() const;

    [[nodiscard]] int getFramebufferHeight() const;

    void swapBuffers() const;

#pragma region Events

    static void resize(GLFWwindow *glfwWindow, int width, int height);

#pragma endregion //Events
};

#endif //GAME_WINDOW_HPP
