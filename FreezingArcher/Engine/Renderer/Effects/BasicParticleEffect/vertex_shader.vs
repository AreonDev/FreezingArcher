#version 410

layout(location = 0) in vec3 InPosition;
layout(location = 1) in vec4 InColor;
layout(location = 2) in vec2 InSize;
layout(location = 3) in float InLife;

layout(location = 1) out vec4 OutColor;
layout(location = 2) out vec2 OutSize;
layout(location = 3) out float OutLife;
layout(location = 4) out vec3 OutPosition;


out gl_PerVertex
{
        vec4 gl_Position;
        float gl_PointSize;
        float gl_ClipDistance[];
};

void main()
{
        //Passthrough
        gl_Position = vec4(InPosition, 1.0);
        OutColor = InColor;
        OutSize = InSize;
        OutLife = InLife;
        OutPosition = InPosition;
}