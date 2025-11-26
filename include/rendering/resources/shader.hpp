#ifndef GAME_RENDERING_RESOURCES_SHADER_HPP
#define GAME_RENDERING_RESOURCES_SHADER_HPP

#include <cstdint>

#include "resource.hpp"

namespace Rendering::Resources {
    class Shader : public Resource {
        enum class ShaderType {
            Vertex,
            Fragment,
            CompiledProgram,
        };

        uint32_t m_id;

        static bool isCompiledSuccessfully(uint32_t shaderId, ShaderType type);

    public:
        Shader(const char *vertexSrc, const char *fragmentSrg);

        ~Shader();

        Shader(Shader &) = delete;

        Shader &operator=(Shader &) = delete;

        Shader(Shader &&) = delete;

        Shader &operator=(Shader &&) = delete;

        [[nodiscard]] uint32_t getGlId() const { return m_id; }
    };
}

#endif //GAME_RENDERING_RESOURCES_SHADER_HPP
