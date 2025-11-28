#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

out vec3 fragPos;
out vec3 worldSpaceNormal;

layout (std140) uniform Matrices
{
    mat4 viewProjectionMat;
};

uniform mat4 model;
uniform mat4 transposedInversedModel;

void main()
{
    fragPos = vec3(model * vec4(aPos, 1.0));
    Normal = mat3(transposedInversedModel) * aNormal;

    gl_Position = viewProjectionMat * vec4(fragPos, 1.0);
}