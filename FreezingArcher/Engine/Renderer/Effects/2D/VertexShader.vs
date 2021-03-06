#version 410

layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec4 InColor;
layout(location = 2) in vec2 InTexCoord1;

layout(location = 3) in mat4 InInstanceWorldMatrix;
layout(location = 7) in vec4 InInstanceColor;
layout(location = 8) in vec4 InOther;

layout(location = 11) in vec2 InTexCoordOverride;

out gl_PerVertex
{
    vec4 gl_Position;
    float gl_PointSize;
    float gl_ClipDistance[];
};

layout(location = 0) out vec4 OutColor;
layout(location = 1) out vec2 OutTexCoord1;

uniform MatricesBlock
{
    mat4 WorldMatrix;
    mat4 ProjectionMatrix;
};

uniform int InstancedDrawing;

uniform int OverrideTextureCoords;

void main() 
{
    if(InstancedDrawing == 1)
        gl_Position = ProjectionMatrix * WorldMatrix * InInstanceWorldMatrix * vec4(InPosition, 1.0);
    else
        gl_Position = ProjectionMatrix * WorldMatrix * vec4(InPosition, 1.0);

    if(OverrideTextureCoords == 1)
        OutTexCoord1 = InTexCoordOverride;
    else
        OutTexCoord1 = InTexCoord1;

    OutColor = InColor * (1 - InInstanceColor.a) + InInstanceColor;
}
