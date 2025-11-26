#ifndef GAME_APPLICATION_HPP
#define GAME_APPLICATION_HPP

#include <memory>

#include "window.hpp"
#include <rendering/renderer.hpp>
#include <rendering/resource_manager.hpp>

class Application {
    const std::unique_ptr<Rendering::Renderer> m_renderer;
    const std::unique_ptr<Rendering::ResourceManager> m_resourceManager;
    const std::unique_ptr<Window> m_window;

    static std::shared_ptr<Rendering::Resources::Shader> shaderFileLoader(const std::string &relPath);

    // std::shared_ptr<Rendering::Resources::Texture> textureFileLoader(const std::string &relPath);
    //
    // std::shared_ptr<Rendering::Resources::VertexBuffers> vertexBuffersLoader(const std::string &relPath);

public:
    explicit Application();

    [[nodiscard]] Rendering::Renderer &getRenderer() const { return *m_renderer; }
    [[nodiscard]] Rendering::ResourceManager &getResourceManager() const { return *m_resourceManager; }

    void run() const;
};

#endif //GAME_APPLICATION_HPP
