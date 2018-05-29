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
thread texture2d<float> SurfaceTexture;
thread sampler SurfaceSampler;

ShaderContainer(
thread texture2d<float> SurfaceTexture_param, thread sampler SurfaceSampler_param
)
:
SurfaceTexture(SurfaceTexture_param), SurfaceSampler(SurfaceSampler_param)
{}
float4 FS( SimpleGui_Resources_Tex_FragmentInput input)
{
    return SurfaceTexture.sample(SurfaceSampler, input.TexCoords);
}


};

fragment float4 FS(SimpleGui_Resources_Tex_FragmentInput input [[ stage_in ]], texture2d<float> SurfaceTexture [[ texture(0) ]], sampler SurfaceSampler [[ sampler(0) ]])
{
return ShaderContainer(SurfaceTexture, SurfaceSampler).FS(input);
}
