#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;

struct Light {
    vec3 worldPos;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constantAttenuation;
    float linearAttenuation;
    float quadraticAttenuation;
};

out LIGHTING_DATA {
    vec3 worldSpaceNormal;
    vec3 tangentLightPos[10];
    vec3 tangentViewPos;
    vec3 tangentFragPos;
} outLightingData;

out vec2 textureCoords;

uniform mat4 viewProjection;
uniform mat4 model;
uniform mat4 transposedInverseModel;

uniform vec3 viewPos;

//common with frag
uniform Light lights[10];
uniform int lightCount;

void main()
{
    textureCoords = aTexCoords;

    mat3 normalMatrix = mat3(transposedInverseModel);
    vec3 T = normalize(aTangent * normalMatrix);
    vec3 N = normalize(aNormal * normalMatrix);
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N, T);

    mat3 TBN = transpose(mat3(T, B, N));

    vec3 worldPos = vec3(vec4(aPos, 1.0) * model);
    outLightingData.tangentViewPos = TBN * viewPos;
    outLightingData.tangentFragPos = TBN * worldPos;

    for (int i = 0; i < lightCount; i++) {
        outLightingData.tangentLightPos[i] = TBN * lights[i].worldPos;
    }

    gl_Position = vec4(aPos, 1.0) * model * viewProjection;
}