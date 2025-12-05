#version 330 core

in vec3 worldSpaceNormal;
in vec2 textureCoords;

out vec4 fragColor;

uniform sampler2D diffuseTexture;

uniform vec2 tiling;
uniform vec2 offset;

void main()
{
    vec2 uv = textureCoords;
    uv *= tiling + offset;
    uv = fract(uv);

    fragColor = texture(diffuseTexture, uv);
}