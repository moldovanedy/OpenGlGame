#include <stdexcept>
#include <window.hpp>

#include "main_info.hpp"

Window::Window(const bool isFullscreen, const int width, const int height) {
    glfwWindowHint(GLFW_CLIENT_API, GLFW_OPENGL_API);
    glfwWindowHint(GLFW_SCALE_TO_MONITOR, GLFW_TRUE);

    glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, MainInfo::MinOpenGlMajorVersion);
    glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, MainInfo::MinOpenGlMinorVersion);
    glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

    //MSAA 4x
    glfwWindowHint(GLFW_SAMPLES, 4);

#ifdef __APPLE__
    glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
#endif

    if (isFullscreen) {
        GLFWmonitor *monitor = glfwGetPrimaryMonitor();
        const GLFWvidmode *mode = glfwGetVideoMode(monitor);

        glfwWindowHint(GLFW_RED_BITS, mode->redBits);
        glfwWindowHint(GLFW_GREEN_BITS, mode->greenBits);
        glfwWindowHint(GLFW_BLUE_BITS, mode->blueBits);
        glfwWindowHint(GLFW_REFRESH_RATE, mode->refreshRate);

        m_width = mode->width;
        m_height = mode->height;
        m_glfwWindow = glfwCreateWindow(mode->width, mode->height, GAME_NAME, monitor, nullptr);
    } else {
        m_width = width;
        m_height = height;

        m_glfwWindow = glfwCreateWindow(width, height, GAME_NAME, nullptr, nullptr);
    }

    if (m_glfwWindow == nullptr) {
        throw std::runtime_error("Failed to create window!");
    }

    glfwMakeContextCurrent(m_glfwWindow);
    if (!gladLoadGLLoader(reinterpret_cast<GLADloadproc>(glfwGetProcAddress))) {
        throw std::runtime_error("Failed to initialize GLAD");
    }

    glfwSetFramebufferSizeCallback(m_glfwWindow, &Window::resize);
    //V-Sync on
    glfwSwapInterval(1);
}

Window::~Window() {
    if (m_glfwWindow == nullptr) {
        return;
    }

    glfwDestroyWindow(m_glfwWindow);
    m_glfwWindow = nullptr;
}

Window::Window(Window &&other) noexcept
    : m_width(other.m_width), m_height(other.m_height), m_glfwWindow(other.m_glfwWindow) {
    other.m_glfwWindow = nullptr;
}

Window &Window::operator=(Window &&other) noexcept {
    if (&other == this) {
        return *this;
    }

    m_glfwWindow = other.m_glfwWindow;
    m_width = other.m_width;
    m_height = other.m_height;

    other.m_glfwWindow = nullptr;
    return *this;
}

bool Window::shouldCloseWindow() const {
    return glfwWindowShouldClose(m_glfwWindow);
}

int Window::getFramebufferWidth() const {
    int width;
    int height;
    glfwGetFramebufferSize(m_glfwWindow, &width, &height);

    return width;
}

int Window::getFramebufferHeight() const {
    int width;
    int height;
    glfwGetFramebufferSize(m_glfwWindow, &width, &height);

    return height;
}

void Window::swapBuffers() const {
    glfwSwapBuffers(m_glfwWindow);
}

#pragma region Events

void Window::resize(GLFWwindow *glfwWindow, [[maybe_unused]] const int width, [[maybe_unused]] const int height) {
    int fbWidth;
    int fbHeight;
    glfwGetFramebufferSize(glfwWindow, &fbWidth, &fbHeight);

    glViewport(0, 0, fbWidth, fbHeight);
}

#pragma endregion //Events
