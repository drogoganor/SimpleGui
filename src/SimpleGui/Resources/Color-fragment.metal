#include <metal_stdlib>
using namespace metal;
struct SimpleGui_Resources_Color_VertexInput
{
    float2 Position [[ attribute(0) ]];
    float4 Color [[ attribute(1) ]];
};

struct SimpleGui_Resources_Color_FragmentInput
{
    float4 SystemPosition [[ position ]];
    float4 Color [[ attribute(0) ]];
};

struct ShaderContainer {

ShaderContainer(

)
{}
float4 FS( SimpleGui_Resources_Color_FragmentInput input)
{
    return input.Color;
}


};

fragment float4 FS(SimpleGui_Resources_Color_FragmentInput input [[ stage_in ]])
{
return ShaderContainer().FS(input);
}
