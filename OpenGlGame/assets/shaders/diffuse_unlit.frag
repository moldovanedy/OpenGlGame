#version 330 core

in vec2 textureCoords;

out vec4 fragColor;

uniform sampler2D diffuseTexture;

void main()
{
    fragColor = texture(diffuseTexture, textureCoords);
}