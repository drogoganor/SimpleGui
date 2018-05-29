#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
struct SimpleGui_Resources_Color_VertexInput
{
    vec2 Position;
    vec4 Color;
};

struct SimpleGui_Resources_Color_FragmentInput
{
    vec4 SystemPosition;
    vec4 Color;
};


vec4 FS( SimpleGui_Resources_Color_FragmentInput input_)
{
    return input_.Color;
}


layout(location = 0) in vec4 fsin_0;
layout(location = 0) out vec4 _outputColor_;

void main()
{
    SimpleGui_Resources_Color_FragmentInput input_;
    input_.SystemPosition = gl_FragCoord;
    input_.Color = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}
