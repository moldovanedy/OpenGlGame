#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

out LIGHTING_DATA {
    vec3 worldSpaceNormal;
    vec3 fragPos;
} outLightingData;

out vec2 textureCoords;

uniform float time;
uniform mat4 viewProjection;
uniform mat4 model;
uniform mat4 transposedInverseModel;

const float WAVE_HEIGHT = 0.07;
const float WAVE_LENGTH = 0.1;

void main()
{
    outLightingData.worldSpaceNormal = aNormal * mat3(transposedInverseModel);
    textureCoords = aTexCoords;
    outLightingData.fragPos = vec3(vec4(aPos, 1.0) * model);

    vec4 displacedPos = vec4(aPos, 1.0);
    float wave = sin(aPos.z / WAVE_LENGTH + time * 3.0) * cos(aPos.x / (WAVE_LENGTH * 2.0) + time * 2.0) * WAVE_HEIGHT;
    displacedPos.y += wave;

    gl_Position = displacedPos * model * viewProjection;
}