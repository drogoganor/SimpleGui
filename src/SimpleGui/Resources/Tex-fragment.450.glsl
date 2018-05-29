#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
struct SimpleGui_Resources_Tex_VertexInput
{
    vec2 Position;
    vec2 TexCoords;
    vec4 Color;
};

struct SimpleGui_Resources_Tex_FragmentInput
{
    vec4 SystemPosition;
    vec2 TexCoords;
    vec4 Color;
};

layout(set = 1, binding = 0) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 1) uniform sampler SurfaceSampler;

vec4 FS( SimpleGui_Resources_Tex_FragmentInput input_)
{
    return texture(sampler2D(SurfaceTexture, SurfaceSampler), input_.TexCoords);
}


layout(location = 0) in vec2 fsin_0;
layout(location = 1) in vec4 fsin_1;
layout(location = 0) out vec4 _outputColor_;

void main()
{
    SimpleGui_Resources_Tex_FragmentInput input_;
    input_.SystemPosition = gl_FragCoord;
    input_.TexCoords = fsin_0;
    input_.Color = fsin_1;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}
