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

layout(location = 6) out vec4 OutNormal;
layout(location = 7) out vec4 OutTangent;
layout(location = 8) out vec4 OutBinormal;

layout(location = 9) out mat4 OutNormalMatrix;


uniform LightBlock
{
        vec4 LightColor;
        vec3 LightPosition;
};
       
uniform MatricesBlock
{
        mat4 WorldMatrix;
        mat4 ViewMatrix;
        mat4 ProjectionMatrix;
};

void main()
{
        gl_Position = ProjectionMatrix * ViewMatrix * WorldMatrix * vec4(InPosition, 1.0);

        OutNormalMatrix = ViewMatrix * WorldMatrix;
        OutNormal = vec4(InNormal, 1.0);

        OutTexCoord1 = InTexCoord1;
        OutTexCoord2 = InTexCoord2;
        OutTexCoord3 = InTexCoord3;

        OutColor1 = InColor1;
        OutColor2 = InColor2;
        OutColor3 = InColor3;
}