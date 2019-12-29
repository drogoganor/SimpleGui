#version 450

layout(set = 1, binding = 0) uniform texture2D SourceTexture;
layout(set = 1, binding = 1) uniform sampler SourceSampler;

layout(location = 0) in vec4 Position0;
layout(location = 1) in vec2 TexCoords0;
layout(location = 2) in vec4 Color0;

layout(location = 0) out vec4 OutputColor;

void main()
{
    OutputColor = texture(sampler2D(SourceTexture, SourceSampler), TexCoords0);
}
