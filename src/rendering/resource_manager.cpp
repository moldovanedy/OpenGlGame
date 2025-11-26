#include <rendering/resource_manager.hpp>

using namespace Rendering;

void ResourceManager::setShaderLoaderFunction(
    const std::function<std::shared_ptr<Resources::Shader>(const std::string &)> &fn
) {
    m_shaderLoaderFunction = fn;
}

void ResourceManager::setTextureLoaderFunction(
    const std::function<std::shared_ptr<Resources::Texture>(const std::string &)> &fn
) {
    m_textureLoaderFunction = fn;
}

void ResourceManager::setVertexBufferLoaderFunction(
    const std::function<std::shared_ptr<Resources::VertexBuffers>(const std::string &)> &fn
) {
    m_vertexBufferLoaderFunction = fn;
}

std::shared_ptr<Resources::Shader> ResourceManager::getShader(const std::string &id) {
    if (m_shaders.contains(id)) {
        return m_shaders[id];
    }

    if (m_shaderLoaderFunction == nullptr) {
        throw std::runtime_error("No texture loader function was set for the resource manager");
    }

    std::shared_ptr<Resources::Shader> givenValue = m_shaderLoaderFunction(id);
    m_shaders[id] = givenValue;
    return givenValue;
}

std::shared_ptr<Resources::Texture> ResourceManager::getTexture(const std::string &id) {
    if (m_textures.contains(id)) {
        return m_textures[id];
    }

    if (m_textureLoaderFunction == nullptr) {
        throw std::runtime_error("No texture loader function was set for the resource manager");
    }

    std::shared_ptr<Resources::Texture> givenValue = m_textureLoaderFunction(id);
    m_textures[id] = givenValue;
    return givenValue;
}

std::shared_ptr<Resources::VertexBuffers> ResourceManager::getVertexBuffers(const std::string &id) {
    if (m_vertexBuffers.contains(id)) {
        return m_vertexBuffers[id];
    }

    if (m_vertexBufferLoaderFunction == nullptr) {
        throw std::runtime_error("No vertex buffer loader function was set for the resource manager");
    }

    std::shared_ptr<Resources::VertexBuffers> givenValue = m_vertexBufferLoaderFunction(id);
    m_vertexBuffers[id] = givenValue;
    return givenValue;
}

std::shared_ptr<Resources::Shader> ResourceManager::createShader(
    const std::string &id,
    const char *vertexSource,
    const char *fragmentSource
) {
    const auto result = m_shaders.emplace(
        id,
        std::make_shared<Resources::Shader>(vertexSource, fragmentSource));

    if (!result.second) {
        throw std::runtime_error("Couldn't add the shader to the resources");
    }

    return result.first->second;
}

std::shared_ptr<Resources::VertexBuffers> ResourceManager::createVertexBuffers(
    const std::string &id,
    GLenum primitiveTopology,
    const float *vertices,
    uint32_t vertexCount,
    const std::vector<Resources::VertexBuffers::ShaderVertexAttributeDescriptor_t> &attributeDescriptors,
    const uint32_t *indices,
    uint32_t indexCount
) {
    const auto result = m_vertexBuffers.emplace(
        id,
        std::make_shared<Resources::VertexBuffers>(
            primitiveTopology,
            vertices,
            vertexCount,
            attributeDescriptors,
            indices,
            indexCount));

    if (!result.second) {
        throw std::runtime_error("Couldn't add the vertex buffer to the resources");
    }

    return result.first->second;
}
