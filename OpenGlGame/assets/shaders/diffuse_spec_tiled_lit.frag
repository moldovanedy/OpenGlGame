#version 330 core

struct Material {
    sampler2D diffuseTex;
    sampler2D specularTex;
    float shininess;
};

struct Light {
    vec3 worldPos;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constantAttenuation;
    float linearAttenuation;
    float quadraticAttenuation;
};

in LIGHTING_DATA {
    vec3 worldSpaceNormal;
    vec3 fragPos;
} inLightingData;

in vec2 textureCoords;

out vec4 fragColor;

uniform vec3 viewPos;
uniform Material material;
uniform Light lights[10];
uniform int lightCount;

uniform vec2 tiling;
uniform vec2 offset;

void main()
{
    vec2 uv = textureCoords;
    uv *= tiling + offset;
    uv = fract(uv);

    vec3 finalOutput = vec3(0.0);

    for (int i = 0; i < lightCount; i++) {
        // ambient
        vec3 ambient = lights[i].ambient * texture(material.diffuseTex, uv).rgb;

        // diffuse 
        vec3 norm = normalize(inLightingData.worldSpaceNormal);
        vec3 lightDir = normalize(lights[i].worldPos - inLightingData.fragPos);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = lights[i].diffuse * (diff * texture(material.diffuseTex, uv).rgb);

        // specular
        vec3 viewDir = normalize(viewPos - inLightingData.fragPos);
        vec3 reflectDir = reflect(-lightDir, norm);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
        vec3 specular = lights[i].specular * (spec * texture(material.specularTex, uv).rgb);

        float distance = length(lights[i].worldPos - inLightingData.fragPos);
        float attenuation = 1.0 /
        (lights[i].constantAttenuation + lights[i].linearAttenuation * distance
        + lights[i].quadraticAttenuation * (distance * distance));

        ambient *= attenuation;
        diffuse *= attenuation;
        specular *= attenuation;

        finalOutput += ambient + diffuse + specular;
    }

    fragColor = vec4(finalOutput, 1.0);
}