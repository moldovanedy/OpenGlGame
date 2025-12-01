#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out vec3 fragPos;
out vec3 worldSpaceNormal;
out vec2 textureCoords;

uniform mat4 viewProjection;
uniform mat4 model;
uniform mat4 transposedInverseModel;

void main()
{
    fragPos = vec3(vec4(aPos, 1.0) * model);
    worldSpaceNormal = mat3(transposedInverseModel) * aNormal;
    textureCoords = aTexCoords;

    gl_Position = vec4(aPos, 1.0) * model * viewProjection;
}