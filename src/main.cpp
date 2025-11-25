#include <iostream>

#include "application.hpp"

int main() {
    try {
        const auto app = Application();
        app.run();
        return 0;
    } catch (const std::exception &e) {
        std::cerr << "Unhandled exception: " << e.what() << std::endl;
        return -1;
    }
}
