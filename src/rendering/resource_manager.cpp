#include <rendering/resource_manager.hpp>

using namespace Rendering;

ResourceManager::ResourceManager() {
    m_freeShaderResIds.push(0);
    m_freeTextureResIds.push(0);
    m_freeVertBufResIds.push(0);
    m_freeVertAttribDescriptorsResIds.push(0);
}

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
    m_vertBufLoaderFunction = fn;
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

    if (m_vertBufLoaderFunction == nullptr) {
        throw std::runtime_error("No vertex buffer loader function was set for the resource manager");
    }

    std::shared_ptr<Resources::VertexBuffers> givenValue = m_vertBufLoaderFunction(id);
    m_vertexBuffers[id] = givenValue;
    return givenValue;
}

std::shared_ptr<Resources::VertexAttributeDescriptors> ResourceManager::getVertexAttributeDescriptors(
    const std::string &id
) {
    if (m_vertexAttributeDescriptors.contains(id)) {
        return m_vertexAttributeDescriptors[id];
    }

    throw std::runtime_error("The vertex attribute descriptors with the specified ID was not found");
}

std::shared_ptr<Resources::Shader> ResourceManager::createShader(
    const std::string &id,
    const char *vertexSource,
    const char *fragmentSource
) {
    if (m_freeShaderResIds.empty()) {
        throw std::runtime_error("Out of shader slots!");
    }

    const auto result = m_shaders.emplace(
        id,
        std::make_shared<Resources::Shader>(m_freeShaderResIds.front(), vertexSource, fragmentSource));

    m_freeShaderResIds.pop();
    if (m_freeShaderResIds.empty() && m_shaders.size() != 4096) {
        m_freeShaderResIds.push(m_shaders.size());
    }

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
    const Resources::VertexAttributeDescriptors &attributeDescriptors,
    const uint32_t *indices,
    uint32_t indexCount
) {
    if (m_freeVertBufResIds.empty()) {
        throw std::runtime_error("Out of vertex buffer slots!");
    }

    const auto result = m_vertexBuffers.emplace(
        id,
        std::make_shared<Resources::VertexBuffers>(
            m_freeVertBufResIds.front(),
            primitiveTopology,
            vertices,
            vertexCount,
            attributeDescriptors,
            indices,
            indexCount));

    m_freeVertBufResIds.pop();
    if (m_freeVertBufResIds.empty() && m_vertexBuffers.size() != 4096) {
        m_freeVertBufResIds.push(m_vertexBuffers.size());
    }

    if (!result.second) {
        throw std::runtime_error("Couldn't add the vertex buffer to the resources");
    }

    return result.first->second;
}

std::shared_ptr<Resources::VertexAttributeDescriptors> ResourceManager::createVertexAttributeDescriptors(
    const std::string &id, const std::vector<Resources::VertexAttributeDescriptor_t> &attributes
) {
    if (m_freeVertAttribDescriptorsResIds.empty()) {
        throw std::runtime_error("Out of vertex attribute descriptor slots!");
    }

    const auto result = m_vertexAttributeDescriptors.emplace(
        id,
        std::make_shared<Resources::VertexAttributeDescriptors>(m_freeVertAttribDescriptorsResIds.front(), attributes));

    m_freeVertAttribDescriptorsResIds.pop();
    if (m_freeVertAttribDescriptorsResIds.empty() && m_vertexAttributeDescriptors.size() != 4096) {
        m_freeVertAttribDescriptorsResIds.push(m_vertexAttributeDescriptors.size());
    }

    if (!result.second) {
        throw std::runtime_error("Couldn't add the vertex attribute descriptors to the resources");
    }

    return result.first->second;
}
