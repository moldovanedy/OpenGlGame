#ifndef GAME_RENDERING_RESOURCE_MANAGER_HPP
#define GAME_RENDERING_RESOURCE_MANAGER_HPP

#include <memory>
#include <unordered_map>
#include <functional>

#include "resources/shader.hpp"
#include "resources/texture.hpp"
#include "resources/vertex_buffers.hpp"

namespace Rendering {
    class ResourceManager {
    public:
        enum class ResourceTypeFlags {
            eNone = 0,
            eShader = 1,
            eTexture = 2,
            eVertexBuffer = 4
        };

        friend ResourceTypeFlags operator|(ResourceTypeFlags op1, const ResourceTypeFlags &op2) {
            return static_cast<ResourceTypeFlags>(static_cast<uint32_t>(op1) | static_cast<uint32_t>(op2));
        }

        friend ResourceTypeFlags operator&(ResourceTypeFlags op1, const ResourceTypeFlags &op2) {
            return static_cast<ResourceTypeFlags>(static_cast<uint32_t>(op1) & static_cast<uint32_t>(op2));
        }

        friend ResourceTypeFlags operator|=(ResourceTypeFlags op1, const ResourceTypeFlags &op2) {
            return static_cast<ResourceTypeFlags>(static_cast<uint32_t>(op1) | static_cast<uint32_t>(op2));
        }

        friend ResourceTypeFlags operator&=(ResourceTypeFlags op1, const ResourceTypeFlags &op2) {
            return static_cast<ResourceTypeFlags>(static_cast<uint32_t>(op1) & static_cast<uint32_t>(op2));
        }

    private:
        std::unordered_map<std::string, std::shared_ptr<Resources::Shader> > m_shaders;
        std::unordered_map<std::string, std::shared_ptr<Resources::Texture> > m_textures;
        std::unordered_map<std::string, std::shared_ptr<Resources::VertexBuffers> > m_vertexBuffers;

        std::function<std::shared_ptr<Resources::Shader>(const std::string &)> m_shaderLoaderFunction = nullptr;
        std::function<std::shared_ptr<Resources::Texture>(const std::string &)> m_textureLoaderFunction = nullptr;
        std::function<std::shared_ptr<Resources::VertexBuffers>(const std::string &)> m_vertexBufferLoaderFunction =
                nullptr;

    public:
        void setShaderLoaderFunction(const std::function<std::shared_ptr<Resources::Shader>(const std::string &)> &fn);

        void setTextureLoaderFunction(
            const std::function<std::shared_ptr<Resources::Texture>(const std::string &)> &fn);

        void setVertexBufferLoaderFunction(
            const std::function<std::shared_ptr<Resources::VertexBuffers>(const std::string &)> &fn);

        std::shared_ptr<Resources::Shader> getShader(const std::string &id);

        std::shared_ptr<Resources::Texture> getTexture(const std::string &id);

        std::shared_ptr<Resources::VertexBuffers> getVertexBuffers(const std::string &id);

        std::shared_ptr<Resources::Shader> createShader(
            const std::string &id,
            const char *vertexSource,
            const char *fragmentSource);

        std::shared_ptr<Resources::VertexBuffers> createVertexBuffers(
            const std::string &id,
            GLenum primitiveTopology,
            const float *vertices,
            uint32_t vertexCount,
            const std::vector<Resources::VertexBuffers::ShaderVertexAttributeDescriptor_t> &attributeDescriptors,
            const uint32_t *indices = nullptr,
            uint32_t indexCount = 0);
    };
}

#endif //GAME_RENDERING_RESOURCE_MANAGER_HPP
