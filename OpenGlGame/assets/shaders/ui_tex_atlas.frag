#version 330 core

in vec2 textureCoords;

out vec4 fragColor;

uniform vec4 atlasOffsetAndSize;
uniform vec4 color;

uniform sampler2D atlasTex;

void main() {
    vec2 tileUV = fract(textureCoords);
    vec2 uv;
    uv.x = atlasOffsetAndSize.x + tileUV.x * atlasOffsetAndSize.z;
    // invert Y, as OpenGL goes from the bottom to top
    uv.y = 1.0 - (atlasOffsetAndSize.y + atlasOffsetAndSize.w) + tileUV.y * atlasOffsetAndSize.w;

    fragColor = color * texture(atlasTex, uv);
}