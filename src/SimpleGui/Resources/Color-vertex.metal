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
constant float4x4& World;
constant float4x4& Projection;

ShaderContainer(
constant float4x4& World_param, constant float4x4& Projection_param
)
:
World(World_param), Projection(Projection_param)
{}
SimpleGui_Resources_Color_FragmentInput VS( SimpleGui_Resources_Color_VertexInput input)
{
    SimpleGui_Resources_Color_FragmentInput output;
    float4 outPos = World * float4(float4(input.Position, 0, 1));
    outPos = Projection * float4(outPos);
    output.SystemPosition = outPos;
    output.Color = input.Color;
    return output;
}


};

vertex SimpleGui_Resources_Color_FragmentInput VS(SimpleGui_Resources_Color_VertexInput input [[ stage_in ]], constant float4x4 &Projection [[ buffer(0) ]], constant float4x4 &World [[ buffer(1) ]])
{
return ShaderContainer(World, Projection).VS(input);
}
