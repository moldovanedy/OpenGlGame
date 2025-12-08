#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out LIGHTING_DATA {
    vec3 worldSpaceNormal;
    vec3 fragPos;
} outLightingData;

out vec2 textureCoords;

uniform mat4 viewProjection;
uniform mat4 model;
uniform mat4 transposedInverseModel;

void main()
{
    outLightingData.worldSpaceNormal = mat3(transposedInverseModel) * aNormal;
    textureCoords = aTexCoords;
    outLightingData.fragPos = vec3(vec4(aPos, 1.0) * model);

    gl_Position = vec4(aPos, 1.0) * model * viewProjection;
}