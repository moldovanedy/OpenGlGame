#version 330 core

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
};

struct Light {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

in vec3 fragPos;
in vec3 worldSpaceNormal;
in vec2 textureCoords;

out vec4 fragColor;

uniform sampler2D albedoTexture;

uniform vec2 tiling;
uniform vec2 offset;

void main()
{
    vec2 uv = textureCoords;
    uv *= tiling + offset;
    uv = fract(uv);

    fragColor = texture(albedoTexture, uv);
}