#version 130

in vec4 inPosition;
in vec3 inCoord;

out vec4 DiffuseColor;

void main ()
{
  DiffuseColor = inPosition;
}
