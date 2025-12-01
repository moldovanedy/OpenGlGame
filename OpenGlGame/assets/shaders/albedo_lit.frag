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

void main()
{
    //    fragColor = vec4(0.2, 0.4, 0.3, 1.0);
    fragColor = texture(albedoTexture, textureCoords);
}