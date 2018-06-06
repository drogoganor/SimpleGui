#include <metal_stdlib>
using namespace metal;
struct SimpleGui_Resources_Tex_VertexInput
{
    float2 Position [[ attribute(0) ]];
    float2 TexCoords [[ attribute(1) ]];
    float4 Color [[ attribute(2) ]];
};

struct SimpleGui_Resources_Tex_FragmentInput
{
    float4 SystemPosition [[ position ]];
    float2 TexCoords [[ attribute(0) ]];
    float4 Color [[ attribute(1) ]];
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
SimpleGui_Resources_Tex_FragmentInput VS( SimpleGui_Resources_Tex_VertexInput input)
{
    SimpleGui_Resources_Tex_FragmentInput output;
    float4 outPos = World * float4(float4(input.Position, 0, 1));
    outPos = Projection * float4(outPos);
    output.SystemPosition = outPos;
    output.Color = input.Color;
    output.TexCoords = input.TexCoords;
    return output;
}


};

vertex SimpleGui_Resources_Tex_FragmentInput VS(SimpleGui_Resources_Tex_VertexInput input [[ stage_in ]], constant float4x4 &Projection [[ buffer(0) ]], constant float4x4 &World [[ buffer(1) ]])
{
return ShaderContainer(World, Projection).VS(input);
}
