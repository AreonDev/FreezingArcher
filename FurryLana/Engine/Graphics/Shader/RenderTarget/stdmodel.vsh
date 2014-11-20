#version 330

layout (std140) uniform objects
{
  mat4 ProjMatrix;
  mat4 ViewMatrix;
  mat4 ModelMatrix;
};

layout (location = 0) in vec3 inPosition;
layout (location = 1) in vec3 inTexcoord;

out vec4 Position;
out vec3 Texcoord;

void main ()
{
  mat4 ModelViewProjMatrix = ViewMatrix * ModelMatrix * ProjMatrix;

  Position = vec4 (inPosition, 1.0);

  Texcoord = inTexcoord;

  gl_Position = ModelViewProjMatrix * Position;
}
