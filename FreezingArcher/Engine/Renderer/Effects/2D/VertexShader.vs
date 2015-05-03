#version 410

layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec4 InColor;
layout(location = 2) in vec2 InTexCoord1;

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


void main() 
{
	gl_Position = ProjectionMatrix * WorldMatrix * vec4(InPosition, 1.0);

    OutTexCoord1 = InTexCoord1;
    OutColor = InColor;
}
