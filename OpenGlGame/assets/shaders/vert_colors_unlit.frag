#version 330 core

in vec3 worldSpaceNormal;
in vec3 vertColors;

out vec4 fragColor;

void main()
{
    fragColor = vec4(vertColors, 1.0);
}