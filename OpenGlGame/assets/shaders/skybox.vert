#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;

out vec3 textureCoords;

uniform mat4 view;
uniform mat4 projection;

void main()
{
    //flip the images
    textureCoords = vec3(aPos.x, -aPos.y, aPos.z);
    vec4 finalPos = projection * view * vec4(aPos, 1.0);
    gl_Position = finalPos.xyww;
}