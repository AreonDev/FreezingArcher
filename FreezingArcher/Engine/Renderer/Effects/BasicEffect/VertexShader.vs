#version 410

//Input format
//####################################################
layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec3 InNormal;
layout(location = 2) in vec3 InTangent;
layout(location = 3) in vec3 InBiTangent;

layout(location = 4) in vec3 InTexCoord1;
layout(location = 5) in vec3 InTexCoord2;
layout(location = 6) in vec3 InTexCoord3;

layout(location = 7) in vec4 InColor1;
layout(location = 8) in vec4 InColor2;
layout(location = 9) in vec4 InColor3;

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
layout(location = 0) out vec3 OutTexCoord1;
layout(location = 1) out vec3 OutTexCoord2;
layout(location = 2) out vec3 OutTexCoord3;

layout(location = 3) out vec4 OutColor1;
layout(location = 4) out vec4 OutColor2;
layout(location = 5) out vec4 OutColor3;
//####################################################
uniform MatricesBlock
{
        mat4 WorldMatrix;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;
};

uniform LightBlock
{
        vec4 LightColor;
        vec3 LightPosition;
};

uniform int InstancedDrawing;

void main() 
{
        vec4 position = vec4(InPosition, 1.0f) + vec4(InstancedDrawing, InstancedDrawing, InstancedDrawing, 1);

        if(InstancedDrawing == 1)
                gl_Position = ProjectionMatrix * ViewMatrix * WorldMatrix * InInstanceWorld * position;
        else
                gl_Position = ProjectionMatrix * ViewMatrix * WorldMatrix * position;

        gl_Position *= 0.000001;

        OutTexCoord1 = InTexCoord1;
	OutTexCoord2 = InTexCoord2;
	OutTexCoord3 = InTexCoord3;

        OutColor1 = gl_Position;
        OutColor2 = InColor2;
        OutColor3 = InColor3;
}
