#version 330 core

//layout (std140) uniform objects
//{
uniform mat4 ProjMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;
//};

layout (location = 0) in vec4 inPosition;
//layout (location = 1) in vec3 inTexcoord;

out vec4 Position;
//out vec3 Texcoord;

void main ()
{
  mat4 ModelViewProjMatrix = ProjMatrix * ModelMatrix * ViewMatrix;

  Position = ModelViewProjMatrix * inPosition;

  //Texcoord = inTexcoord;
  
  gl_Position = Position;
}
