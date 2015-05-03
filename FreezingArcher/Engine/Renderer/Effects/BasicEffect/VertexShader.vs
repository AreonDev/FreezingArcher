#version 410

//Input format
//####################################################
layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec3 InNormal;

layout(location = 2) in vec2 InTexCoord1;
layout(location = 3) in vec2 InTexCoord2;
layout(location = 4) in vec2 InTexCoord3;
layout(location = 5) in vec2 InTexCoord4;
layout(location = 6) in vec2 InTexCoord5;

layout(location = 7) in vec4 InColor;

layout(location = 8) in float InTexCoordIntensity1;
layout(location = 9) in float InTexCoordIntensity2;
layout(location = 10) in float InTexCoordIntensity3;
layout(location = 11) in float InTexCoordIntensity4;
layout(location = 12) in float InTexCoordIntensity5;
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
layout(location = 0) out vec2 OutTexCoord1;
layout(location = 1) out vec2 OutTexCoord2;
layout(location = 2) out vec2 OutTexCoord3;
layout(location = 3) out vec2 OutTexCoord4;
layout(location = 4) out vec2 OutTexCoord5;

layout(location = 5) out vec4 OutColor;

layout(location = 6) flat out float OutTexCoordIntensity1;
layout(location = 7) flat out float OutTexCoordIntensity2;
layout(location = 8) flat out float OutTexCoordIntensity3;
layout(location = 9) flat out float OutTexCoordIntensity4;
layout(location = 10) flat out float OutTexCoordIntensity5;
//####################################################

layout(location = 13) out vec3 vertex_light_direction;
layout(location = 14) out vec3 vertex_normal;

uniform MatricesBlock
{
	mat4 WorldMatrix;
    mat4 ViewMatrix;
    mat4 ProjectionMatrix;
};

uniform ConfigurationBlock
{
	int blah;
	int blah2;
};

void main() 
{
	gl_Position = ProjectionMatrix * ViewMatrix * WorldMatrix * vec4(InPosition, 1.0);

    OutTexCoord1 = InTexCoord1;
	OutTexCoord2 = InTexCoord2;
	OutTexCoord3 = InTexCoord3;
	OutTexCoord4 = InTexCoord4;
	OutTexCoord5 = InTexCoord5;

	OutTexCoordIntensity1 = InTexCoordIntensity1;
	OutTexCoordIntensity2 = InTexCoordIntensity2;
	OutTexCoordIntensity3 = InTexCoordIntensity3;
	OutTexCoordIntensity4 = InTexCoordIntensity4;
	OutTexCoordIntensity5 = InTexCoordIntensity5;

    OutColor = InColor;

	mat4 m = WorldMatrix;
	mat4 mi = transpose(inverse(m));

	vertex_normal = normalize(mi * vec4(InNormal, 1.0)).xyz;

	vertex_light_direction = normalize(vec3(0.0, 1.0, 0.0));
}
