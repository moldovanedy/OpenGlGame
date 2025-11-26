#include <iostream>
#include <ostream>
#include <stdexcept>
#include <glad/glad.h>

#include <rendering/resources/shader.hpp>

using namespace Rendering::Resources;

Shader::Shader(const char *vertexSrc, const char *fragmentSrg) {
    const uint32_t sVertex = glCreateShader(GL_VERTEX_SHADER);
    glShaderSource(sVertex, 1, &vertexSrc, nullptr);
    glCompileShader(sVertex);
    bool success = isCompiledSuccessfully(sVertex, ShaderType::Vertex);
    if (!success) {
        glDeleteShader(sVertex);
        throw std::runtime_error("Failed to compile vertex shader.");
    }

    const uint32_t sFragment = glCreateShader(GL_FRAGMENT_SHADER);
    glShaderSource(sFragment, 1, &fragmentSrg, nullptr);
    glCompileShader(sFragment);
    success = isCompiledSuccessfully(sFragment, ShaderType::Fragment);
    if (!success) {
        glDeleteShader(sVertex);
        glDeleteShader(sFragment);
        throw std::runtime_error("Failed to compile fragment shader.");
    }

    m_id = glCreateProgram();
    glAttachShader(m_id, sVertex);
    glAttachShader(m_id, sFragment);

    glLinkProgram(m_id);
    success = isCompiledSuccessfully(m_id, ShaderType::CompiledProgram);
    if (!success) {
        glDeleteShader(sVertex);
        glDeleteShader(sFragment);
        throw std::runtime_error("Failed to link final shader program.");
    }

    //delete the shaders as they're linked into the program
    glDeleteShader(sVertex);
    glDeleteShader(sFragment);
}

Shader::~Shader() {
    glDeleteProgram(m_id);
    m_id = 0;
}

bool Shader::isCompiledSuccessfully(const uint32_t shaderId, const ShaderType type) {
    int success;
    if (type == ShaderType::CompiledProgram) {
        glGetProgramiv(shaderId, GL_LINK_STATUS, &success);
        if (!success) {
#ifndef NDEBUG
            char errorBuffer[1024];
            glGetProgramInfoLog(shaderId, 1024, nullptr, errorBuffer);
            std::cout << "Shader linking error: " << errorBuffer << std::endl;
#endif

            return false;
        }
    } else {
        glGetShaderiv(shaderId, GL_COMPILE_STATUS, &success);
        if (!success) {
#ifndef NDEBUG
            char errorBuffer[1024];
            glGetShaderInfoLog(shaderId, 1024, nullptr, errorBuffer);
            std::cout <<
                    "Shader compilation error (" <<
                    std::to_string(static_cast<int>(type)) <<
                    "): " <<
                    errorBuffer <<
                    std::endl;
#endif

            return false;
        }
    }

    return true;
}
