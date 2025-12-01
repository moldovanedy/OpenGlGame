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
in vec3 vertColors;

out vec4 fragColor;

void main()
{
    fragColor = vec4(vertColors, 1.0);
}