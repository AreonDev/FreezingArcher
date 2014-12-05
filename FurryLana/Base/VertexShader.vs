#version 330 core

//layout (std140) uniform objects
//{
uniform mat4 ProjMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;
//};

layout(location = 0) in vec4 inPosition;
out vec3 Position;

void main ()
{
  mat4 ModelViewProjMatrix = ProjMatrix * ModelMatrix * ViewMatrix;

  Position = (ModelViewProjMatrix * inPosition).xyz;

  gl_Position = vec4 (Position, 1.0f);
}