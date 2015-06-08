#version 410

//Input format
//####################################################
layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec3 InNormal;
layout(location = 2) in vec3 InTangent;
layout(location = 3) in vec3 InBiNormal;

layout(location = 4) in vec3 InTexCoord1;

layout(location = 7) in vec4 InColor1;

layout(location = 10) in mat4 InInstanceWorld;
layout(location = 14) in vec4 InOther1;
layout(location = 15) in vec4 InOther2;
//####################################################

//Output per Pixel
//####################################################
out gl_PerVertex
{
        vec4 gl_Position;
        float gl_PointSize;
        float gl_ClipDistance[];
};
//####################################################

//Output format
//####################################################
layout(location = 0) out vec4 hpos; // Position (Clip space)
layout(location = 1) out vec2 texcoord; // texture coordinate
layout(location = 2) out vec3 vpos; // Position (View Space)
layout(location = 3) out vec3 normal; // surface normal (view space)
layout(location = 4) out vec3 tangent; // tangent vector (view space)
layout(location = 5) out vec3 binormal; // binormal vector (view space)
//####################################################
 
uniform mat4 WorldMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;

void main()
{
        // Vertex position in clip space
        hpos = ProjectionMatrix * ViewMatrix * WorldMatrix * vec4(InPosition, 1.0);

        //copy texture coordinates
        texcoord = InTexCoord1.xy;

        mat4 modelviewrot = ViewMatrix * WorldMatrix;

        //Vertex position in view space (with model transformations)
        vpos = vec3(modelviewrot * vec4(InPosition, 1.0));

        //Tangent space vectors in view space (with model transformations)
        normal = (modelviewrot * vec4(InNormal, 1.0)).xyz;
        tangent = (modelviewrot * vec4(InTangent, 1.0)).xyz;
        binormal = (modelviewrot * vec4(InBiNormal, 1.0)).xyz;
}