#version 330 core

uniform mat4 ProjMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;

layout(location = 0) in vec4 inPosition;
layout(location = 1) in vec3 inNormal;
layout(location = 2) in vec2 inTexCoord;

out vec4 Position;
out vec3 Normal;
out vec2 TexCoord;

void main ()
{
    mat4 ProjViewModelMatrix = ProjMatrix * ViewMatrix * ModelMatrix;
  
    Position = ProjViewModelMatrix * inPosition;
    Normal = inNormal;
    TexCoord = inTexCoord;
  
    gl_Position = Position;
}
