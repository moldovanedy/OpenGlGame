#include <application.hpp>
#include <fstream>
#include <sstream>

float vertices[] = {
    // positions         // colors
    0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, // bottom right
    -0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, // bottom left
    0.0f, 0.5f, 0.0f, 0.0f, 0.0f, 1.0f // top 
};

Application::Application()
    : m_renderer(std::make_unique<Rendering::Renderer>()),
      m_resourceManager(std::make_unique<Rendering::ResourceManager>()),
      m_window(std::make_unique<Window>(false)) {
    m_resourceManager->setShaderLoaderFunction(&Application::shaderFileLoader);
}

std::shared_ptr<Rendering::Resources::Shader> Application::shaderFileLoader(const std::string &relPath) {
    std::ifstream vShaderFile;
    std::ifstream fShaderFile;

    vShaderFile.exceptions(std::ifstream::failbit | std::ifstream::badbit);
    fShaderFile.exceptions(std::ifstream::failbit | std::ifstream::badbit);

    vShaderFile.open(relPath + ".vert");
    fShaderFile.open(relPath + ".frag");
    std::stringstream vShaderStream, fShaderStream;
    vShaderStream << vShaderFile.rdbuf();
    fShaderStream << fShaderFile.rdbuf();

    vShaderFile.close();
    fShaderFile.close();

    std::string vertexString = vShaderStream.str();
    std::string fragmentString = fShaderStream.str();
    const char *vertexCode = vertexString.c_str();
    const char *fragmentCode = fragmentString.c_str();

    //TODO: this is not OK, we need a resource ID
    return std::make_shared<Rendering::Resources::Shader>(0, vertexCode, fragmentCode);
}

// std::shared_ptr<Rendering::Resources::Texture> Application::textureFileLoader(const std::string &relPath) {
// }
//
// std::shared_ptr<Rendering::Resources::VertexBuffers> Application::vertexBuffersLoader(const std::string &relPath) {
// }

void Application::run() const {
    glEnable(GL_MULTISAMPLE);
    glEnable(GL_DEPTH_TEST);

    const std::shared_ptr<Rendering::Resources::VertexAttributeDescriptors> vertexAttributeDescriptors =
            m_resourceManager->createVertexAttributeDescriptors(
                "triangle_with_vertex_colors",
                {
                    {0, 12, GL_FLOAT, 24, 0},
                    {1, 12, GL_FLOAT, 24, 12}
                });

    const std::shared_ptr<Rendering::Resources::Shader> shader =
            m_resourceManager->getShader("shaders/basic_unlit");
    const std::shared_ptr<Rendering::Resources::VertexBuffers> vertexBuffers =
            m_resourceManager->createVertexBuffers(
                "triangle",
                GL_TRIANGLES,
                vertices,
                3,
                *vertexAttributeDescriptors
            );

    while (!m_window->shouldCloseWindow()) {
        const auto params = Rendering::Renderer::RenderParams_t{
            .FramebufferWidth = m_window->getFramebufferWidth(),
            .FramebufferHeight = m_window->getFramebufferHeight(),
        };

        m_renderer->clearCommands();
        m_renderer->addCommand(Rendering::DrawOperation(*shader, *vertexBuffers, *vertexAttributeDescriptors));

        m_renderer->render(params);
        m_window->swapBuffers();

        glfwWaitEventsTimeout(0.01);
    }
}
