#version 450

layout(set = 0, binding = 0) uniform Projection
{
    mat4 _Proj;
};

layout(set = 0, binding = 1) uniform World
{
    mat4 _World;
};

layout(location = 0) in vec2 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 2) in vec4 Color;

layout(location = 0) out vec4 fsin_Position;
layout(location = 1) out vec2 fsin_TexCoords;
layout(location = 2) out vec4 fsin_Color;

void main()
{
    vec4 outPos = _Proj * _World * vec4(Position.x, Position.y, 0, 1);
    gl_Position = outPos;
    fsin_Position = outPos;
    fsin_TexCoords = TexCoords;
    fsin_Color = Color;
}
