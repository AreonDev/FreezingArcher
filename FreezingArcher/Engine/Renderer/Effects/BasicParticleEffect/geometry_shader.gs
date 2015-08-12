#version 410

layout(points) in;
layout(triangle_strip) out;
layout(max_vertices = 4) out;

uniform mat4 WorldMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ProjectionMatrix;

uniform vec3 CameraPosition;

in gl_PerVertex
{
  vec4 gl_Position;
  float gl_PointSize;
  float gl_ClipDistance[];
} gl_in[];

layout(location = 1) in vec4 InColor[];
layout(location = 2) in vec2 InSize[];
layout(location = 3) in float InLife[];

out gl_PerVertex
{
  vec4 gl_Position;
  float gl_PointSize;
  float gl_ClipDistance[];
};

layout(location = 1) out vec2 TexCoords;
layout(location = 2) out vec4 Color;
layout(location = 6) out vec4 View_Position;

void main()
{
        if(InLife[0] >= 0.05)
        {
                vec3 Pos = gl_in[0].gl_Position.xyz;
                vec3 toCamera = normalize(CameraPosition - Pos);

                vec3 up = normalize(vec3(ViewMatrix[0][1], ViewMatrix[1][1], ViewMatrix[2][1])) * InSize[0].y * InSize[0].y;
                vec3 right = normalize(vec3(ViewMatrix[0][0], ViewMatrix[1][0], ViewMatrix[2][0])) * InSize[0].x * InSize[0].x;

                vec3 va = Pos - (right + up);
                gl_Position = ProjectionMatrix * ViewMatrix * vec4(va, 1.0);
                View_Position = ViewMatrix * vec4(va, 1.0);
                TexCoords = vec2(0.0, 0.0);
                Color = InColor[0] * InLife[0];
                EmitVertex();

                vec3 vb = Pos - (right - up);
                gl_Position = ProjectionMatrix * ViewMatrix * vec4(vb, 1.0);
                View_Position = ViewMatrix * vec4(vb, 1.0);
                TexCoords = vec2(0.0, 1.0);
                Color = InColor[0]  * InLife[0];
                EmitVertex();

                vec3 vd = Pos + (right - up);
                gl_Position = ProjectionMatrix * ViewMatrix * vec4(vd, 1.0);
                View_Position = ViewMatrix * vec4(vd, 1.0);
                TexCoords = vec2(1.0, 0.0);
                Color = InColor[0]  * InLife[0];
                EmitVertex();

                vec3 vc = Pos + (right + up);
                gl_Position = ProjectionMatrix * ViewMatrix * vec4(vc, 1.0);
                View_Position = ViewMatrix * vec4(vc, 1.0);
                TexCoords = vec2(1.0, 1.0);
                Color = InColor[0]  * InLife[0];
                EmitVertex();

                EndPrimitive();
        }
}