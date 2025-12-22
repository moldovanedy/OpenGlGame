#version 330 core

struct Material {
    sampler2D diffuseTex;
    float specularCol;
    sampler2D normalTex;
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
    vec3 tangentLightPos[10];
    vec3 tangentViewPos;
    vec3 tangentFragPos;
} inLightingData;

in vec2 textureCoords;

out vec4 fragColor;

//common with vert
uniform Light lights[10];
uniform int lightCount;

uniform Material material;

uniform vec2 tiling;
uniform vec2 offset;

void main()
{
    vec2 uv = textureCoords;
    uv *= tiling + offset;
    uv = fract(uv);

    // obtain normal from normal map in range [0,1]
    vec3 normalFrag = texture(material.normalTex, uv).rgb;
    // transform normal vector to range [-1,1]
    normalFrag = normalize(normalFrag * 2.0 - 1.0);  // this normal is in tangent space

    vec3 finalOutput = vec3(0.0);

    for (int i = 0; i < lightCount; i++) {
        // ambient
        vec3 ambient = lights[i].ambient * texture(material.diffuseTex, uv).rgb;

        // diffuse 
        vec3 lightDir = normalize(inLightingData.tangentLightPos[i] - inLightingData.tangentFragPos);
        float diff = max(dot(normalFrag, lightDir), 0.0);
        vec3 diffuse = lights[i].diffuse * (diff * texture(material.diffuseTex, uv).rgb);

        // specular
        vec3 viewDir = normalize(inLightingData.tangentViewPos - inLightingData.tangentFragPos);
        vec3 reflectDir = reflect(-lightDir, normalFrag);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
        vec3 specular = lights[i].specular * (spec * material.specularCol);

        float dist = length(inLightingData.tangentLightPos[i] - inLightingData.tangentFragPos);
        float attenuation = 1.0 /
        (lights[i].constantAttenuation + lights[i].linearAttenuation * dist
        + lights[i].quadraticAttenuation * (dist * dist));

        ambient *= attenuation;
        diffuse *= attenuation;
        specular *= attenuation;

        finalOutput += ambient + diffuse + specular;
    }

    fragColor = vec4(finalOutput, 1.0);
}