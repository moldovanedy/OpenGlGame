#version 330 core

layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoords;

out vec2 textureCoords;

uniform mat4 model;

void main()
{
    textureCoords = aTexCoords;
    gl_Position = vec4(aPos, 0.0, 1.0) * model;
}