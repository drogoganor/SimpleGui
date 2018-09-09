#version 450

layout(location = 0) in vec4 Position;
layout(location = 1) in vec4 Color;

layout(location = 0) out vec4 OutputColor;

void main()
{
    OutputColor = Color;
}
