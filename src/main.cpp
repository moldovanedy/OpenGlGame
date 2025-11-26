#include <iostream>

#include "application.hpp"

int main() {
    const int result = glfwInit();
    if (result == GLFW_FALSE) {
        throw std::runtime_error("Failed to initialize GLFW");
    }

    try {
        const auto app = Application();
        app.run();
    } catch (const std::exception &e) {
        std::cerr << "Unhandled exception: " << e.what() << std::endl;
        glfwTerminate();
        return -1;
    }

    //we need to terminate GLFW only after the application object is destroyed, otherwise we'll have crashes
    glfwTerminate();
    return 0;
}
