#version 330 core

struct SamplerDummy { int _dummyValue; };
struct SamplerComparisonDummy { int _dummyValue; };

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


in vec4 fsin_0;
out vec4 _outputColor_;

void main()
{
    SimpleGui_Resources_Color_FragmentInput input_;
    input_.SystemPosition = gl_FragCoord;
    input_.Color = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}
